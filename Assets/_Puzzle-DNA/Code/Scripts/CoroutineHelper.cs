using System.Collections;
using UnityEngine;

public class CoroutineHelper : MonoBehaviour
{
    private static CoroutineHelper _instance;

    public static CoroutineHelper instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject obj = new GameObject("CoroutineHelper");
                _instance = obj.AddComponent<CoroutineHelper>();
                DontDestroyOnLoad(obj);
            }
            return _instance;
        }
    }

    public void StartCoroutineFromStatic(IEnumerator coroutine)
    {
        StartCoroutine(coroutine);
    }
}
