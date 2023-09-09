using System.Reflection;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ZeroX.FsmSystem.Editors
{
    public static class EditorFsmGraphUtility
    {
        private static bool isInitialized = false;

        private static BindingFlags bindingFlags = BindingFlags.Default | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        
        private static FieldInfo listTransitionFi;
        
        private static FieldInfo listNormalStateFi;
        private static FieldInfo listParallelStateFi;
        private static FieldInfo listAnyStateFi;
        private static FieldInfo entryStateIdFi;

        private static HashSet<int> hashSetNormalStateIndexWaitDeleteByError = new HashSet<int>();
        private static HashSet<int> hashSetAnyStateIndexWaitDeleteByError = new HashSet<int>();
        private static HashSet<int> hashSetParallelStateIndexWaitDeleteByError = new HashSet<int>();
        
        private static HashSet<int> hashSetTransitionIndexWaitDeleteByError = new HashSet<int>();

        public static void InitIfNot()
        {
            if(isInitialized)
                return;
            isInitialized = true;

            Type fsmGraphType = typeof(FsmGraph);
            
            listTransitionFi = fsmGraphType.GetField(FsmGraph.fn_listTransition, bindingFlags);
            
            listNormalStateFi = fsmGraphType.GetField(FsmGraph.fn_listNormalState, bindingFlags);
            listAnyStateFi = fsmGraphType.GetField(FsmGraph.fn_listAnyState, bindingFlags);
            listParallelStateFi = fsmGraphType.GetField(FsmGraph.fn_listParallelState, bindingFlags);
         
            entryStateIdFi = fsmGraphType.GetField(FsmGraph.fn_entryStateId, bindingFlags);
        }

        public static List<Transition> GetListTransition(FsmGraph fsmGraph)
        {
            return (List<Transition>) listTransitionFi.GetValue(fsmGraph);
        }
        
        public static List<State> GetListNormalState(FsmGraph fsmGraph)
        {
            return (List<State>) listNormalStateFi.GetValue(fsmGraph);
        }
        
        public static List<State> GetListAnyState(FsmGraph fsmGraph)
        {
            return (List<State>) listAnyStateFi.GetValue(fsmGraph);
        }
        
        public static List<State> GetListParallelState(FsmGraph fsmGraph)
        {
            return (List<State>) listParallelStateFi.GetValue(fsmGraph);
        }


        public static Dictionary<long, Transition> GetDictTransition(FsmGraph fsmGraph)
        {
            var listTransition = GetListTransition(fsmGraph);
            Dictionary<long, Transition> dict = new Dictionary<long, Transition>();
            
            foreach (var transition in listTransition)
            {
                dict.Add(transition.Id, transition);
            }
            
            return dict;
        }
        
        public static Dictionary<long, List<Transition>> GetDictStateTransition(FsmGraph fsmGraph)
        {
            var listTransition = GetListTransition(fsmGraph);
            Dictionary<long, List<Transition>> dict = new Dictionary<long, List<Transition>>();
            
            foreach (var transition in listTransition)
            {
                if (dict.TryGetValue(transition.OriginId, out var listTransitionOfState) == false)
                {
                    listTransitionOfState = new List<Transition>();
                    dict.Add(transition.OriginId, listTransitionOfState);
                }
                
                listTransitionOfState.Add(transition);
            }
            
            return dict;
        }
        
        public static Dictionary<long, State> GetDictNormalState(FsmGraph fsmGraph)
        {
            var listState = GetListNormalState(fsmGraph);
            Dictionary<long, State> dict = new Dictionary<long, State>();
            
            foreach (var state in listState)
            {
                dict.Add(state.Id, state);
            }
            
            return dict;
        }
        
        public static Dictionary<long, State> GetDictAnyState(FsmGraph fsmGraph)
        {
            var listState = GetListAnyState(fsmGraph);
            Dictionary<long, State> dict = new Dictionary<long, State>();
            
            foreach (var state in listState)
            {
                dict.Add(state.Id, state);
            }
            
            return dict;
        }
        
        public static Dictionary<long, State> GetDictParallelState(FsmGraph fsmGraph)
        {
            var listState = GetListParallelState(fsmGraph);
            Dictionary<long, State> dict = new Dictionary<long, State>();
            
            foreach (var state in listState)
            {
                dict.Add(state.Id, state);
            }
            
            return dict;
        }


        public static long GetEntryStateId(FsmGraph fsmGraph)
        {
            return (long) entryStateIdFi.GetValue(fsmGraph);
        }

        public static State GetStateById(StateMachineGraphContext context, long stateId)
        {
            foreach (var dictState in context.listDictState)
            {
                if (dictState.TryGetValue(stateId, out var state))
                    return state;
            }

            return null;
        }

        /// <summary>
        /// If true, graph has error
        /// </summary>
        /// <returns></returns>
        public static bool CheckAndFixIfGraphHasError(StateMachineGraphWindowContext context)
        {
            InitIfNot();
            
            bool hasError = false;

            HashSet<long> hashSetStateId = new HashSet<long>();
            
            var listNormalState = GetListNormalState(context.GraphObject);
            if (DeleteAllStateNullAndDuplicatedId(context, listNormalState, context.ListNormalStateSp, hashSetStateId, hashSetNormalStateIndexWaitDeleteByError))
                hasError = true;
            
            var listAnyState = GetListAnyState(context.GraphObject);
            if (DeleteAllStateNullAndDuplicatedId(context, listAnyState, context.ListAnyStateSp, hashSetStateId, hashSetAnyStateIndexWaitDeleteByError))
                hasError = true;
            
            var listParallelState = GetListParallelState(context.GraphObject);
            if (DeleteAllStateNullAndDuplicatedId(context, listParallelState, context.ListParallelStateSp, hashSetStateId, hashSetParallelStateIndexWaitDeleteByError))
                hasError = true;

            if (DeleteAllTransitionUndefinedAndDuplicatedId(context))
                hasError = true;
            
            return hasError;
        }

        static bool DeleteAllStateNullAndDuplicatedId(StateMachineGraphWindowContext context, List<State> listState, SerializedProperty listStateSp, HashSet<long> hashSetStateId, HashSet<int> hashSetStateIndexWaitDeleteByError)
        {
            bool hasError = false;
            
            for (int i = 0; i < listState.Count; i++)
            {
                var state = listState[i];

                //Check null
                if (state == null)
                {
                    if (hashSetStateIndexWaitDeleteByError.Count == 0 &&
                        hashSetStateIndexWaitDeleteByError.Add(i)) //Thêm count để chỉ cho phép xóa 1 element mỗi lần execute để tránh lỗi sai index của các element sau khi arraySize giảm xuống
                    {
                        int index = i;
                        context.EnqueueEditAction(() =>
                        {
                            listStateSp.DeleteArrayElementAtIndex(index);
                            hashSetStateIndexWaitDeleteByError.Remove(index);
                        });
                        Debug.LogWarningFormat("Delete state null at index: " + i);
                    }

                    hasError = true;
                    continue;
                }

                //Check duplicated
                if (hashSetStateId.Add(state.Id) == false)
                {
                    if (hashSetStateIndexWaitDeleteByError.Count == 0 &&
                        hashSetStateIndexWaitDeleteByError.Add(i))
                    {
                        int index = i;
                        context.EnqueueEditAction(() =>
                        {
                            listStateSp.DeleteArrayElementAtIndex(index);
                            hashSetStateIndexWaitDeleteByError.Remove(index);
                        });
                        Debug.LogWarningFormat("Delete duplicated state id at index: " + i);
                    }

                    hasError = true;
                    continue;
                }
            }

            return hasError;
        }

        static bool DeleteAllTransitionUndefinedAndDuplicatedId(StateMachineGraphWindowContext context)
        {
            bool hasError = false;
            
            HashSet<long> hashSetTransitionId = new HashSet<long>();
            var listTransition = GetListTransition(context.GraphObject);

            for (int i = 0; i < listTransition.Count; i++)
            {
                var transition = listTransition[i];

                //Check undefined id
                if (transition.Id == Transition.undefinedId ||
                    transition.OriginId == State.emptyId ||
                    transition.OriginId == State.undefinedId)
                {
                    if (hashSetTransitionIndexWaitDeleteByError.Count == 0 &&
                        hashSetTransitionIndexWaitDeleteByError.Add(i))
                    {
                        int index = i;
                        context.EnqueueEditAction(() =>
                        {
                            context.ListTransitionSp.DeleteArrayElementAtIndex(index);
                            hashSetTransitionIndexWaitDeleteByError.Remove(index);
                        });
                        Debug.LogWarningFormat("Delete transition undefined at index: " + i);
                    }

                    hasError = true;
                    continue;
                }

                //Check Duplicated
                if (hashSetTransitionId.Add(transition.Id) == false)
                {
                    if (hashSetTransitionIndexWaitDeleteByError.Count == 0 &&
                        hashSetTransitionIndexWaitDeleteByError.Add(i))
                    {
                        int index = i;
                        context.EnqueueEditAction(() =>
                        {
                            context.ListTransitionSp.DeleteArrayElementAtIndex(index);
                            hashSetTransitionIndexWaitDeleteByError.Remove(index);
                        });
                        Debug.LogWarningFormat("Delete duplicated transition id at index: " + i);
                    }

                    hasError = true;
                    continue;
                }
            }

            return hasError;
        }
    }
}