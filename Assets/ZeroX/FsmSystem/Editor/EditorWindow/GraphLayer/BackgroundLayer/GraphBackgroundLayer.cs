using UnityEditor;
using UnityEngine;

namespace ZeroX.FsmSystem.Editors
{
    public class GraphBackgroundLayer : GraphLayer
    {
        private Rect layerRect;

        public GraphBackgroundLayer(StateMachineGraphWindow editorWindow) : base(editorWindow)
        {
        }
        
        protected override void DrawGraphLayer(Rect rect)
        {
            this.layerRect = rect;

            EditorGUI.DrawRect(rect, GraphColors.BackgroundColor);
            
            if (this.Context.IsGridEnabled)
            {
                if (Event.current.type == EventType.Repaint)
                {
                    if (EditorApplication.isPlaying == false)
                    {
                        DrawGrid(12, 1, GraphColors.InnerGridColor, rect.width, rect.height);
                        DrawGrid(120, 1, GraphColors.OuterGridColor, rect.width, rect.height);
                    }
                    else
                    {
                        DrawGrid(12, 1, GraphColors.InnerGridColor_PlayMode, rect.width, rect.height);
                        DrawGrid(120, 1, GraphColors.OuterGridColor_PlayMode, rect.width, rect.height);
                    }
                }
            }
        }
        
        private void DrawGrid(float gridSpacing, float gridOpacity, Color gridColor, float width, float height)
        {
            Vector2 vanishingPoint = new Vector2(width, height) / 2;

            int widthDivs = Mathf.CeilToInt(width / this.Context.ZoomFactor / gridSpacing);
            int heightDivs = Mathf.CeilToInt(height / this.Context.ZoomFactor / gridSpacing);

            Vector2 newOffset = new Vector3(this.Context.DragOffset.x % gridSpacing, this.Context.DragOffset.y % gridSpacing);

            GL.PushMatrix();
            GL.LoadPixelMatrix();
            GL.Begin(GL.LINES);
            GL.Color(gridColor);

            for (int i = -widthDivs; i < widthDivs / this.Context.ZoomFactor; i++)
            {
                float distance = (gridSpacing * i + newOffset.x - vanishingPoint.x) * (1 - this.Context.ZoomFactor);

                float x = gridSpacing * i + newOffset.x - distance;

                Vector2 start = new Vector2(x, -gridSpacing);
                Vector2 end = new Vector2(x, height);

                GL.Vertex(start);
                GL.Vertex(end);
            }

            for (int j = -heightDivs; j < heightDivs / this.Context.ZoomFactor; j++)
            {
                float distance = (gridSpacing * j + newOffset.y - vanishingPoint.y) * (1 - this.Context.ZoomFactor);

                float y = gridSpacing * j + newOffset.y - distance;

                Vector2 start = new Vector2(-gridSpacing + 1, y);
                Vector2 end = new Vector2(width, y);

                GL.Vertex(start);
                GL.Vertex(end);
            }

            GL.End();
            GL.PopMatrix();
        }

        #region Process Events

        protected override void OnMiddleMouseDrag(Vector2 position)
        {
            //Adjust drag offset
            this.Context.DragOffset += Event.current.delta / this.Context.ZoomFactor;

            //Use current event and update GUI
            Event.current.Use();
        }

        protected override void OnScrollWheel(Vector2 position)
        {
            Context.ZoomFactor -= Mathf.Sign(Event.current.delta.y) * ZoomSettings.MaxZoomFactor / 20.0f;
            Event.current.Use();
        }

        protected override void OnRightMouseUp(Vector2 position)
        {
            if(Context.CannotEditGraph)
                return;

            GraphBackgroundContextMenu contextMenu = new GraphBackgroundContextMenu(EditorWindow);
            contextMenu.Show(position);
            Event.current.Use();
        }

        protected override void OnLeftMouseDown(Vector2 position)
        {
            Context.UnSelectAllState();
            Context.UnSelectAllTransition();
            Context.SelectionRect.Position = position;
            
            EditorWindow.InspectSelected();
            Event.current.Use();
        }

        protected override void OnLeftMouseDrag(Vector2 position)
        {
            if (this.Context.SelectionRect.IsActive)
            {
                Context.SelectionRect.Drag(position);
                Event.current.Use();
            }
        }

        protected override void OnLeftMouseUp(Vector2 position)
        {
            var listStateSp = Context.GetAllStateSp();
            foreach (var stateSp in listStateSp)
            {
                Rect transformedRect = GetTransformedRect(stateSp.AsState().CalculateTitleRect());
                if (this.Context.SelectionRect.Contains(transformedRect.max))
                {
                    this.Context.SelectState(stateSp.AsState().Id);
                }
            }

            Context.SelectionRect.Reset();
            
            EditorWindow.InspectSelected(); 
            
            Event.current.Use();
        }

        #endregion
    }
}