using UnityEngine;
using UnityEngine.UI;

public class JavascriptHook : MonoBehaviour
{
#if UNITY_EDITOR
    private void Start()
    {
        PlayerCodeHandler(DataHandler.instance.codeTemp);
    }
#endif

    public void PlayerCodeHandler(string code)
    {
        DataHandler.instance.userData.data.game_url = code;
    }
}