using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class JumpButton : MonoBehaviour, IPointerDownHandler
{
    private PlayerMovementController jumpVariable;
    
    private void Start()
    {
        jumpVariable = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovementController>();
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        jumpVariable.Jump();
    }
}
