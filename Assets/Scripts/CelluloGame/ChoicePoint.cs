using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ChoicePoint : MonoBehaviour
{
    [SerializeField] private bool triggerActive = false;
    private bool isMakingChoice;
    private string previousTextboxText;
    public string choiceText = "";
    public int sentenceNum;
    public int subchoiceNum;
    private int cooldown = 300;
    private bool hasBeenUsed = false;
    private bool hasBeenVisited = false;

    public CelluloGameController gameController;
    
    int choice = -1;

    public MainCelluloController celluloController;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // Do nothing if the choice has already been made
        if(hasBeenUsed){
            return;
        }

        choice = celluloController.checkButtonPressed();

        if(triggerActive && isMakingChoice){
            if(choice == 0 || Input.GetKeyDown(KeyCode.F)){
                // Accept
                Debug.Log("Accepted");
                DialogueManager dialogueManager = FindObjectOfType<DialogueManager>();
                celluloController.set_leds_green();

                // TUTORIAL SPECIFIC INTERACTION
                if(this.transform.parent.gameObject.name == "TutorialChoicePads"){
                    sentenceNum++;
                    choiceText = dialogueManager.DisplayNextSentence(sentenceNum);
                    // Enable the final tutorial dialogue pad
                    Transform child = GameObject.Find("TutorialBus").gameObject.transform.GetChild(0);
                    if(child != null){
                        child.gameObject.SetActive(true);
                    }

                    Transform UICanvas = GameObject.Find("UICanvas").gameObject.transform;
                    for(int i = 0; i < UICanvas.childCount; i++){
                        Transform element = UICanvas.GetChild(i);
                        if(element.gameObject.activeSelf == false){
                            element.gameObject.SetActive(true);
                        } 
                    }
                }else{ 
                    dialogueManager.acceptChanges(subchoiceNum);
                }
                hasBeenUsed = true;
            }
        }else {
            // Do nothing
        }
    }

    // Activate the choice point when a player enters its range
    void OnTriggerEnter(Collider other)
    {
  
        if (other.CompareTag("Player"))
        {
            // When the point is visited, activate the next pad if it exists
            if(!hasBeenVisited){
                hasBeenVisited = true;
                if(this.gameObject.transform.childCount != 0){
                    for(int i = 0; i < this.gameObject.transform.childCount; i++){
                        Transform child = this.gameObject.transform.GetChild(i);
                        if(child != null){
                            if(child.gameObject.name == "HandPointer"){
                                child.gameObject.SetActive(false);
                            }else{
                                child.gameObject.SetActive(true);
                            }
                        }
                    }
                }

                
            }
            if(this.name != "DialoguePad"){
                if(!hasBeenUsed){
                    // Light up the choice buttons
                    celluloController.applyChoiceSelectionColors();
                    isMakingChoice = true;
                }else{
                    celluloController.set_leds_green();
                }
                
            }


            // Save the current dialogue text in case the player leaves the range of the choice point
            TextMeshProUGUI textBox = GameObject.Find("main_dialog").GetComponent<TextMeshProUGUI>();
            previousTextboxText = textBox.text;

            // Change the dialogue text to the choice text~
            if(choiceText == ""){

                DialogueManager dialogueManager = FindObjectOfType<DialogueManager>();
                choiceText = dialogueManager.DisplayNextSentence(sentenceNum);
                Debug.Log("choiceText: " + choiceText);
                
            }else{
                textBox.text = choiceText;
            }
            
            triggerActive = true;

            
        }
    }

    // Deactivate the choice point when a player leaves its range
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Reset colors
            celluloController.reset_leds();

            // Restore the dialogue text
            GameObject.Find("main_dialog").GetComponent<TextMeshProUGUI>().text = previousTextboxText;
            triggerActive = false;
            isMakingChoice = false;
        }
    }



}