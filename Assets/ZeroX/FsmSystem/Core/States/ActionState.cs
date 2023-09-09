using System;

namespace ZeroX.FsmSystem
{
    [System.Serializable]
    public class ActionState : State
    {
        public Action onStateAwake;
        public Action onStateEnter;
        public Action onStateExit;

        public Action onStateUpdate;
        public Action onStateLateUpdate;
        public Action onStateFixedUpdate;
        public Action onStateLateFixedUpdate;
        
        public Action onGraphPause;
        public Action onGraphResume;
        
        
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

        public ActionState RegisterOnStateAwake(Action action)
        {
            onStateAwake += action;
            return this;
        }
        
        public ActionState RegisterOnStateEnter(Action action)
        {
            onStateEnter += action;
            return this;
        }
        
        public ActionState RegisterOnStateExit(Action action)
        {
            onStateExit += action;
            return this;
        }

        
        
        public ActionState RegisterOnStateUpdate(Action action)
        {
            onStateUpdate += action;
            return this;
        }
        
        public ActionState RegisterOnStateLateUpdate(Action action)
        {
            onStateLateUpdate += action;
            return this;
        }
        
        public ActionState RegisterOnStateFixedUpdate(Action action)
        {
            onStateFixedUpdate += action;
            return this;
        }
        
        public ActionState RegisterOnStateLateFixedUpdate(Action action)
        {
            onStateLateFixedUpdate += action;
            return this;
        }
        
        
        
        public ActionState RegisterOnGraphPause(Action action)
        {
            onGraphPause += action;
            return this;
        }
        
        public ActionState RegisterOnGraphResume(Action action)
        {
            onGraphResume += action;
            return this;
        }
        
        #endregion
    }
}