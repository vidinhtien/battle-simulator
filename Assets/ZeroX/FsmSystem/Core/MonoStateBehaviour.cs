using UnityEngine;

namespace ZeroX.FsmSystem
{
    public abstract class MonoStateBehaviour : MonoBehaviour
    {
        [System.NonSerialized] internal bool isAwaken = false;
        public FsmGraph FsmGraph { get; internal set; }
        public MonoScriptState State { get; internal set; }

        #region State Method

        public virtual void OnStateAwake()
        {
        }
        
        public virtual void OnStateEnter()
        {
        }

        public virtual void OnStateExit()
        {
        }
        
        

        public virtual void OnStateUpdate()
        {
        }
        
        public virtual void OnStateLateUpdate()
        {
        }
        
        public virtual void OnStateFixedUpdate()
        {
        }
        
        public virtual void OnStateLateFixedUpdate()
        {
        }

        
        
        public virtual void OnGraphPause()
        {
        }
        
        public virtual void OnGraphResume()
        {
        }

        #endregion

        #region Behaviour Method

        /// <summary>
        /// Use this.State and this.FsmGraph to know which state you just added
        /// </summary>
        public virtual void OnAddToState()
        {
            
        }

        /// <summary>
        /// You can still use this.State and this.FsmGraph to know which state you just removed from
        /// </summary>
        public virtual void OnRemoveFromState()
        {
            
        }

        #endregion

        
        /// <summary>
        /// Unlike FsmGraph.Transition, this only works if FsmGraph.CurrentState is the state of StateBehaviour calling it.
        /// </summary>
        public void SwitchState(State nextState)
        {
            if(FsmGraph.CurrentState != State)
                return;
            
            FsmGraph.SwitchState(nextState);
        }
        
        
        /// <summary>
        /// Unlike FsmGraph.Transition, this only works if FsmGraph.CurrentState is the state of StateBehaviour calling it.
        /// </summary>
        public void SwitchState(string nextStateName)
        {
            if(FsmGraph.CurrentState != State)
                return;
            
            FsmGraph.SwitchState(nextStateName);
        }
        
        
        /// <summary>
        /// Unlike FsmGraph.SetTrigger, this only works if FsmGraph.CurrentState is the state of StateBehaviour calling it.
        /// </summary>
        public void SetTrigger(string trigger)
        {
            if(FsmGraph.CurrentState != State)
                return;
            
            FsmGraph.SetTrigger(trigger);
        }

        
        /// <summary>
        /// Unlike FsmGraph.SetTriggerFinished, this only works if FsmGraph.CurrentState is the state of StateBehaviour calling it.
        /// </summary>
        public void SetTriggerFinished()
        {
            if(FsmGraph.CurrentState != State)
                return;
            
            FsmGraph.SetTrigger(FsmSystem.Transition.finishedName);
        }
    }
}