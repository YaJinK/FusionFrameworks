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
        [SerializeField]
        private uint resources = 0;
        public uint Large { get => large; set => large = value; }
        public uint Middle { get => middle; set => middle = value; }
        public uint Small { get => small; set => small = value; }
        public uint Resources { get => resources; set => resources = value; }

        public VersionData() { }
        public VersionData(uint large, uint middle, uint small, uint resources)
        {
            Large = large;
            Middle = middle;
            Small = small;
            Resources = resources;
        }

        public override string ToString()
        {
            return $"{large}.{middle}.{small}_R{resources}";
        }
    }


    public class VersionManager : Singleton<VersionManager>
    {
       
    }
}


