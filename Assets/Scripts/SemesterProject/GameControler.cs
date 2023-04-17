using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Text;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

//Blake Hill
public enum choices
{
	None, 
	DroneExpert, //1
    BirdExpert, //expert on specific bird (choice2)
    TestLocally, //test in backyard (choice id: 3)
    //OnSiteVisit, //nothern ireland lots of rain and WIND! in winter for example
    OnFieldTesting, //choice id: 4
    userTesting, //choice id: 5
    resevoirDirector, //choice id: 6
    shipIt, //choice id: 7
}

public class GameControler : MonoBehaviour
{
    //TODO perhaps make new script for missionStatementScene so its less cluttered
    //Intro page 
    public DialogueTrigger MissionStatementTriggerButton; //TODO try to trigger on awake using text for now try with button
    public Dialogue MissionStatementDialogue;
    public TextMeshProUGUI MissionStatementDialogueTextBox;
    //Final outcome dialogue box
    public Image scientistImageBox;
    public Sprite[] scientistImages;
    private int currentScientist = 0;
    public DialogueTrigger finalDialogueTriggerButton; 
    public Dialogue finalOutcomeDialogue;
    public TextMeshProUGUI finalOutcomeDialogueTextBox;

    //If locked in ornythologist before drone expert then should i propose choice between two colors??
    //could always propose choice between two colors maybe??
    //private int locked_in_ornythologist:

    //Drone Specs
    private static int protoNoiseLevel = 80; // [db]
    private static int protoDroneSize = 25;
    private static double protoDroneWeight = 1.0;
    private static string protoDroneColor = "Blue";
    private static string protoFrameMaterial = "Aluminium";
    private static int protoDroneLifespan = 10;
    private static string protoPropellerMaterial = "Plastic";
    private static bool has_wetsuit = false;
    private static bool has_manual = false;
    private static bool has_foldable_propellers = false;
    public TextMeshProUGUI droneSpecsText;
    public Image droneImage;
    public Sprite droneWhiteSpriteImage;
    public Sprite dronePurpleSpriteImage;
    public static List<string> colorList = new List<string>{"white", "purple", "blue"};
    //public static int[] droneSizeRange = {20, 150}; //min and max size range
    //public static double[] droneWeightRange = {0.5, 10};

    //Main Tab Feedback text
    public DialogueTrigger EnterButton;
    public TextMeshProUGUI dialogueTextBox;
    public TextMeshProUGUI scrollBarText; //Contains text currently displayed in scrollBar

    //Array of locked choice and choice selection objects
    List<int> locked_choices = new List<int>(); //List of choices locked in by the players
    public DropSlot slot; //slot where choice is dropped into

    /* Tabs and dialogues -------------------------------------------------------*/
    //public DialogueManager dialogueManager;
    public PopUpScript popUp;
    public Button mainTab;
    private int mainTabIndexInLayout; //Used to make sure the "main" tab is always to the right of all tabs
    public TabController tabController;
    //Array of dialogues 
    [SerializeField] private List<Dialogue> choiceFeedbackDialogues;//set in unity directly
    private string[] finalOutcomeDialogueSentences = {"var 0", "var 1", 
    "var 2", "var 3", "var4"};

    /*ACCEPT/REFUSE------------------------------------------------------------------*/
    public int[] numSubChoices; //maps Choices => int representing number of subchoices for this main choice. 
    //(Necessary to know when to start using accept/refuse buttons)
    public int latestChoiceId = 0;
    int acceptedSubChoiceNumber = 0; //represents the current subchoice withing the main choice
    bool accept = false; //record user's choice
    //These buttons spawn when player must make choice of accepting to refusing the proposed changes from the expert
    public Button acceptButton; 
    public Button refuseButton;

    /*Money and Time System ---------------------------------------------------------------*/
    public TextMeshProUGUI availableBalanceText;
    public TextMeshProUGUI remainingTimeText;
    static public float remainingTime = 11; //number of weeks remaning till project deadline 
    static public int availableBalance = 300; //Starting budget
    public int[] mainChoiceFinancialCosts; //Costs of each main choice, set in unity
    public float[] mainChoiceTimeCosts; //timeCosts of each main choice, set in unity
    // -----------------------------------------------------------------------------
    void Start()
    {   //Set on MissionStatement scene set up MissionStatementDialogue
        if(MissionStatementTriggerButton!= null) {
            MissionStatementTriggerButton.dialogueTextBox = MissionStatementDialogueTextBox;
            MissionStatementTriggerButton.dialogue = MissionStatementDialogue;
            MissionStatementTriggerButton.allowRestart = false;
        }

        //If on final scene setup final trigger with its dialogue
        if(finalDialogueTriggerButton!= null) {
            finalDialogueTriggerButton.dialogueTextBox = finalOutcomeDialogueTextBox;
            finalOutcomeDialogue = computeOutcomeDialogue();
            finalDialogueTriggerButton.dialogue = finalOutcomeDialogue;
            finalDialogueTriggerButton.allowRestart = false;
        }
      
        mainTabIndexInLayout = 1; 
        //Print balance and drone specs 
        if(SceneManager.GetActiveScene().name == "EthicalGame") {
            remainingTimeText.text = "Time Left: \n" +  remainingTime.ToString("F1") +" Weeks"; 
            availableBalanceText.text = "Balance: " + availableBalance.ToString() +" CHF"; 
        }
      
        refreshDroneSpecs();
    }
    
    /**
    Manages what happens any time a user lock's in a choice.
    This is triggered any time a user lock's in a choice by clicking the "enter" button.
    */
    public void lockInChoice() {
        if(slot.isEmpty()){
            return;
        } else {
            slot.emptyTheSlot();//reset slot to empty for next choice
        }

        acceptedSubChoiceNumber = 0; //reset subChoice index
        DragDrop lastChoice = slot.droppedChoice;
                
        latestChoiceId = lastChoice.choice_id;
        string choiceCardText = lastChoice.GetComponentInChildren<TextMeshProUGUI>().text;
        
        //reset choice card to its original location
        lastChoice.transform.position = lastChoice.original_position;

        //Get cost of choice and check if have avaible funds
        float mainChoiceTimeCost = mainChoiceTimeCosts[latestChoiceId];
        int mainChoiceFinancialCost = mainChoiceFinancialCosts[latestChoiceId];
        //If sufficient resources
        if(checkIfEnoughResources(mainChoiceTimeCost, mainChoiceFinancialCost)){
            
            locked_choices.Add(latestChoiceId); // Add to List of locked choices TODO CAN REMOVE THIS PROBABLY LATER

            tabController.spawnTab(latestChoiceId, choiceCardText, choiceFeedbackDialogues[latestChoiceId]);
            //Main Tab always displayed the to the right of all other tabs
            mainTab.transform.SetSiblingIndex(++mainTabIndexInLayout);

            //Update game paramaters and UI
            updateMainTabText(choiceFeedbackDialogues[latestChoiceId]); //update text in main tab
            EnterButton.TriggerDialogueMainTab(); //trigger dialogue in MainTab

            //update available time and balance due to locking in this main choice
            updateAvailableBalanceAndTimeForMainChoice(latestChoiceId);
        } else {
            popUp.display();
        }
        //Check if game ended, then activate final scene.
        if(latestChoiceId == (int)choices.shipIt){
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex+ 1);
        }
    }

    /**
    Called by DialogueManager when subChoice is accepted or refused
    */
    public void incrementSubChoiceNum() {
        acceptedSubChoiceNumber++;
    }

    ///
    /// Input: cost of currently selected subchoice
    /// Output: Boolean indicating if have enough resources
    ///
    private bool checkIfEnoughResources(float timeCost, int financialCost){
        //if not enough resources return false
        if(availableBalance < financialCost || remainingTime < timeCost){
            return false;
        }
        return true;
    }
    /**
    This method is called whenever a subChoice is accepted
    Updates drone ranges according to accepted subChoice
    Called whenever choice is accepted
    */
    public void updateDroneRangesAndResources(){
        //Debug.Log("Calls updateDronRanges");
        //Debug.Log(latestChoiceId);
        //TODO here check if availableBalance and RemaningTime is sufficient for this subchoice
        //Once we know what choice was made, need to check if have enough funs before executing changes.
        //If not enough DONT EXECUTE!! + inform user somehow that not enough funds!

        float timeCost= (float)0.0;
        int financialCost = 0;

        if(latestChoiceId == (int)choices.DroneExpert){
            if(acceptedSubChoiceNumber == 0){ //repait drone white
                timeCost= (float)0.5;
                financialCost = 50;
                if(checkIfEnoughResources(timeCost, financialCost)){
                    protoDroneColor = "White";
                } else {
                    Debug.Log("Not enough resources, drone expert choice 0");
                    popUp.display();
                    return;
                }
                
            }
            if(acceptedSubChoiceNumber == 1){ //create manual
                timeCost =(float)2.0;
                financialCost = 0;
                if(checkIfEnoughResources(timeCost, financialCost)){
                    has_manual = true;
                } else {
                    Debug.Log("Not enough resources, drone expert choice 0");
                    popUp.display();
                    return;
                }
            }
        }
        if(latestChoiceId == (int)choices.BirdExpert){
            //Bird expert ultimately suggest not to make it white
            if(acceptedSubChoiceNumber == 0) { //Repaint drone purple
                timeCost = (float)0.5;
                financialCost = 50;
                if(checkIfEnoughResources(timeCost, financialCost)){
                    protoDroneColor = "Purple";
                } else {
                    popUp.display();
                    Debug.Log("Not enough resources, drone expert choice 0");
                    return;
                }
            } else if(acceptedSubChoiceNumber == 1) { //Make drone out of carbon fiber
                timeCost = (float)0.5;
                financialCost = 50;
                if(checkIfEnoughResources(timeCost, financialCost)){
                    protoPropellerMaterial = "Carbon Fiber";
                    protoNoiseLevel -= 10;
                } else {
                    popUp.display();
                    Debug.Log("Not enough resources, drone expert choice 0");
                    return;
                }
            }
        }
        if(latestChoiceId == (int)choices.TestLocally){
            if(acceptedSubChoiceNumber == 0) { // Make bigger for bigger battery
                timeCost = (float)2.0;
                financialCost = 100;
                if(checkIfEnoughResources(timeCost,financialCost)){
                    protoDroneWeight += 0.5;
                    protoDroneSize += 10;
                    protoNoiseLevel += 10;
                    protoDroneLifespan += 5;
                } else {
                    popUp.display();
                    Debug.Log("Not enough resources, drone expert choice 0");
                    return;
                }
            }
        }
        if(latestChoiceId == (int)choices.OnFieldTesting){
            if(acceptedSubChoiceNumber == 0) { //wetsuit
                timeCost = (float)2.0;
                financialCost = 100;
                if(checkIfEnoughResources(timeCost,financialCost)){
                    has_wetsuit = true;
                } else {
                    popUp.display();
                    Debug.Log("Not enough resources, drone expert choice 0");
                    return;
                }
            } else if(acceptedSubChoiceNumber == 1) { //get bigger battery
                timeCost = (float)1.0;
                financialCost = 50;
                if(checkIfEnoughResources(timeCost,financialCost)){
                    protoDroneWeight += 0.5;
                    protoDroneSize += 10;
                    protoNoiseLevel += 10;
                    protoDroneLifespan += 3;
                } else {
                    popUp.display();
                    Debug.Log("Not enough resources, drone expert choice 0");
                    return;
                }
            } else if(acceptedSubChoiceNumber == 2) { //switch to carbon fiber
                timeCost = (float)1.0;
                financialCost = 50;
                if(checkIfEnoughResources(timeCost,financialCost)){
                    //If switching from a heavier material, battery lifespan increases
                    if(protoFrameMaterial == "Wood" || protoFrameMaterial == "Aluminium") {
                        protoDroneWeight -= 0.5;
                        protoNoiseLevel -= 10;
                        protoDroneLifespan += 5;
                    }
                    protoFrameMaterial = "Carbon Fiber";
                    
                } else {
                    popUp.display();
                    Debug.Log("Not enough resources, drone expert choice 0");
                    return;
                }
            }
        }

        if(latestChoiceId == (int)choices.userTesting){
            if(acceptedSubChoiceNumber == 0) { //Foldable propellers
                timeCost = (float)0.0;
                financialCost = 50;
                if(checkIfEnoughResources(timeCost, financialCost)){
                    has_foldable_propellers = true;
                } else {
                    popUp.display();
                    Debug.Log("Not enough resources, drone expert choice 0");
                    return;
                }
            } else if(acceptedSubChoiceNumber == 1) { // Make drone lighter??
                timeCost = (float)1.0;
                financialCost = 50;
                if(checkIfEnoughResources(timeCost,financialCost)){
                    protoDroneWeight -= 0.25;
                    protoNoiseLevel -= 5;
                    protoDroneLifespan += 2;
                } else {
                    popUp.display();
                    Debug.Log("Not enough resources, drone expert choice 0");
                    return;
                }
            }
        }

        if(latestChoiceId == (int)choices.resevoirDirector){
            if(acceptedSubChoiceNumber == 0) { //take director insight and use wood propeller
                timeCost = (float)0.0;
                financialCost = 50;
                if(checkIfEnoughResources(timeCost, financialCost)){
                    if(protoPropellerMaterial == "Plastic") {
                        protoNoiseLevel -= 10;
                    }
                    protoPropellerMaterial = "Wood";
                    
                } else {
                    popUp.display();
                    Debug.Log("Not enough resources, drone expert choice 0");
                    return;
                }
            } else if(acceptedSubChoiceNumber == 1) { //switch to carbon fiber if not already?(reapeat same pro&cons as last time)
                timeCost = (float) 1.0;
                financialCost = 50;
                if(checkIfEnoughResources(timeCost, financialCost)){
                    protoFrameMaterial = "Wood";
                    protoDroneWeight += 0.5;
                } else {
                    popUp.display();
                    Debug.Log("Not enough resources, drone expert choice 0");
                    return;
                }
            } else if(acceptedSubChoiceNumber == 2) { //make drone smaller and lighter
                timeCost = (float) 1.0;
                financialCost = 50;
                if(checkIfEnoughResources(timeCost, financialCost)){
                    protoDroneWeight -= 0.5;
                    protoDroneSize -= 10;
                    protoNoiseLevel -= 5;
                } else {
                    popUp.display();
                    Debug.Log("Not enough resources, drone expert choice 0");
                    return;
                }
            }
        }
        //display updates
        updateAvailableBalanceAndTimeForSubChoices(timeCost, financialCost);
        refreshDroneSpecs();
        
    }
    //Contains logic calculate the next text to display in scrollBar
    //Each choice locked in has a according display text
    private void updateMainTabText(Dialogue new_dialogue){
        EnterButton.dialogueTextBox = this.dialogueTextBox;
        EnterButton.dialogue = new_dialogue;
    }

    //updates the interface showing teh drone specs 
    private void refreshDroneSpecs(){
        StringBuilder sb = new StringBuilder("", 400);
        sb.AppendFormat("Prototype drone specs: \n");
        sb.AppendFormat("Color: " + protoDroneColor + " \n");
        sb.AppendFormat("Weight [kg]: " + string.Format("{0:F1}", protoDroneWeight) + " \n");
        sb.AppendFormat("Size [cm]: " + string.Format("{0:F1}", protoDroneSize) + " \n");
        sb.AppendFormat("Drone Frame Material: " + protoFrameMaterial + " \n");
        sb.AppendFormat("Battery lifespan: " + protoDroneLifespan.ToString() + " minutes \n");
        sb.AppendFormat("Propeller Material: " + protoPropellerMaterial + "\n");
        sb.AppendFormat("Noise level: " + protoNoiseLevel + "\n");
        sb.AppendFormat("\n-----Extra Features----- \n");
        if(has_wetsuit){
            sb.AppendFormat("Wet suit available\n"); 
        } 
        if(has_manual){
            sb.AppendFormat("Drone Manual available\n"); 
        } 
        if(has_foldable_propellers){
            sb.AppendFormat("Foldable Propellers\n"); 
        } 
        droneSpecsText.text = sb.ToString();

        if(protoDroneColor == "White") {
            droneImage.sprite = droneWhiteSpriteImage;
        } else if(protoDroneColor == "Purple") {
            droneImage.sprite = dronePurpleSpriteImage; 
        } else {
            //droneImage.sprite = 
        }
    }

    //Update available time and balance for subchoices
    private void updateAvailableBalanceAndTimeForSubChoices(float timeCost, int financialCost) {
        remainingTime-= timeCost;
        availableBalance -= financialCost;
        remainingTimeText.text = "Time Left: \n" + remainingTime.ToString("F1") +" Weeks"; 
        availableBalanceText.text = "Balance: " + availableBalance.ToString() +" CHF"; 
        
    }
    //Update available balance and time for main choices
    private void updateAvailableBalanceAndTimeForMainChoice(int locked_choice_id) {
        Debug.Log("availableBalance choice" + locked_choice_id.ToString());
        Debug.Log("cost of choice" + mainChoiceFinancialCosts[locked_choice_id].ToString());
        remainingTime-= mainChoiceTimeCosts[locked_choice_id];
        availableBalance -= mainChoiceFinancialCosts[locked_choice_id];
        remainingTimeText.text = "Time Left: \n" + remainingTime.ToString("F1") +" Weeks"; 
        availableBalanceText.text = "Balance: " + availableBalance.ToString() +" CHF"; 
        
    }

    private Dialogue computeOutcomeDialogue(){
        int outcomeNum = 0;
 

        StringBuilder sb = new StringBuilder("", 500);
        string sentence = "";
        sb.AppendFormat("\n\n\n\n          ");
        sb.AppendFormat(" Ansley Smith: \n\n");
        sb.AppendFormat("\"");
        if(protoDroneColor.Equals("Blue")) {
             sentence = "The color of drone is unfortunate because its color blends in with that of" + 
             " the sky, i often lose track of it and then lose time trying to find it.";
        } else if(protoDroneColor.Equals("White")) {
             sentence = "The white color of the drone is easy to spot in the sky however some birds" + 
             " have attacked the drone, maybe because white is seen as aggressive by some birds." ;    
        } else if(protoDroneColor.Equals("Purple")){
             sentence = "I like that you made the drone purple, most birds are not threatened "
             + "by this color and the drone remains clearly visible to the operator." ;  
        }
        sb.AppendFormat(sentence + "\n\n");
        
        if(protoDroneSize <= 30) {
            sentence = "The size of the drone is small and easy to carry!, however on windy days it" 
            + " is not as stable as previous drones.";
        } else if(protoDroneSize > 30) {
            sentence  = "The drone is pretty big and unable to fit in my bag, perhaps a carrying case " + 
            "would be useful";
        }
        sb.AppendFormat(sentence + "\n\n");

        //flight time is influenced by WEIGHT AND BATTERY (baterry shouldnt be in minutes!! thats autonomy)
        //use weight for stability and lfiespan for this
        if(protoDroneLifespan <= 15) {
            sentence =  "Drone was light and easy to carry, but short flying time meant it felt like a lot of " + 
            "work for the brief footage. Although we did get some really great data we couldn’t have got otherwise!";
        }else if(protoDroneLifespan > 15 && protoDroneLifespan <= 20) {
            sentence = "Long flying time from the big battery was a great improvement from our last drone,"
        + " and the drone was stable in the wind.";
        }else {
            sentence = "This drone is capable of flying and observing birds for about 30 minutes, "+
            "it is a slight improvement from our previous drone and the stability of the drone is about the same."; 
        }
        sb.AppendFormat(sentence);
        sb.AppendFormat("\"");
        finalOutcomeDialogueSentences[outcomeNum++] = sb.ToString();
        sb.Clear();

        sb.AppendFormat("\n\n\n\n          ");
        sb.AppendFormat("Davina Murphy: \n\n");
        sb.AppendFormat("\"");
        if(has_wetsuit) {   
            sentence = "The previous drone we used did not have a wet suit, so we are very satisfied" 
            + " to now we are able to conduct our bird observation even in the rough Scottish weather";
        } else {
            sentence = "Unfortunately that just our previous drone we had, we are not able to use" + 
            " it under rainy conditions.";            
        }
        sb.AppendFormat(sentence + "\n\n");

        if(has_manual) {   
            sentence = "Including the drone manual was super useful, allowing new people to pick it up quickly. " 
            + "Although terminology was a bit technical, so they added in some of their own definitions to make it more accessible";
        } else {
            sentence = "Hard to get started using the drone. Mostly only the 2 PhD student researchers were willing to invest time getting competent, "
            + "we will see if those starting next year also will.";            
        } 
        sb.AppendFormat(sentence + "\n\n");
        

        if(protoPropellerMaterial == "Plastic"){
            sentence = "The plastic propellers of the drone make a lot of noise, and on occasion seems to scare off" + 
            " or disturb some of the birds, however the flexibility of the propellers makes the drone less harmful in case of a collision"
            +" with a bird.";
        } else if(protoPropellerMaterial == "Carbon Fiber"){
            sentence = "Quiet drones appear not to bother birds at all, however the carbon fiber propellers are much harder than"+
            " plastic ones, and we need to be really careful flying it too close to the birds as the propeller could seriously injure a curious or aggressive bird.";            
        } else if(protoPropellerMaterial == "Wood"){
            sentence = "The wooden propellers are very silent and appear not to bother birds at all, "+
            " however they are harder than plastic ones, so i am always afraid of injuring a bird who might fly to close."; 
        }
        sb.AppendFormat(sentence + "\"");
        finalOutcomeDialogueSentences[outcomeNum++] = sb.ToString();
        sb.Clear();

        sb.AppendFormat("\n\n\n\n          ");
        sb.AppendFormat("Fiona Wattson: \n\n");
        sb.AppendFormat("\"");
        if(has_foldable_propellers) {
            sentence = "The foldable propellers were a nice upgrade, the drone fits much easier into my bag";
        } else {
            sentence = "The drone is quite big and cumbersome to carry over long distances, perhaps" +  
            " using foldable propellers would make it easier to carry in a smaller bag.";
        }    
        sb.AppendFormat(sentence + "\n\n");

        if(protoDroneWeight <= 1.0) {
            sentence =  "Drone was light and easy to carry.";
        }else if(protoDroneWeight >= 2.0) {
            sentence = "Long flying time from the big battery was a great improvement from our last drone,"
         + " and the drone was stable in the wind."
         + " But overall the drone was too heavy to carry. Most of our researchers are under 160 cm, so the combined weight and size"
         + " made it very difficult to hike with it over wild terrain for 3h. We didn’t take it out very often";
        } else {
            sentence = "In terms of weight, it is a slight improvement from our previous drone and the stability of the drone is about the same."; 
        }
        sb.AppendFormat(sentence);
        /*if(protoFrameMaterial == "Wood") {
            sentence = "Using wood";
        } else if(protoFrameMaterial == "Carbon Fiber") {
            sentence = "Using Carbon Fiber";
        } else if(protoFrameMaterial == "Aluminium") {
            sentence = "Using Aluminium";
        }*/
        sb.AppendFormat("\"");
        finalOutcomeDialogueSentences[outcomeNum++] = sb.ToString();
        sb.Clear();

        //SPECIFIC OUTCOMES AFTER 1 YEAR OF USE:
        //TODO include link
        //https://nanpa.org/2021/06/11/drone-crash-causes-birds-to-abandon-1500-eggs/#:~:text=A%20drone%2C%20flying%20over%20prohibited,AP%20and%20New%20York%20Times.
        if(protoDroneWeight <= 1.0 && protoDroneLifespan <= 10) {
            finalOutcomeDialogueSentences[outcomeNum++] = "Update after 1 year of use: \n"
            + "The drone was used near a nesting colony of elegant terns and unfortunately due to the drones"
            + " short lifespan and high winds that day, we were unable to return the drone in time and it crashed"
            + ", scaring away the colony and abandoning their eggs!.";
        } else if(protoNoiseLevel >= 95) {
             finalOutcomeDialogueSentences[outcomeNum++] = "Update after 1 year of use: \n"
             + "One of our researches flew the drone too close to"
             +" a couple of bird nest's and the high noise levels scared away the parents, unfortunately they never"
             +" returned, and their eggs were abandoned.";
        } else {
            finalOutcomeDialogueSentences[outcomeNum++] = "Update after 1 year of use: \n" 
            + "Although not perfect the drone has been a great help!";
        }

        finalOutcomeDialogueSentences[outcomeNum++] = "Thank you for playing!";
        
        Dialogue outcomeDialogue = new Dialogue();

        //TODO last slide should you be
        //You have reached the end, thank you for playing! or something like that
        
        outcomeDialogue.sentences = finalOutcomeDialogueSentences;
        return outcomeDialogue;
    }

    public void restartGame(){
        resetStaticVariables();
        SceneManager.LoadScene(0);
        
    }
    private void resetStaticVariables(){
        protoDroneSize = 25;
        protoDroneWeight = 1.0;
        protoDroneColor = "Blue";
        protoFrameMaterial = "Aluminium";
        protoDroneLifespan = 10;
        protoPropellerMaterial = "Plastic";
        has_wetsuit = false;
        has_manual = false;
        has_foldable_propellers = false;
        remainingTime = 11; 
        availableBalance = 300; 
    }

    public void updateScientistImage(){
        scientistImageBox.gameObject.SetActive(true);
        currentScientist++;
        if(currentScientist < scientistImages.Length) {
            scientistImageBox.sprite = scientistImages[currentScientist];
        } else {
            scientistImageBox.gameObject.SetActive(false);
        }
    }
}
