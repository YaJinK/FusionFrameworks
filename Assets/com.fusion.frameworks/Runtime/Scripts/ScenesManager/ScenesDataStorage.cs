using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Fusion.Frameworks.Scenes
{
    [DisallowMultipleComponent]
    public class ScenesDataStorage : MonoBehaviour
    {
        private Dictionary<string, List<SceneDataHandler>> scenesDataHandlers = new Dictionary<string, List<SceneDataHandler>>();
        public void Save(List<SceneDataHandler> sceneDataHandlers)
        {
            string sceneName = ScenesManager.Instance.GetCurrentSceneName();
            Save(sceneName, sceneDataHandlers);
        }

        public void Save(string sceneName, List<SceneDataHandler> sceneDataHandlers)
        {
            scenesDataHandlers[sceneName] = sceneDataHandlers;
            for (int i = 0; i < sceneDataHandlers.Count; i++)
            {
                sceneDataHandlers[i].Save();
            }
        }

        public void Load()
        {
            string sceneName = ScenesManager.Instance.GetCurrentSceneName();
            Load(sceneName);
        }

        public void Load(string sceneName)
        {
            if (scenesDataHandlers.ContainsKey(sceneName))
            {
                List<SceneDataHandler> sceneDataHandlers = scenesDataHandlers[sceneName];
                for (int i = 0; i < sceneDataHandlers.Count; i++)
                {
                    sceneDataHandlers[i].Load();
                }
                scenesDataHandlers.Remove(sceneName);
            }
        }
    }
}
