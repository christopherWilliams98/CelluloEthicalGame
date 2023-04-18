using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    public CelluloGameController gameController;


    
    
    private void Update()
    {
        // Check if player wants to interact with the pad
        if(triggerActive && (Input.GetKeyDown(KeyCode.Space)))
        {
            Interact();
            
        }
    }

    // Activate pad interaction when a player enters its range
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            triggerActive = true;
        }
    }

    // Deactivate pad interaction when a player leaves its range
    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            triggerActive = false;
        }
    }

    // Teleport player to the desired house or return to the main map
    public void Interact()
    {
        Debug.Log("Talking with " + gameObject.name + "");

        gameController.enableDialogueBox(true);
        gameController.lockInChoice();

        triggerActive = false;
        RectTransform droneImage = GameObject.Find("DroneImage").GetComponent<RectTransform>();

        if(this.gameObject.name == "ReturnPad")
        {
            // Enable all other house pads
            allPads.SetActive(true);

            // Disable the return pad
            returnPad.SetActive(false);
            gameController.enableDialogueBox(false);
            // Move drone image back to the main map
            droneImage.anchoredPosition3D = new Vector3(0, 0, 0);
            droneImage.sizeDelta = new Vector2(75,75);
            // Exit current location
            foreach(Transform house in GameObject.Find("HouseMaps").transform)
            {
                if(house.gameObject.activeSelf == true){
                    house.gameObject.SetActive(false);
                }
            }
        }
        else
        {
            // Enter the desired house
            teleportLocation.SetActive(true);

            // Move drone image to the desired house
            droneImage.anchoredPosition3D = new Vector3(60f, 240f, 0);
            droneImage.sizeDelta = new Vector2(40,40);

            // Enable the return pad
            returnPad.SetActive(true);

            // Disable all other house pads
            allPads.SetActive(false);
        }
        
    }


}