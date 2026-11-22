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
            CursorWorldInteraction.CursorToFixedCameraPlane(Camera.main, Vector3.Distance(Camera.main.transform.position, this.transform.position), out newPos);
            transform.position = newPos;
            slider.ClampHandlePosition();
        }
    }

    public override void OnPointerDown()
    {
        base.OnPointerDown();
    }

    public override void OnPointerUp()
    {
        base.OnPointerUp();
    }

    public override void OnPointerEnter()
    {
        base.OnPointerEnter();
    }

    public override void OnPointerExit()
    {
        base.OnPointerExit();
    }
}
