using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class WeaponManager : MonoBehaviour
{

    public WeaponInfo[] AllWeaponsInfo;
    [HideInInspector]
    public WeaponInfo CurrentWeaponInfo;
   // [HideInInspector]
   // public Weapon CurrentWeapon;
    public List<Weapon> CurrentWeapon =new List<Weapon>();
        //public GameObject NextWeapon;

    [SerializeField] internal List<int> EnhancementLevelIdsTemporary = new List<int>();
    [SerializeField] internal int WeaponEnhancementCounterTemporary = 0;
    [HideInInspector]
public float Gap;
    [HideInInspector]
    public int Weaponpaircounter = 0;

    public Transform EnhacementPosForRendererTexture;
    public RendererCamera EnhancementRendererCamera;
    [HideInInspector]
    public GameObject EnhacementModelForRendererTexture;
    public GameObject Confitti;
    public GameObject WeaponEnhancementParticle;
    public Transform WeaponEnhancementParticlePos;
  
   // [HideInInspector]
    public GameObject NextInstantiatedWeapon;
    // public Transform ConfittiPos;
    // Start is called before the first frame update
    void Start()
    {
        SetTemporaryWeaponValues();
        Invoke("ResetWeaponFeatures",0.3f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public int FindNextWeaponIndex()
    {

      
        int nextweaponindex = GameManager.Instance.CurrentWeapon + 1;


        if (nextweaponindex >= AllWeaponsInfo.Length)
        {
            nextweaponindex = GameManager.Instance.CurrentWeapon;

        }

       // NextWeapon = AllWeaponsInfo[nextweaponindex].WeaponPrefab;

        return nextweaponindex;
   
    }
  
    public void ResetWeaponFeatures()  // Default Values
    {
        AllWeaponsInfo[GameManager.Instance.CurrentWeapon].FireRate = 0.5f;
        AllWeaponsInfo[GameManager.Instance.CurrentWeapon].FireRange = 1.0f;
        AllWeaponsInfo[GameManager.Instance.CurrentWeapon].FirePower = 40;
    }
    public void SetTemporaryWeaponValues()
    {
        EnhancementLevelIdsTemporary = new List<int>(CurrentWeaponInfo.EnhancementLevelIds);
        WeaponEnhancementCounterTemporary = CurrentWeaponInfo.WeaponEnhancementCounter;
    }
    public void ActivateWeapon(int index=0)
    {
        GameObject weapon=Instantiate(AllWeaponsInfo[index].WeaponPrefab);
        GameManager.Instance.SetWeaponAsChild(weapon.transform);
        CurrentWeapon.Add( weapon.GetComponent<Weapon>());
        CurrentWeaponInfo = AllWeaponsInfo[index];

      

        InstantiatePurchasedWeaponsEnhancement();
    }
    public IEnumerator Fire()
    {
        while (GameManager.Instance.IsGameStart)
        {
            yield return new WaitForSeconds(CurrentWeaponInfo.FireRate);

            if (GameManager.Instance.IsGameStart && GameManager.Instance.IsFireStart)
            {


                foreach(Weapon weapon in CurrentWeapon)
                {

                

                GameObject bullet = Instantiate(CurrentWeaponInfo.BulletPrefab, weapon.BullteInstantiationPos.position, Quaternion.identity);
                bullet.GetComponent<Bullet>().MoveForward(CurrentWeaponInfo.FireRange, CurrentWeaponInfo.FirePower);
                    // print("Fire");

                    weapon.transform.DOLocalRotate(new Vector3(-1, 0, 0), 0.1f).OnComplete(() =>
                {

                    weapon.transform.DOLocalRotate(new Vector3(0, 0, 0), 0.1f);
                });

            }
        }
        }
      

    }

    public void SetWeaponFireRate(char Operator, float Value)
    {
        if(Operator == '+')
        {
            Value = -Value;

        }
        Value = Mathf.Clamp(Value, -CurrentWeaponInfo.FireRateIncrement, CurrentWeaponInfo.FireRateIncrement);
        CurrentWeaponInfo.FireRate = Mathf.Clamp(CurrentWeaponInfo.FireRate + Value, CurrentWeaponInfo.FireRateLowerLimit, CurrentWeaponInfo.FireRateUpperLimit);
    }
    public void SetWeaponFireRange(char Operator, float Value)
    {
        if (Operator == '-')
        {
            Value = -Value;

        }
        Value= Mathf.Clamp(Value, -CurrentWeaponInfo.FireRangeIncrement, CurrentWeaponInfo.FireRangeIncrement);
        CurrentWeaponInfo.FireRange = Mathf.Clamp(CurrentWeaponInfo.FireRange + Value, CurrentWeaponInfo.FireRangeLowerLimit, CurrentWeaponInfo.FireRangeUpperLimit);
    }
    public void SetWeaponFirePower(char Operator, float Value)
    {
        if (Operator == '-')
        {
            Value = -Value;

        }
        Value = Mathf.Clamp(Value, -CurrentWeaponInfo.FirePowerIncrement, CurrentWeaponInfo.FirePowerIncrement);
        CurrentWeaponInfo.FirePower = Mathf.Clamp(CurrentWeaponInfo.FirePower + Value, CurrentWeaponInfo.FirePowerLowerLimit, CurrentWeaponInfo.FirePowerUpperLimit);
    }

    public void WeaponRotate(Vector3 angle, float delay, float setdely=0)
    {
        CurrentWeapon[0].transform.parent.DOLocalRotate(angle, delay).SetRelative().SetEase(Ease.OutBack).OnComplete(()=> {

            GameManager.Instance.FireStatus(true);
            CurrentWeapon[0].transform.parent.DOLocalRotate(new Vector3(0,0,0),0.1f);
           
        }).SetDelay(setdely) ;
    }

    
    public void InstantiatePurchasedWeaponsEnhancement()
    {
 

        for (int i=0; i<CurrentWeapon[0].EnhancementPosition.Count;i++)
        {
         
            GameObject enhancement = Instantiate(CurrentWeaponInfo.WeaponEnhancementPrefabs[i], CurrentWeapon[0].EnhancementPosition[i].transform.localPosition, Quaternion.identity);
            ActivatePurchasedWeaponEnhacements(enhancement, CurrentWeaponInfo.EnhancementLevelIds[i],i);
        }
      
    }
 
    public void ActivatePurchasedWeaponEnhacements(GameObject WeaponEnhancement, int EnhancementLevelId,int WeaponEnhancementPos)
    {

      

        if (EnhancementLevelId < 0)
        {
            Destroy(WeaponEnhancement);
            return;
        }

        foreach(Transform Ehancementlevel in WeaponEnhancement.transform.GetComponentInChildren<Transform>())
        {
         
            Ehancementlevel.gameObject.SetActive(false);
        }

      //  print("Before : "+EnhancementLevelId);
        EnhancementLevelId = Mathf.Clamp(EnhancementLevelId,0, CurrentWeapon[0].EnhancementPosition.Count-1);
      //  print("after "+EnhancementLevelId);

        WeaponEnhancementPos = Mathf.Clamp(WeaponEnhancementPos, 0, CurrentWeapon[0].EnhancementPosition.Count - 1);
        WeaponEnhancement.transform.GetChild(EnhancementLevelId).gameObject.SetActive(true);
        WeaponEnhancement.GetComponent<Enhancement>().ActiveLevel = WeaponEnhancement.transform.GetChild(EnhancementLevelId).gameObject;
        WeaponEnhancement.GetComponent<Enhancement>().ActiveLevel.transform.GetChild(0).gameObject.SetActive(true);
        WeaponEnhancement.GetComponent<Enhancement>().PreviousLevelID = EnhancementLevelId;
        WeaponEnhancement.transform.position = CurrentWeapon[0].EnhancementPosition[WeaponEnhancementPos].transform.position;
        WeaponEnhancement .transform.parent= CurrentWeapon[0].EnhancementParent;
        WeaponEnhancement.GetComponent<Enhancement>().LevelDrop.gameObject.SetActive(true);
        WeaponEnhancement.GetComponent<Enhancement>().SetLevelNoText(EnhancementLevelId);

        CurrentWeapon[0].InstantiatedWeaponEnhacements.Add(WeaponEnhancement);

    }

    public void ActivateWeaponEnhancementTemporary(int LevelNO)
    {
       
        if (EnhancementLevelIdsTemporary[WeaponEnhancementCounterTemporary] < 0)
        {
            GameObject enhancement = Instantiate(CurrentWeaponInfo.WeaponEnhancementPrefabs[WeaponEnhancementCounterTemporary], CurrentWeapon[0].EnhancementPosition[WeaponEnhancementCounterTemporary].transform.localPosition, Quaternion.identity);

            // GameObject enhancement = Instantiate(CurrentWeaponInfo.WeaponEnhancementShadowPrefabs[CurrentWeaponInfo.WeaponEnhancementCounter], CurrentWeapon.EnhancementPosition[CurrentWeaponInfo.WeaponEnhancementCounter].transform.position, Quaternion.identity);
            enhancement.GetComponent<Enhancement>().ActivateEnhancementTemporary(LevelNO, CurrentWeapon[0], CurrentWeaponInfo);



        }
        else
        {
            GameObject enhancement = CurrentWeapon[0].InstantiatedWeaponEnhacements[WeaponEnhancementCounterTemporary];
            enhancement.GetComponent<Enhancement>().ActivateEnhancementTemporary(LevelNO, CurrentWeapon[0], CurrentWeaponInfo);

        }


    }

    public void ActivateWeaponEnhancement(int LevelNO)
    {

        if (CurrentWeaponInfo.EnhancementLevelIds[CurrentWeaponInfo.WeaponEnhancementCounter]<0)
        {
            GameObject enhancement = Instantiate(CurrentWeaponInfo.WeaponEnhancementPrefabs[CurrentWeaponInfo.WeaponEnhancementCounter], CurrentWeapon[0].EnhancementPosition[CurrentWeaponInfo.WeaponEnhancementCounter].transform.localPosition, Quaternion.identity);

            // GameObject enhancement = Instantiate(CurrentWeaponInfo.WeaponEnhancementShadowPrefabs[CurrentWeaponInfo.WeaponEnhancementCounter], CurrentWeapon.EnhancementPosition[CurrentWeaponInfo.WeaponEnhancementCounter].transform.position, Quaternion.identity);
            enhancement.GetComponent<Enhancement>().ActivateEnhancement(LevelNO, CurrentWeapon[0], CurrentWeaponInfo);

         

        }
        else
        {
            GameObject enhancement =CurrentWeapon[0].InstantiatedWeaponEnhacements[CurrentWeaponInfo.WeaponEnhancementCounter];
            enhancement.GetComponent<Enhancement>().ActivateEnhancement(LevelNO, CurrentWeapon[0], CurrentWeaponInfo);
            
        }

  
    }


    public GameObject ActiveWeaponEnhacementShadow(int LevelNO)
    {

        HideLevel(CurrentWeaponInfo.WeaponEnhancementCounter);



        GameObject enhancement = Instantiate(CurrentWeaponInfo.WeaponEnhancementShadowPrefabs[CurrentWeaponInfo.WeaponEnhancementCounter], CurrentWeapon[0].EnhancementPosition[CurrentWeaponInfo.WeaponEnhancementCounter].transform.position, Quaternion.identity);
    
                enhancement.transform.GetChild(LevelNO).gameObject.SetActive(true);
            
        
       // print("activate shadow ");
        return enhancement;
    }

    public void HideLevel(int enhancecounter)
    {
        int activelevelID = CurrentWeaponInfo.EnhancementLevelIds[enhancecounter];
        if (activelevelID >= 0)
        {
            GameObject activatedlevel = CurrentWeapon[0].InstantiatedWeaponEnhacements[CurrentWeaponInfo.WeaponEnhancementCounter].transform.GetChild(activelevelID).gameObject;
            activatedlevel.SetActive(false);
            CurrentWeapon[0].InstantiatedWeaponEnhacements[CurrentWeaponInfo.WeaponEnhancementCounter].GetComponent<Enhancement>().LevelDrop.gameObject.SetActive(false);
        }
    }
    public void ShowLevel(int enhancecounter)
    {
        int activelevelID = CurrentWeaponInfo.EnhancementLevelIds[enhancecounter];
        if (activelevelID >= 0)
        {
            GameObject activatedlevel = CurrentWeapon[0].InstantiatedWeaponEnhacements[CurrentWeaponInfo.WeaponEnhancementCounter].transform.GetChild(activelevelID).gameObject;
            activatedlevel.SetActive(true);
            CurrentWeapon[0].InstantiatedWeaponEnhacements[CurrentWeaponInfo.WeaponEnhancementCounter].GetComponent<Enhancement>().LevelDrop.gameObject.SetActive(true);

        }

    }

    public void InstantiateNextWeapon(Transform InstantiatePos)
    {
      
       NextInstantiatedWeapon = Instantiate(AllWeaponsInfo[FindNextWeaponIndex()].WeaponModel, InstantiatePos.position, Quaternion.identity);
        NextInstantiatedWeapon.transform.parent = InstantiatePos;
        NextInstantiatedWeapon.transform.localScale = new Vector3(2.6f, 2.6f, 2.6f);

        NextInstantiatedWeapon.transform.DOLocalMoveY(2,1.0f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.Linear).id="YMovement";
        NextInstantiatedWeapon.transform.DOLocalRotate(new Vector3(0,20,0),2.0f).SetLoops(-1,LoopType.Incremental).SetEase(Ease.Linear).id = "YRotation";

       // print(NextInstantiatedWeapon.name);

    }

}
