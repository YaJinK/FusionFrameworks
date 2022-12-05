using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Fusion.Frameworks.Scenes
{
    /// <summary>
    /// 用来保存和加载Scene数据，具体逻辑由子类实现
    /// </summary>
    public interface SceneDataHandler
    {
        public abstract void Save();
        public abstract void Load();
    }
}

