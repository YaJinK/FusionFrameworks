using Fusion.Frameworks.Assets.Editor;
using Fusion.Frameworks.DynamicDLL.Mono;
using Fusion.Frameworks.Editor;
using Fusion.Frameworks.UI;
using Fusion.Frameworks.Utilities;
using ILRuntime.Runtime.Enviorment;
using System;
using System.Collections;
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
        private static string customCLRBindingPath = "Assets/Scripts/Generated/CLRBinding";
        private static string customAdapterPath = "Assets/Scripts/Generated/Adapters";
        private static string dllOutput = "FusionTemp/DLL/Extra.dll";

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

        [MenuItem("DLLManager/SwichDynamicDLLState/Disabled", false, 501)]
        private static void SwichDynamicDLLEnabled()
        {
            Builder.DeleteScriptingDefineSymbol("FUSION_DYNAMIC_DLL");
        }

        [MenuItem("DLLManager/SwichDynamicDLLState/Enabled", false, 500)]
        private static void SwichDynamicDLLDisabled()
        {
            Builder.AppendScriptingDefineSymbol("FUSION_DYNAMIC_DLL");
        }

        [MenuItem("DLLManager/GenerateCLRBinding", false, 200)]
        static void GenerateCLRBindingByAnalysis()
        {
            ILRuntime.Runtime.Enviorment.AppDomain appDomain = new ILRuntime.Runtime.Enviorment.AppDomain();
            using (System.IO.FileStream fs = new System.IO.FileStream(dllOutput, System.IO.FileMode.Open, System.IO.FileAccess.Read))
            {
                appDomain.LoadAssembly(fs);
                Type classType = Type.GetType("Fusion.Frameworks.DynamicDLL.DLLCustomBinder, Assembly-CSharp");
                DLLBinder dllBinder = classType != null ? (DLLBinder)Activator.CreateInstance(classType, appDomain) : new DLLBinder(appDomain);
                dllBinder.Register();
                ILRuntime.Runtime.CLRBinding.BindingCodeGenerator.GenerateBindingCode(appDomain, customCLRBindingPath);
            }

            AssetDatabase.Refresh();
        }

        [MenuItem("DLLManager/GenerateBuildInAdapters", false, 200)]
        public static void GenerateBuildInAdapters()
        {
            string buildInAdapterPath = "Assets/com.fusion.frameworks/Runtime/Scripts/DLLManager/Adapters";
            IOUtility.Write($"{buildInAdapterPath}/UIObjectAdapter.cs", CrossBindingCodeGenerator.GenerateCrossBindingAdapterCode(typeof(UIObject), "Fusion.Frameworks.DynamicDLL.Adapters"));
            IOUtility.Write($"{buildInAdapterPath}/UIDataAdapter.cs", CrossBindingCodeGenerator.GenerateCrossBindingAdapterCode(typeof(UIData), "Fusion.Frameworks.DynamicDLL.Adapters"));
            IOUtility.Write($"{buildInAdapterPath}/DLLMonoBaseAdapter.cs", CrossBindingCodeGenerator.GenerateCrossBindingAdapterCode(typeof(DLLMonoBase), "Fusion.Frameworks.DynamicDLL.Adapters"));
            AssetDatabase.Refresh();
        }

        [MenuItem("DLLManager/GenerateCustomBinder", false, 200)]
        public static void GenerateCustomBinder()
        {
            if (AssetDatabase.IsValidFolder(customAdapterPath))
            {
                AssetDatabase.DeleteAsset(customAdapterPath);
            }
            StringBuilder dllCustomBinder = new StringBuilder();
            StringBuilder importString = new StringBuilder("using ILRuntime.Runtime.Enviorment;");

            StringBuilder adapters = null;
            if (dllSetting.customAdapters != null && dllSetting.customAdapters.Length > 0)
            {
                
                Directory.CreateDirectory(customAdapterPath);
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
                    IOUtility.Write($"{customAdapterPath}/{namePrefix}Adapter.cs", CrossBindingCodeGenerator.GenerateCrossBindingAdapterCode(classType, "Fusion.Frameworks.DynamicDLL.Adapters.Custom"));
                    adapters.Append(@$"
            appDomain.RegisterCrossBindingAdaptor(new {namePrefix}Adapter());");
                }
                adapters.Append(@"
        }");
            }

            StringBuilder methodDelegates = null;
            if (dllSetting.customMethodDelegates != null && dllSetting.customMethodDelegates.Length > 0)
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
            dllCustomBinder.AppendLine(@"
        public override void RegisterCLRBinding()
        {
            base.RegisterCLRBinding();
            ILRuntime.Runtime.Generated.CLRBindings.Initialize(appDomain);
        }");
            dllCustomBinder.AppendLine("    }");
            dllCustomBinder.AppendLine("}");
            IOUtility.Write($"{customGeneratedPath}/DLLCustomBinder.cs", dllCustomBinder.ToString());

            AssetDatabase.Refresh();
        }

        [MenuItem("DLLManager/Build", false, 2000)]
        public static void Build()
        {
            if (dllSetting.scriptsForPack == null)
            {
                return;
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

            string productParentFolder = "Assets/GameAssets/Scripts";
            string productOutput = "Assets/GameAssets/Scripts/Extra.bytes";

            AssemblyBuilder assemblyBuilder = new AssemblyBuilder(dllOutput, scriptPaths.ToArray());
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
                    File.Copy(dllOutput, productOutput, true);
                    AssetDatabase.ImportAsset(productOutput);
                }
            };
            assemblyBuilder.Build();
            while (assemblyBuilder.status != AssemblyBuilderStatus.Finished)
            {

            }

            GenerateCustomBinder();
            GenerateCLRBindingByAnalysis();
            AssetDatabase.Refresh();
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

        public static void BackupCSharp()
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

