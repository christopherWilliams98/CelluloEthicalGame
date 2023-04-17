using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interact : AgentBehaviour
{
    public bool wantsInteraction = false;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected void OnTouchBegan(int key)
    {
        Debug.Log("Touch began on key " + key);
        wantsInteraction = true;
    }

}
