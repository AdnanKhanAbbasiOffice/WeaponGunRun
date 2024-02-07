using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Falcon.FalconAnalytics.Scripts;
using Falcon.FalconCore.Editor;
using Falcon.FalconCore.Editor.FPlugins;
using Falcon.FalconCore.Editor.Utils;
using UnityEditor;
using UnityEditor.Build;
#if UNITY_2022_3_OR_NEWER
using UnityEditor.Build;
#endif

namespace Falcon.FalconAnalytics.Editor
{
    public class FalconAnalyticInstallResponder: PluginInstallResponder, IActiveBuildTargetChanged
    {
        public override string GetPackageName()
        {
            return "FalconAnalytics";
        }

        public override IEnumerator OnPluginInstalled(string installLocation)
        {
            DefineSymbols.Add("FALCON_ANALYTIC");

            string dwhMessagePath = Path.Combine(installLocation, "Scripts", "Message", "DWH", "DWHMessage.cs");

            IEnumerator<FPlugin> pluginGet = FPlugin.GetAsync("FalconAnalytics");
            yield return pluginGet;
            
            try
            {
                FalconCoreFileUtils.RewriteLineInFile(dwhMessagePath,
                    "        public string sdkVersion",
                    "        public string sdkVersion = \"" + pluginGet.Current.InstalledConfig.Version + "\";");
            }
            catch (Exception e)
            {
                AnalyticLogger.Instance.Warning(e.Message);
            }
        }

        public int callbackOrder { get; }
        public void OnActiveBuildTargetChanged(BuildTarget previousTarget, BuildTarget newTarget)
        {
            DefineSymbols.Add("FALCON_ANALYTIC");
        }
    }
}