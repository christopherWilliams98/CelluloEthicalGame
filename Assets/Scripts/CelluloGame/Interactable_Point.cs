using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/*
    * This script is attached to the pads in the house maps
    * It is used to teleport the player to the desired house
    * and to return to the main map
*/
public class Interactable_Point : MonoBehaviour
{
    
    [SerializeField] private bool triggerActive = false;
    public GameObject teleportLocation;
    public GameObject allPads;
    public GameObject returnPad;

    public GameObject choicePads;
    public MainCelluloController celluloController;
    public CelluloGameController gameController;
    private string temp;
    private int cooldown;
 
    public bool isTutorial = false;

    public bool isEnding = false;

    public int sentenceNum;
    private bool once = true;
    public AudioSource enterSound;

    private AudioSource greetings; 


    void Start()
    {
        Screen.fullScreen = true;
    }
    private void Update()
    {
        if(cooldown > 0){
            cooldown--;
        }

        if(once && isTutorial){
            gameController.lockInChoice();
            greetings = GameObject.Find("Greetings").GetComponent<AudioSource>();
            greetings.Play();
            once = false;
        }

        int choice = celluloController.checkButtonPressed();

        if(isEnding && triggerActive && cooldown == 0){
            if(Input.GetKeyDown(KeyCode.Space) || choice != -1){
                endingHandler();
                cooldown = 300;
            }
            return;
        }


        // Check if player wants to interact with the pad
        if(triggerActive && (Input.GetKeyDown(KeyCode.Space) || choice != -1 ) && cooldown == 0)
        {
            cooldown = 300;
            Interact();
        }
    }

    public void endingHandler(){

        DialogueManager dialogueManager = FindObjectOfType<DialogueManager>();
        Dialogue dialogue = gameController.computeOutcomeDialogue();
        TextMeshProUGUI textBox = GameObject.Find("ending_dialog").GetComponent<TextMeshProUGUI>();

        if(sentenceNum == 0){
            dialogueManager.StartDialogue(dialogue, textBox);
            sentenceNum++;
        }
        else{
            dialogueManager.DisplayNextSentence(sentenceNum++);
        }

        GameObject allScientists = GameObject.Find("Scientists");
        switch(sentenceNum){
            case 2:
                for(int i = 0; i < allScientists.transform.childCount; i++){
                    if(allScientists.transform.GetChild(i).gameObject.name == "Ansley Smith"){
                        allScientists.transform.GetChild(i).gameObject.SetActive(false);
                    }
                    if(allScientists.transform.GetChild(i).gameObject.name == "Davina Murphy"){
                        allScientists.transform.GetChild(i).gameObject.SetActive(true);
                    }
                }
                break;
            case 3:
                for(int i = 0; i < allScientists.transform.childCount; i++){
                    if(allScientists.transform.GetChild(i).gameObject.name == "Davina Murphy"){
                        allScientists.transform.GetChild(i).gameObject.SetActive(false);
                    }
                    if(allScientists.transform.GetChild(i).gameObject.name == "Fiona Wattson"){
                        allScientists.transform.GetChild(i).gameObject.SetActive(true);
                    }
                }
                break; 
            
        }
    }
    

    // Activate pad interaction when a player enters its range
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            triggerActive = true;
            celluloController.set_leds_white();


            if(isEnding){
                return;
            }
            TextMeshProUGUI textBox = GameObject.Find("main_dialog").GetComponent<TextMeshProUGUI>();
            
            temp = textBox.text;

           
            
            string interactInstructionString = "\n\n\nTouch and hold any light to interact";

            switch(this.gameObject.name){

                case "ReturnPad":
                    textBox.text = "To Dufftown" + interactInstructionString;
                    break;

                case "TechShopPad":
                    textBox.text = "Consult drone expert \n\n" + "COST: 0.5 weeks" + interactInstructionString;
                    break;
                    
                case "OrnithologistPad":
                    textBox.text = "Consult ornithologist \n\n" + "COST: 0.5 weeks" + interactInstructionString;
                    break;

                case "CityParkPad":
                    textBox.text = "Test drone in city park \n\n" + "COST: 0.5 weeks" + interactInstructionString;
                    break;
                
                case "ExternalLocationPad":
                    textBox.text = "Test drone on external location \n\n" + "COST: 1 week" + interactInstructionString;
                    break;

                case "CityHallPad":
                    textBox.text = "Consult local council \n\n" + "COST: 1 week" + interactInstructionString;
                    break;

                case "BirdReservoirPad":
                    textBox.text = "Consult bird reservoir director \n\n" + "COST: 1 week" + interactInstructionString;
                    break;

                case "PostOfficePad":
                    textBox.text = "Enter the post office \n\n" + "COST: Free to visit" + interactInstructionString;
                    break;

                case "TutorialPad":
                    textBox.text = "Amazing work! These grey doorway pads are used to enter locations in the game, and will display information about their respective locations when your robot is on top of them. To interact with the grey pad, please touch and hold any of the white lights on the robot.";
                    AudioSource amazing = GameObject.Find("Amazing").GetComponent<AudioSource>();
                    if(!amazing.isPlaying && !greetings.isPlaying){
                        amazing.Play();
                    }
                    break;

                case "TutorialReturnPad":
                    textBox.text = "You're all set to go! Home pads like this one allow you return to the main map once you're done making choices in a location. To interact with the pad, touch and hold any of the white lights on the robot, just as you would on a grey doorway pad. When you're ready, give it a try!\nThis will start the game.";
                    AudioSource allSet = GameObject.Find("all set to go").GetComponent<AudioSource>();
                    List<AudioSource> audioSources = new List<AudioSource>(FindObjectsOfType<AudioSource>());
                    ChoicePoint.StopAllAudio(audioSources);
                    allSet.Play();
                    break;

                default:

                    break;
            }

        }
    }

    // Deactivate pad interaction when a player leaves its range
    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if(!isTutorial && !isEnding){
                GameObject.Find("main_dialog").GetComponent<TextMeshProUGUI>().text = temp;
            }
            triggerActive = false;
            celluloController.reset_leds();

        }
    }

    // Teleport player to the desired house or return to the main map
    public void Interact()
    {
        Debug.Log("Talking with " + gameObject.name + "");

        gameController.enableDialogueBox(true);

        triggerActive = false;

        if(!isEnding)
        enterSound.Play();

        if(GameObject.Find("DroneImage") != null){
            RectTransform droneImage = GameObject.Find("DroneImage").GetComponent<RectTransform>();
        }

        celluloController.reset_leds();
        if(this.gameObject.name == "ReturnPad" || this.gameObject.name == "TutorialReturnPad")
        {
            // Enable all other house pads, disable return pad and choice pads
            if(allPads!=null){
                allPads.SetActive(true);
            } 
            if(returnPad != null){
                Debug.Log("REMOVING RETURN PAD");
                returnPad.SetActive(false);
                
            } 
            foreach(Transform choicePad in GameObject.Find("ChoicePads").transform){
                if(choicePad.gameObject.activeSelf == true){
                    choicePad.gameObject.SetActive(false);
                }
            }
          


            // Move drone image back to the main map
            //droneImage.anchoredPosition3D = new Vector3(0, 0, 0);
            //droneImage.sizeDelta = new Vector2(75,75);
            // Exit current location

            // Clear the dialogue box
            TextMeshProUGUI textBox = GameObject.Find("main_dialog").GetComponent<TextMeshProUGUI>();
            textBox.text = "";

            
            foreach(Transform house in GameObject.Find("HouseMaps").transform)
            {
                if(house.gameObject.activeSelf == true){
                    house.gameObject.SetActive(false);
                }
            }
        }
        
        else{
            // Enter the desired house 
            if(teleportLocation != null){
                teleportLocation.SetActive(true);
            }
            
            if(teleportLocation.name == "CityPark"){
                GameObject droneImage = GameObject.Find("DroneImage");
                if(droneImage != null){
                    //droneImage.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(-947f, -331f, 0);
                    
                }
            }
            // Move drone image to the desired house
            //droneImage.anchoredPosition3D = new Vector3(60f, 240f, 0);
            //droneImage.sizeDelta = new Vector2(40,40);

            // Disable all other house pads 
            // Enable the return pad and choice pads
            if(allPads != null){
                allPads.SetActive(false);
            }

            if(choicePads != null){
                choicePads.SetActive(true);
            }
            if(returnPad != null){
                returnPad.SetActive(true);
            } 

            gameController.lockInChoice();
        }
        
    }

   


    
}
