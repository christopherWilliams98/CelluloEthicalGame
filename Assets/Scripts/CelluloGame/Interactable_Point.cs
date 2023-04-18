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

    public CelluloAgent agent;

    public CelluloGameController gameController;

    private string temp;


    
    
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
                    textBox.text = "Return to main map";
                    break;

                case "bird_reservoir_choice_1":
                    textBox.text = "Make drone goodn't? 1: Press green to Accept, red to decline the decision";
                    makeOneGreenOneRed();
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
            reset_leds();

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
            // Enable all other house pads
            allPads.SetActive(true);

            // Disable the return pad
            returnPad.SetActive(false);
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
            // Enter the desired location
            teleportLocation.SetActive(true);

            if(this.gameObject.transform.parent.name == "HousePads"){
                gameController.lockInChoice();
            }
            // Move drone image to the desired house
            droneImage.anchoredPosition3D = new Vector3(60f, 240f, 0);
            droneImage.sizeDelta = new Vector2(40,40);

            // Enable the return pad
            returnPad.SetActive(true);

            // Disable all other house pads
            allPads.SetActive(false);
        }
        
    }

    // Make one Cellulo LED green and one red.
    public void makeOneGreenOneRed()
    {
        GameObject _leds = agent.transform.Find("Leds").gameObject;
        _leds.transform.GetChild(1).gameObject.GetComponent<Renderer>().materials[0].color = Color.green;
        _leds.transform.GetChild(2).gameObject.GetComponent<Renderer>().materials[0].color = Color.red;
    }
    //resets all leds to purple
    public void reset_leds()
    {
        agent.SetVisualEffect(VisualEffect.VisualEffectConstAll, new Color(255,0,266,255), 255);
    }

    // Check if the player is pressing a Cellulo led button
    int checkButtonPressed(){
        Cellulo robot = agent._celluloRobot;
        if(robot == null){
            return -1;
        }

        for(int i = 0; i < 6; i++){
            if(robot.TouchKeys[i] == Touch.LongTouch){
                return i;
            }
        }
        return -1;
    }
}
