using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class OnMouseHover : MonoBehaviour, IPointerEnterHandler 
{
    
    public UnityEvent OnMouseOver = new UnityEvent();
    public void OnPointerEnter(PointerEventData eventData)
    {
        OnMouseOver.Invoke();
    }
    

}