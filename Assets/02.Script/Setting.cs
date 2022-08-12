using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class Setting : Singleton<Setting>
{
    [SerializeField]
    AudioMixerGroup masterMixer;
    public Slider[] soundSlider;
    [SerializeField]
    GameObject settingUI;

    void Start()
    {
        DontDestroyOnLoad(this);
        AudioInit();
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            OnSetting();
    }

    public void OnSetting()
    {
        if (settingUI.activeSelf)
            settingUI.SetActive(false);
        else
            settingUI.SetActive(true);
    }

    void AudioInit()
    {
        masterMixer = AudioManager.Instance.GetComponent<AudioSource>().outputAudioMixerGroup;
        for (int i = 0; i < soundSlider.Length; i++)
        {
            int count = i;
            soundSlider[i].onValueChanged.AddListener((float value) => AudioControl(count));
        }
    }


    void AudioControl(int count)
    {
        float sound = 0;
        string stringTarget = "";

        switch (count)
        {
            case 0:
                sound = soundSlider[0].value;
                stringTarget = "Music";
                break;
            case 1:
                sound = soundSlider[1].value;
                stringTarget = "Eff";
                break;
            default:
                break;
        }

        if (sound == -40f)
            masterMixer.audioMixer.SetFloat(stringTarget, -80);
        else
            masterMixer.audioMixer.SetFloat(stringTarget, sound);
    }
}
