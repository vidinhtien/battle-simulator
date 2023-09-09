using UnityEditor;
using UnityEngine;

namespace ZeroX.FsmSystem.Editors
{
    public class Toolbar : StateMachineGraphWindowLayer
    {
        private const float toolbarPaddingTop = 1;
        private const float toolbarPaddingLeft = 3;
        private const float controlHeight = 16;

        public Rect toolbarRect;
        private Rect graphOwnerRect;
        private Rect zoomSliderRect;
        
        public Rect ToolbarRect => toolbarRect;
        
        private GUIStyle buttonShowContextMenuStyle;
        
        public Toolbar(StateMachineGraphWindow editorWindow) : base(editorWindow)
        {
        }

        public override void Draw(Rect rect)
        {
            toolbarRect = new Rect(0, 0, rect.width, EditorStyles.toolbar.fixedHeight);
            GUI.BeginGroup(toolbarRect, EditorStyles.toolbar);
            DrawGraphOwnerField();
            DrawZoomSlider();
            DrawButtonViewToCenter();
            DrawButtonGenerateCode();
            DrawButtonContextMenu();
            GUI.EndGroup();
        }

        #region Process Event

        void UseEventIfInsideToolbar(Vector2 position)
        {
            if(toolbarRect.Contains(position))
                Event.current.Use();
        }

        public override void ProcessEvents()
        {
            UseEventIfInsideToolbar(Event.current.mousePosition);
        }

        #endregion

        void DrawGraphOwnerField()
        {
            graphOwnerRect = new Rect(toolbarPaddingLeft, toolbarRect.y + toolbarPaddingTop, toolbarRect.width * 0.25f, controlHeight);
            
            if (Context.HasGraph)
                EditorGUI.ObjectField(graphOwnerRect, Context.GraphOwner, typeof(Object), true);
            else
                EditorGUI.ObjectField(graphOwnerRect, null, typeof(Object), true);
        }

        void DrawZoomSlider()
        {
            zoomSliderRect = new Rect(toolbarPaddingLeft + graphOwnerRect.width + 8, toolbarRect.y + toolbarPaddingTop, toolbarRect.width * 0.2f, controlHeight);

            Rect labelRect = zoomSliderRect;
            labelRect.width = 50;
            GUI.Label(labelRect, "Zoom");

            Rect sliderRect = zoomSliderRect;
            sliderRect.x += 40;
            sliderRect.width -= 40;
            Context.ZoomFactor = GUI.HorizontalSlider(sliderRect, Context.ZoomFactor, ZoomSettings.MinZoomFactor, ZoomSettings.MaxZoomFactor);
        }

        void DrawButtonViewToCenter()
        {
            Rect rect = new Rect();
            rect.width = 50;
            rect.height = toolbarRect.height;

            Vector2 pos = Vector2.zero;
            pos.y = toolbarRect.position.y;
            pos.x = toolbarRect.xMax - rect.width - 120;
            
            rect.position = pos;

            if(GUI.Button(rect, "Center"))
                EditorWindow.ViewToCenter();
        }

        void DrawButtonGenerateCode()
        {
            Rect rect = new Rect();
            rect.width = 100;
            rect.height = toolbarRect.height;

            Vector2 pos = Vector2.zero;
            pos.y = toolbarRect.position.y;
            pos.x = toolbarRect.xMax - rect.width - 20;
            
            rect.position = pos;

            if(GUI.Button(rect, "Generate Code"))
                PopupGenerateCode.Show(Context);
        }

        void DrawButtonContextMenu()
        {
            Rect rect = new Rect();
            rect.width = 20;
            rect.height = toolbarRect.height;

            Vector2 pos = Vector2.zero;
            pos.y = toolbarRect.position.y;
            pos.x = toolbarRect.xMax - rect.width;
            
            rect.position = pos;

            
            if(buttonShowContextMenuStyle == null)
            {
                buttonShowContextMenuStyle = new GUIStyle(GUI.skin.button)
                {
                    padding = new RectOffset()
                };
            }
            
            if(GUI.Button(rect, EditorGUIUtility.IconContent("_Menu"), buttonShowContextMenuStyle))
            {
                ShowContextMenu();
            }
        }

        void ShowContextMenu()
        {
            GenericMenu genericMenu = new GenericMenu();
            genericMenu.AddItem(new GUIContent("Clear Graph"), false, ClearGraph);
            genericMenu.AddSeparator("");
            genericMenu.AddItem(new GUIContent("About"), false, ShowAbout);
            
            genericMenu.ShowAsContext();
        }

        void ClearGraph()
        {
            if(Context.HasGraph == false)
                return;
            
            bool wantClearGraph = EditorUtility.DisplayDialog("Clear Graph?", "Do you want to clear the graph?", "Yes", "No");

            if (wantClearGraph)
            {
                EditorWindow.InspectEmpty();
                Context.EnqueueEditAction(() =>
                {
                    Context.GraphSp.AsStateMachineGraph().StateIdSeq = 0;
                    Context.GraphSp.AsStateMachineGraph().TransitionIdSeq = 0;
                    Context.GraphSp.AsStateMachineGraph().EntryStateId = State.emptyId;
                
                    Context.GraphSp.AsStateMachineGraph().ListTransitionSp.ClearArray();
                
                    Context.GraphSp.AsStateMachineGraph().ListNormalStateSp.ClearArray();
                    Context.GraphSp.AsStateMachineGraph().ListAnyStateSp.ClearArray();
                    Context.GraphSp.AsStateMachineGraph().ListParallelStateSp.ClearArray();
                });
            }
        }

        void ShowAbout()
        {
            PopupAbout.OpenWindow();
        }
    }
}