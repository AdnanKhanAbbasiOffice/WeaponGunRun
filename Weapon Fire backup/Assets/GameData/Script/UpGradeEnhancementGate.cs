using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
public class UpGradeEnhancementGate : MonoBehaviour
{

 
    public enum OperatorType
    {
     Plus='+',
     Minus='-',
 

    }
    public bool IsGateClosed;
    [HideInInspector]
    public bool IsBelongToUpgradGate;
    [SerializeField] Material GateClosedMat;
    [SerializeField] Material GateOpenedMat;
    [SerializeField] MeshRenderer renderer;
    public OperatorType operatorType = OperatorType.Plus;
 
    [SerializeField] int GateValue = 1;

    public GameObject GateParticle;

    bool IsCollided = false;
  

    [SerializeField] TextMeshProUGUI GateValueText;
   
    [SerializeField] TextMeshProUGUI OperatorText;
                      char GateOperator = '+';
    // Start is called before the first frame update
    void Start()
    {
        Initialize();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Initialize()
    {
       
        char Operator=(char)operatorType;
        OperatorText.text = Operator.ToString();
        GateOperator = Operator;

 
        GateValueText.text= GateValue.ToString();

        GateStatus(IsGateClosed);

    }
    public void GateStatus(bool IsClosed)
    {
        IsGateClosed = IsClosed;

        if (IsClosed)
        {
            renderer.material = GateClosedMat;
          
        }
        else
        {
            renderer.material = GateOpenedMat;

        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<Bullet>())
        {
            GameManager.Instance.PlaySound("GateHit");
            GameManager.Instance.Vibration(MoreMountains.NiceVibrations.HapticTypes.Selection);

            GateHitted();
            Destroy(other.gameObject,0.01f);
        }
        else if(other.GetComponent<PlayerController>() && !IsCollided && !IsGateClosed)
        {
            IsCollided = true;
            GameManager.Instance.SetWeaponEnhancementTemporary(GateValue);
            GameManager.Instance.PlaySound("EnhancementActivate");
            GameManager.Instance.Vibration(MoreMountains.NiceVibrations.HapticTypes.Success);

            if (GateParticle)
            {
                Instantiate(GateParticle, transform.position, Quaternion.identity);

            }


            GameManager.Instance.FireStatus(false);
            if(IsBelongToUpgradGate)
            {
                if (transform.parent.parent.GetComponent<UpgradGate>())
                {
                    transform.parent.parent.DOMoveY(transform.localPosition.y - 5, 0.7f);
                }
                

            }
            else
            {
                transform.DOMoveY(transform.localPosition.y - 5, 0.4f);

            }
            GameManager.Instance.playerController.ActivatePlayerParticle(0);
            // GameManager.Instance.WeaponRotate(new Vector3(0,0,360),0.3f);
            GameManager.Instance.weaponManager.CurrentWeapon[0].transform.parent.DOPunchScale(new Vector3(0.5f, 0.5f, 0.5f), 0.5f, 5, 0.5f).OnComplete(()=> {
                GameManager.Instance.FireStatus(true);
                GameManager.Instance.weaponManager.CurrentWeapon[0].transform.parent.localScale = new Vector3(1, 1, 1);
            });

            Destroy(gameObject, 3f);
        }
    }
    public void GateHitted()
    {

        transform.localScale = new Vector3(1, 1, 1);
        transform.DOPunchScale(new Vector3(0.05f, 0.05f, 0), 0.1f, 1, 0.05f);
        GateValueText.text = GateValue.ToString();
       // GateValueText.transform.DOPunchScale(new Vector3(0.2f,0.2f,0),0.2f,1,0.1f);
    }


}
