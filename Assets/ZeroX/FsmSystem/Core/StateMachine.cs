using UnityEngine;

namespace ZeroX.FsmSystem
{
    public class StateMachine : MonoBehaviour
    {
        public enum FirstStartGraphWhen
        {
            None, Awake, FirstEnable, Start
        }
        
        public enum ActionWhenDisable
        {
            None, PauseGraph, StopGraph
        }
        
        
        
        [SerializeField] FirstStartGraphWhen firstStartGraphWhen = FirstStartGraphWhen.Start;
        [SerializeField] ActionWhenDisable actionWhenDisable = ActionWhenDisable.PauseGraph;
        [SerializeField] private FsmGraph graph;
        [SerializeField] private bool useUpdate = true;
        [SerializeField] private bool useLateUpdate = false;
        [SerializeField] private bool useFixedUpdate = false;
        [SerializeField] private bool useLateFixedUpdate = false;


        private bool firstEnable = true;
        private ActionWhenDisable actionExecutedWhenDisable = ActionWhenDisable.None;

        public FsmGraph Graph => graph;

        private void Awake()
        {
            if(graph == null)
            {
                Debug.LogError("StateMachine cannot start because graph is null");
                return;
            }
            
            FsmCenter.RegisterFsmGraph(this, graph, useUpdate, useLateUpdate, useFixedUpdate, useLateFixedUpdate);
            
            if(firstStartGraphWhen == FirstStartGraphWhen.Awake)
                graph.StartGraph();
        }

        private void OnEnable()
        {
            if (firstEnable)
            {
                firstEnable = false;
                if(firstStartGraphWhen == FirstStartGraphWhen.FirstEnable)
                    graph.StartGraph();
                return;
            }
            

            if (actionExecutedWhenDisable == ActionWhenDisable.PauseGraph)
            {
                graph.ResumeGraph();
                return;
            }

            if (actionExecutedWhenDisable == ActionWhenDisable.StopGraph)
            {
                graph.StartGraph();
                return;
            }
        }
        
        private void Start()
        {
            if(firstStartGraphWhen == FirstStartGraphWhen.Start)
                graph.StartGraph();
        }

        private void OnDestroy()
        {
            FsmCenter.UnRegisterFsmGraph(this);
            graph.StopGraph();
        }

        private void OnDisable()
        {
            if (actionWhenDisable == ActionWhenDisable.PauseGraph)
            {
                actionExecutedWhenDisable = ActionWhenDisable.PauseGraph;
                graph.PauseGraph();
                return;
            }
            
            if (actionWhenDisable == ActionWhenDisable.StopGraph)
            {
                actionExecutedWhenDisable = ActionWhenDisable.StopGraph;
                graph.StopGraph();
                return;
            }
        }

        

        public void StartGraph()
        {
            graph.StartGraph();
        }
        
        public void StopGraph()
        {
            graph.StopGraph();
        }
        
        public void PauseGraph()
        {
            graph.PauseGraph();
        }
        
        public void ResumeGraph()
        {
            graph.ResumeGraph();
        }

        public void SwitchState(State nextState)
        {
            graph.SwitchState(nextState);
        }

        public void SwitchState(string nextStateName)
        {
            graph.SwitchState(nextStateName);
        }
        
        public void SetTrigger(string trigger)
        {
            graph.SetTrigger(trigger);
        }

        public void SetTriggerFinished()
        {
            graph.SetTrigger(FsmSystem.Transition.finishedName);
        }
        
        
        #region Get State

        public State GetState(string stateName)
        {
            return graph.GetState(stateName);
        }

        public T GetState<T>(string stateName) where T : State
        {
            return graph.GetState<T>(stateName);
        }

        public AnyState GetAnyState(string stateName)
        {
            return graph.GetAnyState(stateName);
        }

        #endregion
    }
}