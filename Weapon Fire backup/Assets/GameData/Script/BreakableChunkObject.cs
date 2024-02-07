using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;


public class BreakableChunkObject : MonoBehaviour
{
    [SerializeField] bool IsPlank;
    [SerializeField] float BreakableHealth = 1.0f;
    [SerializeField] float FireValue = 1.0f;
    [SerializeField] GameObject BreakParticlePrefab;
   // public GameObject RewardPrefab;
    public GameObject RootParent;
    bool IsCollided = false;
    [SerializeField] List<Transform> Chunks = new List<Transform>();


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
        if (other.GetComponent<Bullet>() && !IsCollided || other.GetComponent<BulletCompanion>())
        {
            if(other.GetComponent<Bullet>())
            {
                if (other.GetComponent<Bullet>().IsCollided)
                {
          
                    Destroy(other.gameObject);
                    return;
                }
                other.GetComponent<Bullet>().IsCollided = true;
            }

            if (other.GetComponent<BulletCompanion>())
            {
                if (other.GetComponent<BulletCompanion>().IsCollided)
                {

                    Destroy(other.gameObject);
                    return;
                }
                other.GetComponent<BulletCompanion>().IsCollided = true;
            }

            GameManager.Instance.PlaySound("GateHit");
            Destroy(other.gameObject);
       
          

      
            GateHitted();

        }
    }

    public void GateHitted()
    {
        BreakableHealth -= FireValue;

       // transform.DOPunchScale(new Vector3(0.05f, 0.05f, 0), 0.1f, 1, 0.05f);


        if (BreakableHealth <= 0)
        {
            IsCollided = true;
            // Instantiate(BreakParticlePrefab, transform.position, Quaternion.identity);

            foreach (Transform c in Chunks)
            {
                Vector3 rnd=Vector3.zero;
                if(IsPlank)
                {
                    rnd.x = Random.Range(-2, 2);
                    rnd.y = Random.Range(5, 10);
                    rnd.z = Random.Range(-3, -10);
                    c.transform.parent = null;
                   // c.transform.DOLocalJump(c.transform.localPosition + rnd, 3f, 1, 0.5f).SetEase(Ease.Linear);
                    c.GetComponent<Rigidbody>().useGravity = true;
                    c.GetComponent<Rigidbody>().AddForce(rnd * Random.Range(90, 120));
                    c.GetComponent<Rigidbody>().AddRelativeTorque(rnd * Random.Range(90, 120));
                }
                else
                {

                    rnd.x = Random.Range(-2, 2);
                    rnd.y = Random.Range(5, 10);
                    rnd.z = Random.Range(-3, -10);

                    c.transform.parent = null;
                    // c.transform.DOLocalJump(c.transform.localPosition + rnd, 3f, 1, 0.4f).SetEase(Ease.Linear);
                    c.GetComponent<Rigidbody>().useGravity = true;
                    c.GetComponent<Rigidbody>().AddForce(rnd* Random.Range(90, 120));
                    c.GetComponent<Rigidbody>().AddRelativeTorque(rnd * Random.Range(90, 120));
                }

               // c.transform.localPosition +



                Instantiate(BreakParticlePrefab, c.transform.position, Quaternion.identity);

               // c.GetComponent<Rigidbody>().useGravity = true;
                Destroy(c.gameObject, 7.0f);
            }
            //  GameObject reward = Instantiate(RewardPrefab, transform.position, Quaternion.identity);
            //  reward.transform.DOLocalJump(reward.transform.localPosition + new Vector3(0, 0, Random.Range(2, 6)), 2, 1, 0.3f).SetEase(Ease.Linear);
            // reward.transform.parent = null;
            //  reward.GetComponent<Rigidbody>().useGravity = true;

            if (RootParent)
            {
                if (RootParent.GetComponent<BreakableChunksGate>())
                {

                    RootParent.GetComponent<BreakableChunksGate>().CallOnDestroy();

                    Destroy(this.gameObject, 0.0f);

                }


            }
            else
            {
                Destroy(this.gameObject, 0.0f);

            }



        }


    }
}
