using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using System.Linq;
using System.IO;
using System;

namespace SuperUnityBuild.BuildTool
{
    [System.Serializable]
    public class BuildScriptBackend
    {
        private static readonly Dictionary<ScriptingImplementation, string> scriptingImplementationNames = new Dictionary<ScriptingImplementation, string>()
        {
            { ScriptingImplementation.Mono2x, "Mono" },
            { ScriptingImplementation.IL2CPP, "IL2CPP" },
        };

        public ScriptingImplementation scriptImplementation;
        public string name;
        public bool enabled;

        public BuildScriptBackend(ScriptingImplementation scriptImplementation, bool enabled)
        {
            this.scriptImplementation = scriptImplementation;
            this.name = scriptingImplementationNames.TryGetValue(scriptImplementation, out string name) ? name : "Unknown Scripting Backend";
            this.enabled = enabled;
        }

        public override string ToString()
        {
            return name;
        }

        //Utility

        // Code based on https://forum.unity.com/threads/il2cpp-for-mac-only-available-in-mac-editor.529531/

        /// <summary>Determines if the IL2CPP backend is installed.
        /// Note: This isn't used anymore as rogue Unity logs get thrown in certain cases,
        /// but in the future it could be useful for dynamically detecting IL2CPP status.
        /// </summary>
        /// <param name="Target">The build target.</param>
        /// <returns>True if the IL2CPP backend is installed for this target.</returns>
        public static bool IsIL2CPPInstalled(BuildTargetGroup TargetGroup, BuildTarget Target)
        {
            MethodInfo[] Methods = typeof(BuildPipeline).GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
            MethodInfo GetPlaybackEngineDirectory = Methods.First((MethodInfo x) => x.Name == "GetPlaybackEngineDirectory"
                && x.ReturnType == typeof(string) && x.GetParameters().Length == 2);

            //Unfortunately, GetPlaybackEngineDirectory uses Debug.LogError internally
            //if the TargetGroup is not installed and this log is unavoidable.

            //This commented code might be able to detect if the target group is installed first,
            //but sadly it doesn't seem to work when called from ScriptableObject constructor.
            /*
            if (!BuildPipeline.IsBuildTargetSupported(TargetGroup, Target))
            {
                //Module not installed, can't build for this platform anyways
                return false;
            }
            */

            string PlayerPackage;
            try
            {
                PlayerPackage = GetPlaybackEngineDirectory.Invoke(null, new object[] { Target, BuildOptions.None }).ToString();
            }
            catch (Exception)
            {
                return false;
            }

            if(PlayerPackage == null || PlayerPackage == "")
            {
                //If PlaybackEngineDirectory still doesn't exist, just return false
                return false;
            }
            string PlayerName;
            string[] IL2CPPVariations;

            switch (Target)
            {
#if UNITY_2018_1_OR_NEWER
                case BuildTarget.StandaloneWindows:
                case BuildTarget.StandaloneWindows64:
                    {
                        PlayerName = "UnityPlayer.dll";

                        if (Target == BuildTarget.StandaloneWindows)
                        {
                            IL2CPPVariations = new string[]
                            {
                               "win32_development_il2cpp",
                               "win32_nondevelopment_il2cpp"
                            };
                        }
                        else
                        {
                            IL2CPPVariations = new string[]
                            {
                               "win64_development_il2cpp",
                               "win64_nondevelopment_il2cpp"
                            };
                        }

                        break;
                    }

#if UNITY_2019_2_OR_NEWER
#else
                case BuildTarget.StandaloneLinux:
                case BuildTarget.StandaloneLinuxUniversal:
#endif
                case BuildTarget.StandaloneLinux64:
                    {
                        PlayerName = "LinuxPlayer";

#if UNITY_2019_2_OR_NEWER
                        IL2CPPVariations = new string[]
                        {
                            "linux64_headless_development_il2cpp",
                            "linux64_headless_nondevelopment_il2cpp",
                            "linux64_withgfx_development_il2cpp",
                            "linux64_withgfx_nondevelopment_il2cpp",
                        };
#else
                        if (Target == BuildTarget.StandaloneLinux)
                        {
                            IL2CPPVariations = new string[]
                            {
                                "linux32_headless_development_il2cpp",
                                "linux32_headless_nondevelopment_il2cpp",
                                "linux32_withgfx_development_il2cpp",
                                "linux32_withgfx_nondevelopment_il2cpp"
                            };
                        }
                        else
                        {
                            IL2CPPVariations = new string[]
                            {
                                "linux64_headless_development_il2cpp",
                                "linux64_headless_nondevelopment_il2cpp",
                                "linux64_withgfx_development_il2cpp",
                                "linux64_withgfx_nondevelopment_il2cpp",
                            };
                        }
                        
#endif

                        break;
                    }
                case BuildTarget.StandaloneOSX:
                    {
                        PlayerName = "UnityPlayer.app/Contents/MacOS/UnityPlayer";

                        IL2CPPVariations = new string[]
                        {
                            "macosx64_development_il2cpp",
                            "macosx64_nondevelopment_il2cpp"
                        };

                        break;
                    }
#endif
                case BuildTarget.WSAPlayer:
                    {
                        PlayerName = "UnityPlayer.dll";

                        IL2CPPVariations = new string[]
                        {
                            "il2cpp/ARM/debug",
                            "il2cpp/ARM/master",
                            "il2cpp/ARM/release",
                            "il2cpp/x86/debug",
                            "il2cpp/x86/master",
                            "il2cpp/x86/release",
                            "il2cpp/x64/debug",
                            "il2cpp/x64/master",
                            "il2cpp/x64/release",
                        };

                        break;
                    }

                default:
                    return false;
            }

            if (Target == BuildTarget.WSAPlayer) {
                return IL2CPPVariations.ToList().Exists((string x) =>
                    File.Exists(Path.Combine(Path.Combine(PlayerPackage, "Players/UAP"), Path.Combine(x, PlayerName))));
            }
            else {
                return IL2CPPVariations.ToList().Exists((string x) =>
                    File.Exists(Path.Combine(Path.Combine(PlayerPackage, "Variations"), Path.Combine(x, PlayerName))));
            }
        }

    }
}
