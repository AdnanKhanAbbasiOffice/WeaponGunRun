using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Enhancement : MonoBehaviour
{
    public List<GameObject> AllLevels = new List<GameObject>();
    public Transform LevelDrop;
    public GameObject ActiveLevel;

    public TextMeshProUGUI LevelText;
  public int PreviousLevelID=-1;
    // Start is called before the first frame update
    void Start()
    {
        //SetLevelNoText(LevelNO);
       
    }

    // Update is called once per frame
    void Update()
    {
        //if (LevelDrop.gameObject.activeSelf)
        //{
        //    LevelDrop.transform.LookAt(Camera.main.transform);
        //}
    }
    public void ActivateEnhancement(int LevelNO, Weapon CurrentWeapon,WeaponInfo CurrentWeaponInfo)
    {

    
    

        foreach (GameObject e in AllLevels)
        {

            e.SetActive(false);
        }
        AllLevels[LevelNO].SetActive(true);


        if (ActiveLevel)
        {
            PreviousLevelID = ActiveLevel.GetComponent<EnhancementLevel>().LevelID;
          //  print("active level Previos ID : "+ PreviousLevelID);
        }
        else
        {
            PreviousLevelID = AllLevels[LevelNO].GetComponent<EnhancementLevel>().LevelID;
          //  print("Not active level Previos ID : " + PreviousLevelID);
        }

        ActiveLevel = AllLevels[LevelNO];



       transform.position = CurrentWeapon.EnhancementPosition[CurrentWeaponInfo.WeaponEnhancementCounter].transform.position;
        transform.parent = CurrentWeapon.EnhancementParent;
      // StartCoroutine( setpos(LevelNO,CurrentWeapon,CurrentWeaponInfo));
        SetLevelNoText(LevelNO);

        if (CurrentWeaponInfo.EnhancementLevelIds[CurrentWeaponInfo.WeaponEnhancementCounter] < 0)
        {
          //  print("add enhancement instantiated list");
            CurrentWeapon.InstantiatedWeaponEnhacements.Add(gameObject);

        }
        CurrentWeaponInfo.EnhancementLevelIds[CurrentWeaponInfo.WeaponEnhancementCounter] = LevelNO;

        CurrentWeaponInfo.WeaponEnhancementCounter++;

    //   StartCoroutine(CheckLimit(CurrentWeaponInfo));

    }
    IEnumerator CheckLimit(WeaponInfo currentweaponinfo)
    {
        yield return new WaitForSeconds(2.0f);
        if (currentweaponinfo.WeaponEnhancementCounter >= currentweaponinfo.EnhancementLevelIds.Count)
        {
            currentweaponinfo.WeaponEnhancementCounter = 0;
            // print("Level reverse");
        }
    }
    public void ActivateEnhancementTemporary(int LevelNO, Weapon CurrentWeapon, WeaponInfo CurrentWeaponInfo)
    {


        LevelDrop.gameObject.SetActive(false);

        foreach (GameObject e in AllLevels)
        {

            e.SetActive(false);
        }
        AllLevels[LevelNO].SetActive(true);


        if (ActiveLevel)
        {
            PreviousLevelID = ActiveLevel.GetComponent<EnhancementLevel>().LevelID;

        }
        else
        {
            PreviousLevelID = AllLevels[LevelNO].GetComponent<EnhancementLevel>().LevelID;
        }

        ActiveLevel = AllLevels[LevelNO];

        transform.position = CurrentWeapon.EnhancementPosition[GameManager.Instance.weaponManager.WeaponEnhancementCounterTemporary].transform.position;
        transform.parent = CurrentWeapon.EnhancementParent;
        // StartCoroutine( setpos(LevelNO,CurrentWeapon,CurrentWeaponInfo));
        SetLevelNoText(LevelNO);

        if (GameManager.Instance.weaponManager.EnhancementLevelIdsTemporary[GameManager.Instance.weaponManager.WeaponEnhancementCounterTemporary] < 0)
        {
          //  print("add enhancement instantiated list");
            CurrentWeapon.InstantiatedWeaponEnhacements.Add(gameObject);

        }
        GameManager.Instance.weaponManager.EnhancementLevelIdsTemporary[GameManager.Instance.weaponManager.WeaponEnhancementCounterTemporary] = LevelNO;

        GameManager.Instance.weaponManager.WeaponEnhancementCounterTemporary++;

        if (GameManager.Instance.weaponManager.WeaponEnhancementCounterTemporary >= GameManager.Instance.weaponManager.EnhancementLevelIdsTemporary.Count)
        {
            GameManager.Instance.weaponManager.WeaponEnhancementCounterTemporary = 0;
           // print("Level reverse");
        }

    }

    public IEnumerator setpos(int LevelNO, Weapon CurrentWeapon, WeaponInfo CurrentWeaponInfo)
    {
        yield return new WaitForSeconds(5.0f);
        AllLevels[LevelNO].transform.position = CurrentWeapon.EnhancementPosition[CurrentWeaponInfo.WeaponEnhancementCounter].transform.position;
        transform.parent = CurrentWeapon.EnhancementParent;
    }
    public void SetLevelNoText(int levelno)
    {
        LevelText.gameObject.SetActive(true);
        LevelText.text = "Lv." + (levelno + 1);
    }
}
