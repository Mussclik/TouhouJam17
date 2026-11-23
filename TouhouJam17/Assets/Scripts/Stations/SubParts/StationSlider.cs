using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class StationSlider : MonoBehaviour
{
    public bool hasStarted { get; private set; }
    [SerializeField] private float minValue, maxValue;
    [SerializeField] private Transform handle, line;
    private Vector3 startPoint, endPoint;
    private float lastT = 0f;
    public float snapPoints = 20;
    public event Action<float> onChangePercent = (scrap) => { };

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Start()
    {
        if (!hasStarted)
        {
            float length = line.lossyScale.x;
            startPoint = line.TransformPoint(line.right * length * 0.5f);
            endPoint = line.TransformPoint((line.right * -1) * length * 0.5f);
            SetHandlePosition(Random.Range(0f, 1f));
        }
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void ClampHandlePosition()
    {
        handle.position = handle.position.ClosestPointOnLine(startPoint, endPoint, out float t);
        SetHandlePosition(t, out float newT);
        if (!Mathf.Approximately(lastT, newT))
        {
            onChangePercent(newT);
        }
    }

    public void SetHandlePosition(float aTime) => SetHandlePosition(aTime, out _);
    

    public void SetHandlePosition(float aTime, out float aNewTime)
    {
        aTime = Mathf.Clamp01(aTime);

        float snappedT;

        if (snapPoints > 0f)
        {
            snappedT = Mathf.Round(aTime * snapPoints) / snapPoints;
        }
        else
        {
            snappedT = aTime;
        }

        handle.position = Vector3.Lerp(startPoint, endPoint, snappedT);

        aNewTime = snappedT;
        onChangePercent(snappedT);
    }

    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(startPoint, 0.2f);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(endPoint, 0.2f);
    }
}