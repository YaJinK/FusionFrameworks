using Fusion.Frameworks.Utilities;
using System;
using UnityEngine;

namespace Fusion.Frameworks.Version
{
    [Serializable]
    public class VersionData {
        [SerializeField]
        private uint large = 0;
        [SerializeField]
        private uint middle = 0;
        [SerializeField]
        private uint small = 0;
        public uint Large { get => large; set => large = value; }
        public uint Middle { get => middle; set => middle = value; }
        public uint Small { get => small; set => small = value; }

        public override string ToString()
        {
            return $"{large}.{middle}.{small}";
        }
    }


    public class VersionManager : Singleton<VersionManager>
    {
       
    }
}


