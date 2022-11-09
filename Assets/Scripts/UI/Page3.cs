using Fusion.Frameworks.Assets;
using Fusion.Frameworks.Scenes;
using Fusion.Frameworks.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Prefabs.UI
{
    public class Page3 : UIObject
    {
        public Page3(UIData data) : base(data)
        {
            //type = UIType.Pop;
            //sortingOrder = 10000;
        }

        public override void Init()
        {
            base.Init();
            GameObject image1 = UIUtility.Find(gameObject, "Image");
            GameObject setBtn1 = UIUtility.Find(image1.gameObject, "Button");
            UIUtility.RegisterButtonAction(setBtn1, delegate ()
            {
                //AssetsUtility.SetSpriteAsync(image1, "Sprites/big1");
                //AssetsUtility.CreateGameObjectAsync("Prefabs/Role/Spider/Spider", null, false, delegate(GameObject gameObject)
                //{
                //    gameObject.transform.position = new Vector3(-5, 0, 0);
                //});

                ScenesManager.LoadAsyncTask loadAsyncTask = new ScenesManager.LoadAsyncTask("Scenes/Scene1");
                loadAsyncTask.AddSceneDataHandler(new SceneUIHandler());
                loadAsyncTask.finishCallback = delegate
                {
                    UIManager.Instance.Launch("Prefabs/UI/Page3", new UIData { LaunchMode = UILaunchMode.Standard });
                };
                loadAsyncTask.TransformData = new TransformData { info = "aaa" };
                ScenesManager.LoadAsyncOperation loadAsyncOperation = ScenesManager.Instance.Schedule(loadAsyncTask);
            });

            GameObject image2 = UIUtility.Find(gameObject, "Image_1");
            GameObject setBtn2 = UIUtility.Find(image2.gameObject, "Button");
            UIUtility.RegisterButtonAction(setBtn2, delegate ()
            {
                //AssetsUtility.SetSpriteAsync(image2, "Sprites/big1");
                //ScenesManager.LoadAsyncTask loadAsyncTask = new ScenesManager.LoadAsyncTask("Scenes/Scene2");
                ////loadAsyncTask.AddAdditive("Scenes/Scene2");
                ////loadAsyncTask.AddAdditive("Scenes/Scene2");
                ////loadAsyncTask.AddAdditive("Scenes/Scene2");
                ////loadAsyncTask.AddSceneDataHandler(new SceneUIHandler());
                //loadAsyncTask.finishCallback = delegate
                //{
                //    UIManager.Instance.Launch("Prefabs/UI/Page3", new UIData { LaunchMode = UILaunchMode.Standard });
                //};
                //ScenesManager.LoadAsyncOperation loadAsyncOperation = ScenesManager.Instance.Schedule(loadAsyncTask);
                AssetsUtility.CreateGameObjectAsync("Prefabs/Role/Spider/Spider", null, false, delegate (GameObject gameObject)
                {
                    gameObject.transform.position = new Vector3(-5, 0, 0);
                });
            });

            GameObject backBtn = UIUtility.Find(gameObject, "Button");
            UIUtility.RegisterButtonAction(backBtn, delegate ()
            {
                Finish();
            });
        }
    }
}
