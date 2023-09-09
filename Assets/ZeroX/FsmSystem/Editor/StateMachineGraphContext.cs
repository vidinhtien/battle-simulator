using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ZeroX.FsmSystem.Editors
{
    [System.Serializable]
    public class StateMachineGraphContext
    {
        [SerializeField] private Object graphOwner;
        [SerializeField] private string graphSpPath;

        public Object GraphOwner
        {
            get => graphOwner;
            private set => graphOwner = value;
        }

        public string GraphSpPath
        {
            get => graphSpPath;
            private set => graphSpPath = value;
        }
        
        public SerializedProperty GraphSp { get; private set; }
        public FsmGraph GraphObject { get; private set; }
        public SerializedProperty ListTransitionSp { get; private set; }
        
        public SerializedProperty ListNormalStateSp { get; private set; }
        public SerializedProperty ListAnyStateSp { get; private set; }
        public SerializedProperty ListParallelStateSp { get; private set; }

        public virtual void SetGraphSp(SerializedProperty newGraphSp)
        {
            if (GraphSp != null && GraphSp.serializedObject != null)
            {
                GraphSp.serializedObject.Dispose();
                GraphSp.Dispose();
            }
            
            if (newGraphSp == null)
            {
                this.GraphOwner = null;
                this.GraphSpPath = null;
                this.GraphSp = null;
                this.GraphObject = null;

                ListTransitionSp = null;
                
                ListNormalStateSp = null;
                ListAnyStateSp = null;
                ListParallelStateSp = null;
                
                return;
            }
            
            var graphSo = new SerializedObject(newGraphSp.serializedObject.targetObject);
            this.GraphOwner = graphSo.targetObject;
            this.GraphSpPath = newGraphSp.propertyPath;
            this.GraphSp = graphSo.FindPropertyDeep(GraphSpPath);
            this.GraphObject = (FsmGraph)this.GraphSp.GetObject();

            ListTransitionSp = GraphSp.AsStateMachineGraph().ListTransitionSp;
            
            ListNormalStateSp = GraphSp.AsStateMachineGraph().ListNormalStateSp;
            ListAnyStateSp = GraphSp.AsStateMachineGraph().ListAnyStateSp;
            ListParallelStateSp = GraphSp.AsStateMachineGraph().ListParallelStateSp;
        }
        
        public void ReloadIfNeed()
        {
            bool hasGraph = GraphSp != null && GraphOwner != null;

            try
            {
                using (GraphSp.FindPropertyRelative(FsmGraph.fn_entryStateId))
                {
                }
            }
            catch (Exception)
            {
                hasGraph = false;
            }

            if (hasGraph == false)
            {
                if (GraphOwner != null && string.IsNullOrEmpty(GraphSpPath) == false)
                {
                    var graphOwnerSo = new SerializedObject(GraphOwner);
                    var graphSp = graphOwnerSo.FindPropertyDeep(GraphSpPath);
                    SetGraphSp(graphSp);
                }
                else
                {
                    GraphOwner = null;
                    GraphSp = null;
                }
            }
        }

        public IEnumerable<SerializedProperty> GetAllStateSp()
        {
            var listAll = GraphSp.AsStateMachineGraph().GetAllState(this);
            foreach (var stateSp in listAll)
            {
                yield return stateSp;
            }
        }

        #region Reference

        public HashSet<string> listDuplicatedStateName = new HashSet<string>();
        public Dictionary<long, HashSet<string>> dictDuplicatedTransitionName = new Dictionary<long, HashSet<string>>();
        
        public Dictionary<long, Transition> dictTransition;
        public Dictionary<long, List<Transition>> dictStateTransition; //StateId - List<Transition>
        
        public Dictionary<long, State> dictNormalState;
        public Dictionary<long, State> dictAnyState;
        public Dictionary<long, State> dictParallelState;
        public List<Dictionary<long, State>> listDictState = new List<Dictionary<long, State>>();

        public void PrepareReferenceData()
        {
            EditorFsmGraphUtility.InitIfNot();

            dictTransition = EditorFsmGraphUtility.GetDictTransition(GraphObject);
            dictStateTransition = EditorFsmGraphUtility.GetDictStateTransition(GraphObject);
            dictNormalState = EditorFsmGraphUtility.GetDictNormalState(GraphObject);
            dictAnyState = EditorFsmGraphUtility.GetDictAnyState(GraphObject);
            dictParallelState = EditorFsmGraphUtility.GetDictParallelState(GraphObject);
            
            listDictState.Clear();
            listDictState.Add(dictNormalState);
            listDictState.Add(dictAnyState);
            listDictState.Add(dictParallelState);

            listDuplicatedStateName = GetListStateDuplicatedName();
            dictDuplicatedTransitionName = GetDictTransitionDuplicatedName();
        }
        
        private HashSet<string> GetListStateDuplicatedName()
        {
            HashSet<string> listName = new HashSet<string>();
            HashSet<string> listNameDuplicate = new HashSet<string>();

            foreach (var dictState in listDictState)
            {
                foreach (var state in dictState.Values)
                {
                    if (listName.Add(state.Name) == false)
                        listNameDuplicate.Add(state.Name);
                }
            }

            return listNameDuplicate;
        }
        
        private Dictionary<long, HashSet<string>> GetDictTransitionDuplicatedName()
        {
            Dictionary<long, HashSet<string>> dictName = new Dictionary<long, HashSet<string>>();
            Dictionary<long, HashSet<string>> dictNameDuplicate = new Dictionary<long, HashSet<string>>();
            
            HashSet<string> listAnyTransitionName = new HashSet<string>(); //Của AnyState
            HashSet<string> listDuplicatedAnyTransitionName = new HashSet<string>(); //Của AnyState

            foreach (var transition in dictTransition.Values)
            {
                long originId = transition.OriginId;
                bool isAnyTransition = dictAnyState.ContainsKey(originId);
                
                if (dictName.TryGetValue(originId, out var listTransitionName) == false)
                {
                    if (isAnyTransition)
                        listTransitionName = listAnyTransitionName;
                    else
                        listTransitionName = new HashSet<string>();

                    dictName.Add(originId, listTransitionName);
                }
                
                if (dictNameDuplicate.TryGetValue(originId, out var listDuplicatedTransitionName) == false)
                {
                    if (isAnyTransition)
                        listDuplicatedTransitionName = listDuplicatedAnyTransitionName;
                    else
                        listDuplicatedTransitionName = new HashSet<string>();
                    
                    dictNameDuplicate.Add(originId, listDuplicatedTransitionName);
                }
                
                
                if (listTransitionName.Add(transition.Name) == false)
                {
                    dictNameDuplicate[originId].Add(transition.Name);
                }
            }

            return dictNameDuplicate;
        }

        public IEnumerable<State> GetAllState()
        {
            foreach (var dictState in listDictState)
            {
                foreach (var state in dictState.Values)
                {
                    yield return state;
                }
            }
        }
        

        #endregion
    }
}