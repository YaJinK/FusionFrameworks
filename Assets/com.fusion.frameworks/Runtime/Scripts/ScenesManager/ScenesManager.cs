using Fusion.Frameworks.Assets;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Fusion.Frameworks.Scenes
{
    public class ScenesManager : MonoBehaviour
    {
        public class LoadAsyncTask {
            private SceneData singleData = null;
            private int totalWeight = 0;
            private LoadAsyncOperation loadAsyncOperation = null;

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
                        singleData.asyncOperation.allowSceneActivation = false;
                    }, singleFinishCallback);
                } else
                {
                    singleFinishCallback();
                }
                
            }
        }

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
                            currentWeight = singleProgress * 10 / 9 * singleSceneData.weight;
                            if (currentWeight >= singleSceneData.weight)
                            {
                                currentWeight = singleSceneData.weight;
                                singleSceneData.asyncOperation.allowSceneActivation = true;
                            }
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
                            } else
                            {
                                currentIsDone = false;
                            }
                        }
                    }

                    isDone = currentIsDone;
                    progress = currentWeight / loadAsyncTask.TotalWeight;
                }
            }
        }

        private static ScenesManager instance = null;

        public static ScenesManager Instance
        {
            get
            {
                if (instance == null)
                {
                    GameObject scenesManagerObject = new GameObject("ScenesManager");
                    instance = scenesManagerObject.AddComponent<ScenesManager>();
                    DontDestroyOnLoad(scenesManagerObject);
                }
                return instance;
            }
        }

        private Dictionary<string, bool> asyncHandler = new Dictionary<string, bool>();

        private Queue<LoadAsyncTask> loadAsyncTasks = new Queue<LoadAsyncTask>();

        public void Load(string path, LoadSceneMode mode = LoadSceneMode.Single)
        {
            string name = AssetsManager.Instance.GetAssetNameByPath(path);
            AssetBundle assetBundle = AssetsManager.Instance.LoadAssetBundle(path);
            SceneManager.LoadScene(name, mode);
        }

        public void LoadAsync(string path, LoadSceneMode mode = LoadSceneMode.Single, Action<AsyncOperation> startCallback = null, Action finishCallback = null)
        {
            if (asyncHandler.ContainsKey(path))
            {
                Debug.LogError(asyncHandler[path]);
                return;
            }
            if (mode == LoadSceneMode.Single)
            {
                asyncHandler[path] = true;
                Action originFinishCallback = finishCallback;
                finishCallback = delegate
                {
                    asyncHandler.Remove(path);
                    if (originFinishCallback != null)
                    {
                        originFinishCallback();
                    }
                };
            }
            StartCoroutine(LoadAsyncCoroutine(path, mode, startCallback, finishCallback));
        }

        private IEnumerator LoadAsyncCoroutine(string path, LoadSceneMode mode = LoadSceneMode.Single, Action<AsyncOperation> startCallback = null, Action finishCallback = null)
        {
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

        public LoadAsyncOperation Schedule(string singleName)
        {
            LoadAsyncTask loadAsyncTask = new LoadAsyncTask(singleName);
            return Schedule(loadAsyncTask);
        }

        public LoadAsyncOperation Schedule(LoadAsyncTask loadAsyncTask)
        {
            if (loadAsyncTask.CheckValid())
            {
                loadAsyncTasks.Enqueue(loadAsyncTask);
                return loadAsyncTask.LoadAsyncOperation;
            } else
            {
                return null;
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
                }
            }
        }
    }
}
