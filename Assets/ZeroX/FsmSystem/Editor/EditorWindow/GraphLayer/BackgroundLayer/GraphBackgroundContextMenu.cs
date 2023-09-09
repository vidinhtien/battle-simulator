using System;
using UnityEditor;
using UnityEngine;

namespace ZeroX.FsmSystem.Editors
{
    public class GraphBackgroundContextMenu
    {
        private StateMachineGraphWindow EditorWindow { get; set; }
        private StateMachineGraphWindowContext Context => EditorWindow.Context;
        
        public GraphBackgroundContextMenu(StateMachineGraphWindow editorWindow)
        {
            this.EditorWindow = editorWindow;
        }
        
        private Vector2 GetStatePosition(Vector2 mousePosition)
        {
            return TransformationMatrixUtility.AreaPointToWorldPoint(EditorWindow.Rect, Context.DragOffset, Context.ZoomFactor, mousePosition);
        }
        
        public void Show(Vector2 mousePosition)
        {
            Vector2 statePosition = GetStatePosition(mousePosition);
            
            var genericMenu = new GenericMenu();

            //Normal State
            foreach (var kv in EditorStateUtility.dictStateTypeBuiltIn)
            {
                genericMenu.AddItem(new GUIContent("Create " + kv.Key), false, () => CreateNormalState(kv.Value, statePosition));
            }
            
            //Any state
            genericMenu.AddSeparator("");
            genericMenu.AddItem(new GUIContent("Create Any State"), false, () => CreateAnyState(typeof(AnyState), statePosition));
            genericMenu.AddSeparator("");
            
            //Parallel State
            foreach (var kv in EditorStateUtility.dictStateTypeBuiltIn)
            {
                genericMenu.AddItem(new GUIContent("Parallel/Create " + kv.Key), false, () => CreateParallelState(kv.Value, statePosition));
            }
            
            //Paste
            genericMenu.AddSeparator("");
            if (CopyPasteUtility.HasDataToCopy == false)
            {
                genericMenu.AddDisabledItem(new GUIContent("Paste"));
            }
            else
            {
                genericMenu.AddItem(new GUIContent("Paste"), false, () => PasteAllStateCopied(mousePosition));
                genericMenu.AddItem(new GUIContent("Paste & Convert ActionState to UnityEventState"), false, () => PasteAllStateCopied_ConvertActionStateToUnityEventState(mousePosition));
                
            }
                
            
            genericMenu.ShowAsContext();
        }

        void PasteAllStateCopied(Vector2 mousePosition)
        {
            Vector2 worldPoint = GetStatePosition(mousePosition);
            Context.EnqueueEditAction(() =>
            {
                var hashSetStatePasted = CopyPasteUtility.PasteStates(Context, worldPoint);
                Context.GraphSp.AsStateMachineGraph().CheckAndSetGraphEntryStateId(Context);

                //Select các state vừa tạo
                Context.UnSelectAllTransition();
                Context.UnSelectAllState();
                foreach (var stateId in hashSetStatePasted)
                {
                    Context.SelectState(stateId);
                }
                    
                EditorWindow.InspectSelected();
            });
        }
        
        void PasteAllStateCopied_ConvertActionStateToUnityEventState(Vector2 mousePosition)
        {
            Vector2 worldPoint = GetStatePosition(mousePosition);
            Context.EnqueueEditAction(() =>
            {
                var hashSetStatePasted = CopyPasteUtility.PasteStates_ConvertActionStateToUnityEventState(Context, worldPoint);
                Context.GraphSp.AsStateMachineGraph().CheckAndSetGraphEntryStateId(Context);

                //Select các state vừa tạo
                Context.UnSelectAllTransition();
                Context.UnSelectAllState();
                foreach (var stateId in hashSetStatePasted)
                {
                    Context.SelectState(stateId);
                }
                    
                EditorWindow.InspectSelected();
            });
        }


        #region Create State

        void InspectStateJustCreate(SerializedProperty stateSp)
        {
            long stateId = stateSp.AsState().Id;
            
            Context.UnSelectAllTransition();
            Context.SelectOnlyOneState(stateId);
            EditorWindow.InspectSelected();
        }

        void CreateNormalState(Type stateType, Vector2 position)
        {
            Context.EnqueueEditAction(() =>
            {
                var stateSp = Context.GraphSp.AsStateMachineGraph().AddNormalState(Context, stateType, position);
                InspectStateJustCreate(stateSp);
            });
        }
        
        void CreateParallelState(Type stateType, Vector2 position)
        {
            Context.EnqueueEditAction(() =>
            {
                var stateSp = Context.GraphSp.AsStateMachineGraph().AddParallelState(Context, stateType, position);
                InspectStateJustCreate(stateSp);
            });
        }
        
        void CreateAnyState(Type stateType, Vector2 position)
        {
            Context.EnqueueEditAction(() =>
            {
                var stateSp = Context.GraphSp.AsStateMachineGraph().AddAnyState(Context, stateType, position);
                InspectStateJustCreate(stateSp);
            });
        }

        #endregion
    }
}