using Falcon.FalconCore.Scripts;
using Falcon.FalconCore.Scripts.FalconABTesting.Scripts.Model;
using System;
using System.Collections;
using UnityEngine;

public class CorePopupForceUpdate : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
    static void Init()
    {
        FalconMain.OnInitComplete += OnInitComplete;
    }

    private void Awake()
    {
        StartCoroutine(LoadResources());
    }

    IEnumerator LoadResources()
    {
        ForceUpdateConfig config = FalconConfig.Instance<ForceUpdateConfig>();
        string minRemoteVersion = "";
        string targetRemoteVersion = "";
#if UNITY_ANDROID || UNITY_EDITOR
        minRemoteVersion = config.f_core_popupUpdate_minVersion_android;
        targetRemoteVersion = config.f_core_popupUpdate_targetVersion_android;
#elif UNITY_IOS
        minRemoteVersion = config.f_core_popupUpdate_minVersion_ios;
        targetRemoteVersion = config.f_core_popupUpdate_targetVersion_ios;
#endif
        int rs1 = CompareVersion(minRemoteVersion, Application.version);
        int rs2 = CompareVersion(targetRemoteVersion, Application.version);
        if (rs1 > 0)
        {
            //show ok only
            ResourceRequest resourceRequest = Resources.LoadAsync<FalconPopupForceUpdate>("FalconPopupForceUpdate");
            yield return resourceRequest;
            FalconPopupForceUpdate instance = Instantiate(resourceRequest.asset) as FalconPopupForceUpdate;
            instance.ShowOkOnly();
            instance.UpdateUI();
        }
        else if (rs2 > 0)
        {
            //show ok cancel
            ResourceRequest resourceRequest = Resources.LoadAsync<FalconPopupForceUpdate>("FalconPopupForceUpdate");
            yield return resourceRequest;
            FalconPopupForceUpdate instance = Instantiate(resourceRequest.asset) as FalconPopupForceUpdate;
            instance.ShowOkCancel();
            instance.UpdateUI();
        }
    }

    private static void OnInitComplete(object sender, EventArgs e)
    {
        FalconMain.Instance.AddIfNotExist<CorePopupForceUpdate>();
    }

    static int CompareVersion(string v1, string v2)
    {
        string[] arr1 = v1.Split('.');
        string[] arr2 = v2.Split('.');
        int target = arr1.Length > arr2.Length ? arr2.Length : arr1.Length;
        for (int i = 0; i < target; i++)
        {
            bool a = int.TryParse(arr1[i], out int rs1);
            bool b = int.TryParse(arr2[i], out int rs2);
            if (!a || !b) return -1;
            if (rs1 == rs2) continue;
            return rs1 > rs2 ? 1 : -1;
        }
        return 0;
    }
}
