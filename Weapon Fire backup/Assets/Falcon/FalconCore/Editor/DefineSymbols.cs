using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace Falcon.FalconCore.Editor
{
    public static class DefineSymbols
    {
        private const char DefineSeparator = ';';
        private static readonly List<string> AllDefines = new List<string>();
     
        public static void Add(params string[] defines)
        {
            AllDefines.Clear();
            AllDefines.AddRange(GetDefines());
            AllDefines.AddRange(defines.Except(AllDefines));
            UpdateDefines(AllDefines);
        }
 
        public static void Remove(params string[] defines)
        {
            AllDefines.Clear();
            AllDefines.AddRange(GetDefines().Except(defines));
            UpdateDefines(AllDefines);
        }
 
        public static void Clear()
        {
            AllDefines.Clear();
            UpdateDefines(AllDefines);
        }
 
        private static IEnumerable<string> GetDefines() => PlayerSettings.GetScriptingDefineSymbolsForGroup(
            EditorUserBuildSettings.selectedBuildTargetGroup).Split(DefineSeparator).ToList();
 
        private static void UpdateDefines(List<string> allDefines) => PlayerSettings.SetScriptingDefineSymbolsForGroup(
            EditorUserBuildSettings.selectedBuildTargetGroup, string.Join(DefineSeparator.ToString(),
                allDefines.ToArray()));
    }
}