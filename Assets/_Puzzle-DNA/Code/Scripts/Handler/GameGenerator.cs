using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameGenerator : MonoBehaviour
{
    public static GameGenerator instance;
    public int currentGameLevel;

    void Awake()
    {
        instance = this;
    }

    public void GenerateLevel(GameData data)
    {
        StartCoroutine(Generate(data));
    }

    public IEnumerator Generate(GameData data)
    {
        currentGameLevel = data.gameLevel;
        DataHandler.instance.GetAudioHandler("MainMenu").SetAudiosState();
        CommonHandler.instance.LoadSceneAdditive("GameScene");

        yield return new WaitUntil(() => FindObjectOfType<GameController>() != null);

        GameController controller = FindObjectOfType<GameController>();
        controller.Init(
            MainMenuHandler.instance.GetBGMSource().volume, 
            data
            );
    }
}
