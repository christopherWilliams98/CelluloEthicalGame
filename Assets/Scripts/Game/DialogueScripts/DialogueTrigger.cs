using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueTrigger : Button
{
    public Dialogue dialogue;
    public TextMeshProUGUI dialogueTextBox;

    //dont allow same button to trigger dialogue several times
    //This fix was implemented to avoid the missionStatementScene continue button to restart dialogue on even click
    public bool allowRestart = true; //default behavior
    private bool hasBeenTriggered = false;

    public void TriggerDialogue () {
        if(!hasBeenTriggered || allowRestart) {
            FindObjectOfType<DialogueManager>().StartDialogue(dialogue, dialogueTextBox, false);
            hasBeenTriggered = true;
            Debug.Log("trigger Dialogue of buttom with name: " + this.name);
        }
    }

    //By setting bool to true, we notify that this is the main tab and the spawn accept/refuse feature should be active
    public void TriggerDialogueMainTab () {
        if(!hasBeenTriggered || allowRestart) {
            FindObjectOfType<DialogueManager>().StartDialogue(dialogue, dialogueTextBox, true);
            hasBeenTriggered = true;
            Debug.Log("trigger Dialogue of buttom with name: " + this.name);
        }
    }
  
}
