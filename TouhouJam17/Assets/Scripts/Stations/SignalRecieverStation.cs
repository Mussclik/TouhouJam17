using System;
using UnityEngine;

public class SignalRecieverStation : MonoBehaviour
{
    [field: SerializeField, Range(2, 500)] public int points = 20;
    [SerializeField] private float curveInward = 0f;
    [SerializeField] private float waveLength = 1f;
    [SerializeField] private FrequencyData goalWave;
    [SerializeField] private FrequencyData playerWave;

    [SerializeField] private StationSlider sliderFrequency;
    [SerializeField] private StationSlider sliderAmplitude;

    #region Properties
    public int Points
    {
        get { return points; }
    }
    #endregion

    private void Start()
    {
        playerWave.RandomiseValues();
        BindSlider(sliderAmplitude, () => playerWave.myAmplitude, playerWave.SetAmplitude);
        BindSlider(sliderFrequency, () => playerWave.myFrequency, playerWave.SetFrequency);
    }

    void Update()
    {
        float t = Time.deltaTime;
        goalWave.Update(t);
        playerWave.Update(t);
        goalWave.Render(points, waveLength, curveInward);
        playerWave.Render(points, waveLength, curveInward);
    }
    
    public void BindSlider(StationSlider aSlider, Func<float> aGetter, Action<float> aSetter, float aClampMin = 0, float aClampMax = 1)
    {
        if (aSlider == null || aGetter == null || aSetter == null)
        {
            Debug.LogError($"BindSlider was given a null Value! {aSlider != null}, {aSlider != null}, {aSetter != null}", gameObject);
            return;
        }
        aSlider.SetHandlePosition(Mathf.Clamp(aGetter(), aClampMin, aClampMax)) ;
        aSlider.onChangePercent += (aVal) => aSetter(aVal);

    }
}

[Serializable]
public class FrequencyData
{
    public static readonly Vector2 MinMaxFrequency = new Vector2(1f, 10f);
    public static readonly Vector2 MinMaxSpeed = new Vector2(0.1f, 10f);
    public static readonly Vector2 MinMaxAmplitude = new Vector2(5f, 120f);
    public static readonly Vector2 MinMaxLeniency = new Vector2(0.1f, 10f);

    [SerializeField] private LineRenderer myLineRenderer = null;
    [SerializeField] public float myFrequency = 1f;
    [SerializeField] public float myAmplitude = 1f;
    [SerializeField] public float myWaveSpeed = 1f;
    [SerializeField] private float myLeniency = 0.2f;
    [SerializeField] private float myDeltaTime = 0f;


    public void SetFrequency(float aT)
    {
        myFrequency = ((MinMaxFrequency.y - MinMaxFrequency.x) * aT) + MinMaxFrequency.x;
        myWaveSpeed = ((MinMaxSpeed.y - MinMaxSpeed.x) * aT) + MinMaxSpeed.x;
    }    
    public void SetAmplitude(float aT)
    {
        
        myAmplitude = ((MinMaxAmplitude.y - MinMaxAmplitude.x) * aT) + MinMaxAmplitude.x;
    }    
    
    public void RandomiseValues()
    {
        
        SetFrequency(UnityEngine.Random.Range(0.0f, 1.0f));
        SetAmplitude(UnityEngine.Random.Range(0.0f, 1.0f));
        myLeniency = UnityEngine.Random.Range(MinMaxLeniency.x, MinMaxLeniency.y); //determine with difficulty
    }

    public void Update(float aDeltaTime)
    {
        myDeltaTime += aDeltaTime * myWaveSpeed;
        myDeltaTime = Mathf.Repeat(myDeltaTime, Mathf.PI * 2f);
    }

    public void Render(int aPoints, float aWaveLength, float aCurveInward)
    {
        aPoints = Mathf.Max(aPoints, 2);
        myLineRenderer.useWorldSpace = false;
        myLineRenderer.positionCount = aPoints;

        Transform tr = myLineRenderer.transform;

        for (int i = 0; i < aPoints; i++)
        {
            float t = i / (float)(aPoints - 1); // 0..1 along the wave
            float z = Mathf.Abs(t - 0.5f) * aCurveInward;
            // Local-space point
            float x = t * aWaveLength;
            float y = Mathf.Sin(t * myFrequency * Mathf.PI * 2f + myDeltaTime) * myAmplitude;
            x -= aWaveLength * 0.5f;
            
            Vector3 localPoint = new Vector3(x, y, z);
            
            myLineRenderer.SetPosition(i, localPoint);
        }
    }
}