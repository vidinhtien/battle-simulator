using UnityEngine;
using UnityEngine.Events;

namespace ZeroX.FsmSystem
{
    [System.Serializable]
    public class UnityEventStateMultiUpdate : State
    {
        [SerializeField] public UnityEvent onStateEnter = new UnityEvent();
        [SerializeField] public UnityEvent onStateExit = new UnityEvent();
        
        [Header("Updates")]
        [SerializeField] public UnityEvent onStateUpdate = new UnityEvent();
        [SerializeField] public UnityEvent onStateLateUpdate = new UnityEvent();
        [SerializeField] public UnityEvent onStateFixedUpdate = new UnityEvent();
        [SerializeField] public UnityEvent onStateLateFixedUpdate = new UnityEvent();
        
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

        #region Utility

        public UnityEventStateMultiUpdate RegisterOnStateEnter(UnityAction action)
        {
            onStateEnter.AddListener(action);
            return this;
        }
        
        public UnityEventStateMultiUpdate RegisterOnStateExit(UnityAction action)
        {
            onStateExit.AddListener(action);
            return this;
        }

        
        
        public UnityEventStateMultiUpdate RegisterOnStateUpdate(UnityAction action)
        {
            onStateUpdate.AddListener(action);
            return this;
        }
        
        public UnityEventStateMultiUpdate RegisterOnStateLateUpdate(UnityAction action)
        {
            onStateLateUpdate.AddListener(action);
            return this;
        }
        
        public UnityEventStateMultiUpdate RegisterOnStateFixedUpdate(UnityAction action)
        {
            onStateFixedUpdate.AddListener(action);
            return this;
        }
        
        public UnityEventStateMultiUpdate RegisterOnStateLateFixedUpdate(UnityAction action)
        {
            onStateLateFixedUpdate.AddListener(action);
            return this;
        }

        #endregion
    }
}