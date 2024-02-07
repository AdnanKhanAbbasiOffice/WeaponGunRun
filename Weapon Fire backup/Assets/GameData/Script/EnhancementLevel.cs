using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EnhancementLevel : MonoBehaviour
{
   

    [HideInInspector]public Enhancement _Enhancement;
    public int LevelID = 0;
    // [SerializeField] internal float NewLevel;

    internal float PreviousFireRate;
   internal float PreviousFireRange;


    internal float NewFireRate;
   internal float NewFireRange;


    private void Awake()
    {
        _Enhancement = transform.parent.GetComponent<Enhancement>();
        transform.GetChild(0).gameObject.SetActive(false);


       
    }
    // Start is called before the first frame update
    void Start()
    {
      //  print("start");
  // SetNewLevelFeatures();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void SavePreviousFeatures()
    {
        PreviousFireRate = GameManager.Instance.weaponManager.CurrentWeaponInfo.FireRateUI;
        PreviousFireRange = GameManager.Instance.weaponManager.CurrentWeaponInfo.FireRangeUI;

    }
    public void SetNewLevelFeatures()
    {
        //  GameManager.Instance.weaponManager.CurrentWeaponInfo.FirePower += firepower;
        //weaponManager.CurrentWeaponInfo.FireRate += firerate;
        //weaponManager.CurrentWeaponInfo.FireRange += firerange;

        //uiManager.gamePlay.WeaponNameText.text = weaponManager.CurrentWeaponInfo.WeaponName.ToString();
        //uiManager.gamePlay.FirePowerText.text = weaponManager.CurrentWeaponInfo.FirePower.ToString();
        //uiManager.gamePlay.FireRateText.text = weaponManager.CurrentWeaponInfo.FireRate.ToString();

        GameManager.Instance.uiManager.gamePlay.PreviousLevelText.text = "+" + (_Enhancement.PreviousLevelID+1);
        GameManager.Instance.uiManager.gamePlay.PreviousFireRateText.text = "+" + PreviousFireRate;
        GameManager.Instance.uiManager.gamePlay.PreviousFireRangeText.text = "+" +PreviousFireRange ;


        GameManager.Instance.uiManager.gamePlay.NewLevelText.text = "+"+ (LevelID+1);
        GameManager.Instance.uiManager.gamePlay.NewFireRateText.text = "+" + GameManager.Instance.weaponManager.CurrentWeaponInfo.FireRateUI;
        GameManager.Instance.uiManager.gamePlay.NewFireRangeText.text = "+" + GameManager.Instance.weaponManager.CurrentWeaponInfo.FireRangeUI;



    }

   
}
