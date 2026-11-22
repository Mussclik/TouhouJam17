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

    public static void PlayAlarm()
    {
        if (Instance.myAlarmCoroutine == null)
        {
            Instance.myAlarmCoroutine = Instance.StartCoroutine(AlarmCoroutine());
        }
    }

    private static IEnumerator AlarmCoroutine()
    {
        int i = 0;
        Instance.lights.SetActive(true);
        AudioSource audioSource = null;

        while (i < Instance.alarmPlayTimes || audioSource.isPlaying == true)
        {
            if ((!audioSource || audioSource.isPlaying == false) && i < Instance.alarmPlayTimes)
            {
                audioSource = Audio.Sound.PlayFromResource("Alarm", Instance.transform.position, aVolume: 0.01f, aPitchShift: false);
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
        
        Instance.lights.SetActive(false);
        Instance.myAlarmCoroutine = null;
    }
}