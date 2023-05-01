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
                dialogueManager.acceptChanges(subchoiceNum);
                celluloController.reset_leds();
                hasBeenUsed = true; 
            }
            else if (choice == 2 || Input.GetKeyDown(KeyCode.G)){
                // Decline
                Debug.Log("Declined");
                
            }
            else {
                // Do nothing
            }
        }
    }

    // Activate the choice point when a player enters its range
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if(this.name != "DialoguePad" && !hasBeenUsed){
                // Light up the choice buttons
                celluloController.applyChoiceSelectionColors();
                isMakingChoice = true;
            }


            // Save the current dialogue text in case the player leaves the range of the choice point
            TextMeshProUGUI textBox = GameObject.Find("main_dialog").GetComponent<TextMeshProUGUI>();
            previousTextboxText = textBox.text;

            // Change the dialogue text to the choice text~
            if(choiceText == ""){
                DialogueManager dialogueManager = FindObjectOfType<DialogueManager>();
                choiceText = dialogueManager.DisplayNextSentence(sentenceNum);
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