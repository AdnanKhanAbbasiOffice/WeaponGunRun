using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
//using MoreMountains.NiceVibrations;
public class BreakableObject : MonoBehaviour
{
    [SerializeField] bool IsUseConveyourBelt = false;
    [SerializeField] float BreakableHealth = 1.0f;
    [SerializeField] float FireValue = 1.0f;
    [SerializeField] float SpeedToConveyourBelt = 20.0f;
    [SerializeField] float SpeedOnConveyourBelt = 12.0f;
    [SerializeField] GameObject BreakParticlePrefab;
    public GameObject RewardPrefab;
    public GameObject RootParent;
    public GameObject Rop;
    [SerializeField]bool IsCollided = false;


    public float LevelWiseYSpeedOffset = 0.34f;
 float speedYoffset;

    public float LevelWiseSpeedOffset = 0.67f;
   float speedoffset;
    // Start is called before the first frame update
    void Start()
    {
        speedYoffset = GameManager.Instance.currentLevel * LevelWiseYSpeedOffset;
        speedYoffset = Mathf.Clamp(speedYoffset, 0, 10f);

        speedoffset = GameManager.Instance.currentLevel * LevelWiseSpeedOffset;
        speedoffset = Mathf.Clamp(speedYoffset, 0, 20);
    }

    // Update is called once per frame
    void Update()
    {

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Bullet>() || other.GetComponent<BulletCompanion>() && !IsCollided)
        {
            if(other.GetComponent<Bullet>())
            {
                GameManager.Instance.PlaySound("ShieldHit");
                GameManager.Instance.Vibration(MoreMountains.NiceVibrations.HapticTypes.Selection);
            }

          
         
            Destroy(other.gameObject);
            GateHitted();
           
        }
     
      
    }
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.GetComponent<Bullet>() || other.gameObject.GetComponent<BulletCompanion>() && !IsCollided)
        {
            if (other.gameObject.GetComponent<Bullet>())
            {
                GameManager.Instance.PlaySound("ShieldHit");
                GameManager.Instance.Vibration(MoreMountains.NiceVibrations.HapticTypes.Selection);
            }



            Destroy(other.gameObject);
            GateHitted();

        }
    }

    public void GateHitted()
    {
        BreakableHealth -= FireValue;

        transform.DOPunchScale(new Vector3(0.05f, 0.05f, 0), 0.1f, 1, 0.05f);


        if (BreakableHealth <= 0)
        {
            gameObject.layer = 2;
            IsCollided = true;
            Instantiate(BreakParticlePrefab, transform.position, Quaternion.identity);
            GiveCashReward();

            if (RootParent)
            {
                if (RootParent.GetComponent<BreakableGate>())
                {
                    if(Rop)
                    {
                        Destroy(Rop, 0.0f);
                    }
                    RootParent.GetComponent<BreakableGate>().CallOnDestroy();
                    transform.parent = null;
                    if (IsUseConveyourBelt)
                    {
                        transform.DOMoveY(5, 0.15f).OnComplete(() => {
                            //15f + speedoffset
                            transform.DOMove(new Vector3(-5.5f, 1f, transform.position.z), SpeedToConveyourBelt + speedYoffset).SetSpeedBased(true).OnComplete(() => {
                                //SpeedOnConveyourBelt+ speedoffset
                                //  transform.DORotate(new Vector3(0, 0, 0), 0.01f);
                                transform.DOMoveZ(transform.position.z + 1000f, SpeedOnConveyourBelt + speedoffset).SetSpeedBased(true).OnStart(() => {

                                    // transform.DORotate(new Vector3(0, 0, 0), 0.1f);

                                });
                            }).OnStart(() => {

                                //  transform.DORotate(new Vector3(0, 0, 0), 0.1f);

                            });

                        }).OnStart(() => {

                            transform.DORotate(new Vector3(60, 25, 30), 0.1f).SetLoops(2, LoopType.Yoyo).OnComplete(() => {

                              //  transform.DORotate(new Vector3(0, 0, 0), 0.2f);

                            });

                        });//.SetLoops(2, LoopType.Yoyo);



                   

                    }
                    else
                    {
                        Destroy(this.gameObject, 0.0f);
                    }
            
                }

            }
            else
            {
             
                    Destroy(this.gameObject, 0.0f);

                

            }
  
         
    
        }


    }

    void GiveCashReward()
    {
        GameObject reward = Instantiate(RewardPrefab, transform.position, Quaternion.identity);
        reward.transform.DOLocalJump(reward.transform.localPosition + new Vector3(0, 0, Random.Range(2, 6)), 2, 1, 0.3f).SetEase(Ease.Linear);
        reward.transform.parent = null;
        reward.GetComponent<Rigidbody>().useGravity = true;
    }
}
