using DG.Tweening;
using MoreMountains.NiceVibrations;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System;

public class GameManager : MonoBehaviour
{
    #region Variables
    public static GameManager Instance;
    public AdManager AdManagerInstance;
    public PlayerController playerController;
    public CameraControll _CameraControll;
    public bool IsGameStart = false;
    [Header("Other")]
    [SerializeField] internal AudioManager audioManager;
    [SerializeField] internal UiManager uiManager;
    [Header("LevelsSystem")]
    [SerializeField] internal LevelManager[] levelManager;
    [SerializeField] internal WeaponManager weaponManager;
    [SerializeField] internal CompanionManager companionManager;
    [SerializeField] internal KeyChainManager keyChainManager;
    [SerializeField] internal TutorialController tutorialController;
    public GridMap _GridMap;
   [HideInInspector] public bool IsFireStart = false;
    internal int totalTarget = 0;
    internal int achiveTarget = 0;
    private IEnumerator FireCoroutine;

    public bool IsFinishLineCross;
    #endregion Variables
    #region ReturnVariables

    public int CurrentWeapon
    {
        get
        {
            return PlayerPrefs.GetInt("CurrentWeapon",0);
        }
        set
        {
            value = Mathf.Clamp(value, 0, weaponManager.AllWeaponsInfo.Length - 1);
            PlayerPrefs.SetInt("CurrentWeapon", value);
        }

    }
    public int CurrentKeyChain
    {
        get
        {
            return PlayerPrefs.GetInt("CurrentKeyChain", 0);
        }
        set
        {
           
            PlayerPrefs.SetInt("CurrentKeyChain", value);
        }

    }
    public int CurrentCompanion
    {
        get
        {
            return PlayerPrefs.GetInt("CurrentCompanion", -1);
        }
        set
        {
            PlayerPrefs.SetInt("CurrentCompanion", value);
        }

    }
    public int currentLevel
    {
        get { return PlayerPrefs.GetInt("currentLevel") % levelManager.Length; }
        set { PlayerPrefs.SetInt("currentLevel", value); }
    }
    public int chestPercentage
    {
        get { return PlayerPrefs.GetInt("chestPercentage"); }
        set { PlayerPrefs.SetInt("chestPercentage", value); }
    }
    public float totalCash
    {
        get { return PlayerPrefs.GetFloat("totalCash",0); }
        set {
       
            PlayerPrefs.SetFloat("totalCash", value);

            if(PlayerPrefs.GetFloat("totalCash")<0)
            {
                PlayerPrefs.SetFloat("totalCash", 0);
            }
  

        }
    }
    public float LevelPrice
    {
        get { return PlayerPrefs.GetFloat("LevelPrice",30); }
        set { PlayerPrefs.SetFloat("LevelPrice", value); }
    }
    public bool music
    {
        get { return (PlayerPrefs.GetString("Music", "On") == "On") ? true : false; }
        set
        {
            if (value)
                PlayerPrefs.SetString("Music", "Off");
            else
                PlayerPrefs.SetString("Music", "On");


            GlobalAudioPlayer.MusicOnOff();
        }
    }
    public bool sound
    {
        get { return (PlayerPrefs.GetString("Sound", "On") == "On") ? true : false; }
        set { if (value) PlayerPrefs.SetString("Sound", "On"); else PlayerPrefs.SetString("Sound", "Off"); }
    }
    public bool vibration
    {
        get { return (PlayerPrefs.GetString("vibration", "On") == "On") ? true : false; }
        set { if (value) PlayerPrefs.SetString("vibration", "On"); else PlayerPrefs.SetString("vibration", "Off"); }
    }
    public int offlineReward
    {
        get
        {
            return PlayerPrefs.GetInt("HealthPlayer");
        }
        set
        {
            PlayerPrefs.SetInt("HealthPlayer", value);
        }

    }
    public int PowerPlayer
    {
        get
        {
            return PlayerPrefs.GetInt("PowerPlayer");
        }
        set
        {
            PlayerPrefs.SetInt("PowerPlayer", value);
        }

    }

  
    
    #endregion ReturnVariables
    #region UnityFunction
    private void Awake()
    {
        Instance = this;
        Initilize();

    }
    private void Start()
    {
        GlobalAudioPlayer.PlayMusic("Music");

    }
    #endregion UnityFunction
    #region CustomFunction

    private void Initilize()
    {
        //  print(currentLevel);

        AdManagerInstance = FindObjectOfType<AdManager>();

        totalTarget = levelManager[currentLevel].totalTarget;
        Instantiate(levelManager[currentLevel].levelPreb);
        ActivateWeapon(CurrentWeapon);
        SetWeaponFeatureUI();
        AdManagerInstance.ActivateConsentButton();
     //  SetWeaponUIFeatureLevelWise();
        TestControllerValues("initialize");
        LevelPriceButtonStatus();
        if (levelManager[currentLevel].IsLevelBeforeTutorial)
        {

            uiManager.gamePlay.WeaponEnhacemenetPanel.SetActive(false);
           var temp= playerController.transform.position;
            temp.x = 0;
            temp.z = 10;
            playerController.transform.position = temp;

            uiManager.menu.panel.SetActive(true);
            _GridMap.gameObject.SetActive(false);

            _CameraControll.PlayerCamera.SetActive(true);
            _CameraControll.GridCamera.SetActive(false);

            _CameraControll.PlayerCamera.transform.localPosition = _CameraControll.MainCameraPos.transform.localPosition;
            _CameraControll.PlayerCamera.transform.localEulerAngles = _CameraControll.MainCameraPos.transform.localEulerAngles;

            playerController.WeaponRotation.SetActive(false);
           // _CameraControll.PlayerCamera.SetActive(true);

            _CameraControll.Initializ(playerController);
      
        }
        else
        {
            var temp = playerController.transform.position;
            temp.x = -3;
            playerController.transform.position = temp;
            _GridMap.gameObject.SetActive(true);
            _CameraControll.PlayerCamera.transform.localPosition = _CameraControll.GridCameraPos.localPosition;
            _CameraControll.PlayerCamera.transform.localEulerAngles = _CameraControll.GridCameraPos.localEulerAngles;

            _CameraControll.PlayerCamera.SetActive(true);
            _CameraControll.GridCamera.SetActive(true);
            //  _CameraControll.PlayerCamera.SetActive(false);

            _CameraControll.Initializ(playerController);
            uiManager.gamePlay.WeaponEnhacemenetPanel.SetActive(true);


            if (levelManager[currentLevel].IsTutorialLevel)
            {
                playerController.WeaponRotation.SetActive(false);
            }
            else
            {
                playerController.WeaponRotation.SetActive(true);
            }



        }


    }
    public bool IsBattleON;
    public void TapToPlay()
    {
       
        IsGameStart = true;
        FireStatus(true);
        if(companionManager.CurrentCompanion)
        {
            StartCoroutine(companionManager.CurrentCompanion.Fire());

        }
        StartCoroutine(Fire(0.0f));
    }
    public void Battle()
    {
       
        IsBattleON = true;
      uiManager.gamePlay.WeaponEnhacemenetPanel.SetActive(false);
        playerController.WeaponRotation.SetActive(false);
        DeactiveAllEnhancementCanvas();
        _CameraControll.GridCamera.SetActive(false);
        _CameraControll.PlayerCamera.GetComponent<Animator>().enabled = true;

        playerController.transform.DOMove(new Vector3(0, 0, 10), 1f).OnComplete(() =>
        {

            //_CameraControll.PlayerCamera.GetComponent<Animator>().enabled = false ;
            //  playerController.transform.GetChild(0).DOMove(new Vector3(0, 90, 0), 0.5f);

            //   _CameraControll.GridCamera.SetActive(false);
            // _CameraControll.PlayerCamera.SetActive(true);
            //uiManager.gamePlay.WeaponEnhacemenetPanel.SetActive(false);
            //TapToPlay();
            ActivateCompanion(CurrentCompanion);
            ActivateKeyChain(CurrentKeyChain);
      
            // weaponManager.WeaponEnhancementCounterTemporary = weaponManager.CurrentWeaponInfo.WeaponEnhancementCounter;
        }).OnStart(()=> {
           WeaponRotate(new Vector3(0, 0, 360), 0.6f, 0.6f);
            //playerController.transform.GetChild(0).DOScale(new Vector3(1f, 1f, 1f), 0.5f).SetDelay(0.6f).OnStart(() => {

            //   // playerController.transform.GetChild(0).DOLocalRotate(new Vector3(0,360,0),0.6f).SetRelative(true);
     
            //});
            

        }) ;
        //print("start battle");




        // Log an event with a string parameter.
        FirebaseEvents("level_start");

    }
    public void ShowKeyChainPanel()
    {
        keyChainManager.AssignKeyChainValues();
        uiManager.levelComplete.KeyChanePanel.SetActive(true);

    }
    public void GetMultipliereScore()
    {

        keyChainManager.GetMultipliereScore();
     
       
    }
    public void NoThanksMultiplier()
    {
       // print("NoThanks");
        uiManager.levelComplete.RewardPanel.SetActive(true);
       // uiManager.levelComplete.KeyChanePanel.SetActive(false);
  
    }
    public void GameComplete()
    {

        StartCoroutine(uiManager.GameComplete(0.1f));
        // Log an event with a string parameter.
        FirebaseEvents("level_complete");
    }
    public void GameOver()
    {
        StartCoroutine(uiManager.GameOver(1f));
    }
    public void Vibration(HapticTypes type)
    {

    //   GameManager.Instance.Vibration(MoreMountains.NiceVibrations.HapticTypes.Selection);
        if (vibration)
            MMVibrationManager.Haptic(type);
    }
    public void CreateNewWeaponEnhancementLevel()
    {
        if(totalCash>=LevelPrice)
        {

            if (_GridMap.IsGridEmpty())
            {
                return;
            }


            _GridMap.InstantiateLevel(0, 0, false);

            uiManager.AddCashUpdate(-LevelPrice);
            FirebaseEvents("spend_virtual_currency", "EnhancementLevel", LevelPrice + "");

            SetLevelPrice();
        }
        else
        {

            print("Low Cash to Buy");
        }


      

        if (levelManager[currentLevel].IsTutorialLevel)
        {

            tutorialController.Steps(1);
        }
    }

    public void SetWeaponAsChild(Transform weapon)
    {
       
        playerController.SetWeaponAsChild(weapon);

    }
    public void ActivateWeapon(int index)
    {
        weaponManager.ActivateWeapon(index);

    }
    public void ActivateCompanion(int index)
    {
        //print("companion id "+ index);
        companionManager.ActivateCompanion(index);

    }
    public void ActivateKeyChain(int index)
    {
        keyChainManager.ActivateKeyChain(index);

    }
    public void FireStatus(bool status)
    {
        IsFireStart = status;
    
    }
   
    public IEnumerator Fire(float delay=0.5f)
    {
        yield return new WaitForSeconds(delay);

        StartCoroutine(weaponManager.Fire());
    
    }
    public GameObject ActiveWeaponEnhacementShadow(int LevelNO)
    {
       return weaponManager.ActiveWeaponEnhacementShadow(LevelNO);
    }
    public void ActivateWeaponEnhancement(int LevelNO)
    {


     //   print("ActivateWeaponEnhancement");


        weaponManager.ActivateWeaponEnhancement(LevelNO);

        if(weaponManager.EnhacementModelForRendererTexture)
        {
            Destroy(weaponManager.EnhacementModelForRendererTexture);
        }
        if (keyChainManager.KeyChainModelForRendererTexture)
        {
            Destroy(keyChainManager.KeyChainModelForRendererTexture);
        }
      //  print("errror : "+weaponManager.CurrentWeapon[0].InstantiatedWeaponEnhacements[weaponManager.CurrentWeaponInfo.WeaponEnhancementCounter - 1]);
        //  print("Level No : "+ LevelNO);
        weaponManager.EnhacementModelForRendererTexture = Instantiate(weaponManager.CurrentWeapon[0].InstantiatedWeaponEnhacements[weaponManager.CurrentWeaponInfo.WeaponEnhancementCounter - 1], weaponManager.EnhacementPosForRendererTexture.position,Quaternion.identity);
        weaponManager.EnhacementModelForRendererTexture.GetComponent<Enhancement>().ActiveLevel.transform.GetChild(0).gameObject.SetActive(true);

        weaponManager.EnhacementModelForRendererTexture.GetComponent<Enhancement>().ActiveLevel.GetComponent<EnhancementLevel>().SavePreviousFeatures();
        SetWeaponUIFeatureLevelWise();
        weaponManager.EnhacementModelForRendererTexture.GetComponent<Enhancement>().ActiveLevel.GetComponent<EnhancementLevel>().SetNewLevelFeatures();

        weaponManager.EnhancementRendererCamera.Animate();
        uiManager.gamePlay.WeaponNewEnhacemenetLevelPanel.SetActive(true);
        GameManager.Instance.PlaySound("EnhancementActivate");
        GameManager.Instance.Vibration(MoreMountains.NiceVibrations.HapticTypes.Success);


    }


    public void ActivateKeyChainRenderTexture()
    {
        if(keyChainManager.KeyChainModelForRendererTexture)
        {
            Destroy(keyChainManager.KeyChainModelForRendererTexture);
        }
        if (weaponManager.EnhacementModelForRendererTexture)
        {
            Destroy(weaponManager.EnhacementModelForRendererTexture);
        }
        keyChainManager.KeyChainModelForRendererTexture = Instantiate(keyChainManager.currentKeyChainInfo.KeyChain.gameObject, keyChainManager.KeyChainPosForRendererTexture.position, Quaternion.identity);
        keyChainManager.RendererTextureCamera.AnimateKeyChain();
    }
    public void EquipeNewEnhancementLevel()
    {
       // print("EquipeNewEnhancementLevel");

        Enhancement currentenhacement=weaponManager.CurrentWeapon[0].InstantiatedWeaponEnhacements[weaponManager.CurrentWeaponInfo.WeaponEnhancementCounter - 1].GetComponent<Enhancement>();
        currentenhacement.ActiveLevel.transform.GetChild(0).gameObject.SetActive(true);
        currentenhacement.LevelDrop.gameObject.SetActive(true);
     
       
        uiManager.gamePlay.WeaponNewEnhacemenetLevelPanel.SetActive(false);
        weaponManager.CurrentWeapon[0].transform.DOScale(new Vector3(0.2f,0.2f,0.2f),0.5f).SetEase(Ease.InOutBack).SetRelative(true).SetDelay(0.2f).OnComplete(()=> {
          
            weaponManager.CurrentWeapon[0].transform.DOScale(new Vector3(-0.2f, -0.2f, -0.2f), 0.2f).SetEase(Ease.Linear).SetRelative(true).SetDelay(0.2f);

        }).OnStart(()=> { 
        
            Destroy(Instantiate(weaponManager.WeaponEnhancementParticle, weaponManager.WeaponEnhancementParticlePos.position, Quaternion.identity),3.0f);
            
            GameManager.Instance.PlaySound("WeaponThunder");

        });

        if (weaponManager.CurrentWeaponInfo.WeaponEnhancementCounter >= weaponManager.CurrentWeaponInfo.EnhancementLevelIds.Count)
        {
            weaponManager.CurrentWeaponInfo.WeaponEnhancementCounter = 0;
            // print("Level reverse");
        }
    }
    public void DeactiveAllEnhancementCanvas()
    {
     for(int i=0;i< weaponManager.CurrentWeapon[0].InstantiatedWeaponEnhacements.Count;i++)
        {
            weaponManager.CurrentWeapon[0].InstantiatedWeaponEnhacements[i].GetComponent<Enhancement>().LevelDrop.gameObject.SetActive(false);
        }
    }
    public void SetWeaponFireRate(char Operator, float Value)
    {

        weaponManager.SetWeaponFireRate(Operator,Value);

    }
    public void SetWeaponFireRange(char Operator, float Value)
    {

        weaponManager.SetWeaponFireRange(Operator, Value);

    }
    public void SetWeaponFirePower(char Operator, float Value)
    {

        weaponManager.SetWeaponFirePower(Operator, Value);

    }
    public void SetWeaponEnhancementTemporary(int NoOfEnhancements=1)
    {
     //   int enhancementcounter = 0;
        // print("set weapon enhancement");

        weaponManager.SetTemporaryWeaponValues();
     

        for (int i=0;i< NoOfEnhancements; i++)
        {

            int EnhancementNo=weaponManager.WeaponEnhancementCounterTemporary;

          //  print("EnhancementNo : "+ EnhancementNo);
            int LevelNo = weaponManager.EnhancementLevelIdsTemporary[EnhancementNo];

            LevelNo++;
            LevelNo = Mathf.Clamp(LevelNo, 0, weaponManager.EnhancementLevelIdsTemporary.Count - 1);
          //  print("Enhance LevelNO : " + LevelNo + " ,, Enhancment Part : " + EnhancementNo);
            weaponManager.ActivateWeaponEnhancementTemporary(LevelNo);

            Enhancement currentenhacement = weaponManager.CurrentWeapon[0].InstantiatedWeaponEnhacements[EnhancementNo].GetComponent<Enhancement>();
            currentenhacement.ActiveLevel.transform.GetChild(0).gameObject.SetActive(true);
            uiManager.gamePlay.WeaponNewEnhacemenetLevelPanel.SetActive(false);

        }
    }
 
    public void WeaponRotate(Vector3 angle, float delay, float setdely=0)
    {
        weaponManager.WeaponRotate(angle,delay,setdely);
    }
 

    public void SetWeaponFeatureUI()
    {
   
       

        //weaponManager.CurrentWeaponInfo.FirePower += firepower;
        //weaponManager.CurrentWeaponInfo.FireRate += firerate;
        //weaponManager.CurrentWeaponInfo.FireRange += firerange;

        uiManager.gamePlay.WeaponNameText.text = weaponManager.CurrentWeaponInfo.WeaponName.ToString();
        //uiManager.gamePlay.FirePowerText.text = weaponManager.CurrentWeaponInfo.FirePower.ToString();
        //uiManager.gamePlay.FireRateText.text = weaponManager.CurrentWeaponInfo.FireRate.ToString();
        //uiManager.gamePlay.FireRangeText.text = weaponManager.CurrentWeaponInfo.FireRange.ToString();

        //uiManager.gamePlay.FirePowerSlider.value = weaponManager.CurrentWeaponInfo.FirePower+firepower;
        //uiManager.gamePlay.FireRateSlider.value = weaponManager.CurrentWeaponInfo.FireRate+firerate;
        //uiManager.gamePlay.FireRangeSlider.value = weaponManager.CurrentWeaponInfo.FireRange+firerange;

        uiManager.gamePlay.FirePowerSlider.value = weaponManager.CurrentWeaponInfo.FirePowerUI;
        uiManager.gamePlay.FireRateSlider.value = weaponManager.CurrentWeaponInfo.FireRateUI;
        uiManager.gamePlay.FireRangeSlider.value = weaponManager.CurrentWeaponInfo.FireRangeUI;
    }

    public void SetWeaponUIFeatureLevelWise()
    {
        //double roundedNumber = Math.Round(3.435f, 2);
        //float f = (float)Mathf.Round(2.331f);

        float power = weaponManager.CurrentWeaponInfo.FirePowerUI + weaponManager.CurrentWeaponInfo.FirePowerIncrementUI;
        weaponManager.CurrentWeaponInfo.FirePowerUI = (float)Math.Round(power, 1);

        float rate = weaponManager.CurrentWeaponInfo.FireRateUI +weaponManager.CurrentWeaponInfo.FireRateIncrementUI;
        weaponManager.CurrentWeaponInfo.FireRateUI = (float)Math.Round(rate, 1);

        float range = weaponManager.CurrentWeaponInfo.FireRangeUI + weaponManager.CurrentWeaponInfo.FireRangeIncrementUI;
        weaponManager.CurrentWeaponInfo.FireRangeUI = (float)Math.Round(range, 1);
        //weaponManager.CurrentWeaponInfo.FireRangeUI += weaponManager.CurrentWeaponInfo.FireRangeIncrementUI;

        // uiManager.gamePlay.WeaponNameText.text = weaponManager.CurrentWeaponInfo.WeaponName.ToString();
        //uiManager.gamePlay.FirePowerText.text = weaponManager.CurrentWeaponInfo.FirePower.ToString();
        //uiManager.gamePlay.FireRateText.text = weaponManager.CurrentWeaponInfo.FireRate.ToString();
        //uiManager.gamePlay.FireRangeText.text = weaponManager.CurrentWeaponInfo.FireRange.ToString();

        //uiManager.gamePlay.FirePowerSlider.value = weaponManager.CurrentWeaponInfo.FirePowerUI;
        //uiManager.gamePlay.FireRateSlider.value = weaponManager.CurrentWeaponInfo.FireRateUI;
        //uiManager.gamePlay.FireRangeSlider.value = weaponManager.CurrentWeaponInfo.FireRangeUI;

        SetWeaponFeatureUI();

    }








    public void GetWeaponFeatures()
    {
       
        //uiManager.gamePlay.WeaponNameText.text = weaponManager.CurrentWeaponInfo.WeaponName.ToString();

        //uiManager.gamePlay.FirePowerText.text = weaponManager.CurrentWeaponInfo.FirePower.ToString();
        //uiManager.gamePlay.FireRateText.text = weaponManager.CurrentWeaponInfo.FireRate.ToString();
        //uiManager.gamePlay.FireRangeText.text = weaponManager.CurrentWeaponInfo.FireRange.ToString();

        //uiManager.gamePlay.FirePowerSlider.value = weaponManager.CurrentWeaponInfo.FirePower;
        //uiManager.gamePlay.FireRateSlider.value = weaponManager.CurrentWeaponInfo.FireRate;
        //uiManager.gamePlay.FireRangeSlider.value = weaponManager.CurrentWeaponInfo.FireRange;

    }
    public void SettingsButton(bool IsActivate)
    {
        if(IsActivate)
        {
            uiManager.gamePlay.SettingsPanel.SetActive(true);
            Time.timeScale = 0;
        }
        else
        {
            uiManager.gamePlay.SettingsPanel.SetActive(false);
            Time.timeScale = 1;
        }
    
      
  
    }
    public void TestControllerValues(string valuename)
    {

        if(valuename== "MaxSvirve")
        {
            playerController.maxSwerveAmount = uiManager.gamePlay.MaxSwirveSlider.value;
            uiManager.gamePlay.MaxSwirveSliderText.text = uiManager.gamePlay.MaxSwirveSlider.value.ToString();
        }
        else if(valuename == "HorizontalSpeed ")
        {
            playerController.HorizontalSped = uiManager.gamePlay.HorizontaSpeedSlider.value;
            uiManager.gamePlay.HorizontalSpeedSliderText.text = uiManager.gamePlay.HorizontaSpeedSlider.value.ToString();

        }
        else if (valuename == "DragThreshold")
        {
            playerController.DragThreshold = uiManager.gamePlay.DragThresholdSlider.value;
            uiManager.gamePlay.DragThresholdSliderText.text = uiManager.gamePlay.DragThresholdSlider.value.ToString();

        }
        else
        {
          //  print("test sliders");
            uiManager.gamePlay.MaxSwirveSlider.value = playerController.maxSwerveAmount;
            uiManager.gamePlay.MaxSwirveSliderText.text = uiManager.gamePlay.MaxSwirveSlider.value.ToString();

            uiManager.gamePlay.HorizontaSpeedSlider.value = playerController.HorizontalSped;
            uiManager.gamePlay.HorizontalSpeedSliderText.text = uiManager.gamePlay.HorizontaSpeedSlider.value.ToString();

            uiManager.gamePlay.DragThresholdSlider.value = playerController.DragThreshold;
            uiManager.gamePlay.DragThresholdSliderText.text = uiManager.gamePlay.DragThresholdSlider.value.ToString();

        }


    }
    public void ActivateDualWeapon(int TotalDualGates=1)
    {

      FireStatus(false);
      
     playerController.ActivatePlayerParticle(0);
   



        float Gap = GameManager.Instance.weaponManager.Gap;
       
        for (int w = 1; w <= TotalDualGates; w++)
        {

            GameObject DualWeapon = Instantiate(weaponManager.CurrentWeapon[0].gameObject, weaponManager.CurrentWeapon[0].transform.position, Quaternion.identity);
           SetWeaponAsChild(DualWeapon.transform);
          weaponManager.CurrentWeapon.Add(DualWeapon.GetComponent<Weapon>());

            int TotalWeapons = weaponManager.CurrentWeapon.Count;



            if (TotalWeapons % 2 == 0)
            {
               weaponManager.Weaponpaircounter++;
                Gap = weaponManager.Weaponpaircounter * DualWeapon.GetComponent<Weapon>().DualWeaponGap;
              weaponManager.Gap = Gap;
                DualWeapon.transform.DOLocalMoveX(weaponManager.CurrentWeapon[0].transform.localPosition.x + (Gap), 0.3f).SetEase(Ease.Linear);

            }
            else
            {
                DualWeapon.transform.DOLocalMoveX(weaponManager.CurrentWeapon[0].transform.localPosition.x - (Gap), 0.3f).SetEase(Ease.Linear);

            }

            Weapon CurrentWeapon = DualWeapon.GetComponent<Weapon>();

            foreach (GameObject i in CurrentWeapon.InstantiatedWeaponEnhacements)
            {
                i.GetComponent<Enhancement>().ActiveLevel.transform.GetChild(0).gameObject.SetActive(true);
            }

        }

       weaponManager.CurrentWeapon[0].transform.parent.DOPunchScale(new Vector3(0.5f, 0.5f, 0.5f), 0.5f, 5, 0.5f).OnComplete(() => {
            FireStatus(true);
        });
    }
    #endregion CustomFunction
    #region ButtonsEvents
    public void PlaySound(string name)
    {
   
        GlobalAudioPlayer.Play(name);
    }
    public void Replay()
    {
        SceneManager.LoadScene(1);
    }
    public IEnumerator Next(float delay=0.0f)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(1);
    }
    public void DoTweenRestart(string value)
    {
        DOTween.Restart(value);
    }

    public void RemoveAds()
    {

        print("Remove Ads");
    }
    public void ClaimKeyChain()
    {
        GameComplete();
       // GameManager.Instance.uiManager.levelComplete.KeyChaneUnloackedPanel.SetActive(true);
    }
    public void ClaimNewWeapon()
    {
        uiManager.levelComplete.NewWeaponUnloackedPanel.SetActive(false);
        CurrentWeapon++;
       // print("weapon coutner : "+ CurrentWeapon);
  ShowKeyChainPanel();
      //  GameComplete();

    }
    public void SetLevelPrice()
    {
        LevelPrice += 10;

        uiManager.gamePlay.LevelPriceText.text = "$"+LevelPrice;

        LevelPriceButtonStatus();
    }
    public void LevelPriceButtonStatus()
    {

        if (totalCash >= LevelPrice)
        {
            uiManager.gamePlay.NewLevelButton.SetActive(true);
            uiManager.gamePlay.NewLevelInAppButton.SetActive(false);

        }
        else
        {
            uiManager.gamePlay.NewLevelButton.SetActive(false);
            uiManager.gamePlay.NewLevelInAppButton.SetActive(true);

        }

    }
  
    public void ShopButton()
    {
        uiManager.gamePlay.ShopPanel.SetActive(true);
    }
    public void ShopCloseButton()
    {
        uiManager.gamePlay.ShopPanel.SetActive(false);
    }
    public void InventoryButton()
    {
        uiManager.gamePlay.InventoryPanel.SetActive(true);

    }
    public void Settings()
    {

        if(uiManager.menu.Settings.isOn)
        {
          //  print("down");
           // uiManager.gamePlay.SettingsPanel.GetComponent<DOTweenAnimation>().DORestartById("Down");

             uiManager.gamePlay.SettingsPanel.transform.DOLocalMoveY(-210, 0.2f);

        }
        else
        {
           // print("up");
            //uiManager.gamePlay.SettingsPanel.GetComponent<DOTweenAnimation>().DORestartById("Up");

            uiManager.gamePlay.SettingsPanel.transform.DOLocalMoveY(520, 0.2f);
        }

    }
    public void ONOFFUI()
    {

        if(uiManager.gamePlay.CanvasPanel.activeSelf)
        {
            //uiManager.gamePlay.panel.SetActive(false);
            //uiManager.gamePlay.TopPanel.SetActive(false);
            //  uiManager.gamePlay.WeaponEnhacemenetPanel.SetActive(false);
            
                uiManager.gamePlay.CanvasPanel.SetActive(false);
        }
        else
        {
            //uiManager.gamePlay.panel.SetActive(true);
            //uiManager.gamePlay.TopPanel.SetActive(true);

            uiManager.gamePlay.CanvasPanel.SetActive(true);
        }


    }

    public void InAppsBuy(string InAppName)
    {
        if (InAppName == "NewLevel")
        {
            if (_GridMap.IsGridEmpty())
            {
                return;
            }


            _GridMap.InstantiateLevel(0, 0, false);
            SetLevelPrice();
        }
        else if (InAppName == "")
        {

        }
    }

    public void RewardPlayer(string RewardName)
    {

        if (RewardName == "NewLevel")
        {
            if (_GridMap.IsGridEmpty())
            {
                return;
            }


            _GridMap.InstantiateLevel(0, 0, false);
            SetLevelPrice();
        }
        else if (RewardName == "GetMultipliereScore")
        {
            GetMultipliereScore();
        }
    }


    #region Ads Methods

    public void ShowBanner()
    {
        AdManagerInstance.ShowBanner();
    }
    public void HideBanner()
    {
        AdManagerInstance.HideBanner();

    }
    public void DisplayBanner()
    {
        AdManagerInstance.DisplayBanner();
    }
    public void DestroyBanner()
    {
        AdManagerInstance.DestroyBanner();
    }

    public void LoadInterstitial()
    {

        AdManagerInstance.LoadInterstitial();
    }
    public void ShowInterstitial()
    {
        AdManagerInstance.ShowInterstitial();
    }

    public void ShowRewarded(string reward = "")
    {
        AdManagerInstance.ShowRewarded(reward);
    }

    public void ShowConsent()
    {
        AdManagerInstance.ShowConsent();
    }
    #endregion


    public void FirebaseEvents(string EventName = "", string ValueTitle = "", string Value = "")
    {
        AdManagerInstance.FirebaseEvents(EventName, ValueTitle, Value);

    }

    #endregion ButtonsEvents

}
