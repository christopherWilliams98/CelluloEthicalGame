using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragDrop : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, 
IDragHandler, IEndDragHandler {


    [SerializeField] private Canvas canvas;
    public RectTransform rectTransform;

    public int choice_id = 0; //ID to identify choice, This ID is used to and index for dialogue texts
    public Vector3 original_position;
    private CanvasGroup canvasGroup;

    private void Start()
    {

      rectTransform = GetComponent<RectTransform>();
      original_position = this.transform.position;
      canvasGroup = GetComponent<CanvasGroup>();
    }

    public void OnPointerDown(PointerEventData eventData){
      //Debug.Log("OnPointerDown");
    }

    public void OnDrag(PointerEventData eventData){
      //Debug.Log("OnDrag");
      rectTransform.anchoredPosition += eventData.delta/canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData){
      //Debug.Log("OnEndDrag");
      canvasGroup.alpha = 1f;
      canvasGroup.blocksRaycasts = true;
    }

    public void OnBeginDrag(PointerEventData eventData){
      //Debug.Log("OnBeginDrag");
      canvasGroup.alpha = .6f;
      canvasGroup.blocksRaycasts = false;
    }
}
