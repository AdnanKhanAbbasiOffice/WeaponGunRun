using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class UiManager : MonoBehaviour
{
    #region Variables

    [SerializeField] internal Menu menu;
    [SerializeField] internal GamePlay gamePlay;
    [SerializeField] internal LevelComplete levelComplete;
    [SerializeField] internal LevelFailed levelFailed;

    #endregion Variables

    #region LocalVariables
    private int cashImageIndex = 0;
    #endregion LocalVariables
    #region UnityFunction
    void Start()
    {
        Initilize();
    }
    void Update()
    {

    }
    #endregion UnityFunction
    #region CustomFunction
    private void Initilize()
    {
        if (menu.panelOfflineReward != null)
            menu.panelOfflineReward.SetActive(true);
      //  else
           // menu.panel.SetActive(true);
        menu.music.isOn = !GameManager.Instance.music;
        menu.sound.isOn = GameManager.Instance.sound;
        menu.vibration.isOn = GameManager.Instance.vibration;

        if(GameManager.Instance.currentLevel==0)
        {
            gamePlay.textCurrentLevel.text = "Tutorial";

        }
        else
        {
            gamePlay.textCurrentLevel.text = "Level "+(GameManager.Instance.currentLevel);

        }
        gamePlay.textNextLevel.text = (GameManager.Instance.currentLevel + 2).ToString();
        if (gamePlay.levelBar != null)
            gamePlay.levelBar.value = 0;

        gamePlay.textCash.text = ((int)GameManager.Instance.totalCash).ToString();
    }
    internal void TargetCounter()
    {
        GameManager.Instance.achiveTarget++;
        float percentage = GameManager.Instance.achiveTarget * 1f / GameManager.Instance.totalTarget;
        gamePlay.levelBar.value = percentage;
        if (percentage >= 1)
        {
            gamePlay.confetti.SetActive(true);
            StartCoroutine(GameComplete(1f));
        }

    }
    internal void UpdateCashCountTxt(Vector3 cashposition, int Price)
    {
        GameManager.Instance.totalCash = GameManager.Instance.totalCash + Price;
        gamePlay.textCash.text = ((int)GameManager.Instance.totalCash).ToString();
        gamePlay.cashParent.transform.GetChild(cashImageIndex).GetComponent<CashMove>().startingPosition = cashposition;
        gamePlay.cashParent.transform.GetChild(cashImageIndex).gameObject.SetActive(true);
        cashImageIndex++;
        cashImageIndex = (cashImageIndex >= gamePlay.cashParent.transform.childCount) ? 0 : cashImageIndex;
    }
    public void AddCashUpdate(float value)
    {
        GameManager.Instance.playerController.AddLevelScoreCashUpdate(value);
        GameManager.Instance.totalCash += value;
        gamePlay.textCash.text = ((int)GameManager.Instance.totalCash).ToString();

        GameManager.Instance.LevelPriceButtonStatus();
        gamePlay.NewLevelButton.SetActive(true);

    }


    
    public IEnumerator GameComplete(float delay)
    {
        yield return new WaitForSeconds(delay);
       // GlobalAudioPlayer.Play("GameComplete");
       // print("game complete");
        GameManager.Instance.currentLevel++;
       
        StartCoroutine(GameManager.Instance.Next());
       // levelComplete.panel.SetActive(true);
       // levelComplete.CashCollectionControll.totalCash = (GameManager.Instance.currentLevel * 10 + 100f);
       // levelComplete.textCash.text = (GameManager.Instance.currentLevel * 10 + 100).ToString();
    }

    public IEnumerator GameOver(float delay)
    {
        yield return new WaitForSeconds(delay);
        GlobalAudioPlayer.Play("GameOver");
        levelFailed.panel.SetActive(true);
    }
    #endregion CustomFunction

    #region Classes
    [System.Serializable]
    public struct Menu
    {
        [SerializeField] internal GameObject panel;
        [SerializeField] internal GameObject panelOfflineReward;
        [SerializeField] internal Toggle Settings;
        [SerializeField] internal Toggle music;
        [SerializeField] internal Toggle sound;
        [SerializeField] internal Toggle vibration;


    }
    [System.Serializable]
    public struct GamePlay
    {
        [SerializeField] internal GameObject panel;
        [SerializeField] internal GameObject cashParent;
        [SerializeField] internal GameObject confetti;
        [SerializeField] internal Transform cashTargetPositon;
        [SerializeField] internal Text textCurrentLevel;
        [SerializeField] internal Text textNextLevel;
        [SerializeField] internal Text textCash;
        [SerializeField] internal Slider levelBar;

        [SerializeField] internal TextMeshProUGUI WeaponNameText;
        [SerializeField] internal TextMeshProUGUI FirePowerText;
        [SerializeField] internal TextMeshProUGUI FireRateText;
        [SerializeField] internal TextMeshProUGUI FireRangeText;

        [SerializeField] internal Slider FirePowerSlider;
        [SerializeField] internal Slider FireRateSlider;
        [SerializeField] internal Slider FireRangeSlider;



        [SerializeField] internal GameObject WeaponEnhacemenetPanel;
        [SerializeField] internal GameObject WeaponNewEnhacemenetLevelPanel;

        [SerializeField] internal TextMeshProUGUI PreviousLevelText;
        [SerializeField] internal TextMeshProUGUI PreviousFireRateText;
        [SerializeField] internal TextMeshProUGUI PreviousFireRangeText;


        [SerializeField] internal TextMeshProUGUI NewLevelText;
        [SerializeField] internal TextMeshProUGUI NewFireRateText;
        [SerializeField] internal TextMeshProUGUI NewFireRangeText;

        [SerializeField] internal GameObject SettingsPanel;
        [SerializeField] internal Slider MaxSwirveSlider;
        [SerializeField] internal Slider HorizontaSpeedSlider;
        [SerializeField] internal Slider DragThresholdSlider;

        [SerializeField] internal TextMeshProUGUI MaxSwirveSliderText;
        [SerializeField] internal TextMeshProUGUI HorizontalSpeedSliderText;
        [SerializeField] internal TextMeshProUGUI DragThresholdSliderText;
        [SerializeField] internal TextMeshProUGUI LevelScoreText;

        [SerializeField] internal GameObject NewLevelInAppButton;
        [SerializeField] internal GameObject NewLevelButton;
        [SerializeField] internal TextMeshProUGUI LevelPriceText;

        [SerializeField] internal GameObject ShopPanel;
        [SerializeField] internal GameObject InventoryPanel;

        [SerializeField] internal GameObject TopPanel;
        [SerializeField] internal GameObject CanvasPanel;
        [SerializeField] internal GameObject ConsentButton;


    }
    [System.Serializable]
    public struct LevelComplete
    {
        [SerializeField] internal GameObject panel;
        [SerializeField] internal GameObject KeyChanePanel;
        [SerializeField] internal GameObject KeyChaneUnloackedPanel;
        [SerializeField] internal GameObject RewardPanel;
        [SerializeField] internal Image KeyChaneFiller;
        [SerializeField] internal Image KeyChaneFillerBG;

        [SerializeField] internal GameObject NewWeaponUnloackedPanel;


        [SerializeField] internal CashCollectionControll CashCollectionControll;
        [SerializeField] internal Text textCash;
        [SerializeField] internal TextMeshProUGUI FillPercentageText;
        [SerializeField] internal TextMeshProUGUI MultiplierScoreText;
        [SerializeField] internal Transform MultiplierArrow;


        [SerializeField] internal TextMeshProUGUI PreviousLevelText;
        [SerializeField] internal TextMeshProUGUI PreviousFireRateText;
        [SerializeField] internal TextMeshProUGUI PreviousFireRangeText;


        [SerializeField] internal TextMeshProUGUI NewLevelText;
        [SerializeField] internal TextMeshProUGUI NewFireRateText;
        [SerializeField] internal TextMeshProUGUI NewFireRangeText;

    }
    [System.Serializable]
    public struct LevelFailed
    {
        [SerializeField] internal GameObject panel;

    }
    #endregion Classes
    #region ButtonActions



    #endregion ButtonActions
}
