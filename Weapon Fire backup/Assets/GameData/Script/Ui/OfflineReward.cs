using System;
using UnityEngine;
using UnityEngine.UI;

public class OfflineReward : MonoBehaviour
{
    //#region Variables
    //[SerializeField] private GameObject panel;
    //[SerializeField] private GameObject panelMenu;
    //[SerializeField] private Text textCash;
    //[SerializeField] private CashCollectionControll cashCollectionControll;
    //[SerializeField] private float delay = 0.5f;
    //public string OfflineTimer
    //{
    //    get { return PlayerPrefs.GetString("OfflineTimer", DateTime.Now.ToBinary().ToString()); }
    //    set { PlayerPrefs.SetString("OfflineTimer", value); }
    //}
    //DateTime currentDate;
    //#endregion Variables

    //#region unityFunction
    //// Start is called before the first frame update
    //void Start()
    //{
    //   // welcomeBackReward();
    //}
    //#endregion unityFunction




    //void OnApplicationQuit()
    //{
    //    OfflineTimer = DateTime.Now.ToBinary().ToString();
    //}

    //#region CustomFunction
    //void welcomeBackReward()
    //{
    //    /// off line reward
    //    //Store the current time when it starts
    //    currentDate = DateTime.Now;
    //    //Grab the old time from the player prefs as a long
    //    long temp = Convert.ToInt64(OfflineTimer);
    //    //Convert the old time from binary to a DataTime variable
    //    DateTime oldDate = DateTime.FromBinary(temp);
    //    // print("oldDate: " + oldDate);
    //    //Use the Subtract method and store the result as a timespan variable
    //    TimeSpan difference = currentDate.Subtract(oldDate);
    //    int totaltime = (int)((difference.TotalMinutes));
    //    int OfflineReward = 0;
    //    if (totaltime < 1)
    //    {
    //        panel.SetActive(false);
    //      //  panelMenu.SetActive(true);
    //    }
    //    else
    //    {

    //        OfflineReward = totaltime * 1000 / 3600;
    //        panel.SetActive(true);
    //        OfflineTimer = DateTime.Now.ToBinary().ToString();

    //    }


    //    cashCollectionControll.totalCash = OfflineReward;
    //    textCash.text = OfflineReward.ToString();
    //}

    //#endregion CustomFunction
    //#region ButtonAction
    //public void CollectOfflineReward()
    //{
    //    cashCollectionControll.gameObject.SetActive(true);
    //    Invoke("openMenuPanel", delay);
    //}

    //void openMenuPanel()
    //{
    //    panel.SetActive(false);
    //    panelMenu.SetActive(true);
    //}
    //#endregion ButtonAction
}
