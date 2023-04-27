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
    private int cooldown = 300;
    int choice = -1;

    public MainCelluloController celluloController;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
         // The cooldown makes it such that the player can't spam the button.
        if (cooldown < 300){
            cooldown ++;
        }

        choice = celluloController.checkButtonPressed();

        if(choice != -1 && cooldown == 300)
        {
            // TODO: DO STH
            cooldown = 0;
            
        }
        if(triggerActive && isMakingChoice){

            if(choice == 1 || Input.GetKeyDown(KeyCode.F)){
                // Accept
                Debug.Log("Accepted");  
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
            // Save the current dialogue text in case the player leaves the range of the choice point
            TextMeshProUGUI textBox = GameObject.Find("main_dialog").GetComponent<TextMeshProUGUI>();
            previousTextboxText = textBox.text;

            // Change the dialogue text to the choice text~
            if(choiceText == ""){
                DialogueManager dialogueManager = FindObjectOfType<DialogueManager>();
                choiceText = dialogueManager.DisplayNextSentence();
            }else{
                textBox.text = choiceText;
            }
            
            triggerActive = true;
            isMakingChoice = true;


            
        }
    }

    // Deactivate the choice point when a player leaves its range
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Restore the dialogue text
            GameObject.Find("main_dialog").GetComponent<TextMeshProUGUI>().text = previousTextboxText;
            triggerActive = false;
            isMakingChoice = false;
        }
    }



}
