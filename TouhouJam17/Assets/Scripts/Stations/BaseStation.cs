using UnityEngine;

public class BaseStation : MonoBehaviour
{

    
    [Header("BaseStation")]
    [SerializeField] private Transform idealCameraPos;
    [SerializeField] public bool canFocus = false;
    
    [Header("StoredSignals")]
    [SerializeField] protected int unprocessedSignals = 0;
    [SerializeField] protected int processedSignals = 0;
    
    protected bool HasSignal => unprocessedSignals >= 1;
    protected bool HasProcessedSignal => processedSignals >= 1;
    
    public virtual void RecieveSignal()
    {
        ++unprocessedSignals;
        //enable screen 
        //enable light
        //enable interactables
    }
    public virtual void SendSignal()
    {
        if (HasProcessedSignal)
        {
            --processedSignals;
        }
    }
    
    public Transform IdealCameraPos
    {
        get => idealCameraPos;
    }
}
