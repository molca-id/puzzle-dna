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

    [Header("Audio Attributes")]
    [SerializeField] Slider bgmSlider;
    [SerializeField] Slider sfxSlider;
    
    AudioMixer bgmAudioMixer;
    AudioMixer sfxAudioMixer;

    private void Awake()
    {
        bgmAudioMixer = DataHandler.instance.bgmAudioMixer;
        sfxAudioMixer = DataHandler.instance.sfxAudioMixer;
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
        LangEnButton.SetActive(true);

        string currLang = DataHandler.instance.GetLanguage();
        switch (currLang)
        {
            case "id":
                LangEnButton.SetActive(false);
                break;
            case "en":
                LangIdButton.SetActive(false);
                break;
            case "my":
                LangIdButton.SetActive(false);
                break;
        }
    }

    void SetInitVolumePanel()
    {
        bgmSlider.value = GetMasterLevel(bgmAudioMixer);
        sfxSlider.value = GetMasterLevel(sfxAudioMixer);
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

    int GetMasterLevel(AudioMixer mixer)
    {
        mixer.GetFloat("MasterVolume", out float temp);
        return Mathf.RoundToInt(temp);
    }
}
