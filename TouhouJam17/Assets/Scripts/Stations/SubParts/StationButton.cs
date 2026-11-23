using System;
using UnityEngine;
using UnityEngine.Serialization;

public class StationButton : InteractableBase
{
    public static Material glowyGreen;
    public static Material mundaneRed;
    
    
    public event Action OnActivated = () => { };
    [FormerlySerializedAs("light")] [SerializeField] private Light myLight;
    [SerializeField] private MeshRenderer interactablePart;
    
    public override void OnPointerDown()
    {
        base.OnPointerDown();
        OnActivated.Invoke();
    }

    public void SetEnabled()
    {
        if (isEnabled) return;
        
        isEnabled = true;
        myLight.enabled = true;
        interactablePart.material = glowyGreen;
    }

    public void SetDisabled()
    {
        if (!isEnabled) return;
        
        isEnabled = false;
        myLight.enabled = false;
        interactablePart.material = mundaneRed;
    }

    public void Start()
    {
        //SetMaterials
        if (!mundaneRed)
        {
            mundaneRed = Resources.Load<Material>("Materials/Red");
        }

        if (!glowyGreen)
        {
            glowyGreen = Resources.Load<Material>("Materials/Green");
        }

        //make sure correct visual apply.
        if (isEnabled)
        {
            isEnabled = false;
            SetEnabled();
        }
        else
        {
            isEnabled = true;
            SetDisabled();
        }
    }
}
