using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameGenerator : MonoBehaviour
{
    public static GameGenerator instance;
    [SerializeField] int currentGameLevel;

    void Awake()
    {
        instance = this;
    }

    public void SetScoreGameLevel(int score)
    {
        if (currentGameLevel < 0) return;
        DataHandler.instance.GetUserCheckpointData().checkpoint_value[currentGameLevel].checkpoint_level_score = score;
        MainMenuHandler.instance.PatchCheckpointFromMenu();
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
        controller.Init(data);
    }
}
