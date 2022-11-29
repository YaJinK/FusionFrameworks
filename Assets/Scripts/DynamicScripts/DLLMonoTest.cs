using Fusion.Frameworks.DynamicDLL.Mono;
using System.Collections;
using UnityEngine;

public class DLLMonoTest : DLLMonoBase
{
    public DLLMonoTest()
    {
    }

    public DLLMonoTest(MonoBehaviour monoScript) : base(monoScript)
    {
    }

    public override void Awake()
    {
        base.Awake();
        Debug.Log("Awake");
    }

    //public override void FixedUpdate()
    //{
    //    base.FixedUpdate();
    //    Debug.Log("FixedUpdate");
    //}

    //public override void LateUpdate()
    //{
    //    base.LateUpdate();
    //    Debug.Log("LateUpdate");
    //}

    //public override void OnDestroy()
    //{
    //    base.OnDestroy();
    //    Debug.Log("OnDestroy");
    //}

    //public override void OnDisable()
    //{
    //    base.OnDisable();
    //    Debug.Log("OnDisable");
    //}

    //public override void OnEnable()
    //{
    //    base.OnEnable();
    //    Debug.Log("OnEnable");
    //}

    //public override void OnMouseDown()
    //{
    //    base.OnMouseDown();
    //    Debug.Log("OnMouseDown");
    //}

    //public override void OnMouseDrag()
    //{
    //    base.OnMouseDrag();
    //    Debug.Log("OnMouseDrag");
    //}

    //public override void OnMouseEnter()
    //{
    //    base.OnMouseEnter();
    //    Debug.Log("OnMouseEnter");
    //}

    //public override void OnMouseExit()
    //{
    //    base.OnMouseExit();
    //    Debug.Log("OnMouseExit");

    //}

    //public override void OnMouseOver()
    //{
    //    base.OnMouseOver();
    //    Debug.Log("OnMouseOver");
    //}

    //public override void OnMouseUp()
    //{
    //    base.OnMouseUp();
    //    Debug.Log("OnMouseUp");
    //}

    //public override void OnMouseUpAsButton()
    //{
    //    base.OnMouseUpAsButton();
    //    Debug.Log("OnMouseUpAsButton");
    //}

    public override void Start()
    {
        base.Start();
        Debug.Log("Start");
        mono.StartCoroutine(Cor());
    }

    public IEnumerator Cor()
    {
        Debug.Log("Cor begin");
        yield return new WaitForSeconds(3);
        Debug.Log("Cor End");
    }

    //public override void Update()
    //{
    //    base.Update();
    //    Debug.Log("Update");
    //}
}
