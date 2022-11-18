using Fusion.Frameworks.DynamicDLL;
using Fusion.Frameworks.Scenes;
using Fusion.Frameworks.UI;
using Prefabs.UI;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class Test : MonoBehaviour
{
    //ScenesManager.LoadAsyncOperation loadAsyncOperation = null;

    // Start is called before the first frame update
    void Start()
    {
        //Image image = GetComponent<Image>();
        //image.gameObject.SetActive(false);

        //AssetsUtility.SetSprite(gameObject, "Sprites/1");
        //AssetsUtility.SetSprite(gameObject, "Sprites/2");
        //AssetsUtility.SetSprite(gameObject, "Sprites/3");
        //AssetsUtility.SetSprite(gameObject, "Sprites/4");
        //AssetsUtility.SetSprite(gameObject, "Sprites/5");

        //AssetsUtility.SetSpriteAsync(gameObject, "Sprites/1");
        //AssetsUtility.SetSpriteAsync(gameObject, "Sprites/2");
        //AssetsUtility.SetSpriteAsync(gameObject, "Sprites/3");
        //AssetsUtility.SetSpriteAsync(gameObject, "Sprites/4");
        //AssetsUtility.SetSpriteAsync(gameObject, "Sprites/1");
        //AssetsUtility.SetSpriteAsync(gameObject, "Sprites/5");
        //AssetsUtility.SetSpriteAsync(gameObject, "Sprites/6");


        //AssetsManager.Instance.LoadAsync<GameObject>("Prefabs/Shape/Cube", delegate (GameObject gameObject)
        //{
        //    GameObject cubeInstance = Instantiate(gameObject);
        //    cubeInstance.transform.position = Vector3.zero;
        //});
        //UIManager.Instance.Launch("Prefabs/UI/Page3", new UIData { LaunchMode = UILaunchMode.SingleTop });
        ScenesManager.LoadAsyncTask loadAsyncTask = new ScenesManager.LoadAsyncTask("Scenes/Scene1");
        loadAsyncTask.finishCallback = delegate
        {
            UIManager.Instance.Launch("Prefabs/UI/Page1", new UIData { LaunchMode = UILaunchMode.Standard });
            //UIData uiData = DLLManager.Instance.Instantiate<UIData>("Prefabs.UI.Page2Data", new object[] { UILaunchMode.SingleTop, "Sprites/Equips/1" });
            //UIManager.Instance.Launch("Prefabs/UI/Page2", uiData);
        };
        ScenesManager.LoadAsyncOperation loadAsyncOperation = ScenesManager.Instance.Schedule(loadAsyncTask);
        //ScenesManager.LoadAsyncTask loadAsyncTask = new ScenesManager.LoadAsyncTask();
        //loadAsyncTask.AddAdditive("Scenes/Scene2");
        //loadAsyncTask.AddAdditive("Scenes/Scene2");
        //loadAsyncTask.AddAdditive("Scenes/Scene2");
        //loadAsyncOperation = ScenesManager.Instance.Schedule(loadAsyncTask);
    }

    // Update is called once per frame
    void Update()
    {
        //UnityEngine.Debug.LogError(loadAsyncOperation.Progress);
    }

    private void OnDisable()
    {
    }
}
