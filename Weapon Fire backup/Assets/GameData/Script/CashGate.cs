using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
public class CashGate : MonoBehaviour
{
    public bool IsLevelCompleteCashGate;


    public GameObject GateParticle;
    public GameObject SparkParticle;
    [SerializeField] float FireValue = 1.0f;
    [SerializeField] float GateValue = 4.0f;
  

    public GameObject RewardObj;

    [SerializeField] TextMeshProUGUI GateValueText;



    // Start is called before the first frame update
    void Start()
    {
        if(!IsLevelCompleteCashGate)
        {
            GateValueText.text = GateValue.ToString();
        }
        
  

        }

    // Update is called once per frame
    void Update()
    {

    }
    public void Initialize(int gatevalue,float firevalue)
    {

        GateValue = gatevalue;
        FireValue = firevalue;

        GateValueText.text = GateValue.ToString();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Bullet>() || other.GetComponent<BulletCompanion>())
        {
            if (other.GetComponent<Bullet>())
            {
                GameManager.Instance.PlaySound("GateHit");
                GameManager.Instance.Vibration(MoreMountains.NiceVibrations.HapticTypes.Selection);
            }

            Destroy(other.gameObject);
            GateHitted();

        }
        else if (other.GetComponent<PlayerController>() || other.GetComponent<CompanionController>())
        {
            // GetFeatureValue();
            if(IsLevelCompleteCashGate)
            {

            
            if (GateValue != 0)
            {
                // Destroy(gameObject, 0.1f);


                Camera.main.transform.parent.GetComponent<CameraControll>().Rotate();
                print("push back");
            }
        }
            else
            {
                if (other.GetComponent<PlayerController>())
                    {


                    GameManager.Instance.PlaySound("GatePass");

                   // GetFeatureValue();
                    if (GateParticle)
                    {
                        Instantiate(GateParticle, transform.position, Quaternion.identity);

                    }


                    GameManager.Instance.FireStatus(false);
                    transform.DOMoveY(transform.localPosition.y - 5, 0.4f);
                    GameManager.Instance.playerController.ActivatePlayerParticle(0);
                    GameManager.Instance.WeaponRotate(new Vector3(0, 0, 360), 0.3f);
                    Destroy(gameObject, 3f);
                }
            }
        }
    }
    public void GateHitted()
    {
        if(SparkParticle)
        {
            SparkParticle.SetActive(true);

        }

        GateValue = Mathf.Clamp(GateValue -= FireValue, 0,GateValue);
        transform.DOPunchScale(new Vector3(0.05f, 0.05f, 0), 0.1f, 1, 0.05f).OnStart(()=> {
            transform.DOScale(new Vector3(1, 1, 1), 0.01f);
        }).OnComplete(() => {

            transform.DOScale(new Vector3(1, 1, 1), 0.01f);
        });
        GateValueText.text = GateValue.ToString();

        if (GateValue <= 0)
        {
            Instantiate(GateParticle, transform.position, Quaternion.identity);
            RewardObj.SetActive(true);
            RewardObj.transform.parent = null;
            RewardObj.GetComponent<Rigidbody>().useGravity = true;
        
          Destroy(gameObject, 0.01f);
        }
   
     
    }


}
