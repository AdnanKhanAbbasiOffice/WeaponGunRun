using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ObjectTouchRotation: MonoBehaviour
{
    #region Rotating Variables
    float f_lastX = 0.0f;
    float f_difX = 0.5f;
    float f_steps = 0.0f;
    int i_direction = 1;
    bool flag,flag1;
  
    #endregion

   
    // Use this for initialization
    void Start()
    {
        flag = false;
        flag1= false;
    }
    public void OnEnable()
    {
       

    }
    public void CheckUnlocking()
    {
     

    }
    // Update is called once per frame
    void Update()
    {
       

        if (f_difX > 0.5f) f_difX -= 0.05f;
            if (f_difX < 0.5f) f_difX += 0.05f;
            transform.Rotate(Vector3.up, f_difX * i_direction);

            if(Input.GetAxis("Mouse X")==0 && flag1)
            {
                OnExit();
            }

         
    }

    void OnMouseDrag()
    {
   
        flag = true;

        if (Input.GetMouseButtonDown(0))
        {
            f_difX = 0.0f;
        }
        else if (Input.GetMouseButton(0))
        {
            f_difX = Mathf.Abs(f_lastX - Input.GetAxis("Mouse X"));

            if (f_lastX < Input.GetAxis("Mouse X"))
            {
                i_direction = -1;
                transform.Rotate(Vector3.up, -f_difX);
            }

            if (f_lastX > Input.GetAxis("Mouse X"))
            {
                i_direction = 1;
                transform.Rotate(Vector3.up, f_difX);
            }

            f_lastX = -Input.GetAxis("Mouse X");
        }
    }
    private void OnMouseUp()
    {
        flag1 = true;
    }
    private void OnExit()
    {
       
        flag1 = false;
    }
}
