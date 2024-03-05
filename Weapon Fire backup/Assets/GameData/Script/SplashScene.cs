using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SplashScene : MonoBehaviour
{
    bool IsLoadNextScene;
    // Start is called before the first frame update
    void Start()
    {
        Invoke("LoadGamePlay",10.0f); 
    }

    // Update is called once per frame
    void Update()
    {
        //if(IsLoadNextScene && AdManager.Instance.IsFirebaseInitialized)
        //{
        //    IsLoadNextScene = false;
        //    SceneManager.LoadScene(1);
        //}
    }
    void LoadGamePlay()
    {
        IsLoadNextScene = true;
        SceneManager.LoadScene(1);
        //if (AdManager.Instance.IsFirebaseInitialized)
        //{
        //    SceneManager.LoadScene(1);
        //}

    }
}
