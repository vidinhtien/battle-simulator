using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ZeroX.FsmSystem.Editors
{
    public static class StateMachineGraphSpExtension
    {
        private static readonly StateMachineGraphSpHandler handler = new StateMachineGraphSpHandler();
        public static StateMachineGraphSpHandler AsStateMachineGraph(this SerializedProperty serializedProperty)
        {
            handler.serializedProperty = serializedProperty;
            return handler;
        }
    }
    
    public class StateMachineGraphSpHandler
    {
        public SerializedProperty serializedProperty;

        public SerializedProperty ListTransitionSp => serializedProperty.FindPropertyRelative(FsmGraph.fn_listTransition);
        
        public SerializedProperty ListNormalStateSp => serializedProperty.FindPropertyRelative(FsmGraph.fn_listNormalState);
        public SerializedProperty ListAnyStateSp => serializedProperty.FindPropertyRelative(FsmGraph.fn_listAnyState);
        public SerializedProperty ListParallelStateSp => serializedProperty.FindPropertyRelative(FsmGraph.fn_listParallelState);
        
        public SerializedProperty StateIdSeqSp => serializedProperty.FindPropertyRelative(FsmGraph.fn_stateIdSeq);
        public long StateIdSeq
        {
            get => StateIdSeqSp.longValue;
            set => StateIdSeqSp.longValue = value;
        }
        
        public SerializedProperty TransitionIdSeqSp => serializedProperty.FindPropertyRelative(FsmGraph.fn_transitionIdSeq);
        public long TransitionIdSeq
        {
            get => TransitionIdSeqSp.longValue;
            set => TransitionIdSeqSp.longValue = value;
        }
        
        public SerializedProperty EntryStateIdSp => serializedProperty.FindPropertyRelative(FsmGraph.fn_entryStateId);
        public long EntryStateId
        {
            get => EntryStateIdSp.longValue;
            set => EntryStateIdSp.longValue = value;
        }

        public long GetCurrentStateId(StateMachineGraphContext context)
        {
            //Hiện tại chưa làm
            //Sau sẽ get object và lấy khi runtime
            var currentState = context.GraphObject.CurrentState;
            return currentState == null ? State.emptyId : currentState.Id;
        }
        
        public void CheckAndSetGraphEntryStateId(StateMachineGraphContext context)
        {
            if (context.ListNormalStateSp.arraySize == 0)
            {
                EntryStateId = State.emptyId;
                return;
            }

            bool hasEntryState = false;
            var entryStateIdSp = EntryStateIdSp;
            for (int i = 0; i < context.ListNormalStateSp.arraySize; i++)
            {
                var stateSp = context.ListNormalStateSp.GetArrayElementAtIndex(i);
                if (stateSp.AsState().Id == entryStateIdSp.longValue)
                {
                    hasEntryState = true;
                    break;
                }
            }

            if (hasEntryState == false)
            {
                var firstStateSp = context.ListNormalStateSp.GetArrayElementAtIndex(0);
                entryStateIdSp.longValue = firstStateSp.AsState().Id;
            }
        }

        public IEnumerable<SerializedProperty> GetAllState(StateMachineGraphContext context)
        {
            for (int i = 0; i < context.ListNormalStateSp.arraySize; i++)
            {
                yield return context.ListNormalStateSp.GetArrayElementAtIndex(i);
            }
            
            for (int i = 0; i < context.ListAnyStateSp.arraySize; i++)
            {
                yield return context.ListAnyStateSp.GetArrayElementAtIndex(i);
            }
            
            for (int i = 0; i < context.ListParallelStateSp.arraySize; i++)
            {
                yield return context.ListParallelStateSp.GetArrayElementAtIndex(i);
            }
        }

        private bool IsStateInList(SerializedProperty listStateSp, long stateId)
        {
            for (int i = 0; i < listStateSp.arraySize; i++)
            {
                var stateSp = listStateSp.GetArrayElementAtIndex(i);
                if (stateSp.AsState().Id == stateId)
                    return true;
            }

            return false;
        }
        
        public bool IsNormalState(StateMachineGraphContext context, long stateId)
        {
            return IsStateInList(context.ListNormalStateSp, stateId);
        }
        
        public bool IsAnyState(StateMachineGraphContext context, long stateId)
        {
            return IsStateInList(context.ListAnyStateSp, stateId);
        }
        
        public bool IsParallelState(StateMachineGraphContext context, long stateId)
        {
            return IsStateInList(context.ListParallelStateSp, stateId);
        }
        
        bool CheckGraphHasStateId(StateMachineGraphContext context, long stateId)
        {
            var listListStateSp = GetListListState(context);
            foreach (var listStateSp in listListStateSp)
            {
                for (int i = 0; i < listStateSp.arraySize; i++)
                {
                    var stateSp = listStateSp.GetArrayElementAtIndex(i);
                    if (stateSp.AsState().Id == stateId)
                        return true;
                }
            }

            return false;
        }

        
        #region Get State

        public SerializedProperty GetStateById(StateMachineGraphContext context, long stateId)
        {
            var listAll = GetAllState(context);
            foreach (var stateSp in listAll)
            {
                if (stateSp.AsState().Id == stateId)
                    return stateSp;
            }

            return null;
        }

        public List<SerializedProperty> GetListListState(StateMachineGraphContext context)
        {
            List<SerializedProperty> list = new List<SerializedProperty>();
            
            list.Add(context.ListNormalStateSp);
            list.Add(context.ListAnyStateSp);
            list.Add(context.ListParallelStateSp);

            return list;
        }
        
        // public List<SerializedProperty> GetListListStateExcludeListAny(StateMachineGraphContext context)
        // {
        //     List<SerializedProperty> list = new List<SerializedProperty>();
        //     
        //     list.Add(context.ListNormalStateSp);
        //     list.Add(context.ListParallelStateSp);
        //
        //     return list;
        // }

        #endregion

        #region Create State
        
        private bool HasStateWithName(StateMachineGraphContext context, string stateName)
        {
            var listListState = GetListListState(context);
            
            foreach (var listStateSp in listListState)
            {
                for (int i = 0; i < listStateSp.arraySize; i++)
                {
                    var stateSp = listStateSp.GetArrayElementAtIndex(i);
                    if (stateSp.AsState().Name == stateName)
                        return true;
                }
            }

            return false;
        }

        string GenerateNewStateName(StateMachineGraphContext context)
        {
            string newStateName = "state 1";
            int index = 1;
            while (HasStateWithName(context, newStateName))
            {
                index++;
                newStateName = "state " + index;
            }

            return newStateName;
        }

        string GenerateNewAnyStateName(StateMachineGraphContext context)
        {
            string newStateName = "any state 1";
            int index = 1;
            while (HasStateWithName(context, newStateName))
            {
                index++;
                newStateName = "any state " + index;
            }

            return newStateName;
        }
        
        private SerializedProperty AddState(StateMachineGraphContext context, SerializedProperty listStateSp, Type stateType, Vector2 position)
        {
            if (stateType.IsSubclassOf(typeof(State)) == false)
            {
                Debug.LogErrorFormat("Cannot add {0} because it does not inherit from State", stateType.Name);
                return null;
            }
            
            var stateSp = listStateSp.AsListState().AddState();
            
            //Tăng StateIdSeq
            var stateIdSeqSp = StateIdSeqSp;
            stateIdSeqSp.longValue++;

            //Fill các data cho State
            stateSp.managedReferenceValue = Activator.CreateInstance(stateType);
            stateSp.AsState().Id = stateIdSeqSp.longValue;
            stateSp.AsState().Position = position;
            
            string newStateName = GenerateNewStateName(context);
            stateSp.AsState().Name = newStateName;
            
            CheckAndSetGraphEntryStateId(context);
            
            return stateSp;
        }

        private SerializedProperty AddState<T>(StateMachineGraphContext context, SerializedProperty listStateSp, Vector2 position) where T : State
        {
            return AddState(context, listStateSp, typeof(T), position);
        }

        
        
        public SerializedProperty AddNormalState(StateMachineGraphContext context, Type stateType, Vector2 position)
        {
            return AddState(context, context.ListNormalStateSp, stateType, position);
        }
        
        public SerializedProperty AddNormalState<T>(StateMachineGraphContext context, Vector2 position) where T : State
        {
            return AddState<T>(context, context.ListNormalStateSp, position);
        }
        
        
        
        public SerializedProperty AddAnyState(StateMachineGraphContext context, Type stateType, Vector2 position)
        {
            if (stateType != typeof(AnyState) && stateType.IsSubclassOf(typeof(AnyState)) == false)
            {
                Debug.LogErrorFormat("Cannot add {0} because it does not inherit from AnyState", stateType.Name);
                return null;
            }
            
            var stateSp = context.ListAnyStateSp.AsListState().AddState();
            
            //Tăng StateIdSeq
            var stateIdSeqSp = StateIdSeqSp;
            stateIdSeqSp.longValue++;

            //Fill các data cho State
            stateSp.managedReferenceValue = Activator.CreateInstance(stateType);
            stateSp.AsState().Id = stateIdSeqSp.longValue;
            stateSp.AsState().Position = position;

            string newStateName = GenerateNewAnyStateName(context);
            stateSp.AsState().Name = newStateName;

            return stateSp;
        }
        
        public SerializedProperty AddAnyState<T>(StateMachineGraphContext context, Vector2 position) where T : State
        {
            return AddAnyState(context, typeof(T), position);
        }
        
        
        
        public SerializedProperty AddParallelState(StateMachineGraphContext context, Type stateType, Vector2 position)
        {
            return AddState(context, context.ListParallelStateSp, stateType, position);
        }
        
        public SerializedProperty AddParallelState<T>(StateMachineGraphContext context, Vector2 position) where T : State
        {
            return AddState<T>(context, context.ListParallelStateSp, position);
        }

        #endregion

        #region Delete State

        public void DeleteStates(StateMachineGraphContext context, List<long> listStateId)
        {
            var listListStateSp = GetListListState(context);

            foreach (var stateIdWantDelete in listStateId)
            {
                foreach (var listStateSp in listListStateSp)
                {
                    if(listStateSp.AsListState().DeleteStateById(stateIdWantDelete))
                        break;
                }
            }
            
            CheckAndFixMissingTransitionOriginAndTarget(context);
            CheckAndSetGraphEntryStateId(context);
        }

        #endregion

        #region State Validate

        public HashSet<string> GetListStateDuplicateName(StateMachineGraphContext context)
        {
            var listListState = GetListListState(context);
            HashSet<string> listName = new HashSet<string>();
            HashSet<string> listNameDuplicate = new HashSet<string>();

            foreach (var listStateSp in listListState)
            {
                for (int i = 0; i < listStateSp.arraySize; i++)
                {
                    var stateSp = listStateSp.GetArrayElementAtIndex(i);
                    string stateName = stateSp.AsState().Name;
                    if (listName.Add(stateName) == false)
                    {
                        listNameDuplicate.Add(stateName);
                    }
                }
            }

            return listNameDuplicate;
        }

        #endregion

        #region Transition

        public SerializedProperty GetTransitionById(StateMachineGraphContext context, long transitionId)
        {
            for (int i = 0; i < context.ListTransitionSp.arraySize; i++)
            {
                var transitionSp = context.ListTransitionSp.GetArrayElementAtIndex(i);
                if (transitionSp.AsTransition().Id == transitionId)
                    return transitionSp;
            }

            return null;
        }
        
        private bool HasTransitionWithNameInState(StateMachineGraphContext context, long originStateId, string triggerId)
        {
            for (int i = 0; i < context.ListTransitionSp.arraySize; i++)
            {
                var transitionSp = context.ListTransitionSp.GetArrayElementAtIndex(i);
                if (transitionSp.AsTransition().OriginId == originStateId)
                {
                    if (transitionSp.AsTransition().Name == triggerId)
                        return true;
                }
            }

            return false;
        }

        private bool HasAnyTransitionWithName(StateMachineGraphContext context, string transitionName)
        {
            for (int i = 0; i < context.ListTransitionSp.arraySize; i++)
            {
                var transitionSp = context.ListTransitionSp.GetArrayElementAtIndex(i);
                if (IsAnyState(context, transitionSp.AsTransition().OriginId))
                {
                    if (transitionSp.AsTransition().Name == transitionName)
                        return true;
                }
            }

            return false;
        }

        private string GenerateNewTransitionName(StateMachineGraphContext context, long originStateId)
        {
            bool isAnyTransition = IsAnyState(context, originStateId);

            if (isAnyTransition)
            {
                int triggerIdIndex = 1;
                string newTriggerId = "New Any Transition 1";
                
                while (HasAnyTransitionWithName(context, newTriggerId))
                {
                    triggerIdIndex++;
                    newTriggerId = "New Any Transition " + triggerIdIndex;
                }
                return newTriggerId;
            }
            else
            {
                int triggerIdIndex = 1;
                string newTriggerId = "New Transition 1";
                
                while (HasTransitionWithNameInState(context, originStateId, newTriggerId))
                {
                    triggerIdIndex++;
                    newTriggerId = "New Transition " + triggerIdIndex;
                }
                return newTriggerId;
            }
        }
        
        private void CheckAndFixMissingTransitionOriginAndTarget(StateMachineGraphContext context)
        {
            for (int i = 0; i < context.ListTransitionSp.arraySize; i++)
            {
                var transitionSp = context.ListTransitionSp.GetArrayElementAtIndex(i);
                
                long originId = transitionSp.FindPropertyRelative(Transition.fn_originId).longValue;
                if (CheckGraphHasStateId(context, originId) == false)
                {
                    context.ListTransitionSp.DeleteArrayElementAtIndex(i);
                    i--;
                    continue;
                }
                
                long targetId = transitionSp.FindPropertyRelative(Transition.fn_targetId).longValue;
                if (CheckGraphHasStateId(context, targetId) == false)
                {
                    transitionSp.FindPropertyRelative(Transition.fn_targetId).longValue = -1;
                    continue;
                }
            }
        }
        
        public SerializedProperty AddTransition(StateMachineGraphContext context, long originStateId)
        {
            var transitionIdSeqSp = TransitionIdSeqSp;
            
            transitionIdSeqSp.longValue++;
            long newTransitionId = transitionIdSeqSp.longValue;

            var transitionSp = context.ListTransitionSp.AsListTransition().AddNewTransition();
            transitionSp.AsTransition().Id = newTransitionId;
            
            string newTransitionName = GenerateNewTransitionName(context, originStateId);
            transitionSp.AsTransition().Name = newTransitionName;
            
            transitionSp.AsTransition().OriginId = originStateId;
            transitionSp.AsTransition().TargetId = State.emptyId;

            return transitionSp;
        }

        public void DeleteTransition(StateMachineGraphContext context, long transitionId)
        {
            for (int i = 0; i < context.ListTransitionSp.arraySize; i++)
            {
                var transitionSp = context.ListTransitionSp.GetArrayElementAtIndex(i);
                if (transitionSp.AsTransition().Id == transitionId)
                {
                    context.ListTransitionSp.DeleteArrayElementAtIndex(i);
                }
            }
        }
        
        #endregion

        #region Transition Validate

        /// <summary>
        /// Dict<stateId, HashSet<transitionName>>
        /// </summary>
        public Dictionary<long, HashSet<string>> GetDictTransitionDuplicateName(StateMachineGraphContext context)
        {
            Dictionary<long, HashSet<string>> dictName = new Dictionary<long, HashSet<string>>();
            Dictionary<long, HashSet<string>> dictNameDuplicate = new Dictionary<long, HashSet<string>>();

            for (int i = 0; i < context.ListTransitionSp.arraySize; i++)
            {
                var transitionSp = context.ListTransitionSp.GetArrayElementAtIndex(i);

                long originId = transitionSp.AsTransition().OriginId;

                if (dictName.TryGetValue(originId, out var listTransitionName) == false)
                {
                    listTransitionName = new HashSet<string>();
                    dictName.Add(originId, listTransitionName);
                }
                
                if (dictNameDuplicate.TryGetValue(originId, out var listDuplicateTransitionName) == false)
                {
                    listDuplicateTransitionName = new HashSet<string>();
                    dictNameDuplicate.Add(originId, listDuplicateTransitionName);
                }
                

                string transitionName = transitionSp.AsTransition().Name;
                if (listTransitionName.Add(transitionName) == false)
                {
                    dictNameDuplicate[originId].Add(transitionName);
                }
            }

            return dictNameDuplicate;
        }

        #endregion
    }
}