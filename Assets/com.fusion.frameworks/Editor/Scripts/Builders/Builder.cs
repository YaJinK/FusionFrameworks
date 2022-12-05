
using Fusion.Frameworks.Assets.Editor;
using Fusion.Frameworks.DynamicDLL.Editor;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Fusion.Frameworks.Editor
{
    public class Builder
    {
        private static Dictionary<BuildTarget, string> targetSuffix = new Dictionary<BuildTarget, string> {
            { BuildTarget.StandaloneWindows, ".exe" },
            { BuildTarget.StandaloneWindows64, ".exe" },
            { BuildTarget.Android, ".apk" },
        };

        public static string filePathPrefix = "Assets/GameAssets";
        private static BuildSetting buildSetting = null;

        public static BuildSetting BuildSetting { get => buildSetting; }

        [MenuItem("Build/CreateGameAssetsFolder", false, 200)]
        public static void CreateGameAssetsFolder()
        {
            int divideIndex = filePathPrefix.IndexOf("/");
            if (divideIndex != -1)
            {
                if (!AssetDatabase.IsValidFolder(filePathPrefix))
                {
                    AssetDatabase.CreateFolder("Assets", filePathPrefix.Substring(divideIndex + 1));
                }
            }

            buildSetting = AssetDatabase.LoadAssetAtPath<BuildSetting>(string.Format("{0}/{1}.asset", filePathPrefix, typeof(BuildSetting).Name));
            if (buildSetting == null)
            {
                buildSetting = ScriptableObject.CreateInstance<BuildSetting>();
                AssetDatabase.CreateAsset(buildSetting, $"{filePathPrefix}/BuildSetting.asset");
                AssetDatabase.Refresh();
            }

            AssetDatabase.Refresh();
        }

        [MenuItem("Build/Build", false, 2002)]
        public static void BuildAll()
        {
            BuildAssets();
            BuildPlayer();
        }

        [MenuItem("Build/BuildAssets", false, 1000)]
        public static void BuildAssets()
        {
            BuildDynamic();
            AssetsPacker.CopyAssetsToStreamingAssets();
        }

        [MenuItem("Build/BuildDynamic", false, 2001)]
        public static void BuildDynamic()
        {
            Excel.Editor.ExcelConverter.Build();
            DLLPacker.Build();
            AssetsPacker.Build();
        }

        [MenuItem("Build/BuildPlayer", false, 1001)]
        public static void BuildPlayer()
        {
            DLLPacker.BackupCSharp();

            string path = null;
            if (buildSetting.initScene != null)
            {
                path = AssetDatabase.GetAssetPath(buildSetting.initScene);
            }
            else
            {
                path = EditorSceneManager.GetActiveScene().path;
            }
            BuildTarget target = GetCurrentBuildTarget();
            string output = $"Output/{target}";
            if (!Directory.Exists(output))
            {
                Directory.CreateDirectory(output);
            }

            string suffix = targetSuffix.ContainsKey(target) ? targetSuffix[target] : "";

            BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
            buildPlayerOptions.scenes = new[] { path };
            buildPlayerOptions.locationPathName = $"{output}/{Application.productName}{suffix}";
            buildPlayerOptions.target = target;
            buildPlayerOptions.options = BuildOptions.None;

            BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
            BuildSummary summary = report.summary;

            if (summary.result == BuildResult.Succeeded)
            {
                Debug.Log("Build succeeded: " + summary.totalSize + " bytes");
            }

            if (summary.result == BuildResult.Failed)
            {
                Debug.Log("Build failed");
            }
            DLLPacker.RecoverCSharp();
        }

        public static BuildTarget GetCurrentBuildTarget()
        {
            BuildTarget buildTarget = buildSetting.buildTargetType == BuildTargetType.UseCurrentTarget ? EditorUserBuildSettings.activeBuildTarget : (BuildTarget)buildSetting.buildTargetType;
            return buildTarget;
        }

        public static void AppendScriptingDefineSymbol(string scriptingDefineSymbol)
        {
            BuildTargetGroup buildTargetGroup = BuildPipeline.GetBuildTargetGroup(GetCurrentBuildTarget());

            string scriptingDefineSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);
            if (!scriptingDefineSymbols.Contains(scriptingDefineSymbol)) 
            {
                scriptingDefineSymbols = $"{scriptingDefineSymbols};{scriptingDefineSymbol}";
                PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, scriptingDefineSymbols);
            }
            Debug.Log(scriptingDefineSymbol);
            Debug.Log(scriptingDefineSymbols);
            Debug.Log(PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup));
        }

        public static void DeleteScriptingDefineSymbol(string scriptingDefineSymbol)
        {
            BuildTargetGroup buildTargetGroup = BuildPipeline.GetBuildTargetGroup(GetCurrentBuildTarget());

            string scriptingDefineSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);
            if (scriptingDefineSymbols.Contains(scriptingDefineSymbol))
            {
                scriptingDefineSymbols = scriptingDefineSymbols.Replace(scriptingDefineSymbol, "");
                PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, scriptingDefineSymbols);
            }
            Debug.Log(PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup));

        }
    }
}


