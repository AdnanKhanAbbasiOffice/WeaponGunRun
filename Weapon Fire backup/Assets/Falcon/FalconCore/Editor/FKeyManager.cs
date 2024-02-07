using System;
using System.Collections.Generic;
using System.Net.Http;
using Falcon.FalconCore.Scripts.Utils.Sequences.Entity;
using Falcon.FalconCore.Scripts.Utils.Singletons;
using UnityEditor;

namespace Falcon.FalconCore.Editor
{
    public class FKeyManager : FSingleton<FKeyManager>
    {
        private const string ValidateURL = "https://data4game.com/falcon-sdk/validate?fkey=";
        
        private const string FalconKeyPrefName = "FALCON_FKEY";

        public string GetFalconKey() => EditorPrefs.GetString(FalconKeyPrefName, "");

        public void SaveFalconKey(string fKey) => EditorPrefs.SetString(FalconKeyPrefName, fKey);

        public void DeleteFalconKey() => EditorPrefs.DeleteKey(FalconKeyPrefName);

        public bool HasFalconKey() => EditorPrefs.HasKey(FalconKeyPrefName);

        public IEnumerator<bool> ValidateFalconKey(string fKey)
        {
            var httpSequence = new HttpSequence(HttpMethod.Get, ValidateURL + fKey);
            while (httpSequence.MoveNext()) yield return false;

            String response = httpSequence.Current;
            
            if(response != null && response == "ok\n")
            {
                SaveFalconKey(fKey);
                yield return true;
                yield break;
            }

            yield return false;
        }
    }
}