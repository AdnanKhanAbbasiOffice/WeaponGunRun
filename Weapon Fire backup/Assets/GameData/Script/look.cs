using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class look : MonoBehaviour
{
    public Transform target;
    public float speed = 1.0f;
    int i = 0;
    public Transform[] Waypoints;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(!GameManager.Instance.IsBattleON)
        {
            return;
        }

        if(Vector3.Distance(transform.position, Waypoints[i].position)>0)
        {
          //  transform.position=Vector3.MoveTowards(transform.position, Waypoints[i].position, speed*Time.deltaTime);
         
            }
        else
        {
            i++;

            if(i>=Waypoints.Length)
            {
                i = Waypoints.Length - 1;
            }

            if (i == 6)
            {
              //  transform.DOLocalRotate(GameManager.Instance._CameraControll.PlayerCamera.transform.localEulerAngles, 1);
            }
        }
        if (i < 6)
        {
           //  transform.LookAt(target);
           // transform.DOLookAt(target,0.1f);
        }
    }
}
