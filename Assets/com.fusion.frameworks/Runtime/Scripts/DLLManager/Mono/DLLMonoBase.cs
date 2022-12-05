using UnityEngine;

namespace Fusion.Frameworks.DynamicDLL.Mono
{
    public class DLLMonoBase
    {
        protected MonoBehaviour mono = null;

        public DLLMonoBase()
        {
        }

        public DLLMonoBase(MonoBehaviour mono)
        {
            this.mono = mono;
        }

        public virtual void Start()
        {

        }

        // Update is called once per frame
        public virtual void Update()
        {

        }

        public virtual void Awake()
        {

        }

        public virtual void FixedUpdate()
        {

        }

        public virtual void LateUpdate()
        {

        }

        public virtual void OnDestroy()
        {

        }

        public virtual void OnEnable()
        {

        }

        public virtual void OnDisable()
        {

        }

        public virtual void OnTriggerEnter(Collider collider)
        {

        }

        public virtual void OnTriggerExit(Collider collider)
        {

        }

        public virtual void OnTriggerStay(Collider collider)
        {

        }

        public virtual void OnCollisionEnter(Collision collision)
        {

        }

        public virtual void OnCollisionExit(Collision collision)
        {

        }

        public virtual void OnCollisionStay(Collision collision)
        {

        }

        public virtual void OnMouseEnter()
        {

        }

        public virtual void OnMouseExit()
        {

        }

        public virtual void OnMouseDown()
        {

        }

        public virtual void OnMouseUp()
        {

        }

        public virtual void OnMouseDrag()
        {

        }

        public virtual void OnMouseOver()
        {

        }

        public virtual void OnMouseUpAsButton()
        {

        }
    }
}

