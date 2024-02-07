using System;
using System.Collections;
using System.Collections.Generic;
using Falcon.FalconCore.Editor.FPlugins;
using Falcon.FalconCore.Scripts.Utils.Data;
using Falcon.FalconCore.Scripts.Utils.Sequences.Editor;
using UnityEditor;

namespace Falcon.FalconCore.Editor
{
    public class FalconCoreWindow : EditorWindow
    {
        [MenuItem("Falcon/FalconCore/Refresh")]
        public static void ShowWindow()
        {
            new EditorSequence(Refresh()).Start();
        }
        
        [MenuItem("Falcon/FalconCore/ClearData")]
        public static void ClearData()
        {
            new FFile(FData.DataFile).Save(new Dictionary<String, String>());
        }

        private static IEnumerator Refresh()
        {
            var a = new FalconCoreInstallResponder();
            IEnumerator<FPlugin> pluginGetter = FPlugin.GetAsync("FalconCore");
            yield return pluginGetter;
            yield return a.OnPluginInstalled(pluginGetter.Current.InstalledDirectory);
        }
    }
}