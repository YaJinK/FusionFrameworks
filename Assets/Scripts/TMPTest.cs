using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TMPTest : MonoBehaviour
{
    public TextMeshProUGUI textMeshPro;
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < 100; i++)
        {
            TextMeshProUGUI go = Instantiate<TextMeshProUGUI>(textMeshPro);
            go.transform.SetParent(gameObject.transform);
            go.outlineColor = Color.white;
            go.outlineWidth = 0.2f;
            go.gameObject.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
