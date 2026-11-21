using System;
using UnityEngine;

public class UIInteractSymbol : MonoBehaviour
{
    private static UIInteractSymbol Instance;

    private void Start()
    {
        if (Instance)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
        gameObject.SetActive(false);
    }

    public static RectTransform GetUITransform()
    {
        return Instance.GetComponent<RectTransform>();
    }

    public static void SetVisible(bool aIsActive)
    {
        Instance.gameObject.SetActive(aIsActive);
    }
}