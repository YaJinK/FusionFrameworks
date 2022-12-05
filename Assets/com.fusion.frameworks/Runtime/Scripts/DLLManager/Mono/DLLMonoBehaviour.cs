using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fusion.Frameworks.DynamicDLL.Mono
{
    public class DLLMonoBehaviour : MonoBehaviour
    {
        [SerializeField]
        private string classFullName;

        private DLLMonoBase monoBase;

        // Start is called before the first frame update
        void Start()
        {
            monoBase.Start();
        }

        // Update is called once per frame
        void Update()
        {
            monoBase.Update();
        }

        void Awake()
        {
            monoBase = DLLManager.Instance.Instantiate<DLLMonoBase>(classFullName, this);
            monoBase.Awake();
        }

        void FixedUpdate()
        {
            monoBase.FixedUpdate();
        }

        void LateUpdate()
        {
            monoBase.LateUpdate();
        }

        void OnDestroy()
        {
            monoBase.OnDestroy();
        }

        void OnEnable()
        {
            monoBase.OnEnable();
        }

        void OnDisable()
        {
            monoBase.OnDisable();
        }

        void OnTriggerEnter(Collider collider)
        {
            monoBase.OnTriggerEnter(collider);
        }

        void OnTriggerExit(Collider collider)
        {
            monoBase.OnTriggerExit(collider);
        }

        void OnTriggerStay(Collider collider)
        {
            monoBase.OnTriggerStay(collider);
        }

        void OnCollisionEnter(Collision collision)
        {
            monoBase.OnCollisionEnter(collision);
        }

        void OnCollisionExit(Collision collision)
        {
            monoBase.OnCollisionExit(collision);
        }

        void OnCollisionStay(Collision collision)
        {
            monoBase.OnCollisionStay(collision);
        }

        void OnMouseEnter()
        {
            monoBase.OnMouseEnter();
        }

        void OnMouseExit()
        {
            monoBase.OnMouseExit();
        }

        void OnMouseDown()
        {
            monoBase.OnMouseDown();
        }

        void OnMouseUp()
        {
            monoBase.OnMouseUp();
        }

        void OnMouseDrag()
        {
            monoBase.OnMouseDrag();
        }

        void OnMouseOver()
        {
            monoBase.OnMouseOver();
        }

        void OnMouseUpAsButton()
        {
            monoBase.OnMouseUpAsButton();
        }
    }
}
