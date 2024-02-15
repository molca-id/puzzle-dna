using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameGenerator : MonoBehaviour
{
    public void GenerateLevel(GameData data)
    {
        StartCoroutine(Generate(data));
    }

    public IEnumerator Generate(GameData data)
    {
        Debug.Log("generating");
        yield return new WaitUntil(() => FindObjectOfType<GameController>() != null);

        Debug.Log("generated");
        GameController controller = FindObjectOfType<GameController>();
        controller.gameData = data;
        controller.Init();
    }
}
