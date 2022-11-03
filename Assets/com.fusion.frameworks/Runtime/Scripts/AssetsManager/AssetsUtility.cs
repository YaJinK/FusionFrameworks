using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace Fusion.Frameworks.Assets
{
    public class AssetsUtility
    {

        /// <summary>
        /// 设置精灵图片
        /// </summary>
        /// <param name="gameObject"></param>
        /// <param name="path"></param>
        public static void SetSprite(GameObject gameObject, string path)
        {
            Image image = gameObject.GetComponent<Image>();
            Sprite sprite = AssetsManager.Instance.Load<Sprite>(path);
            image.sprite = sprite;

            GameObjectAssetRecorder assetRecorder = gameObject.GetOrAddComponent<GameObjectAssetRecorder>();
            assetRecorder.Record(AssetsConfig.GetAssetBundleName(path));
        }

        /// <summary>
        /// 异步设置精灵图片
        /// </summary>
        /// <param name="gameObject"></param>
        /// <param name="path"></param>
        public static void SetSpriteAsync(GameObject gameObject, string path)
        {
            Image image = gameObject.GetComponent<Image>();
            image.enabled = false;
            string assetAsyncKey = image.GetInstanceID().ToString() + "_sprite";

            Action<UnityEngine.Object> callback = delegate (UnityEngine.Object asset)
            {
                image.enabled = true;
                image.sprite = asset as Sprite;
            };

            HandleAsync<Sprite>(gameObject, path, assetAsyncKey, callback);
        }

        /// <summary>
        /// 设置MeshRenderer的材质
        /// </summary>
        /// <param name="gameObject"></param>
        /// <param name="path"></param>
        public static void SetMaterialOfMeshRenderer(GameObject gameObject, string path)
        {
            MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();
            Material material = AssetsManager.Instance.Load<Material>(path);
            meshRenderer.material = material;
            GameObjectAssetRecorder assetRecorder = gameObject.GetOrAddComponent<GameObjectAssetRecorder>();
            assetRecorder.Record(AssetsConfig.GetAssetBundleName(path));
        }

        /// <summary>
        /// 异步设置MeshRenderer的材质
        /// </summary>
        /// <param name="gameObject"></param>
        /// <param name="path"></param>
        public static void SetMaterialOfMeshRendererAsync(GameObject gameObject, string path)
        {
            MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();
            string assetAsyncKey = meshRenderer.GetInstanceID().ToString() + "_material";
            Action<UnityEngine.Object> callback = delegate (UnityEngine.Object asset)
            {
                meshRenderer.material = asset as Material;
            };

            HandleAsync<Material>(gameObject, path, assetAsyncKey, callback);
        }

        private static void HandleAsync<T>(GameObject gameObject, string path, string assetAsyncKey, Action<UnityEngine.Object> callback) where T : UnityEngine.Object
        {
            AssetAsyncHandler assetAsyncHandler = gameObject.GetOrAddComponent<AssetAsyncHandler>();

            assetAsyncHandler.RegisterCallback(assetAsyncKey, callback);

            AssetsManager.Instance.LoadAsync(path, delegate (T asset)
            {
                Action<T> currentCallback = assetAsyncHandler.GetCallback(assetAsyncKey);
                if (callback == currentCallback)
                {
                    currentCallback(asset);
                    assetAsyncHandler.RemoveCallback(assetAsyncKey);
                }
            });

            GameObjectAssetRecorder assetRecorder = gameObject.GetOrAddComponent<GameObjectAssetRecorder>();
            assetRecorder.Record(AssetsConfig.GetAssetBundleName(path));
        }

        /// <summary>
        /// 通过预设创建新的GameObject
        /// </summary>
        /// <param name="path"></param>
        /// <param name="parent"></param>
        /// <param name="worldPositionStays"></param>
        /// <returns></returns>
        public static GameObject CreateGameObject(string path, GameObject parent = null, bool worldPositionStays = false)
        {
            GameObject originGameObject = AssetsManager.Instance.Load<GameObject>(path);
            GameObject gameObject = UnityEngine.Object.Instantiate(originGameObject);
            if (parent != null)
            {
                gameObject.transform.SetParent(parent.transform, worldPositionStays);
            }

            GameObjectAssetRecorder assetRecorder = gameObject.GetOrAddComponent<GameObjectAssetRecorder>();
            assetRecorder.Record(AssetsConfig.GetAssetBundleName(path));
            return gameObject;
        }

        /// <summary>
        /// 异步通过预设创建新的GameObject
        /// </summary>
        /// <param name="path"></param>
        /// <param name="parent"></param>
        /// <param name="worldPositionStays"></param>
        /// <param name="finishCallback"></param>
        public static void CreateGameObjectAsync(string path, GameObject parent = null, bool worldPositionStays = false, Action<GameObject> finishCallback = null)
        {
            AssetsManager.Instance.LoadAsync<GameObject>(path, delegate(GameObject originGameObject)
            {
                GameObject gameObject = UnityEngine.Object.Instantiate(originGameObject);
                if (parent != null)
                {
                    gameObject.transform.SetParent(parent.transform, worldPositionStays);
                }

                GameObjectAssetRecorder assetRecorder = gameObject.GetOrAddComponent<GameObjectAssetRecorder>();
                assetRecorder.Record(AssetsConfig.GetAssetBundleName(path));
                if (finishCallback != null)
                {
                    finishCallback(gameObject);
                }
            });
        }

        /// <summary>
        /// 销毁GameObject
        /// </summary>
        /// <param name="gameObject"></param>
        public static void Release(GameObject gameObject)
        {
            GameObjectAssetRecorder[] assetRecorders = gameObject.GetComponentsInChildren<GameObjectAssetRecorder>(true);
            for (int index = 0; index < assetRecorders.Length; index++)
            {
                assetRecorders[index].Release();
            }
            UnityEngine.Object.Destroy(gameObject);
        }
    }
}


