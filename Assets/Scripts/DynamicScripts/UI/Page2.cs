using Fusion.Frameworks.Assets;
using Fusion.Frameworks.DataStorages;
using Fusion.Frameworks.UI;
using UnityEngine;

namespace Prefabs.UI
{
    public class Page2Data : UIData
    {
        public string spriteName;

        public Page2Data()
        {
        }

        public Page2Data(UILaunchMode launchMode, string spriteName) : base(launchMode)
        {
            this.spriteName = spriteName;
        }
    }

    public class Page2 : UIObject
    {
        public Page2(UIData data) : base(data)
        {
            //type = UIType.Pop;
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
                UIManager.Instance.Launch("Prefabs/UI/Page3", data);
            });
            
            GameObject image = UIUtility.Find(gameObject, "Image");
            TestTable table = TestTableStorage.Get(2);
            AssetsUtility.SetSprite(image, table.name);
        }
    }
}