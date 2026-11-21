using System;
using UnityEngine;
using UnityEngine.UI;

public class SignalRecieverStation : MonoBehaviour
{
    public static SignalRecieverStation Instance;

    [Header("MinMaxValuesStatic")] 
    [SerializeField] public Vector2 MinMaxFrequency = new Vector2(1f, 8f);
    [SerializeField] public Vector2 MinMaxXOffset = new Vector2(-1f, 1f);
    [SerializeField] public Vector2 MinMaxAmplitude = new Vector2(5f, 120f);
    [SerializeField] public Vector2 MinMaxLeniency = new Vector2(0.1f, 10f);

    [Header("WaveProperties")] 
    [field: SerializeField, Range(2, 500)] public int points = 20;
    [field: SerializeField, Range(2, 30)] public int snapPoints = 20;
    [SerializeField] private float curveInward = 0f;
    [SerializeField] private float waveLength = 1f;
    [SerializeField] private float waveSpeed = 1f;
    [SerializeField] private float waveTime = 0f;
    [SerializeField] private FrequencyData goalWave;
    [SerializeField] private FrequencyData playerWave;
    
    [Header("Refs")] 
    [SerializeField] private StationSlider sliderFrequency;
    [SerializeField] private StationSlider sliderAmplitude;
    [SerializeField] private StationSlider sliderXOffset;
    [SerializeField] private Image progressFillImage;

    #region Properties

    public int Points
    {
        get { return points; }
    }

    #endregion

    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Debug.Log("deleted self :3");
            Destroy(this);
        }
    }

    private void Start()
    {
        playerWave.RandomiseValues();
        goalWave.RandomiseValues();
        BindSlider(sliderFrequency, () => playerWave.myFrequency, (aTime) => playerWave.SetVar(ref playerWave.myFrequency, MinMaxFrequency, aTime, snapPoints));
        BindSlider(sliderXOffset, () => playerWave.myXOffset, (aTime) => playerWave.SetVar(ref playerWave.myXOffset, MinMaxXOffset, aTime, snapPoints));
        BindSlider(sliderAmplitude, () => playerWave.myAmplitude, (aTime) => playerWave.SetVar(ref playerWave.myAmplitude, MinMaxAmplitude, aTime, snapPoints));
    }

    void Update()
    {
        waveTime += Time.deltaTime * waveSpeed;
        waveTime = Mathf.Repeat(waveTime, Mathf.PI * 2f);

        goalWave.Render(points, waveLength, curveInward, waveTime);
        playerWave.Render(points, waveLength, curveInward, waveTime);

        float closeness = 1 * playerWave.CompareCloseness(goalWave);
        progressFillImage.fillAmount = closeness;
    }

    public void BindSlider(StationSlider aSlider, Func<float> aGetter, Action<float> aSetter, float aClampMin = 0, float aClampMax = 1)
    {
        if (aSlider == null || aGetter == null || aSetter == null)
        {
            Debug.LogError($"BindSlider was given a null Value! {aSlider != null}, {aSlider != null}, {aSetter != null}", gameObject);
            return;
        }

        aSlider.snapPoints = snapPoints;
        aSlider.SetHandlePosition(Mathf.Clamp(aGetter(), aClampMin, aClampMax));
        aSlider.onChangePercent += (aVal) => aSetter(aVal);
    }
}

[Serializable]
public class FrequencyData
{
    public static Vector2 MinMaxFrequency => SignalRecieverStation.Instance.MinMaxFrequency;
    public static Vector2 MinMaxXOffset => SignalRecieverStation.Instance.MinMaxXOffset;
    public static Vector2 MinMaxAmplitude => SignalRecieverStation.Instance.MinMaxAmplitude;
    public static Vector2 MinMaxLeniency => SignalRecieverStation.Instance.MinMaxLeniency;

    [SerializeField] private LineRenderer myLineRenderer = null;
    [SerializeField] public float myFrequency = 1f;
    [SerializeField] public float myAmplitude = 1f;
    [SerializeField] public float myXOffset = 1f;
    [SerializeField] private float myLeniency = 0.2f; //20%


    public void SetVar(ref float aVarToSet, Vector2 aMinMax, float aTime, int aSnapPoints)
    {
        float snapLength = (aMinMax.y - aMinMax.x) / aSnapPoints;
        float newValue = (((aMinMax.y - aMinMax.x) * aTime) + aMinMax.x);
        float cutOff = newValue % snapLength;
        aVarToSet = newValue - cutOff;
    }

    public void RandomiseValues()
    {
        SetVar(ref myFrequency, MinMaxFrequency, UnityEngine.Random.Range(0.0f, 1.0f), SignalRecieverStation.Instance.snapPoints);
        SetVar(ref myXOffset, MinMaxXOffset, UnityEngine.Random.Range(0.0f, 1.0f), SignalRecieverStation.Instance.snapPoints);
        SetVar(ref myAmplitude, MinMaxAmplitude, UnityEngine.Random.Range(0.0f, 1.0f), SignalRecieverStation.Instance.snapPoints);
        myLeniency = UnityEngine.Random.Range(MinMaxLeniency.x, MinMaxLeniency.y); //determine with difficulty
    }
    
    public void Render(int aPoints, float aWaveLength, float aCurveInward, float aTime)
    {
        aPoints = Mathf.Max(aPoints, 2);
        myLineRenderer.useWorldSpace = false;
        myLineRenderer.positionCount = aPoints;

        for (int i = 0; i < aPoints; i++)
        {
            float t = i / (float)(aPoints - 1); // 0..1 along the wave
            float z = Mathf.Abs(t - 0.5f);
            float x = t * aWaveLength;
            float y = Mathf.Sin((t + myXOffset) * (myFrequency) * Mathf.PI * 2f + aTime) * myAmplitude;
            x -= aWaveLength * 0.5f;

            Vector3 localPoint = new Vector3(x, y, z);

            myLineRenderer.SetPosition(i, localPoint);
        }
    }

    public float CompareCloseness(FrequencyData otherData)
    {
        float freMaxDif = MinMaxFrequency.y - MinMaxFrequency.x;
        float xofMaxDif = MinMaxXOffset.y - MinMaxXOffset.x;
        float ampMaxDif = MinMaxAmplitude.y - MinMaxAmplitude.x;

        float freDif = Mathf.Abs(myFrequency - otherData.myFrequency);
        float xofDif = Mathf.Abs(myXOffset - otherData.myXOffset);
        float ampDif = Mathf.Abs(myAmplitude - otherData.myAmplitude);
        
        float frePer = 0.2f;
        float xofPer = 0.2f;
        float ampPer = 0.2f;

        if (freDif < freMaxDif)
        {
            frePer = (freMaxDif - freDif) / freMaxDif;
            frePer = Mathf.Clamp(frePer, 0.2f, 1f);
        }        
        
        if (xofDif < xofMaxDif)
        {
            xofPer = (xofMaxDif - xofDif) / xofMaxDif;
            xofPer = Mathf.Clamp(xofPer, 0.2f, 1f);
        }        
        
        if (ampDif < ampMaxDif)
        {
            ampPer = (ampMaxDif - ampDif) / ampMaxDif;
            ampPer = Mathf.Clamp(ampPer, 0.2f, 1f);
        }

        float compoundPercent = frePer * xofPer * ampPer;
        return compoundPercent;
    }
}