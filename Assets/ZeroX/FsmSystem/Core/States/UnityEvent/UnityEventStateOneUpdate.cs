using UnityEngine;
using UnityEngine.Events;

namespace ZeroX.FsmSystem
{
    [System.Serializable]
    public class UnityEventStateOneUpdate : State
    {
        [SerializeField] public UnityEvent onStateEnter = new UnityEvent();
        [SerializeField] public UnityEvent onStateExit = new UnityEvent();
        [SerializeField] public UnityEvent onStateUpdate = new UnityEvent();
        
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

        
        
        #region Utility

        public UnityEventStateOneUpdate RegisterOnStateEnter(UnityAction action)
        {
            onStateEnter.AddListener(action);
            return this;
        }
        
        public UnityEventStateOneUpdate RegisterOnStateExit(UnityAction action)
        {
            onStateExit.AddListener(action);
            return this;
        }

        
        
        public UnityEventStateOneUpdate RegisterOnStateUpdate(UnityAction action)
        {
            onStateUpdate.AddListener(action);
            return this;
        }

        #endregion
    }
}