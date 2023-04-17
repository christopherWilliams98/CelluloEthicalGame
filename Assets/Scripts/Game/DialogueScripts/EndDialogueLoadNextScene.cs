using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndDialogueLoadNextScene : MonoBehaviour
{
    // Start is called before the first frame update
    public DialogueManager dialogueManager;

    // Update is called once per frame
    void Update()
    {
        if(dialogueManager.finishedDialogue){
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex+ 1);
        }
    }
}
