using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class KeyChainManager : MonoBehaviour
{
    public bool AllKeyChainsUnloacked;
    public List<KeyChainInfo> AllKeyChains = new List<KeyChainInfo>();
    public KeyChainInfo currentKeyChainInfo;
    public KeyChainInfo ActivatedKeyChainInfo;
    public KeyChain CurrentActivatedKeyChain;
    [SerializeField]
    float MultiplierScore;

    public Transform KeyChainPosForRendererTexture;
    public RendererCamera RendererTextureCamera;
    [HideInInspector]
    public GameObject KeyChainModelForRendererTexture;
    public GameObject Confitti;
    // Start is called before the first frame update
    void Start()
    {
       // print("current keychain counter : "+GameManager.Instance.CurrentKeyChain);

       

        if (GameManager.Instance.CurrentKeyChain >= AllKeyChains.Count)
        {
            GameManager.Instance.CurrentKeyChain = AllKeyChains.Count;
        }
            if (GameManager.Instance.CurrentKeyChain > 0)
        {
            ActivatedKeyChainInfo = AllKeyChains[GameManager.Instance.CurrentKeyChain - 1];
        }

        if (GameManager.Instance.CurrentKeyChain>= AllKeyChains.Count)
        {
            AllKeyChainsUnloacked = true;
      
            currentKeyChainInfo = AllKeyChains[GameManager.Instance.CurrentKeyChain-1];
            
          //  return;
        }
        else
        {
            currentKeyChainInfo = AllKeyChains[GameManager.Instance.CurrentKeyChain];

        }
        GameManager.Instance.uiManager.levelComplete.KeyChaneFiller.sprite = currentKeyChainInfo.KeyChainSprite;
        GameManager.Instance.uiManager.levelComplete.KeyChaneFillerBG.sprite = currentKeyChainInfo.KeyChainSprite;

        //AssignKeyChainValues();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ActivateKeyChain(int index)
    {

        if (GameManager.Instance.CurrentKeyChain == 0)
        {
         
               // print("current keychain locked " + index);
                return;
          
        }

        //if (!currentKeyChainInfo || !AllKeyChains[GameManager.Instance.CurrentKeyChain].IsActivated)
        //{
        //    print("current keychain locked " + index);
        //    return;
        //}
        //if (AllKeyChains[GameManager.Instance.CurrentKeyChain].IsActivated)
        //{

        //    currentKeyChainInfo = AllKeyChains[GameManager.Instance.CurrentKeyChain];


        //}
        //else
        //{
        //    return;
        //}
        index -= 1;
    
        //if (currentKeyChainInfo.FillPercentage>0)
        //{
        //    print("KeyChain is not unloacked yet");
        //    return;
        //}
        //print("keychain is activated");
      //  GameManager.Instance.CurrentKeyChain = index;
        GameObject keyChain = Instantiate(AllKeyChains[index].KeyChain, GameManager.Instance.weaponManager.CurrentWeapon[0].KeyChainPosParent.transform.position, Quaternion.identity);
        keyChain.transform.parent = GameManager.Instance.weaponManager.CurrentWeapon[0].KeyChainPosParent;
    
        keyChain.transform.DORotate(Vector3.zero, 0.2f).SetDelay(0.2f);
      CurrentActivatedKeyChain = keyChain.GetComponent<KeyChain>();
        //companion.transform.position = GameManager.Instance.playerController.transform.position + CurrentCompanion.Offset;

        // GameManager.Instance.SetWeaponAsChild(weapon.transform);

        // CurrentWeaponInfo = AllWeaponsInfo[index];

        // InstantiatePurchasedWeaponsEnhancement();
    }

    public void AssignKeyChainValues()
    {

        StartCoroutine(FillKeyChain());

        GameManager.Instance.uiManager.levelComplete.MultiplierArrow.DOKill();
        GameManager.Instance.uiManager.levelComplete.MultiplierArrow.DOLocalRotate(new Vector3(0, 0, 90), 0.001f);
        float muliplierevalue=1;
        GameManager.Instance.uiManager.levelComplete.FillPercentageText.text = currentKeyChainInfo.FillPercentage + "%";
        GameManager.Instance.uiManager.gamePlay.LevelScoreText.text = "+"+GameManager.Instance.playerController.LevelScore;

        GameManager.Instance.uiManager.levelComplete.MultiplierArrow.DOLocalRotate(new Vector3(0,0,-90),1.5f).OnUpdate(()=> {

           
            float ArrowAngle=GameManager.Instance.uiManager.levelComplete.MultiplierArrow.rotation.z;
            if (ArrowAngle > 0.5f && ArrowAngle <0.7f)
            {
               // print("X2");
                muliplierevalue = 2;
            }
            else if (ArrowAngle > 0.17f && ArrowAngle <= 0.47f)
            {
               // print("X3");
                muliplierevalue = 3;

            }
            else if (ArrowAngle > -0.15f && ArrowAngle <= 0.14f)
            {
              //  print("X5");
                muliplierevalue = 5;

            }
            else if (ArrowAngle > -0.48f && ArrowAngle <=  - 0.17f)
            {
               // print("X3");
                muliplierevalue = 3;

            }
            else if (ArrowAngle < -0.5f)
            {
              //  print("X2");
                muliplierevalue = 2;

            }
            MultiplierScore = GameManager.Instance.playerController.LevelScore * muliplierevalue;
            GameManager.Instance.uiManager.levelComplete.MultiplierScoreText.text= "+"+ MultiplierScore;

        }).OnStart(()=> {


          



        }).SetLoops(-1,LoopType.Yoyo).SetDelay(0.1f);

    }

    public void GetMultipliereScore()
    {
        GameManager.Instance.uiManager.levelComplete.RewardPanel.GetComponent<CashCollectionControll>().totalCash = MultiplierScore;
        GameManager.Instance.uiManager.levelComplete.RewardPanel.SetActive(true);
       // print("get Mulipliere Score");
  
        //GameManager.Instance.uiManager.levelComplete.KeyChanePanel.SetActive(false);
    }
    public IEnumerator FillKeyChain()
    {
        yield return new WaitForSeconds(0.001f);

        if(AllKeyChainsUnloacked)
        {
            // yield return 0;
          //  yield break;
        }
        GameManager.Instance.uiManager.levelComplete.KeyChaneFiller.fillAmount = currentKeyChainInfo.FillPercentage;

        int fillAmount = Mathf.FloorToInt((1 - currentKeyChainInfo.FillPercentage) * 100);
        GameManager.Instance.uiManager.levelComplete.FillPercentageText.text = fillAmount + "%";

        yield return new WaitForSeconds(0.2f);
        GameManager.Instance.PlaySound("KeyChainProgressionSound");

        
        GameManager.Instance.uiManager.levelComplete.KeyChaneFiller.DOFillAmount(currentKeyChainInfo.FillPercentage-0.1f,1.5f).OnComplete(()=> { 
        
        
        
        }).OnUpdate(()=> {

            fillAmount = Mathf.FloorToInt((1 - GameManager.Instance.uiManager.levelComplete.KeyChaneFiller.fillAmount) * 100);
            GameManager.Instance.uiManager.levelComplete.FillPercentageText.text = fillAmount + "%";
            currentKeyChainInfo.FillPercentage = GameManager.Instance.uiManager.levelComplete.KeyChaneFiller.fillAmount;

            if (fillAmount >= 99)
            {
                fillAmount = 100;
                GameManager.Instance.uiManager.levelComplete.FillPercentageText.text = fillAmount + "%";


                    currentKeyChainInfo.IsActivated = true;
                currentKeyChainInfo.FillPercentage = 0;
                GameManager.Instance.uiManager.levelComplete.KeyChaneFiller.fillAmount = 0;
                

                if (!AllKeyChainsUnloacked)
                {
                    ActivatedKeyChainInfo = currentKeyChainInfo;
                    GameManager.Instance.CurrentKeyChain++;
                }
              
               // print("Hurrah Keychain is unloacked");
              //  Invoke("ActivateKeyChainUnloackedPanel",0.5f);
                GameManager.Instance.uiManager.levelComplete.KeyChaneFiller.DOKill();
            }
        }).SetEase(Ease.Linear);
  
       

    }
    public IEnumerator ActivateKeyChainUnloackedPanel()
    {
        yield return new WaitForSeconds(0.5f);
        GlobalAudioPlayer.Play("GameComplete");
        GameManager.Instance.ActivateKeyChainRenderTexture();
        GameManager.Instance.uiManager.levelComplete.KeyChanePanel.SetActive(false);
        SetNewLevelFeatures();
        GameManager.Instance.uiManager.levelComplete.KeyChaneUnloackedPanel.SetActive(true);
    }

    public IEnumerator ShowKeyChainModelRenderTexture()
    {
        yield return new WaitForSeconds(0.5f);


    }
    public void SetNewLevelFeatures()
    {
        GameManager.Instance.uiManager.levelComplete.PreviousLevelText.text = "+" + (GameManager.Instance.uiManager.gamePlay.PreviousLevelText.text);
        GameManager.Instance.uiManager.levelComplete.PreviousFireRateText.text = "+" + GameManager.Instance.weaponManager.CurrentWeaponInfo.FireRateUI;
        GameManager.Instance.uiManager.levelComplete.PreviousFireRangeText.text = "+" + GameManager.Instance.weaponManager.CurrentWeaponInfo.FireRangeUI;

        GameManager.Instance.SetWeaponUIFeatureLevelWise();

        GameManager.Instance.uiManager.levelComplete.NewLevelText.text = "+" + (GameManager.Instance.uiManager.gamePlay.NewLevelText.text);
        GameManager.Instance.uiManager.levelComplete.NewFireRateText.text = "+" + GameManager.Instance.weaponManager.CurrentWeaponInfo.FireRateUI;
        GameManager.Instance.uiManager.levelComplete.NewFireRangeText.text = "+" + GameManager.Instance.weaponManager.CurrentWeaponInfo.FireRangeUI;
    }
}
