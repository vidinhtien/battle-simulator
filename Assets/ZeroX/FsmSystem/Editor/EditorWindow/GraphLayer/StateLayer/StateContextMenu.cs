using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace ZeroX.FsmSystem.Editors
{
    public class StateContextMenu
    {
        private StateMachineGraphWindow EditorWindow { get; set; }
        private StateMachineGraphWindowContext Context => EditorWindow.Context;
        
        public StateContextMenu(StateMachineGraphWindow editorWindow)
        {
            this.EditorWindow = editorWindow;
        }

        public void Show()
        {
            int countSelectedState = Context.CountSelectedState();
            
            if(countSelectedState == 0)
                return;
            
            if (countSelectedState > 1)
            {
                ShowMultiStateSelectedContextMenu();
                return;
            }

            var selectedStateId = Context.FirstSelectedStateId;
            
            if (Context.GraphSp.AsStateMachineGraph().IsNormalState(Context, selectedStateId))
            {
                ShowNormalStateContextMenu(selectedStateId);
                return;
            }
            
            if (Context.GraphSp.AsStateMachineGraph().IsAnyState(Context, selectedStateId))
            {
                ShowAnyStateContextMenu(selectedStateId);
                return;
            }
            
            if (Context.GraphSp.AsStateMachineGraph().IsParallelState(Context, selectedStateId))
            {
                ShowParallelStateContextMenu(selectedStateId);
                return;
            }
        }

        void ShowMultiStateSelectedContextMenu()
        {
            GenericMenu genericMenu = new GenericMenu();

            if (Context.CanEditGraph)
            {
                genericMenu.AddItem(new GUIContent("Delete"), false, () => DeleteAllStateSelected());
                genericMenu.AddSeparator("");
            }
            
            genericMenu.AddItem(new GUIContent("Copy"), false, () => CopyAllStateSelected());
            
            genericMenu.ShowAsContext();
        }

        void ShowNormalStateContextMenu(long stateId)
        {
            GenericMenu genericMenu = new GenericMenu();
            if (Context.CanEditGraph)
            {
                genericMenu.AddItem(new GUIContent("Add Transition"), false, () => AddStateTransition(stateId));
                genericMenu.AddItem(new GUIContent("Set as Entry"), false, () => SetStateAsEntry(stateId));
                genericMenu.AddSeparator("");
                genericMenu.AddItem(new GUIContent("Delete"), false, () => DeleteAllStateSelected());
                genericMenu.AddSeparator("");
                AddReplaceMenuToGenericMenu(genericMenu);
                genericMenu.AddSeparator("");
            }
            
            genericMenu.AddItem(new GUIContent("Copy"), false, () => CopyAllStateSelected());
            
            genericMenu.ShowAsContext();
        }
        
        void ShowAnyStateContextMenu(long stateId)
        {
            GenericMenu genericMenu = new GenericMenu();

            if (Context.CanEditGraph)
            {
                genericMenu.AddItem(new GUIContent("Add Transition"), false, () => AddStateTransition(stateId));
                genericMenu.AddSeparator("");
                genericMenu.AddItem(new GUIContent("Delete"), false, () => DeleteAllStateSelected());
                genericMenu.AddSeparator("");
            }
            
            genericMenu.AddItem(new GUIContent("Copy"), false, () => CopyAllStateSelected());
            
            genericMenu.ShowAsContext();
        }
        
        void ShowParallelStateContextMenu(long stateId)
        {
            GenericMenu genericMenu = new GenericMenu();

            if (Context.CanEditGraph)
            {
                genericMenu.AddItem(new GUIContent("Delete"), false, () => DeleteAllStateSelected());
                genericMenu.AddSeparator("");
            }
            
            genericMenu.AddItem(new GUIContent("Copy"), false, () => CopyAllStateSelected());
            
            genericMenu.ShowAsContext();
        }

        void AddStateTransition(long stateId)
        {
            Context.EnqueueEditAction(() =>
            {
                var transitionSp = Context.GraphSp.AsStateMachineGraph().AddTransition(Context, stateId);
                long transitionId = transitionSp.AsTransition().Id;
                
                Context.SelectOnlyOneState(stateId);
                Context.SelectOnlyOneTransition(transitionId);
                EditorWindow.InspectSelected();
            });
        }

        void SetStateAsEntry(long stateId)
        {
            Context.EnqueueEditAction(() =>
            {
                Context.GraphSp.AsStateMachineGraph().EntryStateId = stateId;
            });
        }

        void DeleteAllStateSelected()
        {
            EditorWindow.InspectEmpty();
            
            var listStateIdSelected = Context.GetListSelectedState().ToList();
            Context.UnSelectAllState();
            Context.EnqueueEditAction(() =>
            {
                Context.GraphSp.AsStateMachineGraph().DeleteStates(Context, listStateIdSelected);
            });
        }

        void DuplicateAllStateSelected()
        {
            Debug.Log("Coming soon...");
        }

        void CopyAllStateSelected()
        {
            CopyPasteUtility.CopyStates(Context, Context.HashSetSelectedStateId);
        }


        #region Replace State

        void AddReplaceMenuToGenericMenu(GenericMenu genericMenu)
        {
            long stateIdSelected = Context.FirstSelectedStateId; //Menu này chỉ hiển thị cho normal state nên id này chắc chắn là của normalState
            //Normal State
            foreach (var kv in EditorStateUtility.dictStateTypeBuiltIn)
            {
                genericMenu.AddItem(new GUIContent("Replace With/" + kv.Key), false, () => ReplaceNormalState(stateIdSelected, kv.Value));
            }
        }

        void ReplaceNormalState(long stateId, Type newStateType)
        {
            Context.EnqueueEditAction(() =>
            {
                var stateSp = Context.ListNormalStateSp.AsListState().GetStateSpById(stateId);

                Dictionary<StateEventFieldName, int> dictPersistentListenerLength = null; //EventFieldName - persistentListenerLength
                
                State oldState = (State)stateSp.GetObject();
                State newState = (State)Activator.CreateInstance(newStateType);
                EditorStateUtility.CopyOldStateToNewState(oldState, newState);


                //Check nếu là replace giữa 2 UnityEventState... thì lưu lại 1 dict length để tí nữa tạo cho stateMới
                bool replaceBetweenTwoUnityEventState = oldState.GetType().Name.StartsWith("UnityEventState") && newStateType.Name.StartsWith("UnityEventState");
                if (replaceBetweenTwoUnityEventState)
                    dictPersistentListenerLength = GenerateDictPersistentListenerLength(stateSp);
                
                
                stateSp.managedReferenceValue = newState;

                
                if(replaceBetweenTwoUnityEventState)
                    GeneratePersistentListenerForNewState(stateSp, dictPersistentListenerLength);
            });
        }
        
        Dictionary<StateEventFieldName, int> GenerateDictPersistentListenerLength(SerializedProperty stateSp)
        {
            Dictionary<StateEventFieldName, int> dict = new Dictionary<StateEventFieldName, int>();
            
            foreach (var eventFieldName in StateEventFieldNameDefine.list)
            {
                var unityEventSp = stateSp.FindPropertyRelative(eventFieldName.ToString());
                if(unityEventSp == null)
                    continue;
                
                dict.Add(eventFieldName, unityEventSp.AsUnityEvent().CountPersistentListener());
            }
            
            return dict;
        }

        void GeneratePersistentListenerForNewState(SerializedProperty newStateSp, Dictionary<StateEventFieldName, int> dictPersistentListenerLength)
        {
            foreach (var kv in dictPersistentListenerLength)
            {
                if(kv.Value <= 0)
                    continue;
                
                AddPersistentListenerForState(newStateSp, kv.Key);
            }
        }

        void AddPersistentListenerForState(SerializedProperty newStateSp, StateEventFieldName unityEventFieldName)
        {
            var unityEventSp = newStateSp.FindPropertyRelative(unityEventFieldName.ToString());
            if(unityEventSp == null)
                return;
            unityEventSp.AsUnityEvent().AddPersistentListener();
        }

        #endregion
    }
}