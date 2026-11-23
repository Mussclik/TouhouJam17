using System;
using System.Collections;
using UnityEngine;

public class AlarmLight : MonoBehaviour
{
    public static AlarmLight Instance;
    [SerializeField] private GameObject lights = null;
    [SerializeField] private int alarmPlayTimes = 5;
    [SerializeField] private float rotationSpeed = 20f;
    private Coroutine myAlarmCoroutine = null;
    private AudioSource currentAS = null;

    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
            lights.SetActive(false);
        }
        else
        {
            Debug.Log("deleted self :3");
            Destroy(this);
        }
    }

    public static void PlayAlarm(AlarmType aType, int aPlayTimes = 5)
    {
        if (Instance.myAlarmCoroutine == null)
        {
            Instance.myAlarmCoroutine = Instance.StartCoroutine(AlarmCoroutine(aType));
        }
    }

    public static void ForceStopAlarm()
    {
        Instance.StopActiveAlarm();
    }

    private static IEnumerator AlarmCoroutine(AlarmType aType, Func<bool> aPlayUntilFalse = null)
    {
        int i = 0;
        Instance.lights.SetActive(true);
        AudioSource audioSource = null;

        while (LocalShouldContinuePlaying(i))
        {
            if (!audioSource || audioSource.isPlaying == false)
            {
                audioSource = Audio.Sound.PlayFromResource(GetAlarmFromEnum(aType), Instance.transform.position, aPitchShift: false, aVolume: 0.7f);
                ++i;
            }

            Instance.transform.rotation = Quaternion.Euler
            (
                new Vector3
                (
                    Instance.transform.eulerAngles.x,
                    Instance.transform.eulerAngles.y + (Instance.rotationSpeed * Time.deltaTime),
                    Instance.transform.eulerAngles.z
                )
            );

            yield return null;
        }

        audioSource?.Stop();
        Instance.lights.SetActive(false);
        Instance.myAlarmCoroutine = null;


        bool LocalShouldContinuePlaying(int aI)
        {
            if (aPlayUntilFalse != null)
            {
                return aPlayUntilFalse.Invoke();
            }
            else
            {
                return aI < Instance.alarmPlayTimes;
            }
        }
    }

    private void StopActiveAlarm()
    {
        if (myAlarmCoroutine != null)
        {
            StopCoroutine(myAlarmCoroutine);
            myAlarmCoroutine = null;
        }

        CleanupAlarm();
    }

    private void CleanupAlarm()
    {
        if (currentAS)
        {
            currentAS = null;
        }

        lights.SetActive(false);
    }

    private static string GetAlarmFromEnum(AlarmType aType)
    {
        switch (aType)
        {
            case AlarmType.Mimi:
                return "UtsuhoAlarm";
            case AlarmType.MazeFail:
                return "Alarm";

            default:
                return "Alarm";
        }
    }
}

public enum AlarmType
{
    Mimi,
    MazeFail,
}