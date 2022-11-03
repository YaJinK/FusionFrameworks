using Fusion.Frameworks.Assets;
using Fusion.Frameworks.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Prefabs.UI
{
    public class Page2Data : UIData
    {
        public string spriteName;
    }

    public class Page2 : UIObject
    {
        public Page2(UIData data) : base(data)
        {
            //type = UIType.Pop;
            sortingOrder = 10000;
        }

        public override void Init()
        {
            base.Init();
            GameObject backBtn = UIUtility.Find(gameObject, "BackBtn");
            UIUtility.RegisterButtonAction(backBtn, delegate ()
            {
                Finish();
            });

            GameObject goBtn = UIUtility.Find(gameObject, "GoBtn");
            GameObject goBtnText = UIUtility.Find(goBtn, "BackBtnText_1");
            UIUtility.SetText(goBtnText, "GOOOOOOO!");
            UIUtility.RegisterButtonAction(goBtn, delegate ()
            {
                UIData data = new UIData();
                UIManager.Instance.Launch("Prefabs/UI/Page1", data);
            });

            GameObject image = UIUtility.Find(gameObject, "Image");
            Page2Data page2Data = (Page2Data)data;
            AssetsUtility.SetSprite(image, page2Data.spriteName);
        }
    }
}
