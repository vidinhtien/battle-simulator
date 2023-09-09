using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ZeroX.FsmSystem.Editors
{
    public class GraphStateLayer_V2 : GraphLayer
    {
        enum DownedMouseType
        {
            None, DownedOnState, DownedOnTransition
        }
        
        struct TransitionBoxData
        {
            public Transition transition;
            
            public Rect transitionRect;
            public int transitionIndex;
            
            public bool hasData;

            public TransitionBoxData(Transition transition, Rect transitionRect, int transitionIndex)
            {
                this.transition = transition;
                
                this.transitionRect = transitionRect;
                this.transitionIndex = transitionIndex;
                
                this.hasData = true;
            }
        }

        class TransitionLinePreviewData
        {
            public Transition transition;
            public Rect transitionRect;
            public Vector2 endPos;
            public bool isActive;
        }
        
        
        //Fields
        private readonly StateStyles stateStyles;
        private DownedMouseType leftDownedMouseType = DownedMouseType.None;
        private DownedMouseType rightDownedMouseType = DownedMouseType.None;
        
        private readonly List<TransitionBoxData> listTransitionBoxData = new List<TransitionBoxData>();
        private TransitionLinePreviewData transitionLinePreviewData = new TransitionLinePreviewData();

        private bool listSelectedStatePosChanged = false;

        public GraphStateLayer_V2(StateMachineGraphWindow editorWindow) : base(editorWindow)
        {
            stateStyles = new StateStyles();
        }

        
        
        protected override void DrawGraphLayer(Rect rect)
        {
            this.stateStyles.ApplyZoomFactor(this.Context.ZoomFactor);
            
            
            //Stopwatch st = new Stopwatch();
            //st.Start();

            listTransitionBoxData.Clear();
            Context.PrepareReferenceData();
            DrawStates(rect);
            DrawListTransitionLine();
            DrawTransitionLinePreview();
            
            //st.Stop();
            //Debug.Log("Time: " + st.ElapsedMilliseconds);
            
            this.Context.SelectionRect.Draw();
        }



        void DrawBox(Rect rect, string text, GUIStyle style)
        {
            style.Draw(rect, text, false, true, false, false);
        }
        
        void DrawStates(Rect rect)
        {
            DrawListNormalState(rect);
            DrawListAnyState(rect);
            DrawListParallelState(rect);
        }

        
        #region Normal State
        
        void DrawListNormalState(Rect rect)
        {
            long currentStateId = Context.GraphObject.CurrentState == null ? -1 : Context.GraphObject.CurrentState.Id;
            long entryStateId = EditorFsmGraphUtility.GetEntryStateId(Context.GraphObject);

            foreach (var state in Context.dictNormalState.Values)
            {
                if(Context.IsSelectedState(state.Id))
                    DrawSelectedStateOutline(state);
                    
                DrawNormalState(state, currentStateId, entryStateId);
                DrawListTransitionBoxOfState(state);
            }
        }

        private void DrawNormalState(State state, long currentStateId, long entryStateId)
        {
            Rect stateRect = GetTransformedRect(EditorStateUtility.CalculateTitleRect(state));


            string stateName = state.Name ?? "";
            bool duplicateStateName = Context.listDuplicatedStateName.Contains(stateName);
            bool missingName = string.IsNullOrEmpty(stateName);
            
            GUIStyle style = GetNormalStateStyle(state, duplicateStateName, missingName, currentStateId, entryStateId);
            
            
            if(missingName)
                DrawBox(stateRect, "(Missing Name)", style);
            else
                DrawBox(stateRect, stateName, style);
        }
        
        private GUIStyle GetNormalStateStyle(State state, bool duplicatedStateName, bool missingName, long currentStateId, long entryStateId)
        {
            if (duplicatedStateName)
                return this.stateStyles.Get(StateStyles.Style.DuplicatedStateName);
            
            if(missingName)
                return this.stateStyles.Get(StateStyles.Style.MissingStateName);
            
            long stateId = state.Id;

            //Playmode and currentState
            if (Application.isPlaying && stateId == currentStateId)
            {
                return this.stateStyles.Get(StateStyles.Style.CurrentState);
            }
            
            //Entry State
            if (stateId == entryStateId)
            {
                return this.stateStyles.Get(StateStyles.Style.EntryState);
            }
            
            //Normal State
            return this.stateStyles.Get(StateStyles.Style.NormalState);
        }


        #endregion
        

        #region Any State

        void DrawListAnyState(Rect rect)
        {
            foreach (var state in Context.dictAnyState.Values)
            {
                if (Context.IsSelectedState(state.Id))
                    DrawSelectedStateOutline(state);
                    
                DrawAnyState(state);
                DrawListTransitionBoxOfState(state);
            }
        }
        
        private void DrawAnyState(State state)
        {
            Rect nodeRect = GetTransformedRect(EditorStateUtility.CalculateTitleRect(state));

            
            string stateName = state.Name ?? "";
            bool duplicatedStateName = Context.listDuplicatedStateName.Contains(stateName);
            bool missingName = string.IsNullOrEmpty(stateName);
            
            
            GUIStyle style;
            if (duplicatedStateName)
                style = stateStyles.Get(StateStyles.Style.DuplicatedStateName);
            else if(missingName)
                style = stateStyles.Get(StateStyles.Style.MissingStateName);
            else
                style = stateStyles.Get(StateStyles.Style.AnyState);
            
            
            if(missingName)
                DrawBox(nodeRect, "(Missing Name)", style);
            else
                DrawBox(nodeRect, stateName, style);
        }

        #endregion
        
        
        #region Parallel State

        void DrawListParallelState(Rect rect)
        {
            foreach (var state in Context.dictParallelState.Values)
            {
                if (Context.IsSelectedState(state.Id))
                    DrawSelectedStateOutline(state);

                DrawParallelState(state);
            }
        }
        
        private void DrawParallelState(State state)
        {
            Rect nodeRect = GetTransformedRect(EditorStateUtility.CalculateTitleRect(state));

            
            string stateName = state.Name ?? "";
            bool duplicatedStateName = Context.listDuplicatedStateName.Contains(stateName);
            bool missingName = string.IsNullOrEmpty(stateName);
            
            
            GUIStyle style;
            if (duplicatedStateName)
                style = stateStyles.Get(StateStyles.Style.DuplicatedStateName);
            else if(missingName)
                style = stateStyles.Get(StateStyles.Style.MissingStateName);
            else
                style = stateStyles.Get(StateStyles.Style.ParallelSate);
            
            
            if(missingName)
                DrawBox(nodeRect, "(Missing Name)", style);
            else
                DrawBox(nodeRect, stateName, style);
        }

        #endregion
        

        #region Transition Box

        private void DrawListTransitionBoxOfState(State state)
        {
            if(Context.dictStateTransition.TryGetValue(state.Id, out var listTransitionOfState) == false)
                return;

            for (int i = 0; i < listTransitionOfState.Count; i++)
            {
                DrawTransitionBoxOfState(state, listTransitionOfState[i], i);
            }
        }
        
        private void DrawTransitionBoxOfState(State state, Transition transition, int transitionIndex)
        {
            Rect transitionRect = GetTransformedRect(EditorStateUtility.CalculateTransitionRectWithIndex(state, transitionIndex));
            bool isSelected = Context.IsSelectedTransition(transition.Id);
            bool duplicatedTransitionName = Context.dictDuplicatedTransitionName[transition.OriginId].Contains(transition.Name);
            bool missingName = string.IsNullOrEmpty(transition.Name);
            
            var style = this.stateStyles.GetForTransitionBox(isSelected, duplicatedTransitionName, missingName);

            if(missingName)
                DrawBox(transitionRect, "(Missing Name)", style);
            else
                DrawBox(transitionRect, transition.Name, style);
            
            listTransitionBoxData.Add(new TransitionBoxData(transition, transitionRect, transitionIndex));
        }


        #endregion


        #region Transition Line

        private void DrawListTransitionLine()
        {
            var lastTransition = Context.GraphObject.LastTransition;
            long lastTransitionId = lastTransition == null ? Transition.emptyId : lastTransition.Id;

            TransitionBoxData? transitionBoxDataHasLastTransition = null;
            foreach (var transitionBoxData in listTransitionBoxData)
            {
                if (transitionBoxData.transition.Id == lastTransitionId)
                {
                    transitionBoxDataHasLastTransition = transitionBoxData;
                    continue;
                }
                
                DrawTransitionLine(transitionBoxData, lastTransitionId);
            }

            //Vẽ lastTransition cuối cùng để nổi lên trên
            if (transitionBoxDataHasLastTransition != null)
            {
                DrawTransitionLine(transitionBoxDataHasLastTransition.Value, lastTransitionId);
            }
        }

        void DrawTransitionLine(TransitionBoxData transitionBoxData, long lastTransitionId)
        {
            Transition transition = transitionBoxData.transition;
            
            if(transitionLinePreviewData.isActive && transition == transitionLinePreviewData.transition)
                return;
            
            long targetId = transition.TargetId;
            if(targetId == State.emptyId)
                return;
            
            if(Context.dictNormalState.TryGetValue(targetId, out var targetState) == false)
                return;

            //Chọn line color
            Color lineColor;
            if (Context.IsSelectedState(transition.OriginId))
            {
                if(Context.FirstSelectedTransitionId == Transition.emptyId) //Nếu chỉ chọn state thì tất cả line đều selected
                    lineColor = GraphColors.TransitionLineColor_Selected;
                else if(Context.IsSelectedTransition(transition.Id)) //Nếu chọn riêng 1 transition thì chỉ line đó đc selected
                    lineColor = GraphColors.TransitionLineColor_Selected;
                else if (transition.Id == lastTransitionId)
                    lineColor = GraphColors.TransitionLineColor_Last;
                else
                    lineColor = GraphColors.TransitionLineColor;
            }
            else
            {
                if (transition.Id == lastTransitionId)
                    lineColor = GraphColors.TransitionLineColor_Last;
                else
                    lineColor = GraphColors.TransitionLineColor;
            }

            //Draw
            Rect targetRect = GetTransformedRect(EditorStateUtility.CalculateFullRect(Context, targetState));
            LineDrawer.DrawParabola(transitionBoxData.transitionRect, targetRect, lineColor, Context.ZoomFactor);
        }
        
        private void DrawTransitionLinePreview()
        {
            if(transitionLinePreviewData.isActive == false)
                return;
            
            LineDrawer.DrawParabola(transitionLinePreviewData.transitionRect, transitionLinePreviewData.endPos, GraphColors.TransitionLineColor, Context.ZoomFactor);
        }

        #endregion

        
        private void DrawSelectedStateOutline(State state)
        {
            Rect stateFullRectOutline = GetTransformedRect(EditorStateUtility.CalculateFullRectOutline(Context, state));
            DrawBox(stateFullRectOutline, "", stateStyles.Get(StateStyles.Style.SelectedStateOutline));
        }
        
        public State GetClickedState(Vector2 mousePos)
        {
            for (int i = Context.listDictState.Count - 1; i >= 0; i--)
            {
                var dictState = Context.listDictState[i];

                var listStateReverse = dictState.Values.Reverse();
                foreach (var state in listStateReverse)
                {
                    Rect transformedRect = GetTransformedRect(EditorStateUtility.CalculateTitleRect(state));

                    if (transformedRect.Contains(mousePos))
                    {
                        return state;
                    }
                }
            }
            
            return null;
        }

        private TransitionBoxData? GetClickedTransitionBox(Vector2 mousePos)
        {
            foreach (var transitionBoxData in listTransitionBoxData)
            {
                if (transitionBoxData.transitionRect.Contains(mousePos))
                {
                    return transitionBoxData;
                }
                    
            }

            return null;
        }



        
        
        #region Left Mouse Down

        protected override void OnLeftMouseDown(Vector2 position)
        {
            SaveListSelectedStatePosIfChanged();
            listSelectedStatePosChanged = false;
            
            
            
            var clickedState = GetClickedState(position);
            if (clickedState != null)
            {
                OnLeftClickedState(clickedState);
                return;
            }

            var clickedTransitionBoxData = GetClickedTransitionBox(position);
            if (clickedTransitionBoxData != null)
            {
                OnLeftClickedTransitionBox(clickedTransitionBoxData.Value);
                return;
            }
        }

        void OnLeftClickedState(State state)
        {
            leftDownedMouseType = DownedMouseType.DownedOnState;
            long stateId = state.Id;
            this.Context.UnSelectAllTransition();
            
            if (Event.current.control || Event.current.command)
            {
                if (this.Context.IsSelectedState(stateId))
                {
                    this.Context.UnSelectState(stateId);
                }
                else
                {
                    this.Context.SelectState(stateId);
                }
            }
            else if (Context.CountSelectedState() <= 1 || Context.IsSelectedState(stateId) == false)
            {
                this.Context.SelectOnlyOneState(stateId);
            }

            EditorWindow.InspectSelected();
            
            Event.current.Use();
        }

        void OnLeftClickedTransitionBox(TransitionBoxData transitionBoxData)
        {
            leftDownedMouseType = DownedMouseType.DownedOnTransition;
            
            Transition transition = transitionBoxData.transition;
            long originStateId = transition.OriginId;
            long transitionId = transition.Id;

            transitionLinePreviewData.isActive = false;
            transitionLinePreviewData.transition = transition;
            transitionLinePreviewData.transitionRect = transitionBoxData.transitionRect;
            
            
            this.Context.SelectOnlyOneState(originStateId);
            this.Context.SelectOnlyOneTransition(transitionId);
            EditorWindow.InspectSelected();

            Event.current.Use();
        }

        #endregion
        
        #region Left Mouse Drag

        protected override void OnLeftMouseDrag(Vector2 position)
        {
            if (leftDownedMouseType == DownedMouseType.DownedOnState)
            {
                OnLeftMouseDragWhenDownedOnState(position);
                return;
            }

            if (leftDownedMouseType == DownedMouseType.DownedOnTransition)
            {
                OnLeftMouseDragWhenDownedOnTransition(position);
                return;
            }
        }

        void OnLeftMouseDragWhenDownedOnState(Vector2 position)
        {
            Vector2 dragDelta = new Vector2(Event.current.delta.x, Event.current.delta.y) / Context.ZoomFactor;

            var listSelectedState = Context.GetListSelectedState();
            foreach (var stateId in listSelectedState)
            {
                foreach (var dictState in Context.listDictState)
                {
                    if (dictState.TryGetValue(stateId, out var state) == false)
                        continue;

                    state.Position += dragDelta;
                    listSelectedStatePosChanged = true;
                    break;
                }
            }

            Event.current.Use();
        }
        
        void OnLeftMouseDragWhenDownedOnTransition(Vector2 position)
        {
            Event.current.Use();
            
            if(Context.CannotEditGraph)
                return;
            
            long selectedTransitionId = Context.FirstSelectedTransitionId;
            var transitionBoxData = listTransitionBoxData.FirstOrDefault(d => d.transition.Id == selectedTransitionId);
            if (transitionBoxData.hasData == false)
            {
                transitionLinePreviewData.isActive = false;
                return;
            }

            if (transitionBoxData.transitionRect.Contains(position))
            {
                transitionLinePreviewData.isActive = false;
                return;
            }

            //Khi move ra ngoài transition box mới bắt đầu thực hiện
            transitionLinePreviewData.isActive = true;
            transitionLinePreviewData.endPos = position;
        }

        #endregion
        
        #region Left Mouse Up

        protected override void OnLeftMouseUp(Vector2 position)
        {
            //Cần handle xử lý set lại vào sp khi có listSelectedStatePosChanged
            SaveListSelectedStatePosIfChanged();
            
            if (leftDownedMouseType == DownedMouseType.None)
            {
                return;
            }
            
            if (leftDownedMouseType == DownedMouseType.DownedOnState)
            {
                OnLeftMouseUpWhenDownedOnState(position);
                return;
            }

            if (leftDownedMouseType == DownedMouseType.DownedOnTransition)
            {
                OnLeftMouseUpWhenDownedOnTransition(position);
                return;
            }
        }

        void OnLeftMouseUpWhenDownedOnState(Vector2 position)
        {
            leftDownedMouseType = DownedMouseType.None;
            Event.current.Use();
        }
        
        void OnLeftMouseUpWhenDownedOnTransition(Vector2 position)
        {
            leftDownedMouseType = DownedMouseType.None;
            Event.current.Use();

            if(transitionLinePreviewData.isActive == false)
                return;
            transitionLinePreviewData.isActive = false;
            
            //Nếu transitionLinePreview active thì chắc chắn đã move chuột ra ngoài transition box đó
            
            var state = GetClickedState(position);
            if (state == null)
            {
                var transitionSp = Context.GraphSp.AsStateMachineGraph().GetTransitionById(Context, transitionLinePreviewData.transition.Id);
                transitionSp.AsTransition().TargetId = Transition.emptyId;
            }
            else
            {
                if(EditorStateUtility.IsNormalState(Context, state) == false)
                    return;
            
                var transitionSp = Context.GraphSp.AsStateMachineGraph().GetTransitionById(Context, transitionLinePreviewData.transition.Id);
                transitionSp.AsTransition().TargetId = state.Id;
            }
        }

        void SaveListSelectedStatePosIfChanged()
        {
            if(listSelectedStatePosChanged == false)
                return;
            listSelectedStatePosChanged = false;

            var listAllStateSp = Context.GetAllStateSp();
            foreach (var stateSp in listAllStateSp)
            {
                long stateId = stateSp.AsState().Id;

                if (Context.IsSelectedState(stateId) == false)
                    continue;

                State state = EditorFsmGraphUtility.GetStateById(Context, stateId);
                if (state == null)
                    continue;
                    
                stateSp.AsState().Position = state.Position + new Vector2(0.01f, 0.01f);
            }
        }

        #endregion
        
        #region Right Mouse Down

        protected override void OnRightMouseDown(Vector2 position)
        {
            var clickedState = GetClickedState(position);
            if (clickedState != null)
            {
                OnRightClickedState(clickedState);
                return;
            }

            var clickedTransitionBoxData = GetClickedTransitionBox(position);
            if (clickedTransitionBoxData != null)
            {
                OnRightClickedTransitionBox(clickedTransitionBoxData.Value.transition);
                return;
            }
        }

        void OnRightClickedState(State state)
        {
            rightDownedMouseType = DownedMouseType.DownedOnState;
            this.Context.UnSelectAllTransition();
            
            if (Context.IsSelectedState(state.Id) == false)
            {
                Context.SelectOnlyOneState(state.Id);
            }
            
            Event.current.Use();
        }

        void OnRightClickedTransitionBox(Transition transition)
        {
            rightDownedMouseType = DownedMouseType.DownedOnTransition;
            long originStateId = transition.OriginId;
            long transitionId = transition.Id;

            this.Context.SelectOnlyOneState(originStateId);
            this.Context.SelectOnlyOneTransition(transitionId);
            EditorWindow.InspectSelected();
            
            Event.current.Use();
        }

        #endregion
        
        #region Right Mouse Up

        protected override void OnRightMouseUp(Vector2 position)
        {
            if (rightDownedMouseType == DownedMouseType.None)
            {
                return;
            }
            
            if (rightDownedMouseType == DownedMouseType.DownedOnState)
            {
                OnRightMouseUpWhenDownedOnState(position);
                return;
            }

            if (rightDownedMouseType == DownedMouseType.DownedOnTransition)
            {
                OnRightMouseUpWhenDownedOnTransition(position);
                return;
            }
        }

        void OnRightMouseUpWhenDownedOnState(Vector2 position)
        {
            rightDownedMouseType = DownedMouseType.None;
            Event.current.Use();
            
            StateContextMenu contextMenu = new StateContextMenu(EditorWindow);
            contextMenu.Show();
        }
        
        void OnRightMouseUpWhenDownedOnTransition(Vector2 position)
        {
            if(Context.CannotEditGraph)
                return;
            
            rightDownedMouseType = DownedMouseType.None;
            Event.current.Use();

            TransitionContextMenu contextMenu = new TransitionContextMenu(EditorWindow);
            contextMenu.Show();
        }

        #endregion
        
        #region Key Up

        protected override void OnKeyUp(KeyCode keyCode)
        {
            bool wantDelete = false;
            
#if UNITY_EDITOR_OSX
            wantDelete = keyCode == KeyCode.Backspace && Event.current.command;
#else
            wantDelete = keyCode == KeyCode.Delete && Event.current.control;
#endif

            if (wantDelete)
            {
                EditorWindow.InspectEmpty();
            
                var listStateIdSelected = Context.GetListSelectedState().ToList();
                Context.UnSelectAllState();
                Context.GraphSp.AsStateMachineGraph().DeleteStates(Context, listStateIdSelected);
            }
        }

        #endregion
    }
}