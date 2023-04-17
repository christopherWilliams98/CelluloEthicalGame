using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopUpScript : MonoBehaviour
{
    [SerializeField] GameObject popUpPanel;
    public void display(){
        popUpPanel.SetActive(true);
    }
    public void close(){
        popUpPanel.SetActive(false);
    }
}
