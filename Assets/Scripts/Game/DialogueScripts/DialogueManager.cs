using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class DialogueManager : MonoBehaviour
{
    public Button continueButton; 
    public Button acceptButton; //Update drones ranges and and go to next sentence if there is one.
    public Button refuseButton; //same as accept but doesnt updateDroneRanges
    public CelluloGameController gameController; //Needed to notify gameController that drone rangesAndBalance can be updated based on choice to accept or refuse.
    //Alternatively choice could just notify gameController.acceptRefuseChoiceMade();
    private TextMeshProUGUI dialogueText;
    private int currentSentence = 0;
    public bool finishedDialogue = false;
    private Queue<string> sentences; //Load sentences as read through dialog

    private bool waitTillFinishTyping = false; //Equals true when sentence finished typing out on screen, else false.
    private bool acceptRefuseButtonsAreDisplayed = false;

    public Button replayButton;

    void Start() {
        sentences = new Queue<string>();
    }

    /**
    Setup dialogue sentences and dialogue box and begin by displaying first sentence.
    */
    public void StartDialogue(Dialogue dialogue, TextMeshProUGUI dialogueTextBox){
        //Debug.Log("Starting dialogue :" + dialogue.name);
        //nameText.text = dialogue.name;
        finishedDialogue = false;
        sentences.Clear(); //clear previous 
        dialogueText = dialogueTextBox;
        foreach(string sentence in dialogue.sentences) {
            sentences.Enqueue(sentence);
        }
        DisplayNextSentence();

    }
    
    /**
    Display next sentence in queue
    */
    public void DisplayNextSentence() {
        //reach end of queue
        if(sentences.Count == 0) {
            finishedDialogue = true;
            return;
        } 
        
        //Can only display next sentence if finished typing out the previous sentence
        if(waitTillFinishTyping == false) {
            if(SceneManager.GetActiveScene().name == "EndingScene") {
                gameController.updateScientistImage();
            }
            
            waitTillFinishTyping = true;
            string sentence = sentences.Dequeue();
            //StopAllCoroutines();//Stop if click continue before last coroutine ended
            StartCoroutine(TypeSentence(sentence));
        }
    }

    //Code for animating the typing of sentence letter by letter
    IEnumerator TypeSentence(string sentence) {
        dialogueText.text = "";
        foreach(char letter in sentence.ToCharArray()) {
            dialogueText.text += letter;
            yield return new WaitForSeconds(0.005f);
        }
        waitTillFinishTyping = false;
    }

    void spawnRefuseAcceptButtons(){
        acceptRefuseButtonsAreDisplayed = true;
    }

    void UnspawnRefuseAcceptButtons() {
        if(acceptButton == null){
            return;
        }
        acceptRefuseButtonsAreDisplayed = false;
        refuseButton.gameObject.SetActive(false);
        acceptButton.gameObject.SetActive(false);
        //continueButton.gameObject.SetActive(true);
    }

    public void acceptChanges(){
        //Wait till finish typing before activating the button
        if(waitTillFinishTyping == false) {
            refuseButton.gameObject.SetActive(false);
            acceptButton.gameObject.SetActive(false);
            if(sentences.Count > 0){
                continueButton.onClick.Invoke(); //move to next sentence
                //No need to make continueButton visible again.
            }
            gameController.updateDroneRangesAndResources();  //update display of drone Ranges and balance
            gameController.incrementSubChoiceNum(); //increment the index indicating what sub choice we are on
        }
            
    }

    public void refuseChanges() {
        //Wait till finish typing before activating the button
        if(waitTillFinishTyping == false) {
            gameController.incrementSubChoiceNum();//increment the index indicating what sub choice we are on
        }   
    }
}
