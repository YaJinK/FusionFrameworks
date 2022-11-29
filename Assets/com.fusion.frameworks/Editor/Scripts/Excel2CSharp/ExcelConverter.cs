using OfficeOpenXml;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using System.Text;
using Fusion.Frameworks.Utilities;
using System;
using System.Collections;
using System.Reflection;
using LitJson;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Fusion.Frameworks.Excel.Editor
{
    public class ExcelConverter
    {
        [MenuItem("Excels/Build")]
        public static void Build()
        {
            if (!Directory.Exists("Excels"))
            {
                return;
            }

            if (Directory.Exists("Assets/Scripts/DataStorages"))
            {
                string[] files = Directory.GetFiles("Assets/Scripts/DataStorages");
                foreach (string file in files)
                {
                    File.Delete(file);
                }
            } else
            {
                Directory.CreateDirectory("Assets/Scripts/DataStorages");
            }

            if (Directory.Exists("Assets/GameAssets/DataStorages"))
            {
                string[] files = Directory.GetFiles("Assets/GameAssets/DataStorages");
                foreach (string file in files)
                {
                    if (file.EndsWith(".json"))
                        File.Delete(file);
                }
            }
            else
            {
                Directory.CreateDirectory("Assets/GameAssets/DataStorages");
            }

            string[] excels = Directory.GetFiles("Excels");
            foreach (string file in excels)
            {
                FileInfo fileInfo= new FileInfo(file);
                if (fileInfo.Name.EndsWith(".xlsx") && !fileInfo.Name.StartsWith("~$"))
                {
                    UnityEngine.Debug.Log(file);
                    ConvertExcel(file);
                }
            }

            AssetDatabase.Refresh();
        }

        private static void ConvertExcel(string path)
        {
            FileInfo fileInfo = new FileInfo(path);

            ExcelPackage excelPackage = new ExcelPackage(fileInfo);

            ExcelWorksheet excelWorkbook = excelPackage.Workbook.Worksheets[1];
            
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(@"using System.Collections.Generic;
using Fusion.Frameworks.Assets;
using LitJson;
using UnityEngine;

namespace Fusion.Frameworks.DataStorages
{");
            string className = fileInfo.Name.Substring(0, fileInfo.Name.LastIndexOf("."));
            GenerateClass(ref stringBuilder, excelWorkbook, className);

            stringBuilder.Append(@$"
    public class {className}Storage {{
        private static Dictionary<string, {className}> storage = null;
        private static string dataPath = ""DataStorages/{className}"";

        public static {className} Get(int id)
        {{
            if (storage == null)
            {{
                Init();
            }}
            return storage[id.ToString()];
        }}

        private static void Init() 
        {{
            TextAsset textAsset = AssetsManager.Instance.Load<TextAsset>(dataPath);
            storage = JsonMapper.ToObject<Dictionary<string, {className}>>(textAsset.text);
            AssetsUtility.Release(dataPath);
        }}

        public static void Clear()
        {{
            storage.Clear();
            storage = null;
        }}
    }}
");
            stringBuilder.Append(@"
}");
            IOUtility.Write($"Assets/Scripts/DataStorages/{className}Storage.cs", stringBuilder.ToString());

            GenerateJsonData(excelWorkbook, className);
            excelPackage.Dispose();
        }

        private static int GenerateClass(ref StringBuilder stringBuilder, ExcelWorksheet sheet, string className, int index = 1)
        {
            stringBuilder.Append($@"
    public class {className}
    {{");
            while (true)
            {
                string fieldString;
                index = GetClassFieldString(out fieldString, sheet, index);
                string name = sheet.Cells[2, index].Value?.ToString();
                stringBuilder.Append(fieldString);
                if (name == null || name.Equals("}"))
                {
                    break;
                }
            }
            stringBuilder.Append(@"
    }");
            return index + 1;
        }

        private static int GetClassFieldString(out string result, ExcelWorksheet sheet, int index)
        {
            string name = sheet.Cells[2, index].Value?.ToString();
            if (name.Contains("["))
            {
                string type = sheet.Cells[1, index].Value.ToString();
                string realType = $"List<{type}>";
                string realName = name.Replace("[", "");
                result = @$"
        public {realType} {realName} = null;
";
                while (true) {
                    index++;
                    string nextName = sheet.Cells[2, index].Value?.ToString();
                    if ((nextName != null && nextName.Contains("]")) || index >= 10000000)
                    {
                        break;
                    }
                }
                return index + 1;
            } else if (name.Contains("{"))
            {
                string realName = name.Replace("{", "");
                string realClassName = realName.Replace(name[0], char.ToUpper(name[0]));
                StringBuilder stringBuilder = new StringBuilder();
                int nextIndex = GenerateClass(ref stringBuilder, sheet, realClassName, index + 1);
                stringBuilder.Append(@$"
    public {realClassName} {realName} = null;");
                result = stringBuilder.ToString();
                return nextIndex;
            } else if (name.Contains("}"))
            {
                result = "";
                return index;
            }
            else
            {
                string type = sheet.Cells[1, index].Value.ToString();
                string value = type.Equals("string") ? "null" : "0";
                result = @$"
        public {type} {name} = {value};
";
                return index + 1;
            }
        }

        private static void GenerateJsonData(ExcelWorksheet sheet, string className)
        {
            Type classType = Type.GetType($"Fusion.Frameworks.DataStorages.{className}, Assembly-CSharp");
            Type mapType = typeof(Dictionary<,>);
            Type mapGeneric = mapType.MakeGenericType(typeof(string), classType);

            IDictionary map = Activator.CreateInstance(mapGeneric) as IDictionary;
            int rowIndex = 2;
            while (true)
            {
                rowIndex++;
                if (sheet.Cells[rowIndex, 1].Value == null)
                {
                    break;
                }

                object data;
                CreateDataObject(out data, sheet, $"Fusion.Frameworks.DataStorages.{className}", rowIndex, 0);
                int keyValue = int.Parse(sheet.Cells[rowIndex, 1].Value.ToString());

                object obj1 = data.GetType().GetField("obj1").GetValue(data);
                map.Add(keyValue.ToString(), Convert.ChangeType(data, classType));
            }

            JsonWriter jsonWriter = new JsonWriter();
            string result = Regex.Unescape(JsonMapper.ToJson(map));
            IOUtility.Write($"Assets/GameAssets/DataStorages/{className}.json", result);
        }

        private static int CreateDataObject(out object output, ExcelWorksheet sheet, string classFullName, int rowIndex, int colIndex)
        {
            Type classType = Type.GetType($"{classFullName}, Assembly-CSharp");
            object data = Activator.CreateInstance(classType);
            while (true)
            {
                colIndex++;
                string name = sheet.Cells[2, colIndex].Value?.ToString();
                if (name == null || name.Equals("}"))
                {
                    break;
                }

                string type = sheet.Cells[1, colIndex].Value?.ToString();
                if (name.Contains("["))
                {
                    string realName = name.Replace("[", "");
                    Type listType = typeof(List<>);
                    Type listGenerateClass = GetTypeByString(type);
                    Type listGeneraic = listType.MakeGenericType(listGenerateClass);
                    IList list = Activator.CreateInstance(listGeneraic) as IList;

                    while (true)
                    {
                        list.Add(Convert.ChangeType(sheet.Cells[rowIndex, colIndex].Value, listGenerateClass));
                        string currentName = sheet.Cells[2, colIndex].Value?.ToString();
                        if (currentName != null && currentName.Contains("]"))
                        {
                            break;
                        }
                        colIndex++;
                    }
                    var field = data.GetType().GetField(realName);
                    field.SetValue(data, Convert.ChangeType(list, field.FieldType));
                }
                else if (name.Contains("{"))
                {
                    string innerName = name.Replace("{", "");
                    object innerData;
                    colIndex = CreateDataObject(out innerData, sheet, $"{classFullName}+{innerName.Replace(innerName[0], char.ToUpper(innerName[0]))}", rowIndex, colIndex);
                    var field = data.GetType().GetField(innerName);
                    field.SetValue(data, Convert.ChangeType(innerData, field.FieldType)); 
                }
                else
                {
                    var field = data.GetType().GetField(name);
                    field.SetValue(data, Convert.ChangeType(sheet.Cells[rowIndex, colIndex].Value, field.FieldType));
                }
            }

            output = data;

            return colIndex;
        }

        public static Type GetTypeByString(string type)
        {
            switch (type.ToLower())
            {
                case "bool":
                    return Type.GetType("System.Boolean", true, true);
                case "byte":
                    return Type.GetType("System.Byte", true, true);
                case "sbyte":
                    return Type.GetType("System.SByte", true, true);
                case "decimal":
                    return Type.GetType("System.Decimal", true, true);
                case "double":
                    return Type.GetType("System.Double", true, true);
                case "float":
                    return Type.GetType("System.Single", true, true);
                case "int":
                    return Type.GetType("System.Int32", true, true);
                case "uint":
                    return Type.GetType("System.UInt32", true, true);
                case "long":
                    return Type.GetType("System.Int64", true, true);
                case "ulong":
                    return Type.GetType("System.UInt64", true, true);
                case "short":
                    return Type.GetType("System.Int16", true, true);
                case "ushort":
                    return Type.GetType("System.UInt16", true, true);
                case "string":
                    return Type.GetType("System.String", true, true);
                default:
                    return Type.GetType(type, true, true);
            }
        }
    }
}
