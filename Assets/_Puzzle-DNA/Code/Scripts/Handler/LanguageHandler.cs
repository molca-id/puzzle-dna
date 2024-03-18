using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class LanguageHandler : MonoBehaviour
{
    [TextArea(3, 3)] [SerializeField] string en;
    [TextArea(3, 3)] [SerializeField] string id;
    [TextArea(3, 3)] [SerializeField] string my;

    TextMeshProUGUI m_TextMeshProUGUI;

    void OnEnable()
    {
        SetContentByLanguage();
    }

    public void SetContentByLanguage()
    {
        string currLang = DataHandler.instance.GetLanguage();
        m_TextMeshProUGUI = GetComponent<TextMeshProUGUI>();

        switch (currLang)
        {
            case "id":
                m_TextMeshProUGUI.text = id;
                break;
            case "en":
                m_TextMeshProUGUI.text = en;
                break;
            case "my":
                m_TextMeshProUGUI.text = my;
                break;
        }
    }
}
