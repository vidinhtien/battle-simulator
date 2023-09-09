using System;
using System.Collections.Generic;
using UnityEngine;

namespace ZeroX.FsmSystem
{
    [System.Serializable]
    public class FsmGraph : ISerializationCallbackReceiver
    {
        [SerializeField] protected List<Transition> listTransition = new List<Transition>();
        
        [SerializeReference] protected List<State> listNormalState = new List<State>();
        [SerializeReference] protected List<State> listAnyState = new List<State>();
        [SerializeReference] protected List<State> listParallelState = new List<State>();
        
        [SerializeField] private long stateIdSeq = 0;
        [SerializeField] private long transitionIdSeq = 0;
        [SerializeField] private long entryStateId = -1;

        public const string fn_listTransition = "listTransition";
        
        public const string fn_listNormalState = "listNormalState";
        public const string fn_listAnyState = "listAnyState";
        public const string fn_listParallelState = "listParallelState";
        
        public const string fn_stateIdSeq = "stateIdSeq";
        public const string fn_transitionIdSeq = "transitionIdSeq";
        public const string fn_entryStateId = "entryStateId";

        //------
        [NonSerialized] private bool isInitialized = false;
        
        [NonSerialized] private FsmGraphStatus graphStatus = FsmGraphStatus.Stop;
        public FsmGraphStatus GraphStatus => graphStatus;

        private Dictionary<long, State> dictStateById; //StateId - State
        private Dictionary<string, State> dictStateByName; //StateName - State
        private Dictionary<string, Transition> dictAnyTransitionByName; //TransitionName - Transition

        private State prevState;
        private State currentState;
        private State nextState;

        public State PrevState => prevState;
        public State CurrentState => currentState;
        public State NextState => nextState;

        private Transition lastTransition;
        public Transition LastTransition => lastTransition;
        
        //Bonus
        public string PrevStateName => prevState == null ? null : prevState.Name;
        public string CurrentStateName => currentState == null ? null : currentState.Name;
        public string NextStateName => nextState == null ? null : nextState.Name;
        
        //Queue Handle
        private GraphActionFlag graphActionFlag = GraphActionFlag.None;
        private GraphActionQueueType graphActionQueue1 = GraphActionQueueType.None;
        private GraphActionQueueType graphActionQueue2 = GraphActionQueueType.None;
        private State stateForTransitionQueue;
        private Queue<Action> addRemoveActionQueue;
        private bool handingQueue = false;





        void InitIfNot()
        {
            if(isInitialized)
                return;

            //Init Dict State
            dictStateById = new Dictionary<long, State>();
            dictStateByName = new Dictionary<string, State>();
            dictAnyTransitionByName = new Dictionary<string, Transition>();
            
            foreach (var state in listNormalState)
            {
                state.stateType = StateType.Normal;
                state.fsmGraph = this;
                
                dictStateById.Add(state.Id, state);
                dictStateByName.Add(state.Name, state);
            }
            
            foreach (var state in listParallelState)
            {
                state.stateType = StateType.Parallel;
                state.fsmGraph = this;
                
                dictStateById.Add(state.Id, state);
                dictStateByName.Add(state.Name, state);
            }

            foreach (var state in listAnyState)
            {
                state.stateType = StateType.Any;
                state.fsmGraph = this;

                dictStateById.Add(state.Id, state);
                dictStateByName.Add(state.Name, state);
            }
            
            //Init Transition
            foreach (var transition in listTransition)
            {
                if (dictStateById.TryGetValue(transition.OriginId, out var originState) == false)
                {
                    Debug.LogError("Transition has no originId: " + transition.Id + " - " + transition.Name);
                    continue;
                }

                if (originState.stateType == StateType.Any) //Tất cả any state sẽ có chung dictTransitionByName
                    originState.dictTransitionByName = dictAnyTransitionByName;

                originState.dictTransitionByName.Add(transition.Name, transition);
            }
            
            isInitialized = true;
        }
        
        

        #region Handle Queue

        private void EnqueueStartStopPauseResumeRestartGraph(GraphActionQueueType actionQueueType)
        {
            if (graphActionQueue1 == GraphActionQueueType.None && graphActionQueue2 == GraphActionQueueType.None)
            {
                graphActionQueue1 = actionQueueType;
                return;
            }

            if (graphActionQueue1 != GraphActionQueueType.None && graphActionQueue2 != GraphActionQueueType.None)
            {
                Debug.LogWarningFormat("Cannot execute more {0}Graph", actionQueueType);
                return;
            }
            
            //Đến đây thì 1 trong 2 sẽ là None
            
            if (graphActionQueue2 == GraphActionQueueType.AddRemove)
            {
                graphActionQueue1 = actionQueueType;
                return;
            }

            if (graphActionQueue1 == GraphActionQueueType.AddRemove)
            {
                graphActionQueue2 = actionQueueType;
                return;
            }
            
            Debug.LogWarningFormat("Cannot execute more {0}Graph", actionQueueType);
            return;
        }

        private void EnqueueTransition(State state)
        {
            if (graphActionQueue1 == GraphActionQueueType.None && graphActionQueue2 == GraphActionQueueType.None)
            {
                graphActionQueue1 = GraphActionQueueType.Transition;
                stateForTransitionQueue = state;
                return;
            }
            
            if (graphActionQueue1 != GraphActionQueueType.None && graphActionQueue2 != GraphActionQueueType.None)
            {
                Debug.LogWarning("Cannot execute more Transition");
                return;
            }
            
            //Đến đây thì 1 trong 2 sẽ là None
            
            if (graphActionQueue2 == GraphActionQueueType.AddRemove)
            {
                graphActionQueue1 = GraphActionQueueType.Transition;
                stateForTransitionQueue = state;
                return;
            }

            if (graphActionQueue1 == GraphActionQueueType.AddRemove)
            {
                graphActionQueue2 = GraphActionQueueType.Transition;
                stateForTransitionQueue = state;
                return;
            }
            
            Debug.LogWarning("Cannot execute more Transition");
            return;
        }

        private void EnqueueAddRemove(Action action)
        {
            if (graphActionQueue1 == GraphActionQueueType.AddRemove || graphActionQueue2 == GraphActionQueueType.AddRemove)
            {
                if (addRemoveActionQueue == null)
                    addRemoveActionQueue = new Queue<Action>();
                
                addRemoveActionQueue.Enqueue(action);
                return;
            }
            
            //Đến đây tức là cả 1 và 2 đều không phải AddRemove

            if (graphActionQueue1 == GraphActionQueueType.None)
            {
                graphActionQueue1 = GraphActionQueueType.AddRemove;

                if (addRemoveActionQueue == null)
                    addRemoveActionQueue = new Queue<Action>();
                
                addRemoveActionQueue.Enqueue(action);
                return;
            }
            
            if (graphActionQueue2 == GraphActionQueueType.None)
            {
                graphActionQueue2 = GraphActionQueueType.AddRemove;

                if (addRemoveActionQueue == null)
                    addRemoveActionQueue = new Queue<Action>();
                
                addRemoveActionQueue.Enqueue(action);
                return;
            }
            
            Debug.LogWarning("Cannot execute more AddRemove");
            return;
        }

        private void HandleQueue()
        {
            if(handingQueue)
                return;

            handingQueue = true;
            
            while (graphActionQueue1 != GraphActionQueueType.None)
            {
                var actionQueue = graphActionQueue1;
                graphActionQueue1 = graphActionQueue2;
                graphActionQueue2 = GraphActionQueueType.None;

                switch (actionQueue)
                {
                    case GraphActionQueueType.Transition:
                    {
                        if (stateForTransitionQueue != null)
                            SwitchState(stateForTransitionQueue);
                        break;
                    }
                    case GraphActionQueueType.Start:
                    {
                        StartGraph();
                        break;
                    }
                    case GraphActionQueueType.Stop:
                    {
                        StopGraph();
                        break;
                    }
                    case GraphActionQueueType.Pause:
                    {
                        PauseGraph();
                        break;
                    }
                    case GraphActionQueueType.Resume:
                    {
                        ResumeGraph();
                        break;
                    }
                    case GraphActionQueueType.Restart:
                    {
                        RestartGraph();
                        break;
                    }
                    case GraphActionQueueType.AddRemove:
                    {
                        while (addRemoveActionQueue.Count > 0)
                        {
                            addRemoveActionQueue.Dequeue().Invoke();
                        }
                        break;
                    }
                }
            }

            handingQueue = false;
        }

        #endregion


        #region Start Stop Pause Resume Graph

        private void StartGraphInternal()
        {
            //Enter Entry State
            #region Enter Entry State
            
            if (dictStateById.TryGetValue(entryStateId, out var entryState))
            {
                this.currentState = entryState;

                entryState.onStateEnterFrameCount = FsmCenter.frameCount;
                entryState.onStateEnterFixedFrameCount = FsmCenter.fixedFrameCount;
                
                try
                {
                    if (entryState.isAwaken == false)
                    {
                        entryState.isAwaken = true;
                        entryState.OnStateAwake();
                    }
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }

                try
                {
                    entryState.OnStateEnter();
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }
            else
            {
                Debug.LogWarning("Fsm has no entryState");
            }
            
            #endregion
            
            //Enter listParallelState
            #region Enter listParallelState
            
            foreach (var state in listParallelState)
            {
                state.onStateEnterFrameCount = FsmCenter.frameCount;
                state.onStateEnterFixedFrameCount = FsmCenter.fixedFrameCount;
                
                try
                {
                    if (state.isAwaken == false)
                    {
                        state.isAwaken = true;
                        state.OnStateAwake();
                    }
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }

                try
                {
                    state.OnStateEnter();
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }
            
            #endregion
        }
        
        public void StartGraph()
        {
            if(graphStatus != FsmGraphStatus.Stop)
                return;
            
            InitIfNot();

            if (graphActionFlag != GraphActionFlag.None)
            {
                EnqueueStartStopPauseResumeRestartGraph(GraphActionQueueType.Start);
                return;
            }

            graphActionFlag = GraphActionFlag.Starting;
            graphStatus = FsmGraphStatus.Running;

            StartGraphInternal();

            graphActionFlag = GraphActionFlag.None;
            
            HandleQueue();
        }


        private void StopGraphInternal()
        {
            //Exit currentState
            #region Exit currentState
            
            if (currentState != null)
            {
                try
                {
                    currentState.OnStateExit();
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }
            
            #endregion
            
            currentState = null;
            nextState = null;
            
            //Exit Parallel State
            #region Exit Parallel State

            foreach (var state in listParallelState)
            {
                try
                {
                    state.OnStateExit();
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }

            #endregion
            
            prevState = null;
            lastTransition = null;
        }
        
        public void StopGraph()
        {
            if(graphStatus == FsmGraphStatus.Stop)
                return;

            if (graphActionFlag != GraphActionFlag.None)
            {
                EnqueueStartStopPauseResumeRestartGraph(GraphActionQueueType.Stop);
                return;
            }

            graphActionFlag = GraphActionFlag.Stopping;
            graphStatus = FsmGraphStatus.Stop;

            StopGraphInternal();
            
            graphActionFlag = GraphActionFlag.None;
            
            HandleQueue();
        }

        public void PauseGraph()
        {
            if(graphStatus != FsmGraphStatus.Running)
                return;

            if (graphActionFlag != GraphActionFlag.None)
            {
                EnqueueStartStopPauseResumeRestartGraph(GraphActionQueueType.Pause);
                return;
            }

            graphActionFlag = GraphActionFlag.Pausing;
            graphStatus = FsmGraphStatus.Pause;

            //Call OnGraphPause of currentState
            #region Call OnGraphPause of currentState

            if (currentState != null)
            {
                try
                {
                    currentState.OnGraphPause();
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }

            #endregion
            
            //Call OnGraphPause of listParallelState
            #region Call OnGraphPause of listParallelState

            foreach (var state in listParallelState)
            {
                try
                {
                    state.OnGraphPause();
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }

            #endregion

            graphActionFlag = GraphActionFlag.None;
            
            HandleQueue();
        }

        public void ResumeGraph()
        {
            if(graphStatus != FsmGraphStatus.Pause)
                return;
            
            if (graphActionFlag != GraphActionFlag.None)
            {
                EnqueueStartStopPauseResumeRestartGraph(GraphActionQueueType.Resume);
                return;
            }
            
            graphActionFlag = GraphActionFlag.Resuming;
            graphStatus = FsmGraphStatus.Running;
            
            //Call OnGraphResume of currentState
            #region Call OnGraphResume of currentState

            if (currentState != null)
            {
                try
                {
                    currentState.OnGraphResume();
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }

            #endregion
            
            //Call OnGraphResume of listParallelState
            #region Call OnGraphResume of listParallelState

            foreach (var state in listParallelState)
            {
                try
                {
                    state.OnGraphResume();
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }

            #endregion

            graphActionFlag = GraphActionFlag.None;
            
            HandleQueue();
        }
        
        /// <summary>
        /// StartGraph after StopGraph
        /// </summary>
        public void RestartGraph()
        {
            if (graphStatus == FsmGraphStatus.Stop)
            {
                StartGraph();
                return;
            }

            if (graphActionFlag != GraphActionFlag.None)
            {
                EnqueueStartStopPauseResumeRestartGraph(GraphActionQueueType.Restart);
                return;
            }
            
            #region Stop Graph

            graphActionFlag = GraphActionFlag.Stopping;
            graphStatus = FsmGraphStatus.Stop;
            
            StopGraphInternal();
            
            graphActionFlag = GraphActionFlag.None;

            #endregion
            
            

            #region Start Graph

            graphActionFlag = GraphActionFlag.Starting;
            graphStatus = FsmGraphStatus.Running;

            StartGraphInternal();

            graphActionFlag = GraphActionFlag.None;

            #endregion
            
            HandleQueue();
        }
        
        #endregion



        #region Update Graph

        public void UpdateGraph()
        {
            if(graphStatus != FsmGraphStatus.Running)
                return;
            
            if (graphActionFlag != GraphActionFlag.None)
            {
                Debug.LogError("Unable to update graph while in process: " + graphActionFlag);
                return;
            }
            
            //Call OnStateUpdate of currentState
            #region Call OnStateUpdate of currentState

            if (currentState != null)
            {
                if (currentState.onStateEnterFrameCount != FsmCenter.frameCount)
                {
                    try
                    {
                        currentState.OnStateUpdate();
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(e);
                    }
                }
            }

            #endregion
            
            //Call OnStateUpdate of listParallelState
            #region Call OnStateUpdate of listParallelState

            foreach (var state in listParallelState)
            {
                if (state.onStateEnterFrameCount != FsmCenter.frameCount)
                {
                    try
                    {
                        state.OnStateUpdate();
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(e);
                    }
                }
            }

            #endregion
        }
        
        public void LateUpdateGraph()
        {
            if(graphStatus != FsmGraphStatus.Running)
                return;
            
            if (graphActionFlag != GraphActionFlag.None)
            {
                Debug.LogError("Unable to update graph while in process: " + graphActionFlag);
                return;
            }
            
            //Call OnStateLateUpdate of currentState
            #region Call OnStateLateUpdate of currentState

            if (currentState != null)
            {
                if (currentState.onStateEnterFrameCount != FsmCenter.frameCount)
                {
                    try
                    {
                        currentState.OnStateLateUpdate();
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(e);
                    }
                }
            }

            #endregion
            
            //Call OnStateLateUpdate of listParallelState
            #region Call OnStateLateUpdate of listParallelState

            foreach (var state in listParallelState)
            {
                if (state.onStateEnterFrameCount != FsmCenter.frameCount)
                {
                    try
                    {
                        state.OnStateLateUpdate();
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(e);
                    }
                }
            }

            #endregion
        }

        public void FixedUpdateGraph()
        {
            if(graphStatus != FsmGraphStatus.Running)
                return;
            
            if (graphActionFlag != GraphActionFlag.None)
            {
                Debug.LogError("Unable to update graph while in process: " + graphActionFlag);
                return;
            }
            
            //Call OnStateFixedUpdate of currentState
            #region Call OnStateFixedUpdate of currentState

            if (currentState != null)
            {
                if (currentState.onStateEnterFixedFrameCount != FsmCenter.fixedFrameCount)
                {
                    try
                    {
                        currentState.OnStateFixedUpdate();
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(e);
                    }
                }
            }

            #endregion
            
            //Call OnStateFixedUpdate of listParallelState
            #region Call OnStateFixedUpdate of listParallelState

            foreach (var state in listParallelState)
            {
                if (state.onStateEnterFixedFrameCount != FsmCenter.fixedFrameCount)
                {
                    try
                    {
                        state.OnStateFixedUpdate();
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(e);
                    }
                }
            }

            #endregion
        }
        
        public void LateFixedUpdateGraph()
        {
            if(graphStatus != FsmGraphStatus.Running)
                return;
            
            if (graphActionFlag != GraphActionFlag.None)
            {
                Debug.LogError("Unable to update graph while in process: " + graphActionFlag);
                return;
            }
            
            //Call OnStateLateFixedUpdate of currentState
            #region Call OnStateLateFixedUpdate of currentState

            if (currentState != null)
            {
                if (currentState.onStateEnterFixedFrameCount != FsmCenter.fixedFrameCount)
                {
                    try
                    {
                        currentState.OnStateLateFixedUpdate();
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(e);
                    }
                }
            }

            #endregion
            
            //Call OnStateLateFixedUpdate of listParallelState
            #region Call OnStateLateFixedUpdate of listParallelState

            foreach (var state in listParallelState)
            {
                if (state.onStateEnterFixedFrameCount != FsmCenter.fixedFrameCount)
                {
                    try
                    {
                        state.OnStateLateFixedUpdate();
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(e);
                    }
                }
            }

            #endregion
        }

        #endregion
        
        
        public void SwitchState(State nextState)
        {
            if(graphStatus != FsmGraphStatus.Running)
                return;

            if (nextState == null)
            {
                Debug.LogError("State want transition is null");
                return;
            }

            if (nextState.stateType != StateType.Normal)
            {
                Debug.LogError("Cannot transition to " + nextState.stateType);
                return;
            }

            if (this.nextState != null)
            {
                Debug.LogWarning("Cannot transition in OnStateExit");
                return;
            }

            if (graphActionFlag != GraphActionFlag.None)
            {
                EnqueueTransition(nextState);
                return;
            }

            graphActionFlag = GraphActionFlag.Transitioning;
            this.nextState = nextState;
            
            //Exit currentState
            #region Exit currentState
            
            if (currentState != null)
            {
                try
                {
                    currentState.OnStateExit();
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }

            #endregion
            
            this.prevState = this.currentState;
            this.currentState = this.nextState;
            this.nextState = null;

            //Enter nextState
            #region Enter nextState

            this.currentState.onStateEnterFrameCount = FsmCenter.frameCount;
            this.currentState.onStateEnterFixedFrameCount = FsmCenter.fixedFrameCount;
            
            try
            {
                if (this.currentState.isAwaken == false)
                {
                    this.currentState.isAwaken = true;
                    this.currentState.OnStateAwake();
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }

            try
            {
                this.currentState.OnStateEnter();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }

            #endregion
            
            graphActionFlag = GraphActionFlag.None;
            
            HandleQueue();
        }

        public void SwitchState(string stateName)
        {
            if (dictStateByName.TryGetValue(stateName, out var state))
            {
                SwitchState(state);
            }
        }

        public void SetTrigger(string trigger)
        {
            if (currentState == null)
            {
                Debug.LogError("Cannot set trigger because there is no currentState");
                return;
            }
            
            if(trigger == null)
                return;

            if (currentState.dictTransitionByName.TryGetValue(trigger, out var transition))
            {
                if (dictStateById.TryGetValue(transition.TargetId, out var targetState))
                {
                    lastTransition = transition;
                    SwitchState(targetState);
                    return;
                }
            }

            //Check đến AnyState
            if (dictAnyTransitionByName.TryGetValue(trigger, out var anyTransition))
            {
                if (dictStateById.TryGetValue(anyTransition.OriginId, out var originState))
                {
                    if (dictStateById.TryGetValue(anyTransition.TargetId, out var targetState))
                    {
                        var anyState = (AnyState) originState;
                        if (anyState.canTransitionToSelf)
                        {
                            lastTransition = anyTransition;
                            SwitchState(targetState);
                        }
                        else
                        {
                            if (currentState == null || currentState.Id != targetState.Id)
                            {
                                lastTransition = anyTransition;
                                SwitchState(targetState);
                            }
                        }
                        
                        return;
                    }
                }
                else
                {
                    Debug.LogError("Any transition has no origin state");
                }
            }
            
            Debug.LogErrorFormat("Trigger '{0}' not found on state '{1}' and AnyState", trigger, currentState.Name);
        }

        /// <summary>
        /// Set Trigger "finished"
        /// </summary>
        public void SetTriggerFinished()
        {
            SetTrigger(FsmSystem.Transition.finishedName);
        }

        public void SetEntryState(State state)
        {
            if (state == null)
            {
                Debug.LogError("Cannot set entry state null");
                return;
            }
            
            InitIfNot();

            if (state.fsmGraph != this)
            {
                Debug.LogError("Cannot set entry state because state does not belong to this graph");
                return;
            }

            if (state.stateType != StateType.Normal)
            {
                Debug.LogError("Cannot set entry state because state is not normal");
                return;
            }

            entryStateId = state.Id;
        }

        public void SetEntryState(string stateName)
        {
            InitIfNot();

            if (dictStateByName.TryGetValue(stateName, out var state) == false)
            {
                Debug.LogError("State not found with name: " + stateName);
                return;
            }
            
            SetEntryState(state);
        }

        #region Add Normal State

        public T AddNormalState<T>(T state) where T : State
        {
            if (state == null)
            {
                Debug.LogError("Cannot add null state");
                return null;
            }

            if (graphActionFlag != GraphActionFlag.None)
            {
                EnqueueAddRemove(() => AddNormalState(state));
                return state;
            }
            
            //Lý do không check đủ các điều kiện trước vì rất có thể sau khi thực hiện queue thì điều kiện lại đúng
            
            if (state.fsmGraph != null)
            {
                if (state.fsmGraph == this)
                {
                    Debug.LogError("State is already in this graph");
                    return state;
                }
                else
                {
                    Debug.LogError("State is already in another graph");
                    return state;
                }
            }
            
            InitIfNot();

            if (dictStateByName.ContainsKey(state.Name))
            {
                Debug.LogError("Can't add state because of the duplicate name: " + state.Name);
                return null;
            }

            stateIdSeq++;
            state.Id = stateIdSeq;
            state.stateType = StateType.Normal;
            state.fsmGraph = this;

            //listNormalState.Add(state);
            dictStateById.Add(state.Id, state);
            dictStateByName.Add(state.Name, state);

            return state;
        }
        
        public T AddNormalState<T>(string stateName, Vector2 statePos) where T : State, new()
        {
            T state = new T
            {
                Name = stateName,
                Position = statePos
            };

            AddNormalState(state);
            return state;
        }
        
        public T AddNormalState<T>(string stateName, float statePosX, float statePosY) where T : State, new()
        {
            T state = new T
            {
                Name = stateName,
                Position = new Vector2(statePosX, statePosY)
            };

            AddNormalState(state);
            return state;
        }


        #endregion

        #region Add Parallel State

        public T AddParallelState<T>(T state) where T : State
        {
            if (state == null)
            {
                Debug.LogError("Cannot add null state");
                return null;
            }

            if (graphActionFlag != GraphActionFlag.None)
            {
                EnqueueAddRemove(() => AddParallelState(state));
                return state;
            }
            
            //Lý do không check đủ các điều kiện trước vì rất có thể sau khi thực hiện queue thì điều kiện lại đúng
            
            if (state.fsmGraph != null)
            {
                if (state.fsmGraph == this)
                {
                    Debug.LogError("State is already in this graph");
                    return state;
                }
                else
                {
                    Debug.LogError("State is already in another graph");
                    return state;
                }
            }
            
            InitIfNot();

            if (dictStateByName.ContainsKey(state.Name))
            {
                Debug.LogError("Can't add state because of the duplicate name: " + state.Name);
                return null;
            }

            stateIdSeq++;
            state.Id = stateIdSeq;
            state.stateType = StateType.Parallel;
            state.fsmGraph = this;

            listParallelState.Add(state);
            dictStateById.Add(state.Id, state);
            dictStateByName.Add(state.Name, state);
            return state;
        }
        
        public T AddParallelState<T>(string stateName, Vector2 statePos) where T : State, new()
        {
            T state = new T
            {
                Name = stateName,
                Position = statePos
            };

            AddParallelState(state);
            return state;
        }

        public T AddParallelState<T>(string stateName, float statePosX, float statePosY) where T : State, new()
        {
            T state = new T
            {
                Name = stateName,
                Position = new Vector2(statePosX, statePosY)
            };

            AddParallelState(state);
            return state;
        }
        
        #endregion

        #region Add Any State

        public T AddAnyState<T>(T state) where T : AnyState
        {
            if (state == null)
            {
                Debug.LogError("Cannot add null state");
                return null;
            }

            if (graphActionFlag != GraphActionFlag.None)
            {
                EnqueueAddRemove(() => AddAnyState(state));
                return state;
            }
            
            //Lý do không check đủ các điều kiện trước vì rất có thể sau khi thực hiện queue thì điều kiện lại đúng
            
            if (state.fsmGraph != null)
            {
                if (state.fsmGraph == this)
                {
                    Debug.LogError("State is already in this graph");
                    return state;
                }
                else
                {
                    Debug.LogError("State is already in another graph");
                    return state;
                }
            }
            
            InitIfNot();
            
            if (dictStateByName.ContainsKey(state.Name))
            {
                Debug.LogError("Can't add state because of the duplicate name: " + state.Name);
                return null;
            }

            stateIdSeq++;
            state.Id = stateIdSeq;
            state.stateType = StateType.Any;
            state.fsmGraph = this;
            state.dictTransitionByName = dictAnyTransitionByName; //Tất cả anyState sẽ chung 1 dictTransitionByName

            //listAnyState.Add(state);
            dictStateById.Add(state.Id, state);
            dictStateByName.Add(state.Name, state);
            return state;
        }

        public AnyState AddAnyState(string stateName, bool canTransitionToSelf, Vector2 statePos)
        {
            AnyState anyState = new AnyState();
            anyState.Name = stateName;
            anyState.Position = statePos;
            anyState.canTransitionToSelf = canTransitionToSelf;

            AddAnyState(anyState);
            return anyState;
        }
        
        public AnyState AddAnyState(string stateName, bool canTransitionToSelf, float statePosX, float statePosY)
        {
            AnyState anyState = new AnyState
            {
                Name = stateName,
                Position = new Vector2(statePosX, statePosY),
                canTransitionToSelf = canTransitionToSelf
            };

            AddAnyState(anyState);
            return anyState;
        }

        public AnyState AddAnyState(string stateName, Vector2 statePos)
        {
            AnyState anyState = new AnyState();
            anyState.Name = stateName;
            anyState.Position = statePos;
            anyState.canTransitionToSelf = true;

            AddAnyState(anyState);
            return anyState;
        }

        public AnyState AddAnyState(string stateName, float statePosX, float statePosY)
        {
            AnyState anyState = new AnyState
            {
                Name = stateName,
                Position = new Vector2(statePosX, statePosY),
                canTransitionToSelf = true
            };

            AddAnyState(anyState);
            return anyState;
        }
        
        #endregion

        #region Remove State

        public void RemoveState(State state)
        {
            if (state == null)
                return;
            
            if (graphActionFlag != GraphActionFlag.None)
            {
                EnqueueAddRemove(() => RemoveState(state));
                return;
            }

            if (state == currentState)
            {
                Debug.LogError("Cannot remove current state");
                return;
            }

            if (state.fsmGraph == null)
            {
                Debug.LogError("State does not belong to any graph");
                return;
            }

            if (state.fsmGraph != this)
            {
                Debug.LogError("State does not belong to this graph");
                return;
            }

            InitIfNot();

            state.fsmGraph = null;
            dictStateById.Remove(state.Id);
            dictStateByName.Remove(state.Name);

            if (state.stateType == StateType.Normal)
            {
                //Xóa state khỏi list
                //listNormalState.Remove(state);
                
                //Xóa tất cả transition thuộc về state
                state.dictTransitionByName.Clear();
                return;
            }
            
            
            
            if(state.stateType == StateType.Any)
            {
                //listAnyState.Remove(state);
                
                //Xóa tất cả transition thuộc về state
                List<Transition> listTransitionOfState = new List<Transition>();
                foreach (var transition in dictAnyTransitionByName.Values)
                {
                    if(transition.OriginId == state.Id)
                        listTransitionOfState.Add(transition);
                }

                foreach (var transition in listTransitionOfState)
                {
                    dictAnyTransitionByName.Remove(transition.Name);
                }

                return;
            }
            
            
            
            if (state.stateType == StateType.Parallel)
            {
                listParallelState.Remove(state);
                return;
            }
            
            
            Debug.LogError("Not code for stateType: " + state.stateType);
        }
        
        public void RemoveState(string stateName)
        {
            InitIfNot();
            
            if(dictStateByName.TryGetValue(stateName, out var state) == false)
                return;
            
            RemoveState(state);
        }

        public void RemoveState(long stateId)
        {
            InitIfNot();
            
            if(dictStateById.TryGetValue(stateId, out var state) == false)
                return;
            
            RemoveState(state);
        }

        #endregion


        #region Get State

        public State GetState(string stateName)
        {
            InitIfNot();
            if (dictStateByName.TryGetValue(stateName, out var state))
                return state;

            return null;
        }

        public T GetState<T>(string stateName) where T : State
        {
            InitIfNot();
            if (dictStateByName.TryGetValue(stateName, out var state) == false)
                return null;

            if (state is T tState)
            {
                return tState;
            }
            else
            {
                Debug.LogErrorFormat("{0} state is not of type {1}, correct type is {2}", stateName, typeof(T).Name, state.GetType().Name);
                return null;
            }
        }

        public AnyState GetAnyState(string stateName)
        {
            InitIfNot();
            if (dictStateByName.TryGetValue(stateName, out var state) == false)
                return null;

            if (state is AnyState anyState)
            {
                return anyState;
            }
            else
            {
                Debug.LogErrorFormat("{0} state is not of type AnyState, correct type is {1}", stateName, state.GetType().Name);
                return null;
            }
        }

        #endregion
        
        

        #region Add Transition

        public void AddTransition(State originState, string transitionName, State targetState)
        {
            if (string.IsNullOrEmpty(transitionName))
            {
                Debug.LogError("Transition name cannot empty");
                return;
            }

            if (originState == null)
            {
                Debug.LogError("Origin state cannot null");
                return;
            }

            if (graphActionFlag != GraphActionFlag.None)
            {
                EnqueueAddRemove(() => AddTransition(originState, transitionName, targetState));
                return;
            }
            
            InitIfNot();

            //Check OriginState(Graph - Type - Duplicated)
            #region Check Origin State

            if (originState.fsmGraph != this)
            {
                Debug.LogError("OriginState does not belong to this fsmGraph");
                return;
            }

            if (originState.stateType == StateType.Parallel)
            {
                Debug.LogError("Parallel state cannot have transition");
                return;
            }

            if (originState.dictTransitionByName.ContainsKey(transitionName))
            {
                Debug.LogError("Duplicated transition name");
                return;
            }

            #endregion


            
            //Check Target(Graph - Type)
            #region Check Target State

            if (targetState != null)
            {
                if (targetState.fsmGraph != this)
                {
                    Debug.LogError("TargetState does not belong to this fsmGraph");
                    return;
                }
                
                if (targetState.stateType == StateType.Parallel)
                {
                    Debug.LogError("Parallel state cannot have transition");
                    return;
                }
            }

            #endregion
            

            
            Transition transition = new Transition();
            transitionIdSeq++;
            transition.Id = transitionIdSeq;
            transition.Name = transitionName;
            transition.OriginId = originState.Id;
            
            //listTransition.Add(transition);
            originState.dictTransitionByName.Add(transitionName, transition);

            if (targetState != null)
                transition.TargetId = targetState.Id;
        }

        public void AddTransition(State originState, string transitionName)
        {
            AddTransition(originState, transitionName, null);
        }

        public void AddTransition(string originStateName, string transitionName, string targetStateName)
        {
            if (string.IsNullOrEmpty(transitionName))
            {
                Debug.LogError("Transition name cannot empty");
                return;
            }

            if (string.IsNullOrEmpty(originStateName))
            {
                Debug.LogError("Origin state name cannot empty");
                return;
            }
            
            if (graphActionFlag != GraphActionFlag.None)
            {
                EnqueueAddRemove(() => AddTransition(originStateName, transitionName, targetStateName));
                return;
            }
            
            InitIfNot();

            //Check OriginState(Found - Type - Duplicated)
            #region Check Origin State

            if (dictStateByName.TryGetValue(originStateName, out var originState) == false)
            {
                Debug.LogError("Origin state not found with name: " + originStateName);
                return;
            }
            
            if (originState.stateType == StateType.Parallel)
            {
                Debug.LogError("Parallel state cannot have transition");
                return;
            }
            
            if (originState.dictTransitionByName.ContainsKey(transitionName))
            {
                Debug.LogError("Duplicated transition name");
                return;
            }

            #endregion
            
            

            //Check TargetState(Found - Type)
            #region Check Target State

            State targetState = null;
            if (string.IsNullOrEmpty(targetStateName) == false)
            {
                if (dictStateByName.TryGetValue(targetStateName, out targetState) == false)
                {
                    Debug.LogError("Target state not found with name: " + targetStateName);
                    return;
                }
                
                if (targetState.stateType == StateType.Parallel)
                {
                    Debug.LogError("Parallel state cannot have transition");
                    return;
                }
            }

            #endregion
            
            
            
            Transition transition = new Transition();
            transitionIdSeq++;
            transition.Id = transitionIdSeq;
            transition.Name = transitionName;
            transition.OriginId = originState.Id;

            //listTransition.Add(transition);
            originState.dictTransitionByName.Add(transitionName, transition);

            if (targetState != null)
                transition.TargetId = targetState.Id;
        }

        public void AddTransition(string originState, string transitionName)
        {
            AddTransition(originState, transitionName, null);
        }
        
        #endregion

        #region Set Transition

        public void SetTransition(State originState, string transitionName, State targetState)
        {
            if (string.IsNullOrEmpty(transitionName))
            {
                Debug.LogError("Transition name cannot empty");
                return;
            }

            if (originState == null)
            {
                Debug.LogError("Origin state cannot null");
                return;
            }

            if (graphActionFlag != GraphActionFlag.None)
            {
                EnqueueAddRemove(() => AddTransition(originState, transitionName, targetState));
                return;
            }
            
            InitIfNot();

            //Check OriginState(Graph - Type)
            #region Check Origin State

            if (originState.fsmGraph != this)
            {
                Debug.LogError("OriginState does not belong to this fsmGraph");
                return;
            }

            if (originState.stateType == StateType.Parallel)
            {
                Debug.LogError("Parallel state cannot have transition");
                return;
            }

            #endregion


            
            //Check Target(Graph - Type)
            #region Check Target State

            if (targetState != null)
            {
                if (targetState.fsmGraph != this)
                {
                    Debug.LogError("TargetState does not belong to this fsmGraph");
                    return;
                }
                
                if (targetState.stateType == StateType.Parallel)
                {
                    Debug.LogError("Parallel state cannot have transition");
                    return;
                }
            }

            #endregion
            

            
            if (originState.dictTransitionByName.TryGetValue(transitionName, out var transition))
            {
                if (targetState != null)
                    transition.TargetId = targetState.Id;
                return;
            }
            else
            {
                transition = new Transition();
                transitionIdSeq++;
                transition.Id = transitionIdSeq;
                transition.Name = transitionName;
                transition.OriginId = originState.Id;
            
                //listTransition.Add(transition);
                originState.dictTransitionByName.Add(transitionName, transition);

                if (targetState != null)
                    transition.TargetId = targetState.Id;
            }
        }
        
        public void SetTransition(State originState, string transitionName)
        {
            SetTransition(originState, transitionName, null);
        }
        
        public void SetTransition(string originStateName, string transitionName, string targetStateName)
        {
            if (string.IsNullOrEmpty(transitionName))
            {
                Debug.LogError("Transition name cannot empty");
                return;
            }

            if (string.IsNullOrEmpty(originStateName))
            {
                Debug.LogError("Origin state name cannot empty");
                return;
            }
            
            if (graphActionFlag != GraphActionFlag.None)
            {
                EnqueueAddRemove(() => AddTransition(originStateName, transitionName, targetStateName));
                return;
            }
            
            InitIfNot();

            //Check OriginState(Found - Type)
            #region Check Origin State

            if (dictStateByName.TryGetValue(originStateName, out var originState) == false)
            {
                Debug.LogError("Origin state not found with name: " + originStateName);
                return;
            }
            
            if (originState.stateType == StateType.Parallel)
            {
                Debug.LogError("Parallel state cannot have transition");
                return;
            }

            #endregion
            
            

            //Check TargetState(Found - Type)
            #region Check Target State

            State targetState = null;
            if (string.IsNullOrEmpty(targetStateName) == false)
            {
                if (dictStateByName.TryGetValue(targetStateName, out targetState) == false)
                {
                    Debug.LogError("Target state not found with name: " + targetStateName);
                    return;
                }
                
                if (targetState.stateType == StateType.Parallel)
                {
                    Debug.LogError("Parallel state cannot have transition");
                    return;
                }
            }

            #endregion
            
            
            
            if (originState.dictTransitionByName.TryGetValue(transitionName, out var transition))
            {
                if (targetState != null)
                    transition.TargetId = targetState.Id;
                return;
            }
            else
            {
                transition = new Transition();
                transitionIdSeq++;
                transition.Id = transitionIdSeq;
                transition.Name = transitionName;
                transition.OriginId = originState.Id;

                //listTransition.Add(transition);
                originState.dictTransitionByName.Add(transitionName, transition);

                if (targetState != null)
                    transition.TargetId = targetState.Id;
            }
        }
        
        public void SetTransition(string originState, string transitionName)
        {
            SetTransition(originState, transitionName, null);
        }
        
        #endregion

        #region Remove Transition

        public void RemoveTransition(string originStateName, string transitionName)
        {
            if (string.IsNullOrEmpty(transitionName))
            {
                Debug.LogError("Transition name cannot empty");
                return;
            }

            if (string.IsNullOrEmpty(originStateName))
            {
                Debug.LogError("Origin state name cannot empty");
                return;
            }

            if (graphActionFlag != GraphActionFlag.None)
            {
                EnqueueAddRemove(() => RemoveTransition(originStateName, transitionName));
                return;
            }
            
            InitIfNot();

            if (dictStateByName.TryGetValue(originStateName, out var originState) == false)
            {
                Debug.LogError("Origin state not found with name: " + originStateName);
                return;
            }

            if (originState.dictTransitionByName.TryGetValue(transitionName, out var transition))
            {
                originState.dictTransitionByName.Remove(transitionName);
                //listTransition.Remove(transition);
            }
        }

        /// <summary>
        /// Remove transition with origin state is AnyState
        /// </summary>
        public void RemoveAnyTransition(string transitionName)
        {
            if (string.IsNullOrEmpty(transitionName))
            {
                Debug.LogError("Transition name cannot empty");
                return;
            }

            if (graphActionFlag != GraphActionFlag.None)
            {
                EnqueueAddRemove(() => RemoveAnyTransition(transitionName));
                return;
            }
            
            InitIfNot();

            if (dictAnyTransitionByName.TryGetValue(transitionName, out var transition))
            {
                dictAnyTransitionByName.Remove(transitionName);
                //listTransition.Remove(transition);
            }
        }

        #endregion
        
        
        

        /// <summary>
        /// Remove all state, transition, reset entryState...
        /// </summary>
        public void ClearGraph()
        {
            if (graphStatus != FsmGraphStatus.Stop)
            {
                Debug.LogError("Can only clear graph while stopping");
                return;
            }
            
            if (graphActionFlag != GraphActionFlag.None)
            {
                EnqueueAddRemove(() => ClearGraph());
                return;
            }

            isInitialized = false;
            
            stateIdSeq = 0;
            transitionIdSeq = 0;
            entryStateId = State.emptyId;
            
            
            listTransition.Clear();
            
            listNormalState.Clear();
            listParallelState.Clear();
            listAnyState.Clear();
            
            dictStateById?.Clear();
            dictStateByName?.Clear();
            dictAnyTransitionByName?.Clear();
        }

        public void OnBeforeSerialize()
        {
            //Vì OnAfterDeserialize không thực hiện chuyển từ list sang dict nên khi chưa initialize thì dict sẽ là rỗng
            //Nên nếu thực hiện chuyển từ dict sang list lúc chưa initialize thì sẽ là chuyển dict rỗng sang list -> khiến list cũng bị xóa
            if(isInitialized == false)
                return;
            
            //Clear
            listNormalState.Clear();
            listAnyState.Clear();
            listParallelState.Clear();
            
            listTransition.Clear();
            

            //Các listState
            if (dictStateById != null)
            {
                foreach (var state in dictStateById.Values)
                {
                    if(state.stateType == StateType.Normal)
                        listNormalState.Add(state);
                    else if(state.stateType == StateType.Any)
                        listAnyState.Add(state);
                    else if(state.stateType == StateType.Parallel)
                        listParallelState.Add(state);
                    else
                    {
                        Debug.LogError("Not code for stateType: " + state.stateType);
                    }
                }
            }
            

            //normal transition
            foreach (var state in listNormalState)
            {
                foreach (var transition in state.dictTransitionByName.Values)
                {
                    listTransition.Add(transition);
                }
            }

            //any transition
            if (dictAnyTransitionByName != null)
            {
                foreach (var transition in dictAnyTransitionByName.Values)
                {
                    listTransition.Add(transition);
                }
            }
        }

        public void OnAfterDeserialize()
        {
            
        }
    }
}