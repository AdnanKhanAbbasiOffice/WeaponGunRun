// Publisher: Vörös Gergely

using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChange : MonoBehaviour
{

    
    // This Script is for testing SceneActions Load and Unload Events
     

    public int SceneBuildIndex = 0;
    public LoadSceneMode mode;

    public void LoadScene()
    {
        SceneManager.LoadScene(SceneBuildIndex, mode);
    }

    public void UnloadScene()
    {
        SceneManager.UnloadSceneAsync(SceneBuildIndex);
    }

}
