using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class StationSlider : MonoBehaviour
{
    [SerializeField] private float minValue, maxValue;
    [SerializeField] private Transform handle, line;
    private Vector3 startPoint, endPoint;
    private float lastT = 0f;

    public event Action<float> onChangePercent = (scrap) => { };

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        float length = line.lossyScale.x;
        startPoint = line.TransformPoint(line.right * length * 0.5f);
        endPoint = line.TransformPoint((line.right * -1) * length * 0.5f);
        SetHandlePosition(Random.Range(minValue, maxValue));
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void ClampHandlePosition()
    {
        handle.position = handle.position.ClosestPointOnLine(startPoint, endPoint, out float t);
        if (!Mathf.Approximately(lastT, t))
        {
            onChangePercent(t);
        }
    }

    public void SetHandlePosition(float aT)
    {
        handle.position = Vector3.Lerp(startPoint, endPoint, aT);
        onChangePercent(aT);
    }

    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(startPoint, 0.2f);        
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(endPoint, 0.2f);
    }
}