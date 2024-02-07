using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class LevelComplete : MonoBehaviour
{
    public int InitialGateValue = 4;
    public int MultiplierLimit = 10;
    public float FireValue = 3;
    [SerializeField] Transform MultiplierParent;
    [SerializeField] Multiplier MultiplierPrefab;

    [SerializeField] Transform LevelCompleteTrigger;
    public List<Multiplier> AllMultipliers= new List<Multiplier>();
    [SerializeField] Color[] BGColor;
    int cashgatevalue = 0;

    public Transform NewWeaponBasePrefab;
    Transform NewWeaponBase;
   // public Camera NewWeaponRendererCamera;
    // Start is called before the first frame update
    void Start()
    {


        Invoke(nameof(Initialization),0.5f);


      
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void Initialization()
    {
        int pos = 0;
    
         MultiplierLimit += GameManager.Instance.currentLevel;
        MultiplierLimit = Mathf.Clamp(MultiplierLimit, 10,20);

        for (int i = 0; i < MultiplierLimit; i++)
        {
            pos += 8;
            cashgatevalue += InitialGateValue;
            Multiplier multiplier = Instantiate(MultiplierPrefab, transform.position, Quaternion.identity);
            multiplier.transform.parent = MultiplierParent;
            multiplier.transform.localPosition = new Vector3(0, 0, pos);

            AllMultipliers.Add(multiplier);

            if (i >= BGColor.Length)
            {
                multiplier.Initialize(i + 1, BGColor[Random.Range(0, BGColor.Length)], cashgatevalue, FireValue);
            }
            else
            {
                multiplier.Initialize(i + 1, BGColor[i], cashgatevalue, FireValue);
            }



        }

        pos += 8;
        //NewWeaponBase.transform.localPosition = new Vector3(0, 0, pos);
        LevelCompleteTrigger.transform.localPosition = new Vector3(0, 0, pos);
        pos += 8;

         NewWeaponBase = Instantiate(NewWeaponBasePrefab, new Vector3(0, 0, 100), Quaternion.identity);
        NewWeaponBase.transform.parent = MultiplierParent;
        NewWeaponBase.transform.localPosition = new Vector3(0, 0, pos);

        GameManager.Instance.weaponManager.InstantiateNextWeapon(NewWeaponBase.GetChild(0).GetChild(0));
    }
    public void FinishLineCross()
    {
        GameManager.Instance.IsFinishLineCross = true;
       Camera.main.transform.parent.GetComponent<CameraControll>(). ZoomIn();
    }
    public void LevelCompleteFunc()
    {
       
        GameManager.Instance.IsGameStart = false;

        Transform weaponModel=GameManager.Instance.weaponManager.NextInstantiatedWeapon.transform;

        weaponModel.DOKill();
        weaponModel.DOLocalRotate(new Vector3(0, 90, 0), 0.3f);
        GameManager.Instance._CameraControll.transform.DOLocalMoveX(0f, 0.5f);
        GameManager.Instance._CameraControll.transform.GetChild(0).DOLocalMove(new Vector3(0f, 17f, -18), 0.5f);

        var temprotation = GameManager.Instance._CameraControll.transform.GetChild(0).transform.localEulerAngles;
        temprotation.x = 29;
        temprotation.y = 2;
        temprotation.z = 0;


        GameManager.Instance._CameraControll.transform.GetChild(0).DOLocalRotate(temprotation, 0.52f).SetDelay(0f).OnComplete(() =>
        {



            weaponModel.DOScale(new Vector3(0f, 0f, 0f), 1f).OnComplete(() =>
            {


                // NewWeaponBase.GetChild(1).DOScale(new Vector3(0, 0, 0), 0.3f).OnComplete(() =>
                //{
             
                GameManager.Instance.uiManager.levelComplete.NewWeaponUnloackedPanel.transform.localScale = new Vector3(0, 0, 0);
                GameManager.Instance.uiManager.levelComplete.NewWeaponUnloackedPanel.SetActive(true);

               
                GameManager.Instance.uiManager.levelComplete.NewWeaponUnloackedPanel.transform.DOScale(new Vector3(1, 1, 1), 0.3f).OnComplete(() =>
                {
                   
                    GameManager.Instance._CameraControll.NewWeponRendererCam.transform.localPosition = GameManager.Instance._CameraControll.PlayerCamera.transform.localPosition;
                    GameManager.Instance._CameraControll.NewWeponRendererCam.transform.localEulerAngles = GameManager.Instance._CameraControll.PlayerCamera.transform.localEulerAngles;
                    GameManager.Instance._CameraControll.NewWeponRendererCam.gameObject.SetActive(true);
                    weaponModel.DOLocalMove(new Vector3(0f, 0f, 0f), 0.01f);
                    weaponModel.DOScale(new Vector3(2.6f, 2.6f, 2.6f), 0.5f).OnComplete(()=> {
                        GameManager.Instance.PlaySound("GameComplete");
                        weaponModel.DOLocalRotate(new Vector3(0, 20, 0), 4.0f).SetLoops(-1, LoopType.Incremental).SetEase(Ease.Linear).id = "YRotation";
                    });

                });
                // });

            }).SetEase(Ease.InBack);//.SetLoops(1, LoopType.Yoyo);




        });



        //Camera.main.transform.parent.GetComponent<CameraControll>().ZoomOut();

    }
 
}
