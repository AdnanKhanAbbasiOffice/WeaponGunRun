using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;


public class BreakableGate : MonoBehaviour
{
    [SerializeField] bool IsPushable = false;
   // [SerializeField] float BreakableHealth=1.0f;
    [SerializeField] float FireValue = 1.0f;
    [SerializeField] GameObject ParticlePrefab;
    public GameObject BreakablesParent;
    public List<GameObject> AllBreakableObjects = new List<GameObject>();
    int gatebreakableobjects = 0;
    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerController>() && IsPushable)
        {

            GameManager.Instance.playerController.PushBack();
            // Destroy(other.gameObject);
            //   GateHitted();

        }
    }
    public void CallOnDestroy()
    {
        gatebreakableobjects += 1;
      
        if(gatebreakableobjects >= AllBreakableObjects.Count)
        {
          
        
            AllBreakableObjects.Clear();
            GetComponent<Collider>().enabled = false;
            Instantiate(ParticlePrefab, transform.position, Quaternion.identity);
        }
     

    }
 
}
