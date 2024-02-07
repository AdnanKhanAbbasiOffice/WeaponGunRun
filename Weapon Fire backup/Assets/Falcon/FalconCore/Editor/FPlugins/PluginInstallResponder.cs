using System;
using System.Collections;

namespace Falcon.FalconCore.Editor.FPlugins
{
    public abstract class PluginInstallResponder
    {
        public abstract String GetPackageName();
        public abstract IEnumerator OnPluginInstalled(String installLocation);
    }
}