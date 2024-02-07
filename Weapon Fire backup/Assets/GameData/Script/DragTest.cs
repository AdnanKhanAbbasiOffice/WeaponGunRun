using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragTest : MonoBehaviour
{
    public Camera camera;
    Vector3 offset;
    public string destinationTag = "DropArea";

    void OnMouseDown()
    {
        offset = transform.position - MouseWorldPosition();
        transform.GetComponent<Collider>().enabled = false;
      
    }
    Vector3 initial;
    void OnMouseDrag()
    {
        transform.position = MouseWorldPosition() + offset;
        var temp = transform.position;
      //  temp.x += gap;
        transform.position = temp;

    }

    void OnMouseUp()
    {
        var rayOrigin = camera.transform.position;
        var rayDirection = MouseWorldPosition() - camera.transform.position;
        RaycastHit hitInfo;
        if (Physics.Raycast(rayOrigin, rayDirection, out hitInfo))
        {
            if (hitInfo.transform.tag == destinationTag)
            {
                transform.position = hitInfo.transform.position;
            }
        }
        transform.GetComponent<Collider>().enabled = true;
    }
    public float gap;
    Vector3 MouseWorldPosition()
    {
        var mouseScreenPos = Input.mousePosition;
        mouseScreenPos.z = camera.WorldToScreenPoint(transform.position).z;

      //   gap=(10 - mouseScreenPos.z);

      //  print(gap);
        return camera.ScreenToWorldPoint(mouseScreenPos);
    }
}
