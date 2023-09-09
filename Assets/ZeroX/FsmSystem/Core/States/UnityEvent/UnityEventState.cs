using UnityEngine;
using UnityEngine.Events;

namespace ZeroX.FsmSystem
{
    [System.Serializable]
    public class UnityEventState : State
    {
        [Header("Common")]
        [SerializeField] public UnityEvent onStateAwake = new UnityEvent();
        [SerializeField] public UnityEvent onStateEnter = new UnityEvent();
        [SerializeField] public UnityEvent onStateExit = new UnityEvent();
        
        [Header("Updates")]
        [SerializeField] public UnityEvent onStateUpdate = new UnityEvent();
        [SerializeField] public UnityEvent onStateLateUpdate = new UnityEvent();
        [SerializeField] public UnityEvent onStateFixedUpdate = new UnityEvent();
        [SerializeField] public UnityEvent onStateLateFixedUpdate = new UnityEvent();
        
        [Header("Graph")]
        [SerializeField] public UnityEvent onGraphPause = new UnityEvent();
        [SerializeField] public UnityEvent onGraphResume = new UnityEvent();


        public override void OnStateAwake()
        {
            onStateAwake?.Invoke();
        }
        
        public override void OnStateEnter()
        {
            onStateEnter?.Invoke();
        }
        
        public override void OnStateExit()
        {
            onStateExit?.Invoke();
        }
        
        

        public override void OnStateUpdate()
        {
            onStateUpdate?.Invoke();
        }
        
        public override void OnStateLateUpdate()
        {
            onStateLateUpdate?.Invoke();
        }

        public override void OnStateFixedUpdate()
        {
            onStateFixedUpdate?.Invoke();
        }

        public override void OnStateLateFixedUpdate()
        {
            onStateLateFixedUpdate?.Invoke();
        }
        
        

        public override void OnGraphPause()
        {
            onGraphPause?.Invoke();
        }

        public override void OnGraphResume()
        {
            onGraphResume?.Invoke();
        }
        
        

        #region Utility

        public UnityEventState RegisterOnStateAwake(UnityAction action)
        {
            onStateAwake.AddListener(action);
            return this;
        }
        
        public UnityEventState RegisterOnStateEnter(UnityAction action)
        {
            onStateEnter.AddListener(action);
            return this;
        }
        
        public UnityEventState RegisterOnStateExit(UnityAction action)
        {
            onStateExit.AddListener(action);
            return this;
        }

        
        
        public UnityEventState RegisterOnStateUpdate(UnityAction action)
        {
            onStateUpdate.AddListener(action);
            return this;
        }
        
        public UnityEventState RegisterOnStateLateUpdate(UnityAction action)
        {
            onStateLateUpdate.AddListener(action);
            return this;
        }
        
        public UnityEventState RegisterOnStateFixedUpdate(UnityAction action)
        {
            onStateFixedUpdate.AddListener(action);
            return this;
        }
        
        public UnityEventState RegisterOnStateLateFixedUpdate(UnityAction action)
        {
            onStateLateFixedUpdate.AddListener(action);
            return this;
        }
        
        
        
        public UnityEventState RegisterOnGraphPause(UnityAction action)
        {
            onGraphPause.AddListener(action);
            return this;
        }
        
        public UnityEventState RegisterOnGraphResume(UnityAction action)
        {
            onGraphResume.AddListener(action);
            return this;
        }
        #endregion
    }
}