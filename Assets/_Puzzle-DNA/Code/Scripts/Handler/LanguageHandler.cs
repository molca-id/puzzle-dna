using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class LanguageHandler : MonoBehaviour
{
    [TextArea(10, 10)] [SerializeField] string en;
    [TextArea(10, 10)] [SerializeField] string id;
    [TextArea(10, 10)] [SerializeField] string my;

    [SerializeField] UnityEvent enEvent;
    [SerializeField] UnityEvent idEvent;
    [SerializeField] UnityEvent myEvent;

    TextMeshProUGUI m_TextMeshProUGUI;

    void OnEnable()
    {
        SetContentByLanguage();
    }

    public void SetContentByLanguage()
    {
        string currLang = DataHandler.instance.GetLanguage();
        if (GetComponent<TextMeshProUGUI>() != null) 
            m_TextMeshProUGUI = GetComponent<TextMeshProUGUI>();

        switch (currLang)
        {
            case "id":
                idEvent.Invoke();
                if (GetComponent<TextMeshProUGUI>() != null)
                    m_TextMeshProUGUI.text = id;
                break;
            case "en":
                enEvent.Invoke();
                if (GetComponent<TextMeshProUGUI>() != null)
                    m_TextMeshProUGUI.text = en;
                break;
            case "my":
                myEvent.Invoke();
                if (GetComponent<TextMeshProUGUI>() != null)
                    m_TextMeshProUGUI.text = my;
                break;
        }
    }
}
