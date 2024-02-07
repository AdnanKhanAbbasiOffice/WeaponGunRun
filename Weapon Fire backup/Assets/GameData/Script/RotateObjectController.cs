using UnityEngine;
using DG.Tweening;
public class RotateObjectController : MonoBehaviour
{
    public Transform Target;
    public float PCRotationSpeed = 10f;
    public float MobileRotationSpeed = 0.4f;
    //Drag the camera object here
    public Camera cam;

    bool IsDrag;
    private void Start()
    {
       // Invoke("Initialize",1.0f);
    }
    void Initialize()
    {

       // cam=GameManager.Instance._CameraControll.PlayerCamera.GetComponent<Camera>();

    }
    void OnMouseDrag()
    {
        //float rotX = Input.GetAxis("Mouse X") * PCRotationSpeed;
        //float rotY = Input.GetAxis("Mouse Y") * PCRotationSpeed;

        //Vector3 right = Vector3.Cross(cam.transform.up, transform.position - cam.transform.position);
        //Vector3 up = Vector3.Cross(transform.position - cam.transform.position, right);
        //transform.rotation = Quaternion.AngleAxis(-rotX, up) * transform.rotation;
        //transform.rotation = Quaternion.AngleAxis(rotY, right) * transform.rotation;
    }

    void Update ()
    {
        // get the user touch input
        foreach (Touch touch in Input.touches) {
            Debug.Log("Touching at: " + touch.position);
            Ray camRay = cam.ScreenPointToRay (touch.position);
            RaycastHit raycastHit;
            if(Physics.Raycast (camRay, out raycastHit, 10))
            {
                if (touch.phase == TouchPhase.Began) {
                    Debug.Log("Touch phase began at: " + touch.position);
                } else if (touch.phase == TouchPhase.Moved) {
                    IsDrag = true;
                    Debug.Log("Touch phase Moved");
                    Target.Rotate (0, 
                        -touch.deltaPosition.x * MobileRotationSpeed, 0, Space.World);
                } else if (touch.phase == TouchPhase.Ended) {
                    IsDrag = false;
                    Invoke("DefaultState",0.1f);
                    Debug.Log("Touch phase Ended");    
                }    
            }
        }
    }

    public void DefaultState()
    {
        if(IsDrag)
        {
            return;
        }


        Target.DORotate(new Vector3(0,0,0),0.3f);

    }
}
