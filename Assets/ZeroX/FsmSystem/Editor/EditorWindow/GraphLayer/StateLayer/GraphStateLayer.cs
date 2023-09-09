using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace ZeroX.FsmSystem.Editors
{
    public class GraphStateLayer : GraphLayer
    {
        enum DownedMouseType
        {
            None, DownedOnState, DownedOnTransition
        }

        struct TransitionBoxData
        {
            public SerializedProperty originStateSp;
            public SerializedProperty transitionSp;
            public long transitionId;
            
            public Rect transitionRect;
            public int transitionIndex;
            
            public bool hasData;

            public TransitionBoxData(SerializedProperty originStateSp, SerializedProperty transitionSp, long transitionId, Rect transitionRect, int transitionIndex)
            {
                this.originStateSp = originStateSp;
                this.transitionSp = transitionSp;
                this.transitionId = transitionId;
                
                this.transitionRect = transitionRect;
                this.transitionIndex = transitionIndex;
                
                this.hasData = true;
            }
        }

        class TransitionLinePreviewData
        {
            public SerializedProperty transitionSp;
            public Rect transitionRect;
            public Vector2 endPos;
            public bool isActive;
        }
        
        
        
        
        private readonly StateStyles stateStyles;
        private readonly List<TransitionBoxData> listTransitionBoxData = new List<TransitionBoxData>();
        private DownedMouseType leftDownedMouseType = DownedMouseType.None;
        private DownedMouseType rightDownedMouseType = DownedMouseType.None;
        private TransitionLinePreviewData transitionLinePreviewData = new TransitionLinePreviewData();
        private HashSet<string> listDuplicatedStateName = new HashSet<string>();
        private Dictionary<long, HashSet<string>> dictDuplicatedTransitionName = new Dictionary<long, HashSet<string>>();

        public GraphStateLayer(StateMachineGraphWindow editorWindow) : base(editorWindow)
        {
            stateStyles = new StateStyles();
        }

        protected override void DrawGraphLayer(Rect rect)
        {
            //Stopwatch st = new Stopwatch();
            //st.Start();
            
            this.stateStyles.ApplyZoomFactor(this.Context.ZoomFactor);
            
            listTransitionBoxData.Clear();
            listDuplicatedStateName = Context.GraphSp.AsStateMachineGraph().GetListStateDuplicateName(Context);
            dictDuplicatedTransitionName = Context.GraphSp.AsStateMachineGraph().GetDictTransitionDuplicateName(Context);
            
            DrawStates(rect);
            DrawListTransitionLine();
            DrawTransitionLinePreview();
            
            this.Context.SelectionRect.Draw();
            
            //st.Stop();
            //Debug.Log(st.ElapsedMilliseconds);
        }

        void DrawStates(Rect rect)
        {
            DrawListNormalState(rect);
            DrawListAnyState(rect);
            DrawListParallelState(rect);
        }

        void DrawBox(Rect rect, string text, GUIStyle style)
        {
            style.Draw(rect, text, false, true, false, false);
        }

        #region Normal State

        void DrawListNormalState(Rect rect)
        {
            long currentStateId = Context.GraphSp.AsStateMachineGraph().GetCurrentStateId(Context);
            long entryStateId = Context.GraphSp.AsStateMachineGraph().EntryStateId;

            for (int i = 0; i < Context.ListNormalStateSp.arraySize; i++)
            {
                var stateSp = Context.ListNormalStateSp.GetArrayElementAtIndex(i);
                
                if(Context.IsSelectedState(stateSp.AsState().Id))
                    DrawSelectedStateOutline(stateSp);
                    
                DrawNormalState(stateSp, currentStateId, entryStateId);
                DrawListTransitionBoxOfState(stateSp);
            }
        }
        
        private void DrawNormalState(SerializedProperty stateSp, long currentStateId, long entryStateId)
        {
            string stateName = stateSp.AsState().Name;
            if (stateName == null)
                stateName = "";

            bool duplicateStateName = listDuplicatedStateName.Contains(stateName);

            Rect stateRect = GetTransformedRect(stateSp.AsState().CalculateTitleRect());
            DrawBox(stateRect, stateName, GetNormalStateStyle(stateSp, duplicateStateName, currentStateId, entryStateId));
        }
        
        private GUIStyle GetNormalStateStyle(SerializedProperty stateSp, bool duplicatedStateName, long currentStateId, long entryStateId)
        {
            if (duplicatedStateName)
                return this.stateStyles.Get(StateStyles.Style.DuplicatedStateName);
            
            long stateId = stateSp.AsState().Id;

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
            for (int i = 0; i < Context.ListAnyStateSp.arraySize; i++)
            {
                var stateSp = Context.ListAnyStateSp.GetArrayElementAtIndex(i);
                
                if (Context.IsSelectedState(stateSp.AsState().Id))
                    DrawSelectedStateOutline(stateSp);
                    
                DrawAnyState(stateSp);
                DrawListTransitionBoxOfState(stateSp);
            }
        }
        
        private void DrawAnyState(SerializedProperty stateSp)
        {
            string stateName = stateSp.AsState().Name;
            if (stateName == null)
                stateName = "";
            
            bool duplicatedStateName = listDuplicatedStateName.Contains(stateName);
            
            GUIStyle style;
            if (duplicatedStateName)
                style = stateStyles.Get(StateStyles.Style.DuplicatedStateName);
            else
                style = stateStyles.Get(StateStyles.Style.AnyState);
            
            Rect nodeRect = GetTransformedRect(stateSp.AsState().CalculateTitleRect());
            DrawBox(nodeRect, stateName, style);
        }

        #endregion
        

        #region Parallel State

        void DrawListParallelState(Rect rect)
        {
            for (int i = 0; i < Context.ListParallelStateSp.arraySize; i++)
            {
                var stateSp = Context.ListParallelStateSp.GetArrayElementAtIndex(i);

                if (Context.IsSelectedState(stateSp.AsState().Id))
                    DrawSelectedStateOutline(stateSp);
                    
                DrawParallelState(stateSp);
            }
        }
        
        private void DrawParallelState(SerializedProperty stateSp)
        {
            string stateName = stateSp.AsState().Name;
            if (stateName == null)
                stateName = "";

            bool duplicatedStateName = listDuplicatedStateName.Contains(stateName);

            GUIStyle style;
            if (duplicatedStateName)
                style = stateStyles.Get(StateStyles.Style.DuplicatedStateName);
            else
                style = stateStyles.Get(StateStyles.Style.ParallelSate);
            
            Rect stateRect = GetTransformedRect(stateSp.AsState().CalculateTitleRect());
            DrawBox(stateRect, stateName, style);
        }

        #endregion
        
        
        #region Transition Box

        private void DrawListTransitionBoxOfState(SerializedProperty stateSp)
        {
            var listTransitionSp = stateSp.AsState().GetListTransition(Context);

            int index = 0;
            foreach (var transitionSp in listTransitionSp)
            {
                DrawTransitionBoxOfState(stateSp, transitionSp, index);
                index++;
            }
        }
        
        private void DrawTransitionBoxOfState(SerializedProperty stateSp, SerializedProperty transitionSp, int transitionIndex)
        {
            string transitionName = transitionSp.AsTransition().Name;
            
            Rect transitionRect = GetTransformedRect(stateSp.AsState().CalculateTransitionRectWithIndex(transitionIndex));
            long transitionId = transitionSp.AsTransition().Id;
            bool isSelected = Context.IsSelectedTransition(transitionId);
            bool duplicatedTransitionName = dictDuplicatedTransitionName[transitionSp.AsTransition().OriginId].Contains(transitionSp.AsTransition().Name);
            bool missingName = string.IsNullOrEmpty(transitionName);
            
            var style = this.stateStyles.GetForTransitionBox(isSelected, duplicatedTransitionName, missingName);

            
            if (missingName)
                transitionName = "(Missing Name)";
            
            DrawBox(transitionRect, transitionName, style);
            listTransitionBoxData.Add(new TransitionBoxData(stateSp, transitionSp, transitionId, transitionRect, transitionIndex));
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
                if (transitionBoxData.transitionId == lastTransitionId)
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
            long targetId = transitionBoxData.transitionSp.AsTransition().TargetId;
            if(targetId == State.emptyId)
                return;
            
            var targetStateSp = Context.GraphSp.AsStateMachineGraph().GetStateById(Context, targetId);
            if(targetStateSp == null)
                return;

            Color lineColor = transitionBoxData.transitionId == lastTransitionId
                ? GraphColors.TransitionLineColor_Last
                : GraphColors.TransitionLineColor;

            Rect targetRect = GetTransformedRect(targetStateSp.AsState().CalculateFullRect(Context));
            LineDrawer.DrawParabola(transitionBoxData.transitionRect, targetRect, lineColor, Context.ZoomFactor);
        }

        private void DrawTransitionLinePreview()
        {
            if(transitionLinePreviewData.isActive == false)
                return;
            
            LineDrawer.DrawParabola(transitionLinePreviewData.transitionRect, transitionLinePreviewData.endPos, GraphColors.TransitionLineColor, Context.ZoomFactor);
        }

        #endregion
        
        
        private void DrawSelectedStateOutline(SerializedProperty stateSp)
        {
            Rect stateFullRectOutline = GetTransformedRect(stateSp.AsState().CalculateFullRectOutline(Context));
            DrawBox(stateFullRectOutline, "", stateStyles.Get(StateStyles.Style.SelectedStateOutline));
        }
        
        public SerializedProperty GetClickedState(Vector2 mousePos)
        {
            var reverse = Context.GetAllStateSp().Reverse();

            foreach (var stateSp in reverse)
            {
                Rect transformedRect = GetTransformedRect(stateSp.AsState().CalculateTitleRect());

                if (transformedRect.Contains(mousePos))
                {
                    return stateSp;
                }
            }

            return null;
        }

        public SerializedProperty GetClickedTransitionBox(Vector2 mousePos)
        {
            foreach (var kv in listTransitionBoxData)
            {
                if (kv.transitionRect.Contains(mousePos))
                    return kv.transitionSp;
            }

            return null;
        }
        

        


        #region Left Mouse Down

        protected override void OnLeftMouseDown(Vector2 position)
        {
            var clickedStateSp = GetClickedState(position);
            if (clickedStateSp != null)
            {
                OnLeftClickedState(clickedStateSp);
                return;
            }

            var clickedTransitionSp = GetClickedTransitionBox(position);
            if (clickedTransitionSp != null)
            {
                OnLeftClickedTransitionBox(clickedTransitionSp);
                return;
            }
        }

        void OnLeftClickedState(SerializedProperty stateSp)
        {
            leftDownedMouseType = DownedMouseType.DownedOnState;
            long stateId = stateSp.AsState().Id;
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

        void OnLeftClickedTransitionBox(SerializedProperty transitionSp)
        {
            leftDownedMouseType = DownedMouseType.DownedOnTransition;
            long originStateId = transitionSp.AsTransition().OriginId;
            long transitionId = transitionSp.AsTransition().Id;
            
            var stateSp = Context.GraphSp.AsStateMachineGraph().GetStateById(Context, originStateId);
            if (stateSp == null)
            {
                Debug.LogError("Cannot found origin state of transition: " + transitionId);
                return;
            }
            
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

            var listAllState = Context.GetAllStateSp();
            foreach (var stateSp in listAllState)
            {
                if(Context.IsSelectedState(stateSp.AsState().Id) == false)
                    continue;

                var positionSp = stateSp.AsState().PositionSp;

                positionSp.vector2Value += dragDelta;
            }
            
            Event.current.Use();
        }
        
        void OnLeftMouseDragWhenDownedOnTransition(Vector2 position)
        {
            Event.current.Use();
            
            long selectedTransitionId = Context.FirstSelectedTransitionId;
            var transitionBoxData = listTransitionBoxData.FirstOrDefault(d => d.transitionId == selectedTransitionId);
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
            transitionBoxData.transitionSp.AsTransition().TargetId = Transition.emptyId;
            
            transitionLinePreviewData.isActive = true;
            transitionLinePreviewData.transitionRect = transitionBoxData.transitionRect;
            transitionLinePreviewData.endPos = position;
            transitionLinePreviewData.transitionSp = transitionBoxData.transitionSp;
        }

        #endregion

        #region Left Mouse Up

        protected override void OnLeftMouseUp(Vector2 position)
        {
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

            var stateSp = GetClickedState(position);
            if (stateSp == null)
                return;
            
            if(stateSp.AsState().IsNormalState(Context) == false)
                return;
            
            var transitionSp = transitionLinePreviewData.transitionSp;
            transitionSp.AsTransition().TargetId = stateSp.AsState().Id;
        }

        #endregion

        #region Right Mouse Down

        protected override void OnRightMouseDown(Vector2 position)
        {
            var clickedStateSp = GetClickedState(position);
            if (clickedStateSp != null)
            {
                OnRightClickedState(clickedStateSp);
                return;
            }

            var clickedTransitionSp = GetClickedTransitionBox(position);
            if (clickedTransitionSp != null)
            {
                OnRightClickedTransitionBox(clickedTransitionSp);
                return;
            }
        }

        void OnRightClickedState(SerializedProperty stateSp)
        {
            rightDownedMouseType = DownedMouseType.DownedOnState;
            this.Context.UnSelectAllTransition();

            long stateId = stateSp.AsState().Id;
            if (Context.IsSelectedState(stateId) == false)
            {
                Context.SelectOnlyOneState(stateId);
            }
            
            Event.current.Use();
        }

        void OnRightClickedTransitionBox(SerializedProperty transitionSp)
        {
            rightDownedMouseType = DownedMouseType.DownedOnTransition;
            long originStateId = transitionSp.AsTransition().OriginId;
            long transitionId = transitionSp.AsTransition().Id;
            
            var stateSp = Context.GraphSp.AsStateMachineGraph().GetStateById(Context, originStateId);
            if (stateSp == null)
            {
                Debug.LogError("Cannot found origin state of transition: " + transitionId);
                return;
            }
            
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