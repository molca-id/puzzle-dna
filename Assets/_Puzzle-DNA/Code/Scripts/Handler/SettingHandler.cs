using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingHandler : MonoBehaviour
{
    [SerializeField] GameObject settingPanel;

    [Header("Language Attributes")]
    [SerializeField] GameObject LangIdButton;
    [SerializeField] GameObject LangEnButton;
    [SerializeField] GameObject LangMyButton;

    [Header("Audio Attributes")]
    [SerializeField] Slider bgmSlider;
    [SerializeField] Slider sfxSlider;
    [SerializeField] Slider voSlider;
    
    AudioMixer bgmAudioMixer;
    AudioMixer sfxAudioMixer;
    AudioMixer voAudioMixer;

    private void Awake()
    {
        bgmAudioMixer = DataHandler.instance.bgmAudioMixer;
        sfxAudioMixer = DataHandler.instance.sfxAudioMixer;
        voAudioMixer = DataHandler.instance.voAudioMixer;
    }

    public void OpenSettingPanel()
    {
        SetInitVolumePanel();
        SetInitLanguagePanel();
        settingPanel.SetActive(true);
    }

    public void CloseSettingPanel()
    {
        settingPanel.SetActive(false);
        DataHandler.instance.IEPatchAllVolumeData();
    }

    void SetInitLanguagePanel()
    {
        LangIdButton.SetActive(true);
        LangEnButton.SetActive(true);
        LangMyButton.SetActive(true);

        string currLang = DataHandler.instance.GetLanguage();
        switch (currLang)
        {
            case "id":
                LangIdButton.SetActive(false);
                LangEnButton.SetActive(false);
                break;
            case "en":
                LangEnButton.SetActive(false);
                LangMyButton.SetActive(false);
                break;
            case "my":
                LangMyButton.SetActive(false);
                LangIdButton.SetActive(false);
                break;
        }
    }

    void SetInitVolumePanel()
    {
        bgmSlider.value = GetMasterLevel(bgmAudioMixer);
        sfxSlider.value = GetMasterLevel(sfxAudioMixer);
        voSlider.value = GetMasterLevel(voAudioMixer);
    }

    public void SelectLanguage(string lang)
    {
        MainMenuHandler.instance.SelectLanguage(lang);
        MainMenuHandler.instance.SubmitLanguage(null);
        DataHandler.instance.RefreshAllTextLanguage();
        OpenSettingPanel();
    }

    public void SetBGMVolume(float value)
    {
        bgmAudioMixer.SetFloat("MasterVolume", Mathf.RoundToInt(value));
    }

    public void SetSFXVolume(float value)
    {
        sfxAudioMixer.SetFloat("MasterVolume", Mathf.RoundToInt(value));
    }

    public void SetVOVolume(float value)
    {
        voAudioMixer.SetFloat("MasterVolume", Mathf.RoundToInt(value));
    }

    int GetMasterLevel(AudioMixer mixer)
    {
        mixer.GetFloat("MasterVolume", out float temp);
        return Mathf.RoundToInt(temp);
    }
}
