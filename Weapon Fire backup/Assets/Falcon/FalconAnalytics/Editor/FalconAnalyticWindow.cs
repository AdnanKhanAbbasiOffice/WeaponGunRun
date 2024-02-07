using System.Collections;
using System.Collections.Generic;
using Falcon.FalconCore.Editor.FPlugins;
using Falcon.FalconCore.Scripts.Utils.Sequences.Editor;
using UnityEditor;

namespace Falcon.FalconAnalytics.Editor
{
    public class FalconAnalyticWindow : EditorWindow
    {
        [MenuItem("Falcon/Falcon Analytic/Refresh")]
        public static void ShowWindow()
        {
            new EditorSequence(Refresh()).Start();
        }

        private static IEnumerator Refresh()
        {
            var a = new FalconAnalyticInstallResponder();
            IEnumerator<FPlugin> pluginGetter = FPlugin.GetAsync("FalconAnalytics");
            yield return pluginGetter;
            yield return a.OnPluginInstalled(pluginGetter.Current.InstalledDirectory);
        }
    }
}