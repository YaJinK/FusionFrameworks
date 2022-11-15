using Fusion.Frameworks.Assets.Editor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.VisualScripting.YamlDotNet.Core;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;

namespace Fusion.Frameworks.ILRuntime.Editor
{
    public class DLLPacker
    {
        private static List<string> scriptPaths = new List<string>();
        private static DLLSetting dllSetting = null;

        static DLLPacker()
        {
            dllSetting = AssetDatabase.LoadAssetAtPath<DLLSetting>(string.Format("{0}/{1}.asset", AssetsPacker.FilePathPrefix, typeof(DLLSetting).Name));
            if (dllSetting == null)
            {
                dllSetting = ScriptableObject.CreateInstance<DLLSetting>();
                AssetDatabase.CreateAsset(dllSetting, $"{AssetsPacker.FilePathPrefix}/DLLSetting.asset");
                AssetDatabase.Refresh();
            }
        }

        [MenuItem("ILRuntime/Extensions/Pack")]
        public static void Pack()
        {
            if (!AssetDatabase.IsValidFolder("Assets/StreamingAssets"))
            {
                AssetDatabase.CreateFolder("Assets", "StreamingAssets");
            }
            scriptPaths.Clear();
            for (int i = 0; i < dllSetting.scriptsForPack.Length; i++)
            {
                string path = AssetDatabase.GetAssetPath(dllSetting.scriptsForPack[i]);
                if (AssetDatabase.IsValidFolder(path))
                {
                    CollectCSharpFile(path);
                }
            }

            if (!Directory.Exists("FusionTemp/DLL"))
            {
                Directory.CreateDirectory("FusionTemp/DLL");
            }

            string output = "FusionTemp/DLL/Extra.dll";
            string productParentFolder = "Assets/GameAssets/Scripts";
            string productOutput = "Assets/GameAssets/Scripts/Extra.dll";

            AssemblyBuilder assemblyBuilder = new AssemblyBuilder(output, scriptPaths.ToArray());
            Debug.LogError(EditorApplication.applicationContentsPath);
            assemblyBuilder.excludeReferences = new string[] {
                $"{EditorApplication.applicationContentsPath}/Managed/UnityEngine.dll".Replace("/", "\\")
            };

            assemblyBuilder.additionalReferences = new string[] {
                $"{EditorApplication.applicationContentsPath}/Managed/UnityEngine/UnityEngine.CoreModule.dll"
            };

            assemblyBuilder.buildFinished += delegate (string assemblyPath, CompilerMessage[] compilerMessages)
            {
                var errorCount = compilerMessages.Count(m => m.type == CompilerMessageType.Error);
                var warningCount = compilerMessages.Count(m => m.type == CompilerMessageType.Warning);

                Debug.LogFormat("Assembly build finished for {0}", assemblyPath);
                Debug.LogFormat("Warnings: {0} - Errors: {0}", errorCount, warningCount);

                for (int i = 0; i < compilerMessages.Length; i++)
                {
                    Debug.LogError(compilerMessages[i].message);
                }

                if (errorCount == 0)
                {
                    if (!Directory.Exists(productParentFolder))
                    {
                        Directory.CreateDirectory(productParentFolder);
                        AssetDatabase.ImportAsset(productParentFolder);

                    }
                    File.Copy(output, productOutput, true);
                    AssetDatabase.ImportAsset(productOutput);
                }
            };
            assemblyBuilder.Build();
            while (assemblyBuilder.status != AssemblyBuilderStatus.Finished)
            {

            }
        }

        private static void CollectCSharpFile(string path)
        {
            string[] fileList = Directory.GetFiles(path);

            for (int i = 0; i < fileList.Length; i++)
            {
                FileInfo fileInfo = new FileInfo(fileList[i]);

                if (CheckFileValid(fileInfo))
                {
                    string filePath = fileList[i].Replace("/", "/");
                    if (!scriptPaths.Contains(filePath))
                    {
                        scriptPaths.Add(filePath);
                    }
                }
            }

            string[] dirList = Directory.GetDirectories(path);
            for (int index = 0; index < dirList.Length; index++)
            {
                CollectCSharpFile(dirList[index]);
            }
        }

        private static bool CheckFileValid(FileInfo fileInfo)
        {
            if ((fileInfo.Attributes & FileAttributes.Directory) == FileAttributes.Directory)
                return false;
            return fileInfo.Name.EndsWith(".cs");
        }

        private static void BackupCSharp()
        {
            for (int i = 0; i < dllSetting.scriptsForPack.Length; i++)
            {
                string path = AssetDatabase.GetAssetPath(dllSetting.scriptsForPack[i]);
                if (AssetDatabase.IsValidFolder(path) && !path.EndsWith("~"))
                {
                    AssetDatabase.RenameAsset(path, $"{dllSetting.scriptsForPack[i].name}~");
                }
            }

            AssetDatabase.Refresh();
        }

        public static void RecoverCSharp()
        {
            for (int i = 0; i < dllSetting.scriptsForPack.Length; i++)
            {
                string path = AssetDatabase.GetAssetPath(dllSetting.scriptsForPack[i]);
                if (AssetDatabase.IsValidFolder(path) && path.EndsWith("~"))
                {
                    AssetDatabase.RenameAsset(path, $"{dllSetting.scriptsForPack[i].name.Substring(0, dllSetting.scriptsForPack[i].name.LastIndexOf("~"))}");
                }
            }
            AssetDatabase.Refresh();
        }
    }

}

