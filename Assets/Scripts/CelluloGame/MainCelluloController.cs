using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCelluloController : MonoBehaviour
{
    public CelluloAgent agent;
    public bool wantsInteraction = false;
    private bool playerMenuEnabled = true;

    public GameObject statMenu;

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
           // display_menu();
        }
        
    }


    // Toggle the player menu
    void TogglePlayerMenuEnabled(){
        playerMenuEnabled = !playerMenuEnabled;
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
    public void applyChoiceSelectionColors()
    {
        GameObject _leds = agent.transform.Find("Leds").gameObject;
        agent.SetVisualEffect(VisualEffect.VisualEffectConstSingle, new Color(230f/255f, 97f/255f, 0/255f, 1f), 0);
        //_leds.transform.GetChild(0).gameObject.GetComponent<Renderer>().materials[0].color = new Color(1.0f, 194f/255f, 10f/255f, 1f);
        //_leds.transform.GetChild(3).gameObject.GetComponent<Renderer>().materials[0].color = new Color(12f/255f, 123f/255f, 220/255f, 1f);
    }

    // Resets all leds to purple
    public void reset_leds()
    {
        agent.SetVisualEffect(VisualEffect.VisualEffectConstAll, new Color(0.0f,0f,0.0f,0f), 255);
    }

    // Sets LEDs to white
    public void set_leds_white()
    {
        agent.SetVisualEffect(VisualEffect.VisualEffectConstAll, new Color(1.0f,1.0f,1.0f,1f), 255);
    }

    public void set_leds_green(){
        agent.SetVisualEffect(VisualEffect.VisualEffectConstAll, new Color(0.0f,1.0f,0.0f,1f), 255);
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
