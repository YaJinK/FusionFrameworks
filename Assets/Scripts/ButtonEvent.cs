using Fusion.Frameworks.Assets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonEvent : MonoBehaviour
{
    public GameObject obj;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Click()
    {
        AssetsUtility.Release(obj);
    }

    public void Create()
    {
        AssetsUtility.CreateGameObjectAsync("Prefabs/Shape/Cube");
    }

    public void M1()
    {
        AssetsUtility.SetMaterialOfMeshRendererAsync(obj, "Materials/M2");
    }

    public void M2()
    {
        AssetsUtility.SetMaterialOfMeshRendererAsync(obj, "Materials/M3");
    }
}
