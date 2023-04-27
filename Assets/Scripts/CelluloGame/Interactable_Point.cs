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
   
 
    void Start()
    {

    }
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
            TextMeshProUGUI textBox = GameObject.Find("main_dialog").GetComponent<TextMeshProUGUI>();
            temp = textBox.text;
            switch(this.gameObject.name){
                case "CityParkPad":
                    textBox.text = "Test drone in city park \n" + "COST: 0.5 weeks";
                    break;
                
                case "TechShopPad":
                    textBox.text = "Consult drone expert \n" + "COST: 0.5 weeks";
                    break;

                case "CityHallPad":
                    textBox.text = "Consult local council \n" + "COST: X weeks";
                    break;

                case "BirdReservoirPad":
                    textBox.text = "Consult bird reservoir director \n" + "COST: 1 week";
                    break;

                case "PostOfficePad":
                    textBox.text = "Ship finished product";
                    break;

                case "FarmPad":
                    textBox.text = "Test drone on external location \n" + "COST: 1 week";
                    break;

                case "ReturnPad":
                    textBox.text = "Return back to city";
                    break;

                default:
                /*
                    if(this.gameObject.transform.parent.name == "ChoicePads"){
                        textBox.text = "decision 1: Press green to Accept, red to decline the decision";
                        celluloController.makeOneGreenOneRed();
                        isMakingChoice = true;
                    }
                */
                    break;
            }

        }
    }

    // Deactivate pad interaction when a player leaves its range
    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameObject.Find("main_dialog").GetComponent<TextMeshProUGUI>().text = temp;
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
        RectTransform droneImage = GameObject.Find("DroneImage").GetComponent<RectTransform>();

        if(this.gameObject.name == "ReturnPad")
        {
            // Enable all other house pads, disable return pad and choice pads
            if(allPads!=null && returnPad != null && choicePads != null){
                allPads.SetActive(true);
                choicePads.SetActive(false);
                returnPad.SetActive(false);
            }
            else{
                Debug.Log("allPads or returnPad is null");
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
        else
        {
            // Enter the desired house 
            if(teleportLocation != null){
                teleportLocation.SetActive(true);
            }
            

            if(this.gameObject.transform.parent.name == "HousePads"){
                gameController.lockInChoice();
            }

            // Move drone image to the desired house
            //droneImage.anchoredPosition3D = new Vector3(60f, 240f, 0);
            //droneImage.sizeDelta = new Vector2(40,40);

            // Disable all other house pads 
            // Enable the return pad and choice pads
            if(allPads != null && returnPad != null && choicePads != null){
                allPads.SetActive(false);
                returnPad.SetActive(true);
                Debug.Log(teleportLocation.name);
                choicePads.SetActive(true);

            }
            
        }
        
    }

   


    
}
