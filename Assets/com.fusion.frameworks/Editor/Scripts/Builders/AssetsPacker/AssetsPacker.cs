using Codice.Utils;
using Fusion.Frameworks.Editor;
using Fusion.Frameworks.Utilities;
using Fusion.Frameworks.Version;
using LitJson;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Security.Cryptography;
using UnityEditor;
using UnityEditor.U2D;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.U2D;

namespace Fusion.Frameworks.Assets.Editor
{
    /// <summary>
    /// 资源打包类
    /// 将GameAssets文件夹下的所有文件打包成AssetBundle
    /// 可以通过在资源目录下创建BuildProperty和AtlasProperty设置AssetBundle的属性
    /// </summary>
    public class AssetsPacker
    {
        private static string filePathPrefix = Builder.filePathPrefix;
        private static string assetBundleSuffix = AssetsConfig.assetBundleSuffix;

        private static Dictionary<string, string> assetBundlesNameMap = new Dictionary<string, string>();

        private static string assetsOutput = "FusionTemp/AssetsCache";

        private static string[] ignoreFileList =
        {
            "AtlasProperty.asset",
            "BuildProperty.asset",
            "BuildSetting.asset",
            "DLLSetting.asset",
        };

        public static string FilePathPrefix { get => filePathPrefix; }

        [MenuItem("AssetsManager/CopyAssetsToStreamingAssets", false, 205)]
        public static void CopyAssetsToStreamingAssets()
        {
            if (Directory.Exists($"{Application.streamingAssetsPath}/ManagedAssets"))
            {
                Directory.Delete($"{Application.streamingAssetsPath}/ManagedAssets", true);
            }
            string output = GetAssetsOutput();
            if (Directory.Exists(output))
            {
                CopyDirectoryToStreamingAssets(output);
            }
        }

        [MenuItem("AssetsManager/SwichEditorAssetLoadType/AssetDatabase", false, 500)]
        private static void SwichEditorAssetLoadTypeToADB()
        {
            Builder.DeleteScriptingDefineSymbol("FUSION_ASSETBUNDLE");
        }

        [MenuItem("AssetsManager/SwichEditorAssetLoadType/AssetBundle", false, 500)]
        private static void SwichEditorAssetLoadTypeToAB()
        {
            Builder.AppendScriptingDefineSymbol("FUSION_ASSETBUNDLE");
        }

        private static void Pack()
        {
            string outputDir = GetAssetsOutput();
            if (!Directory.Exists(outputDir))
            {
                Directory.CreateDirectory(outputDir);
            }
            assetBundlesNameMap.Clear();
            PackFolder(Application.dataPath.Substring(0, Application.dataPath.LastIndexOf("/") + 1) + filePathPrefix);
            GenerateConfigFile();
            GenerateVersionFile();
            EditorUtility.ClearProgressBar();
            AssetDatabase.Refresh();
        }

        [MenuItem("AssetsManager/ClearAssetBundleName", false, 201)]
        private static void ClearAssetBundleName()
        {
            ClearAssetBundleName(Application.dataPath.Substring(0, Application.dataPath.LastIndexOf("/") + 1) + filePathPrefix);
            EditorUtility.ClearProgressBar();
            AssetDatabase.Refresh();
        }

        [MenuItem("AssetsManager/Build", false, 2000)]
        public static void Build()
        {
            Pack();
            PackAssetBundle();
            GenerateMD5();
            GeneratePatchs();
        }

        private static string GetAssetsOutput()
        {
            string outputDir = $"{GetVersionTempPath()}/ManagedAssets";
            return outputDir;
        }

        private static string GetVersionTempPath()
        {
            string outputDir = $"{GetBuildModeTempPath()}/{Builder.BuildSetting.version}";
            return outputDir;
        }
        private static string GetBuildModeTempPath()
        {
            string outputDir = $"{assetsOutput}/{GetCurrentBuildTarget()}/{Builder.BuildSetting.buildMode}";
            return outputDir;
        }

        private static string GetPatchTempPath()
        {
            string patchsPath = $"{GetVersionTempPath()}/Patchs";
            return patchsPath;
        }

        private static void PackAssetBundle()
        {
            string outputDir = GetAssetsOutput();
            if (!Directory.Exists(outputDir))
            {
                Directory.CreateDirectory(outputDir);
            }

            BuildAssetBundleOptions compressType = (BuildAssetBundleOptions)Builder.BuildSetting.compressType;
            BuildAssetBundleOptions optionsAll = compressType;

            BuildPipeline.BuildAssetBundles(outputDir, optionsAll, GetCurrentBuildTarget());
            DeleteManifest(outputDir);
            AssetDatabase.Refresh();
        }

        private static BuildTarget GetCurrentBuildTarget()
        {
            return Builder.GetCurrentBuildTarget();
        }

        private static void SetAssetBundleName(AssetImporter assetImporter, BuildProperty buildProperty, FileInfo fileInfo)
        {
            string path = fileInfo.Directory.FullName.Replace("\\", "/");

            if (buildProperty.type == BuildType.File)
                assetImporter.assetBundleName = path.Substring(path.IndexOf(filePathPrefix) + filePathPrefix.Length + 1) + "/" + fileInfo.Name.Substring(0, fileInfo.Name.LastIndexOf(".")) + assetBundleSuffix;
            else if (buildProperty.type == BuildType.Folder)
            {
                assetImporter.assetBundleName = path.Substring(path.IndexOf(filePathPrefix) + filePathPrefix.Length + 1) + assetBundleSuffix;
                assetBundlesNameMap[path.Substring(path.IndexOf(filePathPrefix) + filePathPrefix.Length + 1) + "/" + fileInfo.Name.Substring(0, fileInfo.Name.LastIndexOf("."))] = assetImporter.assetBundleName;
            }
        }

        private static void BuildFolderAssetBundle(string path, BuildProperty buildProperty)
        {
            path = path.Replace("\\", "/");

            string[] fileList = Directory.GetFiles(path);
            string releativePath = path.Substring(path.IndexOf(filePathPrefix));

            List<AssetBundleBuild> assetBundleBuilds = new List<AssetBundleBuild>();
            string assetBundlePath = releativePath.Substring(filePathPrefix.Length + 1);
            if (Directory.Exists($"{GetAssetsOutput()}/{assetBundlePath}"))
            {
                Directory.Delete($"{GetAssetsOutput()}/{assetBundlePath}", true);
            }
            if (buildProperty.type == BuildType.Folder)
            {
                AssetBundleBuild assetBundleBuild = new AssetBundleBuild();
                string assetBundleName = $"{assetBundlePath}{assetBundleSuffix}";
                assetBundleBuild.assetBundleName = assetBundleName;
                List<string> assetNames = new List<string>();
                for (int index = 0; index < fileList.Length; index++)
                {
                    FileInfo fileInfo = new FileInfo(fileList[index]);
                    if (CheckFileValid(fileInfo))
                    {
                        string fileFullName = releativePath + "/" + fileInfo.Name;
                        AssetImporter assetImporter = AssetImporter.GetAtPath(fileFullName);
                        assetImporter.assetBundleName = null;
                        string assetName = $"{releativePath}/{fileInfo.Name}";
                        assetNames.Add(assetName);
                        assetBundlesNameMap[$"{assetBundlePath}/{fileInfo.Name.Substring(0, fileInfo.Name.LastIndexOf("."))}"] = assetBundleName.ToLower();
                        EditorUtility.DisplayProgressBar(string.Format("Packing: {0}", releativePath), string.Format("Atlas: Progress: {0}/{1}", index, fileList.Length), (float)index / (float)fileList.Length);
                    }
                }
                assetBundleBuild.assetNames = assetNames.ToArray();
                assetBundleBuilds.Add(assetBundleBuild);

            }
            else if (buildProperty.type == BuildType.File)
            {
                for (int index = 0; index < fileList.Length; index++)
                {
                    FileInfo fileInfo = new FileInfo(fileList[index]);
                    if (CheckFileValid(fileInfo))
                    {
                        string fileFullName = releativePath + "/" + fileInfo.Name;
                        AssetImporter assetImporter = AssetImporter.GetAtPath(fileFullName);
                        assetImporter.assetBundleName = null;
                        AssetBundleBuild assetBundleBuild = new AssetBundleBuild();
                        string assetBundleName = $"{assetBundlePath}/{fileInfo.Name.Substring(0, fileInfo.Name.LastIndexOf("."))}";
                        assetBundleBuild.assetBundleName = assetBundleName;
                        string assetName = $"{releativePath}/{fileInfo.Name}";
                        assetBundleBuild.assetNames = new string[] { assetName };
                        EditorUtility.DisplayProgressBar(string.Format("Packing: {0}", releativePath), string.Format("Atlas: Progress: {0}/{1}", index, fileList.Length), (float)index / (float)fileList.Length);
                        assetBundleBuilds.Add(assetBundleBuild);
                    }
                }
            }

            BuildAssetBundleOptions compressType = (BuildAssetBundleOptions)buildProperty.compressType;
            BuildAssetBundleOptions optionsAll = compressType;

            BuildPipeline.BuildAssetBundles(GetAssetsOutput(), assetBundleBuilds.ToArray(), optionsAll, GetCurrentBuildTarget());

            AssetDatabase.Refresh();
        }

        private static void SetFolderAssetBundleName(string path, BuildProperty buildProperty, AtlasProperty atlasProperty)
        {
            string[] fileList = Directory.GetFiles(path);
            string releativePath = path.Substring(path.IndexOf(filePathPrefix));

            if (atlasProperty != null && atlasProperty.packUnit != 0)
            {
                for (int index = 0; index < fileList.Length; index++)
                {
                    FileInfo fileInfo = new FileInfo(fileList[index]);
                    if (fileInfo.Name.EndsWith(".spriteatlas"))
                    {
                        // 删除存在图集
                        string fileFullName = releativePath + "/" + fileInfo.Name;
                        AssetDatabase.DeleteAsset(fileFullName);
                    }
                }

                Vector2 ignoreSpriteSize = atlasProperty.ignoreSize;
                int spriteNoOfAtlas = 0;
                int atlasNo = 0;
                List<Object> list = new List<Object>();
                for (int index = 0; index < fileList.Length; index++)
                {
                    FileInfo fileInfo = new FileInfo(fileList[index]);
                    if (CheckFileValid(fileInfo))
                    {
                        string fileFullName = releativePath + "/" + fileInfo.Name;
                        AssetImporter assetImporter = AssetImporter.GetAtPath(fileFullName);
                        Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(fileFullName);
                        if (sprite != null && sprite.texture.width * sprite.texture.height < ignoreSpriteSize.x * ignoreSpriteSize.y)
                        {
                            string atlasName = null;

                            // 启用图集
                            if (atlasProperty.packUnit > 0)
                            {
                                if (spriteNoOfAtlas < atlasProperty.packUnit)
                                    spriteNoOfAtlas = spriteNoOfAtlas + 1;
                                else
                                {
                                    spriteNoOfAtlas = 1;
                                    atlasNo = atlasNo + 1;
                                    list = new List<Object>();
                                }
                                atlasName = string.Format("atlas@{0}", atlasNo);
                            }
                            else if (atlasProperty.packUnit == -1)
                            {
                                spriteNoOfAtlas = spriteNoOfAtlas + 1;
                                atlasName = "atlas";
                            }

                            string abName = releativePath.Substring(filePathPrefix.Length + 1) + "/" + atlasName + assetBundleSuffix;

                            // 还没有创建过图集
                            SpriteAtlas spriteAtlas = AssetDatabase.LoadAssetAtPath<SpriteAtlas>(releativePath + "/" + atlasName);
                            if (spriteAtlas == null)
                            {
                                spriteAtlas = new SpriteAtlas();
                                AssetDatabase.CreateAsset(spriteAtlas, releativePath + "/" + atlasName + ".spriteatlas");
                            }

                            // sprite加入图集
                            list.Add(sprite);
                            spriteAtlas.Add(list.ToArray());

                            SpriteAtlasPackingSettings saPackSetting = new SpriteAtlasPackingSettings();
                            saPackSetting.enableRotation = false;
                            saPackSetting.enableTightPacking = false;
                            saPackSetting.padding = 4;
                            spriteAtlas.SetPackingSettings(saPackSetting);

                            TextureImporterPlatformSettings texImpPlatSettings = spriteAtlas.GetPlatformSettings("iPhone");
                            texImpPlatSettings.overridden = true;
                            texImpPlatSettings.format = TextureImporterFormat.ASTC_8x8;
                            spriteAtlas.SetPlatformSettings(texImpPlatSettings);

                            assetBundlesNameMap[releativePath.Substring(filePathPrefix.Length + 1) + "/" + fileInfo.Name.Substring(0, fileInfo.Name.LastIndexOf("."))] = abName.ToLower();
                            assetImporter.assetBundleName = abName;
                            AssetDatabase.SaveAssets();

                            EditorUtility.DisplayProgressBar(string.Format("Packing: {0}", releativePath), string.Format("Atlas: Progress: {0}/{1}  PackUnit: {2}", index, fileList.Length, atlasProperty.packUnit), (float)index / (float)fileList.Length);
                        }
                        else
                        {
                            SetAssetBundleName(assetImporter, buildProperty, fileInfo);
                            EditorUtility.DisplayProgressBar(string.Format("Packing: {0}", releativePath), string.Format("Atlas: Progress: {0}/{1}", index, fileList.Length), (float)index / (float)fileList.Length);
                        }
                    }
                }
            }
            else
            {
                for (int index = 0; index < fileList.Length; index++)
                {
                    FileInfo fileInfo = new FileInfo(fileList[index]);
                    if (CheckFileValid(fileInfo))
                    {
                        string fileFullName = releativePath + "/" + fileInfo.Name;
                        AssetImporter assetImporter = AssetImporter.GetAtPath(fileFullName);
                        SetAssetBundleName(assetImporter, buildProperty, fileInfo);
                        EditorUtility.DisplayProgressBar(string.Format("Packing: {0}", releativePath), string.Format("Atlas: Progress: {0}/{1}", index, fileList.Length), (float)index / (float)fileList.Length);
                    }
                }
            }
        }

        private static void PackFolder(string path)
        {
            path = path.Replace("\\", "/");
            string releativePath = path.Substring(path.IndexOf(filePathPrefix));

            BuildProperty buildProperty = GetProperty<BuildProperty>(releativePath);
            if (buildProperty == null)
                buildProperty = ScriptableObject.CreateInstance<BuildProperty>();

            if (buildProperty.compressType == FolderCompressType.UseBuildSetting || (CompressType)buildProperty.compressType == Builder.BuildSetting.compressType)
            {
                AtlasProperty atlasProperty = GetProperty<AtlasProperty>(releativePath);
                SetFolderAssetBundleName(path, buildProperty, atlasProperty);
            } else
            {
                BuildFolderAssetBundle(path, buildProperty);
            }

            AssetDatabase.Refresh();
            string[] dirList = Directory.GetDirectories(path);
            for (int index = 0; index < dirList.Length; index++)
            {
                PackFolder(dirList[index]);
            }

        }

        private static void ClearAssetBundleName(string path)
        {
            path = path.Replace("\\", "/");
            string[] fileList = Directory.GetFiles(path);
            string releativePath = path.Substring(path.IndexOf(filePathPrefix));
            for (int index = 0; index < fileList.Length; index++)
            {
                FileInfo fileInfo = new FileInfo(fileList[index]);
                if (CheckFileValid(fileInfo))
                {
                    string fileFullName = releativePath + "/" + fileInfo.Name;
                    AssetImporter assetImporter = AssetImporter.GetAtPath(fileFullName);
                    assetImporter.assetBundleName = null;
                    EditorUtility.DisplayProgressBar(string.Format("Clear AssetBundleName: {0}", releativePath.Substring(filePathPrefix.Length + 1)), fileFullName, (float)index / (float)fileList.Length);
                }
            }
            string[] dirList = Directory.GetDirectories(path);
            for (int index = 0; index < dirList.Length; index++)
            {
                ClearAssetBundleName(dirList[index]);
            }

        }

        private static bool CheckFileValid(FileInfo fileInfo)
        {
            if ((fileInfo.Attributes & FileAttributes.Directory) == FileAttributes.Directory)
                return false;

            if (fileInfo.Name.EndsWith(".meta"))
                return false;

            if (fileInfo.Name.EndsWith(".spriteatlas"))
                return false;

            if (ignoreFileList.Contains(fileInfo.Name))
                return false;

            return true;
        }

        private static T GetProperty<T>(string path) where T : UnityEngine.Object
        {
            T property = AssetDatabase.LoadAssetAtPath<T>(string.Format("{0}/{1}.asset", path, typeof(T).Name));
            if (property == null)
            {
                int index = path.LastIndexOf("/");
                if (index == -1)
                    return default(T);
                else
                    return GetProperty<T>(path.Substring(0, index));
            }
            else
                return property;
        }

        private static void GenerateConfigFile()
        {
            string jsonFileName = GetAssetsOutput() + "/AssetBundleConfig.json";
            string jsonStr = JsonMapper.ToJson(assetBundlesNameMap);
            IOUtility.Write(jsonFileName, jsonStr);
        }

        private static void DeleteManifest(string path)
        {
            path = path.Replace("\\", "/");
            string[] fileList = Directory.GetFiles(path);
            for (int index = 0; index < fileList.Length; index++)
            {
                FileInfo fileInfo = new FileInfo(fileList[index]);
                if (fileInfo.Name.EndsWith(".manifest"))
                {
                    File.Delete(fileList[index]);
                }
            }
            string[] dirList = Directory.GetDirectories(path);
            for (int index = 0; index < dirList.Length; index++)
            {
                DeleteManifest(dirList[index]);
            }
        }

        private static void GenerateVersionFile()
        {
            string jsonFileName = $"{GetAssetsOutput()}/Version.json";
            string jsonStr = JsonMapper.ToJson(Builder.BuildSetting.version);
            IOUtility.Write(jsonFileName, jsonStr);
        }


        private static void CopyDirectoryToStreamingAssets(string path)
        {
            path = path.Replace("\\", "/");
            string[] fileList = Directory.GetFiles(path);
            for (int index = 0; index < fileList.Length; index++)
            {
                FileInfo fileInfo = new FileInfo(fileList[index]);
                string fileDirName = fileInfo.DirectoryName.Replace("\\", "/");
                string targetDir = $"{Application.streamingAssetsPath}/{fileInfo.DirectoryName.Substring(fileDirName.LastIndexOf("ManagedAssets"))}";
                if (!Directory.Exists(targetDir))
                {
                    Directory.CreateDirectory(targetDir);
                }
                
                File.Copy(fileInfo.FullName, $"{targetDir}/{fileInfo.Name}");
            }
            string[] dirList = Directory.GetDirectories(path);
            for (int index = 0; index < dirList.Length; index++)
            {
                CopyDirectoryToStreamingAssets(dirList[index]);
            }
        }

        private static void GenerateMD5() { 
            Dictionary<string, string> md5Map = new Dictionary<string, string>();

            string output = $"{GetAssetsOutput()}/";

            System.Action<string> filesCollecter = null;
            filesCollecter = delegate (string path)
            {
                string[] fileStrs = Directory.GetFiles(path);

                for (int i=0; i <fileStrs.Length; i++)
                {
                    FileStream fileStream = File.OpenRead(fileStrs[i]);
                    MD5 md5 = MD5.Create();
                    byte[] md5Bytes = md5.ComputeHash(fileStream);
                    fileStream.Close();
                    string md5Str = System.BitConverter.ToString(md5Bytes).Replace("-", "");
                    md5Map.Add(fileStrs[i].Replace("\\", "/").Replace(output, ""), md5Str);
                    md5.Clear();
                }

                string[] dirs = Directory.GetDirectories(path);
                for (int i= 0; i < dirs.Length; i++)
                {
                    filesCollecter(dirs[i]);
                }
            };

            filesCollecter(output);

            string jsonFileName = GetVersionTempPath() + "/MD5.json";
            string jsonStr = JsonMapper.ToJson(md5Map);
            IOUtility.Write(jsonFileName, jsonStr);
        }

        private static void GeneratePatchs()
        {
            VersionData currentVersion = Builder.BuildSetting.version;
            string patchsPath = GetPatchTempPath();
            if (Directory.Exists(patchsPath))
            {
                Directory.Delete(patchsPath, true);
            }

            Directory.CreateDirectory(patchsPath);
            for (uint index = 0; index < currentVersion.Resources; index++)
            {
                VersionData oldVersion = new VersionData(currentVersion.Large, currentVersion.Middle, currentVersion.Small, index);

                string oldPatchsPath = $"{GetBuildModeTempPath()}/{oldVersion}/Patchs";
                if (Directory.Exists(oldPatchsPath))
                {
                    Directory.Delete(oldPatchsPath, true);
                }

                string oldAssetsPath = $"{GetBuildModeTempPath()}/{oldVersion}/ManagedAssets";
                if (Directory.Exists(oldAssetsPath))
                {
                    Directory.Delete(oldAssetsPath, true);
                }
                GeneratePatch(oldVersion);
            }
        }

        private static void GeneratePatch(VersionData oldVersion)
        {
            
            string buildModePath = GetBuildModeTempPath();
            string oldVersionPath = $"{buildModePath}/{oldVersion}";
            if (Directory.Exists(oldVersionPath))
            {
                string patchsPath = GetPatchTempPath();
                string tempAssets = $"{patchsPath}/R{oldVersion.Resources}/ManagedAssets";
                Directory.CreateDirectory(tempAssets);
                Dictionary<string, string> currentMD5Map = GetAssetsMD5(Builder.BuildSetting.version);
                Dictionary<string, string> oldMD5Map = GetAssetsMD5(oldVersion);

                List<string> currentMD5Keys = currentMD5Map.Keys.ToList();
                for (int index = 0; index < currentMD5Keys.Count; index++)
                {
                    if (oldMD5Map.ContainsKey(currentMD5Keys[index]))
                    {
                        string currentMD5 = currentMD5Map[currentMD5Keys[index]];
                        string oldMD5 = oldMD5Map[currentMD5Keys[index]];

                        if (currentMD5 != oldMD5)
                        {
                            string targetPath = $"{patchsPath}/R{oldVersion.Resources}/ManagedAssets/{currentMD5Keys[index]}";
                            string parentPath = targetPath.Substring(0, targetPath.LastIndexOf("/"));
                            if (!Directory.Exists(parentPath))
                            {   
                                Directory.CreateDirectory(parentPath);
                            }
                            File.Copy($"{GetAssetsOutput()}/{currentMD5Keys[index]}", targetPath, true);
                        }
                    }
                }

                ZipUtility.Compress(tempAssets, $"{patchsPath}/R{oldVersion.Resources}/Patchs.zip");
                Directory.Delete(tempAssets, true);
            }
        }

        private static Dictionary<string, string> GetAssetsMD5(VersionData version)
        {
            string md5FilePath = $"{GetBuildModeTempPath()}/{version}/MD5.json";

            string md5Str = IOUtility.Read(md5FilePath);
            return md5Str != null ? JsonMapper.ToObject<Dictionary<string, string>>(md5Str) : null;
        }
    }
}

