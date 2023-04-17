using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable_Point : MonoBehaviour
{
    
    [SerializeField] private bool triggerActive = false;
    public GameObject teleportLocation;
    public GameObject allPads;
    public GameObject returnPad;


    
    
    private void Update()
    {
        if(triggerActive && (Input.GetKeyDown(KeyCode.Space)))
        {
            Interact();
            
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            triggerActive = true;

        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            triggerActive = false;
        }
    }
    public void Interact()
    {
        Debug.Log("Talking with " + gameObject.name + "");
        triggerActive = false;
        if(this.gameObject.name == "ReturnPad")
        {
            // Enable all other house pads
            allPads.SetActive(true);

            // Disable the return pad
            returnPad.SetActive(false);

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

            // Enable the return pad
            returnPad.SetActive(true);

            // Disable all other house pads
            allPads.SetActive(false);
        }
        
    }


}
