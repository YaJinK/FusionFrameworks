using Fusion.Frameworks.UI;
using UnityEngine;
using UnityEngine.UI;

public class Page1 : UIObject
{
    public Page1(UIData data) : base(data)
    {

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
        UIUtility.RegisterButtonAction(goBtn, delegate ()
        {
            Page2Data page2Data = new Page2Data();
            page2Data.spriteName = "Sprites/9";
            UIManager.Instance.LaunchAsync("Prefabs/UI/Page2", page2Data);
        });

        Page2Data page2Data = new Page2Data();
        page2Data.spriteName = "Sprites/7";
        UIObject page2 = LaunchChildAsync("Prefabs/UI/Page2", page2Data);
    }
}
