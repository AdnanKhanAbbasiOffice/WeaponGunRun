using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEvents : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void PassEvent(string eventname)
    {
        if(eventname =="ActivatePlayerCamera")
        {
            //   GameManager.Instance. _CameraControll.GridCamera.SetActive(false);
            //  GameManager.Instance._CameraControll.PlayerCamera.SetActive(true);
            GameManager.Instance.TapToPlay();
            GameManager.Instance._CameraControll.PlayerCamera.GetComponent<Animator>().enabled = false;

            GameManager.Instance.uiManager.gamePlay.WeaponEnhacemenetPanel.SetActive(false);
           
        }
    

    }
}
