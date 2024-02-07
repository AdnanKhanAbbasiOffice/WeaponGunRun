using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SplashScene : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Invoke("LoadGamePlay",5.0f); 
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void LoadGamePlay()
    {
        SceneManager.LoadScene(1);
    }
}
