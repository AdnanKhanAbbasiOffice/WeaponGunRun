// Publisher: Vörös Gergely

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class SceneActions : MonoBehaviour
{

    // Works perfectly if scenes are loaded and unloaded Additively

    #region Public Variables
    //[HideInInspector]
    public UnityEvent OnLevelLoaded;
    //[HideInInspector]
    public UnityEvent OnLevelUnloaded;
    #endregion

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }

    private void OnSceneUnloaded(Scene scene)
    {
        OnLevelUnloaded.Invoke();
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        OnLevelLoaded.Invoke();
    }

   
}
