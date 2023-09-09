using UnityEngine;
using UnityEngine.Events;

namespace ZeroX.FsmSystem
{
    [System.Serializable]
    public class UnityEventStateLite : State
    {
        [SerializeField] public UnityEvent onStateEnter = new UnityEvent();
        [SerializeField] public UnityEvent onStateExit = new UnityEvent();

        public override void OnStateEnter()
        {
            onStateEnter?.Invoke();
        }

        public override void OnStateExit()
        {
            onStateExit?.Invoke();
        }

        #region Utility

        public UnityEventStateLite RegisterOnStateEnter(UnityAction action)
        {
            onStateEnter.AddListener(action);
            return this;
        }
        
        public UnityEventStateLite RegisterOnStateExit(UnityAction action)
        {
            onStateExit.AddListener(action);
            return this;
        }

        #endregion
    }
}