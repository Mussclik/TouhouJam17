using UnityEngine;

public class BaseStation : MonoBehaviour
{

    
    [Header("BaseStation")]
    [SerializeField] private Transform idealCameraPos;
    public Transform IdealCameraPos => idealCameraPos;
    [SerializeField] public bool canFocus = false;
    [SerializeField] private BaseStation stationToSendSignalTo;
    
    [Header("StoredSignals")]
    [SerializeField] protected int unprocessedSignals = 0;
    [SerializeField] protected int processedSignals = 0;
    
    public bool HasUnprocessedSignal => unprocessedSignals >= 1;
    public bool HasProcessedSignal => processedSignals >= 1;
    
    public void RecieveSignal()
    {
        ++unprocessedSignals;
        CheckStatus();
    }

    public virtual void ProcessSignal()
    {
        if (unprocessedSignals > 0)
        {
            --unprocessedSignals;
            ++processedSignals;
        }
    }
    
    public virtual void SendSignal()
    {
        if (HasProcessedSignal)
        {
            if (stationToSendSignalTo)
            {
                stationToSendSignalTo.RecieveSignal();
            }
            --processedSignals;
            CheckStatus();
        }
    }

    public void CheckStatus()
    {
        if (unprocessedSignals > 0 && processedSignals <= 0)
        {
            //disable light
            //disable interactables
        }
        else
        {
            //enable light
            //enable interactables
        }
    }
    
    
}
