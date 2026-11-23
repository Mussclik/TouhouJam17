using UnityEngine;

public abstract class InteractableBase : MonoBehaviour
{
    protected bool isHeld = false;
    public bool isEnabled = true;
    [SerializeField] protected bool isLogging = false;
    
    public virtual void OnPointerDown()
    {
        if(isLogging) Debug.Log("Pointer Down");
        isHeld = true;
    }
    public virtual void OnPointerUp()
    {
        WorldToUI.PlaceUIOverWorld(transform.position, WorldToUI.InteractSymbol.GetComponent<RectTransform>());
        WorldToUI.InteractSymbol.gameObject.SetActive(true);
        if(isLogging) Debug.Log("Pointer Up");
        isHeld = false;
    }

    public virtual void OnPointerEnter()
    {
        WorldToUI.PlaceUIOverWorld(transform.position, WorldToUI.InteractSymbol.GetComponent<RectTransform>());
        WorldToUI.InteractSymbol.gameObject.SetActive(true);

        if(isLogging) Debug.Log("Pointer Entered");
    }

    public virtual void OnPointerExit()
    {
        WorldToUI.InteractSymbol.gameObject.SetActive(false);
        if(isLogging) Debug.Log("Pointer Exited");
    }
}