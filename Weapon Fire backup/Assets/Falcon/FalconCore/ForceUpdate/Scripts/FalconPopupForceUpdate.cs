using Falcon.FalconCore.Scripts.FalconABTesting.Scripts.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FalconPopupForceUpdate : MonoBehaviour
{
    public GameObject groupOkCancel;
    public GameObject groupOk;

    public Text textTitle;
    public Text textUpdate;
    public Text textUpdate1;
    public Text textCancel;
    public void ShowOkCancel()
    {
        groupOkCancel.SetActive(true);
        groupOk.SetActive(false);
    }

    public void ShowOkOnly()
    {
        groupOkCancel.SetActive(false);
        groupOk.SetActive(true);
    }

    public void ButtonUpdate()
    {
        ForceUpdateConfig config = FalconConfig.Instance<ForceUpdateConfig>();
#if UNITY_ANDROID
        if (config.f_core_popupUpdate_url_store_android == "")
        {
            Application.OpenURL("market://details?id=" + Application.identifier);
        }
        else
        {
            Application.OpenURL(config.f_core_popupUpdate_url_store_android);
        }
#elif UNITY_IOS
        if (config.f_core_popupUpdate_url_store_ios == "")
        {
            Application.OpenURL("itms-apps://itunes.apple.com/app/" + Application.identifier);
        }
        else
        {
            Application.OpenURL(config.f_core_popupUpdate_url_store_ios);
        }
#endif
    }

    public void ButtonCancel()
    {
        gameObject.SetActive(false);
    }

    public void UpdateUI()
    {
        string l = Application.systemLanguage.ToString();
        FalconPopupForceUpdateLanguage lang = Resources.Load("FalconPopupForceUpdateLanguage") as FalconPopupForceUpdateLanguage;
        for (int i = 0; i < lang.localizeInfos.Count; i++)
        {
            if (lang.localizeInfos[i].Language.Equals(l))
            {
                textTitle.text = lang.localizeInfos[i].Title;
                textUpdate.text = lang.localizeInfos[i].Update;
                textUpdate1.text = lang.localizeInfos[i].Update;
                textCancel.text = lang.localizeInfos[i].Cancel;
                break;
            }
        }
    }
}
