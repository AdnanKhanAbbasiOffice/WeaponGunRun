using System;
using System.Collections;
using System.IO;
using Falcon.FalconCore.Editor.FPlugins;
using Falcon.FalconCore.Editor.Utils;
using Falcon.FalconCore.Scripts;
using Falcon.FalconCore.Scripts.Utils;
using Falcon.FalconCore.Scripts.Utils.Sequences.Editor;
using UnityEditor;
using UnityEngine;

namespace Falcon.FalconCore.Editor
{
    namespace Falcon
    {
        public class FalconWindow : EditorWindow
        {
            private ExecState loginState = ExecState.NotStarted;

            private Texture2D trashIcon;

            string usingStr = "Using";
            string useStr = "Use";
            string updateStr = "Update";
            string maxSdkStr = "MaxSdk";
            string ironSourceStr = "IronSource";
            string ggStr = "GoogleMobileAds";


            private void Awake()
            {
                if (ExecStates.CanStart(loginState))
                    loginState = FKeyManager.Instance.HasFalconKey() ? ExecState.Succeed : ExecState.Failed;

                trashIcon = new Texture2D(2, 2);
                trashIcon.LoadImage(File.ReadAllBytes(FalconCoreFileUtils.GetFalconPluginFolder() +
                                                      @"/FalconCore/Editor/images/trash.png"));
            }

            private void OnDisable()
            {
                if (!Application.isPlaying) DestroyImmediate(FalconMain.Instance.gameObject);
            }

            private void OnGUI()
            {
                if (ExecStates.CanStart(loginState))
                    RenderLoginMenu();
                else if (loginState.Equals(ExecState.Processing))
                    RenderWaitingMenu("Logging In, please wait");
                else
                    RenderPluginMenu();
            }

            [MenuItem("Falcon/FalconMenu", priority = 0)]
            public static void ShowWindow()
            {
                var window = GetWindow<FalconWindow>("Falcon Settings", true);
                window.minSize = new Vector2(460, 600);
                window.maxSize = new Vector2(460, 800);

                window.Show();
            }

            private void RenderWaitingMenu(string message)
            {
                GUIVertical(() =>
                {
                    GUILayout.Space(20);

                    GUILayout.Label(message);
                });
            }

            private void RenderPluginMenu()
            {
                if (FPlugin.PullState.Equals(ExecState.Processing))
                {
                    RenderWaitingMenu("Plugin are being Loaded. please wait!!!");
                }
                else
                {
                    var plugins = FPlugin.GetAllSync();

                    GUIVertical(() =>
                    {
                        GUILayout.Space(20);
                        GUILayout.Label("Loaded " + plugins.Count + "/" + FPlugin.RemotePluginCount +
                                        " plugin, some may are still being loaded");
                    });

                    GUIHorizon(() =>
                    {
                        if (GUILayout.Button("Refresh", GUILayout.Width(100), GUILayout.Height(20)))
                        {
                            FPlugin.Clear();
                            new EditorSequence(FPlugin.Init()).Start();
                        }

                        GUILayout.Space(20);
                        if (GUILayout.Button("LogOut", GUILayout.Width(100), GUILayout.Height(20)))
                        {
                            loginState = ExecState.NotStarted;
                            FKeyManager.Instance.DeleteFalconKey();
                        }
                    });

                    foreach (var plugin in plugins) RenderPluginItem(plugin);
                }
            }

            private void RenderPluginItem(FPlugin plugin)
            {
                GUIVertical(() => { GUILayout.Space(20); });

                GUIHorizon(() =>
                {
                    SpaceFirst();
                    if (!plugin.Installed)
                    {
                        RenderUninstalledPlugin(plugin);
                    }
                    else
                    {
                        if (!plugin.InstalledNewest())
                        {
                            RenderOldPlugin(plugin);
                        }
                        else
                        {
                            RenderNewestPlugin(plugin);
                        }
                    }
                    SpaceEnd();
                });
            }

            void SpaceFirst()
            {
                GUILayout.Space(10);
            }

            void SpaceEnd()
            {
                GUILayout.Space(5);
            }

            private void RenderUninstalledPlugin(FPlugin plugin)
            {
                GUILayout.Label(plugin.PluginName + " v" + plugin.RemoteConfig.Version);
                if (GUILayout.Button("Install", GUILayout.Width(100), GUILayout.Height(20)))
                    new EditorSequence(plugin.Install()).Start();

                GUI.enabled = false;
                GUILayout.Button(trashIcon, GUILayout.Width(20), GUILayout.Height(20));

                GUI.enabled = true;
            }

            private void RenderOldPlugin(FPlugin plugin)
            {
                if (plugin.IsFalconMediation())
                {
                    GUI.enabled = true;

                    GUILayout.Label(plugin.PluginName + " v" + plugin.RemoteConfig.Version + " (current v" + plugin.InstalledConfig.Version + ")");
                    if (string.CompareOrdinal(plugin.InstalledConfig.Version, plugin.RemoteConfig.Version) < 0)
                    {
                        if (GUILayout.Button(updateStr, GUILayout.Width(100), GUILayout.Height(20)))
                            new EditorSequence(plugin.Install()).Start();
                    }
                    else if (plugin.IsUseIrs() && string.CompareOrdinal(plugin.InstalledConfig.VersionIrs, plugin.RemoteConfig.VersionIrs) < 0)
                    {
                        if (GUILayout.Button(updateStr, GUILayout.Width(100), GUILayout.Height(20)))
                            new EditorSequence(plugin.InstallIronsource()).Start();
                    }
                    else if (plugin.IsUseMax() && string.CompareOrdinal(plugin.InstalledConfig.VersionMax, plugin.RemoteConfig.VersionMax) < 0)
                    {
                        if (GUILayout.Button(updateStr, GUILayout.Width(100), GUILayout.Height(20)))
                            new EditorSequence(plugin.InstallMax()).Start();
                    }
                    else if (plugin.IsUseGG() && string.CompareOrdinal(plugin.InstalledConfig.VersionGG, plugin.RemoteConfig.VersionGG) < 0)
                    {
                        if (GUILayout.Button(updateStr, GUILayout.Width(100), GUILayout.Height(20)))
                            new EditorSequence(plugin.InstallGG()).Start();
                    }
                    if (GUILayout.Button(trashIcon, GUILayout.Width(20), GUILayout.Height(20)))
                        plugin.UnInstall();
                    if (plugin.InstalledConfig != null)
                    {
                        SpaceEnd();
                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal();
                        SpaceFirst();
                        if (plugin.IsUseIrs())
                        {
                            GUILayout.Label("    ➥ " + ironSourceStr + " v" + plugin.RemoteConfig.VersionIrs +
                                    " (current v" +
                                    plugin.InstalledConfig.VersionIrs + ")");
                        }
                        else
                        {
                            GUILayout.Label("    ➥ " + ironSourceStr + " v" + plugin.RemoteConfig.VersionIrs);
                        }
                        if (!plugin.IsUseIrs())
                        {
                            if (GUILayout.Button(useStr, GUILayout.Width(100), GUILayout.Height(20)))
                                new EditorSequence(plugin.InstallIronsource()).Start();
                        }
                        else
                        {
                            GUI.enabled = false;
                            GUILayout.Button(usingStr, GUILayout.Width(100), GUILayout.Height(20));
                            GUI.enabled = true;
                        }
                        SpaceEnd();
                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal();
                        SpaceFirst();
                        if (plugin.IsUseGG())
                        {
                            GUILayout.Label("    ➥ " + ggStr + " v" + plugin.RemoteConfig.VersionGG +
                                    " (current v" +
                                    plugin.InstalledConfig.VersionGG + ")");
                        }
                        else
                        {
                            GUILayout.Label("    ➥ " + ggStr + " v" + plugin.RemoteConfig.VersionGG);
                        }
                        if (!plugin.IsUseIrs())
                        {
                            if (GUILayout.Button(useStr, GUILayout.Width(100), GUILayout.Height(20)))
                                new EditorSequence(plugin.InstallGG()).Start();
                        }
                        else
                        {
                            GUI.enabled = false;
                            GUILayout.Button(usingStr, GUILayout.Width(100), GUILayout.Height(20));
                            GUI.enabled = true;
                        }
                        SpaceEnd();
                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal();
                        SpaceFirst();
                        if (plugin.IsUseMax())
                        {
                            GUILayout.Label("------" + maxSdkStr + " v" + plugin.RemoteConfig.VersionMax +
                                        " (current v" +
                                        plugin.InstalledConfig.VersionMax + ")");
                        }
                        else
                        {
                            GUILayout.Label("------" + maxSdkStr + " v" + plugin.RemoteConfig.VersionMax);
                        }
                        if (!plugin.IsUseMax())
                        {
                            if (GUILayout.Button(useStr, GUILayout.Width(100), GUILayout.Height(20)))
                                new EditorSequence(plugin.InstallMax()).Start();
                        }
                        else
                        {
                            GUI.enabled = false;
                            GUILayout.Button(usingStr, GUILayout.Width(100), GUILayout.Height(20));
                            GUI.enabled = true;
                        }
                    }
                }
                else
                {
                    GUILayout.Label(plugin.PluginName + " v" + plugin.RemoteConfig.Version +
                                " (current v" +
                                plugin.InstalledConfig.Version + ")");
                    GUI.enabled = true;
                    if (GUILayout.Button(updateStr, GUILayout.Width(100), GUILayout.Height(20)))
                        new EditorSequence(plugin.Install()).Start();

                    GUI.enabled = !plugin.IsFalconCore();

                    if (GUILayout.Button(trashIcon, GUILayout.Width(20), GUILayout.Height(20)))
                        plugin.UnInstall();

                    GUI.enabled = true;
                }
            }

            private void RenderNewestPlugin(FPlugin plugin)
            {
                if (plugin.IsFalconMediation())
                {
                    GUILayout.Label(plugin.PluginName + " v" + plugin.RemoteConfig.Version);
                    GUI.enabled = false;
                    GUILayout.Button("Install", GUILayout.Width(100), GUILayout.Height(20));

                    GUI.enabled = true;
                    if (GUILayout.Button(trashIcon, GUILayout.Width(20), GUILayout.Height(20)))
                        plugin.UnInstall();
                    if (plugin.InstalledConfig != null)
                    {
                        SpaceEnd();
                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal();
                        SpaceFirst();
                        GUILayout.Label("    ➥ " + ironSourceStr + " v" + plugin.RemoteConfig.VersionIrs);
                        if (!plugin.IsUseIrs())
                        {
                            if (GUILayout.Button(useStr, GUILayout.Width(100), GUILayout.Height(20)))
                                new EditorSequence(plugin.InstallIronsource()).Start();
                        }
                        else
                        {
                            GUI.enabled = false;
                            GUILayout.Button(usingStr, GUILayout.Width(100), GUILayout.Height(20));
                            GUI.enabled = true;
                        }
                        SpaceEnd();
                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal();
                        SpaceFirst();
                        GUILayout.Label("    ➥ " + ggStr + " v" + plugin.RemoteConfig.VersionGG);
                        if (!plugin.IsUseGG())
                        {
                            if (GUILayout.Button(useStr, GUILayout.Width(100), GUILayout.Height(20)))
                                new EditorSequence(plugin.InstallGG()).Start();
                        }
                        else
                        {
                            GUI.enabled = false;
                            GUILayout.Button(usingStr, GUILayout.Width(100), GUILayout.Height(20));
                            GUI.enabled = true;
                        }
                        SpaceEnd();
                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal();
                        SpaceFirst();
                        GUILayout.Label("    ➥ " + maxSdkStr + " v" + plugin.RemoteConfig.VersionMax);
                        if (!plugin.IsUseMax())
                        {
                            if (GUILayout.Button(useStr, GUILayout.Width(100), GUILayout.Height(20)))
                                new EditorSequence(plugin.InstallMax()).Start();
                        }
                        else
                        {
                            GUI.enabled = false;
                            GUILayout.Button(usingStr, GUILayout.Width(100), GUILayout.Height(20));
                            GUI.enabled = true;
                        }
                    }
                }
                else
                {
                    GUILayout.Label(plugin.PluginName + " v" + plugin.RemoteConfig.Version);
                    GUI.enabled = false;
                    GUILayout.Button("Install", GUILayout.Width(100), GUILayout.Height(20));

                    GUI.enabled = !plugin.IsFalconCore();

                    if (GUILayout.Button(trashIcon, GUILayout.Width(20), GUILayout.Height(20)))
                        plugin.UnInstall();

                    GUI.enabled = true;
                }
            }

            private void GUIHorizon(Action action)
            {
                GUILayout.BeginHorizontal();
                action.Invoke();
                GUILayout.EndHorizontal();
            }

            private void GUIVertical(Action action)
            {
                GUILayout.BeginVertical();
                action.Invoke();
                GUILayout.EndVertical();
            }


            #region Login

            private string userInputFalconKey;

            private void RenderLoginMenu()
            {
                //Module Login
                GUIVertical(() =>
                {
                    GUILayout.Space(20);
                    GUILayout.Label("Falcon Key : ");
                    userInputFalconKey = GUILayout.TextField(userInputFalconKey);
                    GUILayout.Space(5);
                    if (GUILayout.Button("Login", GUILayout.Width(100), GUILayout.Height(20)))
                    {
                        loginState = ExecState.Processing;
                        new EditorSequence(ValidateLogin(userInputFalconKey)).Start();
                    }
                });
                GUILayout.BeginVertical();


                GUILayout.EndVertical();
            }

            private IEnumerator ValidateLogin(string key)
            {
                var validation = FKeyManager.Instance.ValidateFalconKey(key);
                yield return validation;
                if (validation.Current)
                {
                    loginState = ExecState.Succeed;
                }
                else
                {
                    loginState = ExecState.Failed;
                    EditorUtility.DisplayDialog("Notification", "Information invalid, please retry!", "Ok");
                }
            }

            #endregion
        }
    }
}