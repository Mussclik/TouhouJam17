using System;
using UnityEngine;

public class StationButton : InteractableBase
{
    public event Action OnActivated = () => { };
    public override void OnPointerDown()
    {
        base.OnPointerDown();
        OnActivated.Invoke();
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
