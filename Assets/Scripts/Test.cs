using Fusion.Frameworks.Assets;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Image image = GetComponent<Image>();
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
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnDisable()
    {
    }
}
