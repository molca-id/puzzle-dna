using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
class LevelButtonData
{
    public Button levelButton;
    public TextMeshProUGUI scoreText;
}

public class MainMenuHandler : MonoBehaviour
{
    [SerializeField] List<LevelButtonData> levelButtonDatas;

    private void Start()
    {
        int i = 1;
        levelButtonDatas.ForEach(data =>
        {
            data.scoreText.text = i.ToString();
            i++;
        });
    }
}
