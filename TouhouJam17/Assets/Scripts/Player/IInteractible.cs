using UnityEngine;

public abstract class InteractableBase : MonoBehaviour
{
    protected bool isHeld = false;

    public virtual void OnPointerDown()
    {
        
    }
    public virtual void OnPointerUp()
    {
        WorldToUI.PlaceUIOverWorld(transform.position, WorldToUI.InteractSymbol.GetComponent<RectTransform>());
        WorldToUI.InteractSymbol.gameObject.SetActive(true);;
    }

    public virtual void OnPointerEnter()
    {
        WorldToUI.PlaceUIOverWorld(transform.position, WorldToUI.InteractSymbol.GetComponent<RectTransform>());
        WorldToUI.InteractSymbol.gameObject.SetActive(true);
    }

    public virtual void OnPointerExit()
    {
        WorldToUI.InteractSymbol.gameObject.SetActive(false);
    }
}