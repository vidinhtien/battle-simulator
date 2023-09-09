using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ZeroX.FsmSystem.Editors
{
    public static class CopyPasteUtility
    {
        private static class ClipBoard
        {
            public static List<State> listNormalState = new List<State>();
            public static List<State> listAnyState = new List<State>();
            public static List<State> listParallelState = new List<State>();
            
            public static List<Transition> listTransition = new List<Transition>();

            public static Dictionary<long, Dictionary<StateEventFieldName, int>> dictActionStateInvocationLength = new Dictionary<long, Dictionary<StateEventFieldName, int>>();
            
            public static void Clear()
            {
                listNormalState.Clear();
                listAnyState.Clear();
                listParallelState.Clear();
            
                listTransition.Clear();
                
                dictActionStateInvocationLength.Clear();
            }

            public static IEnumerable<State> GetAllState()
            {
                foreach (var state in listNormalState)
                {
                    yield return state;
                }
                
                foreach (var state in listAnyState)
                {
                    yield return state;
                }
                
                foreach (var state in listParallelState)
                {
                    yield return state;
                }
            }

            public static bool HasData => listNormalState.Count > 0 || listAnyState.Count > 0 || listParallelState.Count > 0;
        }

        public static bool HasDataToCopy => ClipBoard.HasData;
        
        
        

        static void CopyListState(StateMachineGraphContext context, SerializedProperty listStateSp, HashSet<long> listStateIdWantCopy, List<State> listState, Dictionary<long, Dictionary<StateEventFieldName, int>> dictActionStateInvocationLength)
        {
            HashSet<long> listStateIdCopied = new HashSet<long>();
            List<State> listStateTemp = new List<State>();
            List<Transition> listTransitionTemp = new List<Transition>();
            
            
            //Get state need copy
            for (int i = 0; i < listStateSp.arraySize; i++)
            {
                var stateSp = listStateSp.GetArrayElementAtIndex(i);
                if(listStateIdWantCopy.Contains(stateSp.AsState().Id) == false)
                    continue;
                
                var state = (State)stateSp.GetObject();
                listStateTemp.Add(state);
                listStateIdCopied.Add(state.Id);
                
                if(dictActionStateInvocationLength != null && state is ActionState actionState)
                    GenerateDictActionStateInvocationLength(dictActionStateInvocationLength, actionState);
            }

            //Get transition need copy
            for (int i = 0; i < context.ListTransitionSp.arraySize; i++)
            {
                var transitionSp = context.ListTransitionSp.GetArrayElementAtIndex(i);
                if(listStateIdCopied.Contains(transitionSp.AsTransition().OriginId) == false)
                    continue;
                
                var transition = (Transition)transitionSp.GetObject();
                listTransitionTemp.Add(transition);
            }
            
            List<State> listStateClone = StateCloner.Clone(listStateTemp);
            List<Transition> listTransitionClone = TransitionCloner.Clone(listTransitionTemp);
            
            listState.AddRange(listStateClone);
            ClipBoard.listTransition.AddRange(listTransitionClone);
        }

        static void GenerateDictActionStateInvocationLength(Dictionary<long, Dictionary<StateEventFieldName, int>> dictActionStateInvocationLength, ActionState actionState)
        {
            Dictionary<StateEventFieldName, int> dictActionInvocationLength = new Dictionary<StateEventFieldName, int>();
            dictActionStateInvocationLength.Add(actionState.Id, dictActionInvocationLength);

            foreach (var eventFieldName in StateEventFieldNameDefine.list)
            {
                var action = EditorActionStateUtility.GetAction(actionState, eventFieldName.ToString());
                if (action != null)
                {
                    dictActionInvocationLength.Add(eventFieldName, action.GetInvocationList().Length);
                }
            }
            return;
        }
        
        public static void CopyStates(StateMachineGraphContext context, HashSet<long> listStateId)
        {
            ClipBoard.Clear();

            CopyListState(context, context.ListNormalStateSp, listStateId, ClipBoard.listNormalState, ClipBoard.dictActionStateInvocationLength);
            CopyListState(context, context.ListAnyStateSp, listStateId, ClipBoard.listAnyState, null);
            CopyListState(context, context.ListParallelStateSp, listStateId, ClipBoard.listParallelState, ClipBoard.dictActionStateInvocationLength);
        }

        /// <summary>
        /// Return list stateId pasted
        /// </summary>
        public static HashSet<long> PasteStates(StateMachineGraphContext context, Vector2 position)
        {
            //Tạo ra các list mới
            List<State> listNormalState = StateCloner.Clone(ClipBoard.listNormalState);
            List<State> listAnyState = StateCloner.Clone(ClipBoard.listAnyState);
            List<State> listParallelState = StateCloner.Clone(ClipBoard.listParallelState);
            List<Transition> listTransition = TransitionCloner.Clone(ClipBoard.listTransition);
            
            
            //New state pos
            ChangeListStateToNewPos(position, listNormalState, listAnyState, listParallelState);
            
            
            //New state id
            long stateIdSeq = context.GraphSp.AsStateMachineGraph().StateIdSeq;
            Dictionary<long, long> dictStateOldNewId = new Dictionary<long, long>(); //OldId - NewId
            
            GenerateNewStateId(listNormalState, dictStateOldNewId, ref stateIdSeq);
            GenerateNewStateNameIfDuplicated(context, listNormalState);
            
            GenerateNewStateId(listAnyState, dictStateOldNewId, ref stateIdSeq);
            GenerateNewStateNameIfDuplicated(context, listAnyState);
            
            GenerateNewStateId(listParallelState, dictStateOldNewId, ref stateIdSeq);
            GenerateNewStateNameIfDuplicated(context, listParallelState);

            context.GraphSp.AsStateMachineGraph().StateIdSeq = stateIdSeq;

            
            //New transition id
            long transitionIdSeq = context.GraphSp.AsStateMachineGraph().TransitionIdSeq;
            GenerateNewTransitionId(listTransition, ref transitionIdSeq);
            
            context.GraphSp.AsStateMachineGraph().TransitionIdSeq = transitionIdSeq;
            
            
            //New Transition originId and targetId
            GenerateNewTransitionOriginTargetId(listTransition, dictStateOldNewId);
            
            //Add to graph
            AddListStateToListStateSp(listNormalState, context.ListNormalStateSp);
            AddListStateToListStateSp(listAnyState, context.ListAnyStateSp);
            AddListStateToListStateSp(listParallelState, context.ListParallelStateSp);
            
            AddListTransitionToListTransitionSp(listTransition, context.ListTransitionSp);

            return GenerateHashSetStateId(listNormalState, listAnyState, listParallelState);
        }

        public static HashSet<long> PasteStates_ConvertActionStateToUnityEventState(StateMachineGraphContext context, Vector2 position)
        {
            //Tạo ra các list mới
            List<State> listNormalStateNonConverted = StateCloner.Clone(ClipBoard.listNormalState);
            List<State> listAnyState = StateCloner.Clone(ClipBoard.listAnyState);
            List<State> listParallelStateNonConverted = StateCloner.Clone(ClipBoard.listParallelState);
            List<Transition> listTransition = TransitionCloner.Clone(ClipBoard.listTransition);

            var listNormalState = GenerateNewListState_ConvertActionStateToUnityEventState(listNormalStateNonConverted);
            var listParallelState = GenerateNewListState_ConvertActionStateToUnityEventState(listParallelStateNonConverted);
            
            
            //New state pos
            ChangeListStateToNewPos(position, listNormalState, listAnyState, listParallelState);
            
            
            //New state id
            long stateIdSeq = context.GraphSp.AsStateMachineGraph().StateIdSeq;
            Dictionary<long, long> dictStateOldNewId = new Dictionary<long, long>(); //OldId - NewId
            
            GenerateNewStateId(listNormalState, dictStateOldNewId, ref stateIdSeq);
            GenerateNewStateNameIfDuplicated(context, listNormalState);
            
            GenerateNewStateId(listAnyState, dictStateOldNewId, ref stateIdSeq);
            GenerateNewStateNameIfDuplicated(context, listAnyState);
            
            GenerateNewStateId(listParallelState, dictStateOldNewId, ref stateIdSeq);
            GenerateNewStateNameIfDuplicated(context, listParallelState);

            context.GraphSp.AsStateMachineGraph().StateIdSeq = stateIdSeq;

            
            //New transition id
            long transitionIdSeq = context.GraphSp.AsStateMachineGraph().TransitionIdSeq;
            GenerateNewTransitionId(listTransition, ref transitionIdSeq);
            
            context.GraphSp.AsStateMachineGraph().TransitionIdSeq = transitionIdSeq;
            
            
            //New Transition originId and targetId
            GenerateNewTransitionOriginTargetId(listTransition, dictStateOldNewId);
            
            //Add States to graph
            List<SerializedProperty> listNormalStateSpAdded = AddListStateToListStateSp(listNormalState, context.ListNormalStateSp);
            AddListStateToListStateSp(listAnyState, context.ListAnyStateSp);
            List<SerializedProperty> listParallelStateSpAdded = AddListStateToListStateSp(listParallelState, context.ListParallelStateSp);
            
            //Add Transitions to graph
            AddListTransitionToListTransitionSp(listTransition, context.ListTransitionSp);
            
            //Thêm persistentListener
            GenerateListUnityEventStatePersistentListener(listNormalStateNonConverted, listNormalStateSpAdded);
            GenerateListUnityEventStatePersistentListener(listParallelStateNonConverted, listParallelStateSpAdded);

            
            return GenerateHashSetStateId(listNormalState, listAnyState, listParallelState);
        }

        static void GenerateNewStateId(List<State> listState, Dictionary<long, long> dictOldNewId, ref long stateIdSeq)
        {
            foreach (var state in listState)
            {
                stateIdSeq++;
                dictOldNewId.Add(state.Id, stateIdSeq);
                EditorStateUtility.SetId(state, stateIdSeq);
            }
        }

        static void GenerateNewStateNameIfDuplicated(StateMachineGraphContext context, List<State> listState)
        {
            HashSet<string> listStateName = new HashSet<string>();
            var allState = context.GetAllStateSp();
            foreach (var state in allState)
            {
                listStateName.Add(state.AsState().Name);
            }


            foreach (var state in listState)
            {
                string stateName = state.Name;
                string newStateName = stateName;
                int index = 2;
                while (listStateName.Contains(newStateName))
                {
                    newStateName = stateName + " " + index;
                    index++;
                }
                
                EditorStateUtility.SetName(state, newStateName);
            }
        }
        
        static void GenerateNewTransitionId(List<Transition> listTransition, ref long transitionIdSeq)
        {
            foreach (var transition in listTransition)
            {
                transitionIdSeq++;
                EditorTransitionUtility.SetId(transition, transitionIdSeq);
            }
        }

        static void GenerateNewTransitionOriginTargetId(List<Transition> listTransition, Dictionary<long, long> dictStateOldNewId)
        {
            foreach (var transition in listTransition)
            {
                long newOriginId = dictStateOldNewId[transition.OriginId];
                long newTargetId = State.emptyId;
                if (transition.TargetId != State.emptyId && dictStateOldNewId.TryGetValue(transition.TargetId, out newTargetId))
                {
                    
                }
                
                EditorTransitionUtility.SetOriginId(transition, newOriginId);
                EditorTransitionUtility.SetTargetId(transition, newTargetId);
            }
        }

        static List<SerializedProperty> AddListStateToListStateSp(List<State> listState, SerializedProperty listStateSp)
        {
            List<SerializedProperty> listStateSpAdded = new List<SerializedProperty>();
            foreach (var state in listState)
            {
                int index = listStateSp.arraySize;
                listStateSp.InsertArrayElementAtIndex(index);
                var stateSp = listStateSp.GetArrayElementAtIndex(index);
                stateSp.managedReferenceValue = state;
                
                listStateSpAdded.Add(stateSp);
            }

            return listStateSpAdded;
        }
        
        static void AddListTransitionToListTransitionSp(List<Transition> listTransition, SerializedProperty listTransitionSp)
        {
            foreach (var transition in listTransition)
            {
                int index = listTransitionSp.arraySize;
                listTransitionSp.InsertArrayElementAtIndex(index);
                
                var transitionSp = listTransitionSp.GetArrayElementAtIndex(index);

                transitionSp.AsTransition().Id = transition.Id;
                transitionSp.AsTransition().OriginId = transition.OriginId;
                transitionSp.AsTransition().TargetId = transition.TargetId;
                transitionSp.AsTransition().Name = transition.Name;
            }
        }

        static HashSet<long> GenerateHashSetStateId(params List<State>[] listListState)
        {
            HashSet<long> hashset = new HashSet<long>();

            foreach (var listState in listListState)
            {
                foreach (var state in listState)
                {
                    hashset.Add(state.Id);
                }
            }

            return hashset;
        }

        static List<State> GenerateNewListState_ConvertActionStateToUnityEventState(List<State> listState)
        {
            List<State> listStateConverted = new List<State>();

            foreach (var state in listState)
            {
                if (state is ActionState actionState)
                {
                    UnityEventState unityEventState = new UnityEventState();
                    listStateConverted.Add(unityEventState);
                    
                    EditorStateUtility.CopyOldStateToNewState(actionState, unityEventState);
                }
                else
                {
                    listStateConverted.Add(state);
                    continue;
                }
            }

            return listStateConverted;
        }

        static void GenerateListUnityEventStatePersistentListener(List<State> listStateNonConvert, List<SerializedProperty> listStateSpAdded)
        {
            for (int i = 0; i < listStateNonConvert.Count; i++)
            {
                var state = listStateNonConvert[i];
                if (state is ActionState actionState)
                {
                    var stateSp = listStateSpAdded[i];
                    GenerateUnityEventStatePersistentListener(actionState, stateSp);
                }
            }
        }

        static void GenerateUnityEventStatePersistentListener(ActionState actionState, SerializedProperty unityEventStateSp)
        {
            foreach (var stateEventFieldName in StateEventFieldNameDefine.list)
            {
                GenerateUnityEventPersistentListener(actionState.Id, unityEventStateSp.FindPropertyRelative(stateEventFieldName.ToString()), stateEventFieldName);
            }
        }

        static void GenerateUnityEventPersistentListener(long oldStateId, SerializedProperty unityEventSp, StateEventFieldName stateEventId)
        {
            var dictActionInvocationLength = ClipBoard.dictActionStateInvocationLength[oldStateId];
            dictActionInvocationLength.TryGetValue(stateEventId, out int invocationListLength);
            
            if(invocationListLength == 0)
                return;

            unityEventSp.AsUnityEvent().AddPersistentListener();
        }
        
        #region Utility

        static void ChangeListStateToNewPos(Vector2 newPos, params List<State>[] listListState)
        {
            Vector2 center = CalculateListStateCenterPos(listListState);
            Vector2 delta = newPos - center;

            foreach (var listState in listListState)
            {
                foreach (var state in listState)
                {
                    state.Position += delta;
                }
            }
        }
        
        static Vector2 CalculateListStateCenterPos(params List<State>[] listListState)
        {
            Vector2 centerPos = Vector2.zero;
            int numberState = 0;

            foreach (var listState in listListState)
            {
                foreach (var state in listState)
                {
                    centerPos += state.Position;
                    numberState++;
                }
            }
            
            
            if(numberState > 0)
                centerPos /= numberState;

            return centerPos;
        }

        #endregion
    }
}