using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class MenuFuncs : MonoBehaviour
{
    [SerializeField] private AudioMixerGroup musicGroup;
    public void Start()
    {
        AudioSource source = Audio.Sound.PlayFromResource("Music/A World of Nightmares Never Seen Before rearrangement wowww", Vector3.zero, false, aPitchShift: false, aMixerGroup:musicGroup);
        source.loop = true;
    }

    public void LoadMainScene(int aSceneID)
    {
        SceneManager.LoadScene(aSceneID);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
