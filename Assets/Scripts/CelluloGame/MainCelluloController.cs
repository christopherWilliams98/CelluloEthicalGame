using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCelluloController : MonoBehaviour
{
    public CelluloAgent agent;
    public bool wantsInteraction = false;
    private bool playerMenuEnabled = true;


    public GameObject statMenu;

    private GameObject currentCollider = null;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Check if player wants to access the stat menu
        if(playerMenuEnabled && (Input.GetKeyDown(KeyCode.Space)))
        {
            display_menu();
        }
        
        // Fix the stat menu bug when return pads disappear and do not trigger OnTriggerExit
        if(!playerMenuEnabled){
            if(currentCollider == null){
                playerMenuEnabled = true;
            }
        }
    }

    // Disable stat menu when interacting with pads
    void OnTriggerEnter(Collider other)
    {
        playerMenuEnabled = false;
        currentCollider = other.gameObject;
    }

    // Enable stat menu when leaving pads
    void OnTriggerExit(Collider other)
    {
        playerMenuEnabled = true;
    }

    // Display or hide the stat menu
    private void display_menu(){
        if(statMenu.activeSelf == true){
            statMenu.SetActive(false);
        }
        else{
            statMenu.SetActive(true);
        }
    }


     // Make one Cellulo LED green and one red.
    public void makeOneGreenOneRed()
    {
        GameObject _leds = agent.transform.Find("Leds").gameObject;
        _leds.transform.GetChild(0).gameObject.GetComponent<Renderer>().materials[0].color = new Color(1.0f, 194f/255f, 10f/255f, 1f);
        _leds.transform.GetChild(3).gameObject.GetComponent<Renderer>().materials[0].color = new Color(12f/255f, 123f/255f, 220/255f, 1f);
    }

    // Resets all leds to purple
    public void reset_leds()
    {
        agent.SetVisualEffect(VisualEffect.VisualEffectConstAll, new Color(1.0f,0f,1.0f,1f), 255);
    }

    // Check if the player is pressing a Cellulo led button
    public int checkButtonPressed(){
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
