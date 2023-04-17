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
    public GameControler gameController; //Needed to notify gameController that drone rangesAndBalance can be updated based on choice to accept or refuse.
    //Alternatively choice could just notify gameController.acceptRefuseChoiceMade();
    private TextMeshProUGUI dialogueText;
    public bool isMainTabText = false;
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
    public void StartDialogue(Dialogue dialogue, TextMeshProUGUI dialogueTextBox, bool isMainTabTextBox){
        isMainTabText = isMainTabTextBox; //if in main tab then will want to spawn buttons else no
        //Debug.Log("Starting dialogue :" + dialogue.name);
        //nameText.text = dialogue.name;
        continueButton.gameObject.SetActive(true);
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
            EndDialogue();
            finishedDialogue = true;
            return;
        } 
        
        //Can only display next sentence if finished typing out the previous sentence
        if(waitTillFinishTyping == false) {
            UnspawnRefuseAcceptButtons();//Remove old refuse/accept buttons if they are still there
            continueButton.gameObject.SetActive(false);
            //If have reached the the texts with possible subchoices and we are in the mainTabText
            //Then spawn accept and refuse buttons to make the subChoices
            if(isMainTabText && sentences.Count <= gameController.numSubChoices[gameController.latestChoiceId] + 1) {
                if(sentences.Count != 0 && sentences.Count != 1) //last sentence is "Please select next choice"(so dont want to spawn buttons here)
                    spawnRefuseAcceptButtons();
            }
            if(SceneManager.GetActiveScene().name == "EndingScene" && sentences.Count == 1) {
                replayButton.gameObject.SetActive(true);
            }
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
      
        if(acceptRefuseButtonsAreDisplayed == false){
            continueButton.gameObject.SetActive(true); //show continue button when finished typing
        } 
        
        if(SceneManager.GetActiveScene().name != "MissionStatement" && sentences.Count == 0){
            continueButton.gameObject.SetActive(false);
        }
        waitTillFinishTyping = false;
    }

    void EndDialogue() {
        continueButton.gameObject.SetActive(false);
        Debug.Log("End of conversation");
    }

    void spawnRefuseAcceptButtons(){
        acceptRefuseButtonsAreDisplayed = true;
        refuseButton.gameObject.SetActive(true);
        acceptButton.gameObject.SetActive(true);
        continueButton.gameObject.SetActive(false);
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
            refuseButton.gameObject.SetActive(false);
            acceptButton.gameObject.SetActive(false);
            if(sentences.Count > 0){
                continueButton.onClick.Invoke();
                if(acceptRefuseButtonsAreDisplayed == false && sentences.Count > 0){
                    continueButton.gameObject.SetActive(true);
                }
            }
            gameController.incrementSubChoiceNum();//increment the index indicating what sub choice we are on
        }   
    }
}
