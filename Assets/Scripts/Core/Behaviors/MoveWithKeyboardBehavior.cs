﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Input Keys
public enum InputKeyboard{
    arrows =0, 
    wasd = 1
}
public class MoveWithKeyboardBehavior : AgentBehaviour
{
    public InputKeyboard inputKeyboard; 

    public override Steering GetSteering()
    {
        Steering steering = new Steering();
        //implement your code here
        if(inputKeyboard == InputKeyboard.arrows) {
            float horizontal = Input.GetAxis("HorizontalPlayer1");
            float vertical = Input.GetAxis ("VerticalPlayer1");

            steering.linear = new Vector3(-horizontal, 0, -vertical)* agent.maxAccel ;
            steering.linear = this.transform.parent.TransformDirection(Vector3.ClampMagnitude(steering.
            linear , agent.maxAccel)) ;
        } else if(inputKeyboard == InputKeyboard.wasd) {
            float horizontal = Input.GetAxis ("HorizontalPlayer2");
            float vertical = Input.GetAxis ("VerticalPlayer2");

            steering.linear = new Vector3(-horizontal, 0, -vertical)* agent.maxAccel ;
            steering.linear = this.transform.parent.TransformDirection(Vector3.ClampMagnitude(steering.
            linear , agent.maxAccel)) ;
        }
       
        return steering;
    }

}