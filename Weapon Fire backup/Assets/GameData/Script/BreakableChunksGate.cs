using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableChunksGate : MonoBehaviour
{
    bool IsPushedBack;
 
   // [SerializeField] float BreakableHealth = 1.0f;
    [SerializeField] float FireValue = 1.0f;
    [SerializeField] GameObject ParticlePrefab;
    public GameObject BreakablesParent;
    public List<GameObject> AllBreakableObjects = new List<GameObject>();
   [SerializeField] int gatebreakableobjects = 0;
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
        if (other.GetComponent<PlayerController>())
        {

            if (!IsPushedBack)
            {
                GameManager.Instance.playerController.PushBack();
                IsPushedBack = true;
            }
            else
            {

                Destroy(gameObject, 3f);
            }

        }


       

    }
    public void CallOnDestroy()
    {
        gatebreakableobjects += 1;

        if (gatebreakableobjects >= AllBreakableObjects.Count)
        {
         //   print("Hurrah");

            AllBreakableObjects.Clear();
            GetComponent<Collider>().enabled = false;
            Instantiate(ParticlePrefab, transform.position, Quaternion.identity);
           Destroy(gameObject);
        }


    }


}
