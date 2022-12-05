using Fusion.Frameworks.Assets;
using Fusion.Frameworks.Timer;
using Fusion.Frameworks.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Fusion.Frameworks.Scenes
{
    public class TransformData
    {
        public string fromScene = null;
        public string info = null;
    }

    /// <summary>
    /// 场景加载类
    /// </summary>
    [DisallowMultipleComponent]
    public class ScenesManager : MonoBehaviour
    {
        /// <summary>
        ///异步加载任务
        ///一个任务下包含需包含 SingleScene <= 0 AdditiveScenes >= 0
        ///同一任务下的所有Scenes共享加载进度和加载完成标志
        /// </summary>
        public class LoadAsyncTask {
            private SceneData singleData = null;
            private int totalWeight = 0;
            private LoadAsyncOperation loadAsyncOperation = null;

            public Action finishCallback = null;

            private List<SceneDataHandler> sceneDataHandlers = null;
            private TransformData transformData = null;
            public class SceneData {
                private string name;
                public SceneData(string name)
                {
                    this.name = name;
                }
                public AsyncOperation asyncOperation = null;
                public int weight = 0;

                public string Name { get => name; }
            }

            private List<SceneData> additiveList = null;

            private bool isScheduling = false;

            public LoadAsyncOperation LoadAsyncOperation 
            { 
                get => loadAsyncOperation; 
            }
            public bool IsScheduling { get => isScheduling; }
            public int TotalWeight { get => totalWeight; }
            public SceneData SingleData { get => singleData; }
            public List<SceneData> AdditiveList { get => additiveList; }
            public List<SceneDataHandler> SceneDataHandlers { get => sceneDataHandlers; }
            public TransformData TransformData { get => transformData; set => transformData = value; }

            public LoadAsyncTask() : this(null)
            {
            }

            public LoadAsyncTask(string singleName)
            {
                if (singleName != null)
                {
                    singleData = new SceneData(singleName);
                    singleData.weight = 5;
                    totalWeight = totalWeight + singleData.weight;
                }
                loadAsyncOperation = new LoadAsyncOperation(this);
            }

            public bool CheckValid()
            {
                return singleData != null || additiveList.Count > 0;
            }

            public void AddAdditive(string name)
            {
                if (additiveList == null)
                {
                    additiveList = new List<SceneData>(4);
                }
                SceneData additiveData = new SceneData(name);
                additiveData.weight = 10;
                additiveList.Add(additiveData);
                totalWeight += additiveData.weight;
            }

            public void AddSceneDataHandler(SceneDataHandler sceneDataHandler)
            {
                if (sceneDataHandlers == null)
                {
                    sceneDataHandlers = new List<SceneDataHandler>(2);
                }
                sceneDataHandlers.Add(sceneDataHandler);
            }

            private void LoadAdditive(int index)
            {
                if (additiveList == null || index >= additiveList.Count)
                {
                    return;
                }

                Instance.LoadAsync(additiveList[index].Name, LoadSceneMode.Additive, delegate (AsyncOperation asyncOperation)
                {
                    additiveList[index].asyncOperation = asyncOperation;
                    LoadAdditive(++index);
                });
            }

            public void Schedule()
            {
                if (isScheduling)
                {
                    return;
                }

                if (sceneDataHandlers != null)
                {
                    scenesDataStorage.Save(sceneDataHandlers);
                }

                if (transformData != null)
                {
                    
                    transformData.fromScene = Instance.GetCurrentSceneName();
                    Instance.transformDatas[singleData.Name] = transformData;
                }

                isScheduling = true;
                Action singleFinishCallback = delegate
                {
                    LoadAdditive(0);
                };
                if (singleData != null)
                {
                    Instance.LoadAsync(singleData.Name, LoadSceneMode.Single, delegate (AsyncOperation asyncOperation)
                    {
                        singleData.asyncOperation = asyncOperation;
                    }, singleFinishCallback);
                } else
                {
                    singleFinishCallback();
                }
                
            }
        }

        /// <summary>
        /// 异步加载任务结果
        /// 包含加载进度和完成标志
        /// </summary>
        public class LoadAsyncOperation
        {
            private float progress = 0;
            private bool isDone = false;

            private LoadAsyncTask loadAsyncTask = null;
            public float Progress { get => progress; }
            public bool IsDone { get => isDone; }

            public LoadAsyncOperation(LoadAsyncTask loadAsyncTask)
            {
                this.loadAsyncTask = loadAsyncTask;
            }

            public void Update()
            {
#if FUSION_ASSETBUNDLE || !UNITY_EDITOR
                if (loadAsyncTask.IsScheduling)
                {
                    bool currentIsDone = true;
                    float currentWeight = 0;
                    LoadAsyncTask.SceneData singleSceneData = loadAsyncTask.SingleData;
                    if (singleSceneData != null)
                    {
                        if (singleSceneData.asyncOperation != null)
                        {
                            float singleProgress = singleSceneData.asyncOperation.progress;
                            currentWeight = singleProgress * singleSceneData.weight;
                            currentIsDone = currentIsDone && singleSceneData.asyncOperation.isDone;
                        }
                        else
                        {
                            currentIsDone = false;
                        }
                    }
                    if (loadAsyncTask.AdditiveList != null)
                    {
                        for (int index = 0; index < loadAsyncTask.AdditiveList.Count; index++)
                        {
                            LoadAsyncTask.SceneData additiveSceneData = loadAsyncTask.AdditiveList[index];
                            if (additiveSceneData.asyncOperation != null)
                            {
                                float additiveProgress = additiveSceneData.asyncOperation.progress;
                                currentWeight += additiveProgress * additiveSceneData.weight;
                                currentIsDone = currentIsDone && additiveSceneData.asyncOperation.isDone;
                            }
                            else
                            {
                                currentIsDone = false;
                            }
                        }
                    }

                    if (currentIsDone)
                    {
                        float newProgress = currentWeight / loadAsyncTask.TotalWeight;
                        progress = Mathf.Lerp(progress, newProgress, Time.unscaledDeltaTime * 100);
                        if (newProgress - progress <= 0.001f)
                        {
                            isDone = true;
                            progress = newProgress;
                        }
                    }
                    else
                    {
                        float newProgress = currentWeight / loadAsyncTask.TotalWeight;
                        float oldProgress = progress;
                        progress = Mathf.SmoothStep(oldProgress, newProgress, Time.unscaledDeltaTime * 50);
                    }
                }
#else
                progress += 0.1f;
                if (progress >= 1)
                {
                    isDone = true;
                    progress = 1;
                }
#endif
            }
        }

        private static ScenesManager instance = null;
        private static ScenesDataStorage scenesDataStorage = null;

        public static ScenesManager Instance
        {
            get
            {
                if (instance == null)
                {
                    GameObject scenesManagerObject = new GameObject("ScenesManager");
                    instance = scenesManagerObject.AddComponent<ScenesManager>();
                    scenesDataStorage = scenesManagerObject.AddComponent<ScenesDataStorage>();
                    DontDestroyOnLoad(scenesManagerObject);
                }
                return instance;
            }
        }

        private Dictionary<string, bool> asyncHandler = new Dictionary<string, bool>();

        private Queue<LoadAsyncTask> loadAsyncTasks = new Queue<LoadAsyncTask>();
        private Dictionary<string, bool> loadAsyncTaskSingleKey = new Dictionary<string, bool>();

        private Dictionary<string, TransformData> transformDatas = new Dictionary<string, TransformData>(); 

        public TransformData GetTransformData(string name)
        {
            if (transformDatas.ContainsKey(name))
            {
                TransformData transformData = transformDatas[name];
                transformDatas.Remove(name);
                return transformData;
            } else
            {
                return null;
            }
        }

        public void Load(string path, LoadSceneMode mode = LoadSceneMode.Single)
        {
            TimerManager.Instance.Clear();
            string name = AssetsManager.Instance.GetAssetNameByPath(path);
#if FUSION_ASSETBUNDLE || !UNITY_EDITOR
            AssetsManager.Instance.LoadAssetBundle(path);
#endif
            SceneManager.LoadScene(name, mode);
            if (mode == LoadSceneMode.Single)
            {
                UIManager.Instance.Clear();
                AssetReferences.Instance.Clear(new string[] { AssetsConfig.GetAssetBundleName(path) });
            }
        }

        public void LoadAsync(string path, LoadSceneMode mode = LoadSceneMode.Single, Action<AsyncOperation> startCallback = null, Action finishCallback = null)
        {
            TimerManager.Instance.Clear();
#if FUSION_ASSETBUNDLE || !UNITY_EDITOR
            if (asyncHandler.ContainsKey(path))
            {
                return;
            }
            
            StartCoroutine(LoadAsyncCoroutine(path, mode, startCallback, finishCallback));
#else
            if (startCallback != null)
            {
                startCallback(null);
            }
            Load(path, mode);
            if (finishCallback != null)
            {
                finishCallback();
            }
#endif
        }

        private IEnumerator LoadAsyncCoroutine(string path, LoadSceneMode mode = LoadSceneMode.Single, Action<AsyncOperation> startCallback = null, Action finishCallback = null)
        {
            if (mode == LoadSceneMode.Single)
            {
                asyncHandler[path] = true;
                Action originFinishCallback = finishCallback;
                finishCallback = delegate
                {
                    asyncHandler.Remove(path);
                    UIManager.Instance.Clear();
                    AssetReferences.Instance.Clear(new string[] { AssetsConfig.GetAssetBundleName(path) });
                    if (originFinishCallback != null)
                    {
                        originFinishCallback();
                    }
                };
            }
            string name = AssetsManager.Instance.GetAssetNameByPath(path);
            yield return AssetsManager.Instance.LoadAssetBundleAsync(path);
            AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(name, mode);
            if (startCallback != null)
            {
                startCallback(asyncOperation);
            }
            yield return asyncOperation;
            if (finishCallback != null)
            {
                finishCallback();
            }
        }

        public string GetCurrentSceneName()
        {
            Scene currentScene = SceneManager.GetActiveScene();
            return GetSceneName(currentScene);
        }
        public string GetSceneName(Scene scene)
        {
            string fullName = scene.path;
            string tempString = fullName.Substring(fullName.IndexOf("GameAssets/") + 11);
            return tempString.Substring(0, tempString.LastIndexOf("."));
        }

        public LoadAsyncOperation Schedule(string singleName)
        {
            LoadAsyncTask loadAsyncTask = new LoadAsyncTask(singleName);
            return Schedule(loadAsyncTask);
        }

        public LoadAsyncOperation Schedule(LoadAsyncTask loadAsyncTask)
        {
            if (loadAsyncTask.CheckValid())
            {
                LoadAsyncTask.SceneData singleData = loadAsyncTask.SingleData;
                if (singleData == null || !loadAsyncTaskSingleKey.ContainsKey(singleData.Name))
                {
                    loadAsyncTaskSingleKey[singleData.Name] = true;
                    loadAsyncTasks.Enqueue(loadAsyncTask);
                    return loadAsyncTask.LoadAsyncOperation;
                } else
                {
                    return null;
                }
            } else
            {
                return null;
            }

        }

        public void Release(string sceneName)
        {
            StartCoroutine(ReleaseCoroutine(sceneName));
        }

        private IEnumerator ReleaseCoroutine(string sceneName)
        {
            int divideIndex = sceneName.LastIndexOf("/") + 1;
            string simpleName = sceneName.Substring(divideIndex);
            AsyncOperation operation = SceneManager.UnloadSceneAsync(simpleName);
            yield return operation;
            if (operation != null)
            {
                AssetsUtility.Release(sceneName);
            }
        }

        // Update is called once per frame
        private void Update()
        {
            LoadAsyncTask loadAsyncTask = null;
            bool peekResult = loadAsyncTasks.TryPeek(out loadAsyncTask);
            if (peekResult)
            {
                if (!loadAsyncTask.IsScheduling)
                {
                    loadAsyncTask.Schedule();
                }
                loadAsyncTask.LoadAsyncOperation.Update();
                if (loadAsyncTask.LoadAsyncOperation.IsDone)
                {
                    loadAsyncTasks.Dequeue();
                    LoadAsyncTask.SceneData singleData = loadAsyncTask.SingleData;
                    if (singleData != null)
                    {
                        loadAsyncTaskSingleKey.Remove(singleData.Name);
                    }
                    scenesDataStorage.Load();
                    if (loadAsyncTask.finishCallback != null)
                    {
                        loadAsyncTask.finishCallback();
                    }
                }
            }
        }
    }
}
