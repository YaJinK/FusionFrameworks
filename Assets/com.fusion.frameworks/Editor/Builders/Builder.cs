
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

        static Builder()
        {
            buildSetting = AssetDatabase.LoadAssetAtPath<BuildSetting>(string.Format("{0}/{1}.asset", filePathPrefix, typeof(BuildSetting).Name));
            if (buildSetting == null)
            {
                buildSetting = ScriptableObject.CreateInstance<BuildSetting>();
                AssetDatabase.CreateAsset(buildSetting, $"{filePathPrefix}/BuildSetting.asset");
                AssetDatabase.Refresh();
            }
        }

        [MenuItem("Build/BuildAll")]
        public static void BuildAll()
        {
            BuildAssets();
            BuildPlayer();
        }

        [MenuItem("Build/BuildAssets")]
        public static void BuildAssets()
        {
            DLLPacker.Build();
            AssetsPacker.Build();
        }

        [MenuItem("Build/BuildPlayer")]
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
    }
}


