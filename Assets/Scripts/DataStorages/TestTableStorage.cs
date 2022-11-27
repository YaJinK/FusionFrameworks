using System.Collections.Generic;
using Fusion.Frameworks.Assets;
using LitJson;
using UnityEngine;

namespace Fusion.Frameworks.DataStorages
{
    public class TestTable
    {
        public int id = 0;

        public string name = null;

        public long longNum = 0;

        public double doubleNum = 0;

        public List<int> intList = null;

        public int speed = 0;

    public class Obj1
    {
        public int id = 0;

        public string name = null;

        public List<string> strList = null;

    }
    public Obj1 obj1 = null;
    public class Obj2
    {
        public int id = 0;

        public string name = null;

        public List<string> strList = null;

    }
    public Obj2 obj2 = null;
    }
    public class TestTableStorage {
        private static Dictionary<string, TestTable> storage = null;
        private static string dataPath = "DataStorages/TestTable";

        public static TestTable Get(int id)
        {
            if (storage == null)
            {
                Init();
            }
            return storage[id.ToString()];
        }

        private static void Init() 
        {
            TextAsset textAsset = AssetsManager.Instance.Load<TextAsset>(dataPath);
            storage = JsonMapper.ToObject<Dictionary<string, TestTable>>(textAsset.text);
            AssetsUtility.Release(dataPath);
        }

        public static void Clear()
        {
            storage.Clear();
            storage = null;
        }
    }

}