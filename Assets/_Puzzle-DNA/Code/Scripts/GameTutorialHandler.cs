using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTutorialHandler : MonoBehaviour
{
    [SerializeField] GameObject tutorialPanel;

    void Awake()
    {
        tutorialPanel.SetActive(false);       
    }
}
