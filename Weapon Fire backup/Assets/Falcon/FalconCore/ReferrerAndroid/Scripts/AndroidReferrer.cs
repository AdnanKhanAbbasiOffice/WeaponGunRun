using System;
using UnityEngine;
using Ugi.PlayInstallReferrerPlugin;

public class AndroidReferrer : MonoBehaviour
{
    public static void GetInstallReferrer(Action<string> success, Action<string> fail = null)
    {
        PlayInstallReferrer.GetInstallReferrerInfo((installReferrerDetails) =>
        {
            // check for error
            if (installReferrerDetails.Error != null)
            {
                Debug.LogError("Error occurred!");
                if (installReferrerDetails.Error.Exception != null)
                {
                    Debug.LogError("Exception message: " + installReferrerDetails.Error.Exception.Message);
                }
                Debug.LogError("Response code: " + installReferrerDetails.Error.ResponseCode);
                fail?.Invoke(installReferrerDetails.Error.Exception != null ? installReferrerDetails.Error.Exception.Message : "Unknown");
                return;
            }

            if (installReferrerDetails.InstallReferrer != null)
            {
                success(installReferrerDetails.InstallReferrer);
            }
        });
    }
}
