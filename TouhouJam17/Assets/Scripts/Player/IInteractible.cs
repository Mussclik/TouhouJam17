using UnityEngine;

public abstract class InteractableBase : MonoBehaviour
{
    protected bool isHeld = false;

    public virtual void OnPointerDown()
    {
        Debug.Log("Pointer Down");
        isHeld = true;
    }
    public virtual void OnPointerUp()
    {
        WorldToUI.PlaceUIOverWorld(transform.position, WorldToUI.InteractSymbol.GetComponent<RectTransform>());
        WorldToUI.InteractSymbol.gameObject.SetActive(true);
        Debug.Log("Pointer Up");
        isHeld = false;
    }

    public virtual void OnPointerEnter()
    {
        WorldToUI.PlaceUIOverWorld(transform.position, WorldToUI.InteractSymbol.GetComponent<RectTransform>());
        WorldToUI.InteractSymbol.gameObject.SetActive(true);

        Debug.Log("Pointer Entered");
    }

    public virtual void OnPointerExit()
    {
        WorldToUI.InteractSymbol.gameObject.SetActive(false);
        Debug.Log("Pointer Exited");
    }
}