using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameGenerator : MonoBehaviour
{
    public static GameGenerator instance;
    [SerializeField] Sprite playerSprite;

    void Awake()
    {
        instance = this;    
    }

    public void GenerateLevel(GameData data)
    {
        CommonHandler.instance.LoadSceneAdditive("GameScene");
        StartCoroutine(Generate(data));
    }

    public IEnumerator Generate(GameData data)
    {
        yield return new WaitUntil(() => FindObjectOfType<GameController>() != null);

        GameController controller = FindObjectOfType<GameController>();
        controller.characterPlayerSprite = playerSprite;
        controller.Init(data);
    }
}
