using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class CommonHandler : MonoBehaviour
{
    public static CommonHandler instance;
    public UnityEvent whenSceneLoading;
    public UnityEvent whenSceneLoaded;
    public UnityEvent whenSceneUnloaded;
    
    [HideInInspector] public UnityEvent whenSceneLoadingCustom;
    [HideInInspector] public UnityEvent whenSceneLoadedCustom;
    [HideInInspector] public UnityEvent whenSceneUnloadedCustom;

    private void Awake()
    {
        instance = this;
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void LoadSceneAdditive(string sceneName)
    {
        StartCoroutine(LoadAdditiveScene(sceneName));
    }

    public void UnloadSceneAdditive(string sceneName)
    {
        StartCoroutine(UnloadAdditiveScene(sceneName));
    }

    IEnumerator LoadAdditiveScene(string sceneName)
    {
        whenSceneLoading.Invoke();
        whenSceneLoadingCustom.Invoke();
        whenSceneLoadingCustom.RemoveAllListeners();

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        yield return new WaitUntil(() => asyncLoad.isDone);
        
        whenSceneLoaded.Invoke();
        whenSceneLoadedCustom.Invoke();
        whenSceneLoadedCustom.RemoveAllListeners();
    }

    IEnumerator UnloadAdditiveScene(string sceneName)
    {
        AsyncOperation asyncLoad = SceneManager.UnloadSceneAsync(sceneName);
        yield return new WaitUntil(() => asyncLoad.isDone);
        
        whenSceneUnloaded.Invoke();
        whenSceneUnloadedCustom.Invoke();
        whenSceneUnloadedCustom.RemoveAllListeners();
    }
}
