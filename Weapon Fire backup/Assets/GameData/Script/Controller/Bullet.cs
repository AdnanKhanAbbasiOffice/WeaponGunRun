using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField]Rigidbody rb;

    [SerializeField] Vector3 RaySize;

    [SerializeField] LayerMask CollisionLayers;

    [SerializeField] float RaycastRange;
    [SerializeField]float m_MaxHitDistance=7;
    bool m_HitDetect;

    Collider m_Collider;
    RaycastHit m_Hit;
   [HideInInspector] public bool IsCollided = false;
    bool IsReaycastDraw;
    private void Awake()
    {
       // RaycastRange = 100.0f;
        m_Collider = GetComponent<Collider>();


    }
    // Start is called before the first frame update
    void Start()
    {
        //Choose the distance the Box can reach to

  

    }

    // Update is called once per frame
    void Update()
    {
        if(AllowToMove)
        {
          //  
            rb.MovePosition(transform.position + dir * Time.deltaTime);

        }
    }
    Vector3 temp;
   public  bool AllowToMove;
    Vector3 dir;
    public void MoveForward(float range,float power)
    {
      if(IsReaycastDraw)
        {
            return;
        }
        IsReaycastDraw = true;
        FireRaycast(range, power);
     
    }
    public void FireRaycast(float range, float power)
    {
       
        //Test to see if there is a hit using a BoxCast
        //Calculate using the center of the GameObject's Collider(could also just use the GameObject's position), half the GameObject's size, the direction, the GameObject's rotation, and the maximum distance as variables.
        //Also fetch the hit data
        m_HitDetect = Physics.BoxCast(m_Collider.bounds.center, RaySize, transform.forward, out m_Hit, transform.rotation, RaycastRange, ~CollisionLayers);
        if (m_HitDetect && m_Hit.distance>m_MaxHitDistance)
        {
            //Output the name of the Collider your Box hit
      //  Debug.Log("Hit : " + m_Hit.collider.name);
          temp = (m_Hit.point - transform.position).normalized;
            //  rb.AddForce(temp * power, ForceMode.VelocityChange);
            dir = temp* power;
          //  rb.MovePosition(transform.position + (dir) * Time.deltaTime);
            AllowToMove = true;

            Destroy(gameObject, range);
        }
        else
        {
           // Debug.Log("Not Collided ");
            Fire(range,power);
        }

    }
    public void Fire(float range, float power)
    {
       // ForceMode.VelocityChange
       rb.AddForce(Vector3.forward * power, ForceMode.VelocityChange);
     //   MoveBullete(range,power);
        Destroy(gameObject, range);
    }

    public void MoveBullete(float range, float power)
    {

        rb.MovePosition(Vector3.forward * power);
        Destroy(gameObject, range);
    }

    //Draw the BoxCast as a gizmo to show where it currently is testing. Click the Gizmos button to see this
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        //Check if there has been a hit yet
        if (m_HitDetect)
        {
            //Draw a Ray forward from GameObject toward the hit
            Gizmos.DrawRay(transform.position, transform.forward * m_Hit.distance);
            //Draw a cube that extends to where the hit exists
            Gizmos.DrawWireCube(transform.position + transform.forward * m_Hit.distance, RaySize);
        }
        //If there hasn't been a hit yet, draw the ray at the maximum distance
        else
        {
            //Draw a Ray forward from GameObject toward the maximum distance
            Gizmos.DrawRay(transform.position, transform.forward * RaycastRange);
            //Draw a cube at the maximum distance
            Gizmos.DrawWireCube(transform.position + transform.forward * RaycastRange, RaySize);
        }
    }


}
