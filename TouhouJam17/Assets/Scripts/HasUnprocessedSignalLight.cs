using System;
using UnityEngine;

public class HasUnprocessedSignalLight : MonoBehaviour
{
    [SerializeField] private BaseStation baseStation;
    [SerializeField] private MeshRenderer myMeshRenderer;
    [SerializeField] private Light myLight = null;
    private static bool globalIsLoaded = false;
    private static Material globalGlowyGreen;
    private static Material globalNotGlowyGreen;
    private bool isEnabled = false;

    private void Start()
    {
        if (globalIsLoaded == false)
        {        
            globalGlowyGreen = Resources.Load<Material>("Materials/Green");
            globalNotGlowyGreen = Resources.Load<Material>("Materials/Green1");
        }

        isEnabled = !isEnabled;
    }

    private void Update()
    {
        if (baseStation.HasUnprocessedSignal)
        {
            Activate();
        }
        else
        {
            Deactivate();
        }
    }

    private void Activate()
    {
        if (!isEnabled)
        {
            myMeshRenderer.material = globalGlowyGreen;
            isEnabled = true;
            myLight.enabled = true;
        }
    }

    private void Deactivate()
    {
        if (isEnabled)
        {
            myMeshRenderer.material = globalNotGlowyGreen;
            isEnabled = false;
            myLight.enabled = false;
        }
    }
}