using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class UpgradGate : MonoBehaviour
{
    [SerializeField] float BreakableHealthPerGate = 6;
    public GameObject InstantiateBreakablePrefabe;
    public Transform InstantiatePos;
    public Transform TargetPos;
    int TotalGates = 0;
   
    public Transform Door;
float ScaleFactor = 0.0005f;
    [HideInInspector] float BreakableHealthcounter = 0;

    [SerializeField] List<GameObject> AllGates = new List<GameObject>();
   int OpenedGateCounter = 0;
    [SerializeField] Transform WheelHand;
    //public Transform PieceMover;
    // Start is called before the first frame update
    void Start()
    {
        TotalGates = AllGates.Count;
        BreakableHealthcounter = BreakableHealthPerGate;
        ScaleFactor = (Door.localScale.x / (BreakableHealthPerGate * TotalGates));
       // print(ScaleFactor);
        Invoke("ClosedAllGates",0.3f);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<BreakableObject>())
        {
           
            Destroy(other.gameObject);
           
            MoveInPipe();
        }
   

    }

    public void MoveInPipe()
    {

        BreakableHealthcounter -= 1 ;

      

        Door.DOScaleX(Door.localScale.x-ScaleFactor,0.001f).OnComplete(()=> {


            if (Door.localScale.x < 0)
            {
                var temp = Door.localScale;
                temp.x = 0;
                Door.localScale = temp;
            }

            //   print((Door.localScale.x / 1.2f) * 100);
            //   float ClosedPercentage=(Door.localScale.x / 1.2f) * 100;


            if (BreakableHealthcounter <= 0)
            {
              //  print("open gate");
                AllGates[OpenedGateCounter].GetComponent<UpGradeEnhancementGate>().GateStatus(false);
                BreakableHealthcounter = BreakableHealthPerGate;
                OpenedGateCounter++;
                if(OpenedGateCounter> AllGates.Count - 1)
                {
                    OpenedGateCounter = 2;
                }
            }


        });

       

       GameObject PieceMover= Instantiate(InstantiateBreakablePrefabe, InstantiatePos.position,Quaternion.Euler(0,0,-90));
        PieceMover.transform.DOLocalMove(TargetPos.position, 4f).OnComplete(() =>
        {

            PieceMover.transform.eulerAngles = new Vector3(0,0,0);
            PieceMover.GetComponent<Collider>().isTrigger = false;
            PieceMover.GetComponent<Rigidbody>().useGravity = true;
            WheelHand.DOLocalRotate(new Vector3(90,-360,0),0.3f,RotateMode.FastBeyond360).SetLoops(3).SetEase(Ease.Linear);

        }).OnStart(()=> {

            PieceMover.transform.DORotate(new Vector3(0, 0, -90), 0.1f);
        }).SetSpeedBased(true);

          
    }
    public void ClosedAllGates()
    {
        foreach(GameObject g in AllGates)
        {
            g.GetComponent<UpGradeEnhancementGate>().GateStatus(true);
            g.GetComponent<UpGradeEnhancementGate>().IsBelongToUpgradGate = true;

        }
    }
}
