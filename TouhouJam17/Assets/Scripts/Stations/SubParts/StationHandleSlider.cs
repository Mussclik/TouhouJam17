using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class StationHandleSlider : InteractableBase
{
    [SerializeField] private StationSlider slider;
    
    public void Update()
    {
        if (isHeld)
        {
            Vector3 newPos;
            CursorWorldInteraction.CursorToPlane(Camera.main, Vector3.Distance(Camera.main.transform.position, this.transform.position), out newPos);
            transform.position = newPos;
            slider.ClampHandlePosition();
        }
    }

    public override void OnPointerDown()
    {
        base.OnPointerDown();
        Debug.Log("Pointer Down");
        isHeld = true;
    }

    public override void OnPointerUp()
    {
        base.OnPointerUp();
        Debug.Log("Pointer Up");
        isHeld = false;
    }

    public override void OnPointerEnter()
    {
        base.OnPointerEnter();
        Debug.Log("Pointer Entered");
    }

    public override void OnPointerExit()
    {
        base.OnPointerExit();
        Debug.Log("Pointer Exitted");
    }
}
