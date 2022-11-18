using Fusion.Frameworks.Assets.Editor;
using Fusion.Frameworks.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;
using UnityEngine.Rendering;

namespace Fusion.Frameworks.DynamicDLL.Editor
{
    public class DLLPacker
    {
        private static List<string> scriptPaths = new List<string>();
        private static DLLSetting dllSetting = null;
        private static string customGeneratedPath = "Assets/Scripts/Generated";
        private static string customAdapterPath = "Assets/Scripts/Generated/Adapters";
        static DLLPacker()
        {
            dllSetting = AssetDatabase.LoadAssetAtPath<DLLSetting>(string.Format("{0}/{1}.asset", AssetsPacker.FilePathPrefix, typeof(DLLSetting).Name));
            if (dllSetting == null)
            {
                dllSetting = ScriptableObject.CreateInstance<DLLSetting>();
                AssetDatabase.CreateAsset(dllSetting, $"{AssetsPacker.FilePathPrefix}/DLLSetting.asset");
                AssetDatabase.Refresh();
            }
            if (!Directory.Exists(customGeneratedPath))
            {
                Directory.CreateDirectory(customGeneratedPath);
            }
        }

        [MenuItem("DLLManager/GeneratBuildInAdapters")]
        public static void GenerateBuildInAdapters()
        {
            string buildInAdapterPath = "Assets/com.fusion.frameworks/Runtime/Scripts/DLLManager/Adapters";
            using (System.IO.StreamWriter sw = new System.IO.StreamWriter($"{buildInAdapterPath}/UIObjectAdapter.cs"))
            {
                sw.WriteLine(ILRuntime.Runtime.Enviorment.CrossBindingCodeGenerator.GenerateCrossBindingAdapterCode(typeof(UIObject), "Fusion.Frameworks.DynamicDLL.Adapters"));
            }

            using (System.IO.StreamWriter sw = new System.IO.StreamWriter($"{buildInAdapterPath}/UIDataAdapter.cs"))
            {
                sw.WriteLine(ILRuntime.Runtime.Enviorment.CrossBindingCodeGenerator.GenerateCrossBindingAdapterCode(typeof(UIData), "Fusion.Frameworks.DynamicDLL.Adapters"));
            }

            AssetDatabase.Refresh();
        }

        [MenuItem("DLLManager/GenerateCustomBinder")]
        public static void GenerateCustomVBinder()
        {   
            if (AssetDatabase.IsValidFolder(customAdapterPath))
            {
                AssetDatabase.DeleteAsset(customAdapterPath);
            }
            Directory.CreateDirectory(customAdapterPath);

            StringBuilder dllCustomBinder = new StringBuilder();
            StringBuilder importString = new StringBuilder("using ILRuntime.Runtime.Enviorment;");

            StringBuilder adapters = null;
            if (dllSetting.customAdapters != null)
            {
                adapters = new StringBuilder(@"
        public override void RegisterAdapters()
        {
            base.RegisterAdapters();");
                for (int index = 0; index < dllSetting.customAdapters.Length; index++)
                {
                    string className = dllSetting.customAdapters[index];
                    string[] classNameAndAssemblyName = className.Split(",");
                    if (classNameAndAssemblyName.Length == 1)
                    {
                        className = $"{className},Assembly-CSharp";
                    }
                    Type classType = Type.GetType(className);

                    string namePrefix = classNameAndAssemblyName[0].Substring(classNameAndAssemblyName[0].LastIndexOf(".") + 1);
                    using (System.IO.StreamWriter sw = new System.IO.StreamWriter($"{customAdapterPath}/{namePrefix}Adapter.cs"))
                    {
                        sw.WriteLine(ILRuntime.Runtime.Enviorment.CrossBindingCodeGenerator.GenerateCrossBindingAdapterCode(classType, "Fusion.Frameworks.DynamicDLL.Adapters.Custom"));
                    }
                    adapters.Append(@$"
            appDomain.RegisterCrossBindingAdaptor(new {namePrefix}Adapter());");
                }
                adapters.Append(@"
        }");
            }

            StringBuilder methodDelegates = null;
            if (dllSetting.customMethodDelegates != null)
            {
                methodDelegates = new StringBuilder(@"
        public override void RegisterMethodDelegate()
        {
            base.RegisterMethodDelegate();");
                for (int index = 0; index < dllSetting.customMethodDelegates.Length; index++)
                {
                    string className = dllSetting.customMethodDelegates[index];
                    methodDelegates.Append(@$"
            appDomain.DelegateManager.RegisterMethodDelegate<{className}>();");
                }
                methodDelegates.Append(@"
        }");
            }

            dllCustomBinder.AppendLine(importString.ToString());
            dllCustomBinder.AppendLine(@"
namespace Fusion.Frameworks.DynamicDLL
{
    public class DLLCustomBinder : DLLBinder
    {
        public DLLCustomBinder(AppDomain appDomain) : base(appDomain)
        {
        }");
            if (adapters != null)
            {
                dllCustomBinder.AppendLine(adapters.ToString());
            }
            if (methodDelegates != null)
            {
                dllCustomBinder.AppendLine(methodDelegates.ToString());
            }
            dllCustomBinder.AppendLine("    }");
            dllCustomBinder.AppendLine("}");
            using (System.IO.StreamWriter sw = new System.IO.StreamWriter($"{customGeneratedPath}/DLLCustomBinder.cs"))
            {
                sw.WriteLine(dllCustomBinder);
            }
            AssetDatabase.Refresh();
        }

        [MenuItem("DLLManager/Pack")]
        public static void Pack()
        {
            if (dllSetting.scriptsForPack == null)
            {
                return;
            }

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
            string productOutput = "Assets/GameAssets/Scripts/Extra.bytes";

            AssemblyBuilder assemblyBuilder = new AssemblyBuilder(output, scriptPaths.ToArray());
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
                    if (compilerMessages[i].type == CompilerMessageType.Error)
                    {
                        Debug.LogError(compilerMessages[i].message);
                    } else
                    {
                        Debug.LogWarning(compilerMessages[i].message);
                    }
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

