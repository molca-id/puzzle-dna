using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingHandler : MonoBehaviour
{
    [SerializeField] GameObject settingPanel;
    [SerializeField] GameObject LangIdButton;
    [SerializeField] GameObject LangEnButton;
    [SerializeField] AudioMixer bgmAudioMixer;
    [SerializeField] AudioMixer sfxAudioMixer;

    public void OpenSettingPanel()
    {
        settingPanel.SetActive(true);
        LangIdButton.SetActive(true);
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
        }
    }

    public void SelectLanguage(string lang)
    {
        MainMenuHandler.instance.SelectLanguage(lang);
        DataHandler.instance.RefreshAllTextLanguage();
        OpenSettingPanel();
    }
}
