using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
public class FeatureGate : MonoBehaviour
{
    public enum GateType
    {
        FIRERATE,
        FIRERANGE,
        FIREPOWER

    }
    public GateType gateType = GateType.FIRERATE;
    public enum OperatorType
    {
     Plus='+',
     Minus='-',
 

    }
    public OperatorType operatorType = OperatorType.Plus;

    public GameObject DependentGate;
    public bool IsLefToRightMoving;
    public bool IsRightToLeftMoving;
   
    [SerializeField] float MovingGateSpeed = 2.5f;
    public MeshRenderer Base;
    public MeshRenderer Inner;

    public Material PositiveBaseMat;
    public Material PositiveInnerMat;


    public Material NegativeBaseMat;
    public Material NegativeInnerMat;


    bool IsCollided;

    [SerializeField] float FireValue = 1.0f;
    [SerializeField] float GateValue = 1.0f;

    public GameObject GateParticle;
    string GateTitle="FIRE RATE";
    [SerializeField] TextMeshProUGUI TitleText;
    [SerializeField] TextMeshProUGUI FireValueText;

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
        TitleText.text = gateType.ToString();
        char Operator=(char)operatorType;
        OperatorText.text = Operator.ToString();
        GateOperator = Operator;

        FireValueText.text = FireValue.ToString();
        GateValueText.text= GateValue.ToString();
        if(IsLefToRightMoving)
        {
            var pos = transform.localPosition;
            pos.x = -2;
            transform.localPosition = pos;

            transform.DOMoveX(2, MovingGateSpeed).SetSpeedBased(true).SetLoops(-1,LoopType.Yoyo).SetEase(Ease.Linear);
            if(DependentGate)
            {
                var poss = DependentGate.transform.localPosition;
                poss.x = -2;
                DependentGate.transform.localPosition = poss;

                DependentGate.transform.DOMoveX(2, MovingGateSpeed).SetSpeedBased(true).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.Linear);
            }
        }
        else if(IsRightToLeftMoving)
        {
            var pos=transform.localPosition;
            pos.x = 2;
            transform.localPosition = pos;
            transform.DOMoveX(-2, MovingGateSpeed).SetSpeedBased(true).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.Linear);
            if (DependentGate)
            {
                var poss = DependentGate.transform.localPosition;
                poss.x = 2;
                DependentGate.transform.localPosition = poss;

                DependentGate.transform.DOMoveX(-2, MovingGateSpeed).SetSpeedBased(true).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.Linear);
            }
        }

        ChangeMaterial();
    }
    private void OnTriggerEnter(Collider other)
    {
        if(DependentGate!=null)
        {
            return;
        }

        if(other.GetComponent<Bullet>() || other.GetComponent<BulletCompanion>())
        {
            if (other.GetComponent<Bullet>())
            {
                GameManager.Instance.PlaySound("GateHit");
                GameManager.Instance.Vibration(MoreMountains.NiceVibrations.HapticTypes.Selection);
            }
            GateHitted();
            Destroy(other.gameObject,0.0f);
        }
        else if(other.GetComponent<PlayerController>() && !IsCollided)
        {

            IsCollided = true;
                GameManager.Instance.PlaySound("GatePass");
        
            GetFeatureValue();
            if(GateParticle)
            {
             //   Instantiate(GateParticle, transform.position, Quaternion.identity);

            }


            GameManager.Instance.FireStatus(false);
            transform.DOMoveY(transform.localPosition.y-5,0.4f);
            GameManager.Instance.playerController.ActivatePlayerParticle(0);
            GameManager.Instance.WeaponRotate(new Vector3(0,0,360),0.3f);
            Destroy(gameObject, 3f);
        }
    }
    public void GateHitted()
    {
       
       
        DecremenetValue();
        if (FireValue<0)
        {
            if(GateValue<0)
            {
               // GateValue = 0;
            }
        }
      //  transform.DOScale(new Vector3(1, 1, 1), 0);
        transform.localScale = new Vector3(1,1,1);
     

        transform.DOPunchScale(new Vector3(0.05f, 0.05f, 0), 0.1f, 1, 0.05f).OnStart(() => {
          



       }).OnComplete(()=> {

           transform.DOScale(new Vector3(1, 1, 1), 0.001f);
       });
        GateValueText.text = GateValue.ToString();
       // GateValueText.transform.DOPunchScale(new Vector3(0.2f,0.2f,0),0.2f,1,0.1f);
    }

    public void GetFeatureValue()
    {
        
        switch (gateType)
        {
            
            case GateType.FIRERATE:
                
                GameManager.Instance.SetWeaponFireRate(GateOperator,GateValue);
                break;
            case GateType.FIRERANGE:

                GameManager.Instance.SetWeaponFireRange(GateOperator, GateValue);
                break;
            case GateType.FIREPOWER:

                GameManager.Instance.SetWeaponFirePower(GateOperator, GateValue);
                break;


        }
    }

    public void DecremenetValue()
    {
        switch (operatorType)
        {

            case OperatorType.Plus:
                GateValue += FireValue;

                if (GateValue <= 0)
                {
                    GateValue = -GateValue;


                    operatorType = OperatorType.Minus;
                    char Operator = (char)operatorType;
                    OperatorText.text = Operator.ToString();
                    GateOperator = Operator;
                    ChangeMaterial();
                }

                break;
            case OperatorType.Minus:
                GateValue -= FireValue;

                if(GateValue<=0)
                {
                    GateValue = 0;

                   
                   operatorType= OperatorType.Plus;
                    char Operator = (char)operatorType;
                    OperatorText.text = Operator.ToString();
                    GateOperator = Operator;
                    ChangeMaterial();
                }
               
                break;
         


        }

    }
    public void ChangeMaterial()
    {
        switch (operatorType)
        {
            case OperatorType.Plus:
                Base.material = PositiveBaseMat;
                Inner.material = PositiveInnerMat;

                break;
            case OperatorType.Minus:
                Base.material = NegativeBaseMat;
                Inner.material = NegativeInnerMat;


                break;
        }
    }

        
}
