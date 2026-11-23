using System;
using TMPro;
using UnityEngine;

public class StationKeypadButton : InteractableBase
{
    public static Material glowyGreen;
    public static Material mundaneRed;
    [SerializeField] private MeshRenderer interactablePart;
    [SerializeField] private TextMeshPro myText;
    public event Action OnActivated = () => { };
    
    void Start()
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
    
    public override void OnPointerDown()
    {
        base.OnPointerDown();
        OnActivated.Invoke();
    }

    public void SetEnabled()
    {
        if (isEnabled) return;
        
        isEnabled = true;
        interactablePart.material = glowyGreen;
    }

    public void SetDisabled()
    {
        if (!isEnabled) return;
        
        isEnabled = false;
        interactablePart.material = mundaneRed;
    }
    
    public void SetSymbol(string newSymbol)
    {
        myText.text = newSymbol;
    }
}
