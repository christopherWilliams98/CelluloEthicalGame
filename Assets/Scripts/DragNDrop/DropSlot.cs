using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DropSlot : MonoBehaviour, IDropHandler {
   public DragDrop droppedChoice;

   private bool isEmptySlot = true;
   public void OnDrop(PointerEventData eventData){
        //Debug.Log("OnDrop");
        
        if(eventData.pointerDrag != null) {
            eventData.pointerDrag.GetComponent<RectTransform>().position = GetComponent<RectTransform>().position;
            droppedChoice = eventData.pointerDrag.GetComponent<DragDrop>();
            isEmptySlot = false;
        }  
   }

   public void emptyTheSlot(){
        isEmptySlot = true;
   }

   public bool isEmpty(){
    return isEmptySlot;
   }
}
