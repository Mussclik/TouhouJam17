using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuFuncs : MonoBehaviour
{
    [Header("Sliders (linear amplitude 0..1)")]
    [SerializeField] private Slider sliderMaster;
    [SerializeField] private Slider sliderGeneral;
    [SerializeField] private Slider sliderMusic;

    [Header("Mixer Groups (exposed params named '<GroupName>Volume')")]
    [SerializeField] private AudioMixerGroup master;
    [SerializeField] private AudioMixerGroup general;
    [SerializeField] private AudioMixerGroup music;
    [SerializeField] private bool isMainMenu = false;
    
    public void Start()
    {
        if (isMainMenu)
        {
            AudioSource source = Audio.Sound.PlayFromResource("Music/A World of Nightmares Never Seen Before rearrangement wowww", Vector3.zero, false, aPitchShift: false, aMixerGroup: music, aStoppable:true);
            source.loop = true;
        }
        
        InitSlider(sliderMaster, master);
        InitSlider(sliderGeneral, general);
        InitSlider(sliderMusic, music);

        // 2) Wire up live updates (added AFTER init so we don't double-fire)
        sliderMaster.onValueChanged.AddListener(v => SetGroupVolume(master, v));
        sliderGeneral.onValueChanged.AddListener(v => SetGroupVolume(general, v));
        sliderMusic.onValueChanged.AddListener(v => SetGroupVolume(music, v));
    }

    public void OnEnable()
    {
        Time.timeScale = 0.001f;
    }

    public void OnDisable()
    {
        Time.timeScale = 1f;
    }

    public void LoadMainScene(int aSceneID)
    {
        SceneManager.LoadScene(aSceneID);
    }

    public void Quit()
    {
        Application.Quit();
    }
    
    private void OnDestroy()
    {
        sliderMaster.onValueChanged.RemoveAllListeners();
        sliderGeneral.onValueChanged.RemoveAllListeners();
        sliderMusic.onValueChanged.RemoveAllListeners();
    }
    
    // dB = Log10(amp) * 20   =>   amp = 10^(dB / 20)
    private void InitSlider(Slider slider, AudioMixerGroup group)
    {

        string param = group.name + "Volume";

        if (group.audioMixer.GetFloat(param, out float db))
        {
            slider.SetValueWithoutNotify(Mathf.Clamp(Mathf.Pow(10f, db / 20f), 0f, 1f));
        }
    }
    
    private void SetGroupVolume(AudioMixerGroup group, float linearValue)
    {
        if (group == null || group.audioMixer == null) return;

        // Log10(0) is -inf; clamp to ~-80 dB
        float amp = Mathf.Max(linearValue, 0.0001f);
        float db  = Mathf.Log10(amp) * 20f;

        group.audioMixer.SetFloat(group.name + "Volume", db);
    }
}
