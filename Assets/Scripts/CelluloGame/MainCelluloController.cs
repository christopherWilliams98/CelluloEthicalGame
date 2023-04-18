using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCelluloController : MonoBehaviour
{
    public bool wantsInteraction = false;
    private bool playerMenuEnabled = true;

    public CelluloAgent agent;

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
            display_menu();
        }
    }

    // Disable stat menu when interacting with pads
    public void OnTriggerEnter(Collider other)
    {
        playerMenuEnabled = false;
        makeOneGreenOneRed();
    }

    // Enable stat menu when leaving pads
    public void OnTriggerExit(Collider other)
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
        /*
         GameObject _leds = agent.transform.Find("Leds").gameObject;
        agent.SetVisualEffect(VisualEffect.VisualEffectConstSingle, Color.green, 255);
        _leds.transform.GetChild(i).gameObject.GetComponent<Renderer>().materials[0].color = color;
*/
    }

}
