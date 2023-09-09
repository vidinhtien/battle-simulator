using UnityEditor;
using UnityEngine;

namespace ZeroX.FsmSystem.Editors
{
    public class GraphInfoLayer : GraphLayer
    {
        private Rect layerRect;
        private GUIStyle graphFsmLabelStyle;
        private GUIStyle graphStatusLabelStyle;
        
        public GraphInfoLayer(StateMachineGraphWindow editorWindow) : base(editorWindow)
        {
        }

        protected override void DrawGraphLayer(Rect rect)
        {
            this.layerRect = rect;

            if(Context.HasGraph == false)
                return;
            
            InitStyleIfNot();
            DrawGraphStatusInfo();
            DrawGraphInfo();
        }

        void InitStyleIfNot()
        {
            if (graphFsmLabelStyle == null)
            {
                graphFsmLabelStyle = new GUIStyle(EditorStyles.label);
                graphFsmLabelStyle.normal.textColor = new Color(1, 1, 1, 0.5f);
                graphFsmLabelStyle.fontSize = 12;
                graphFsmLabelStyle.alignment = TextAnchor.UpperLeft;
            }
            
            if (graphStatusLabelStyle == null)
            {
                graphStatusLabelStyle = new GUIStyle(EditorStyles.label);
                graphStatusLabelStyle.normal.textColor = new Color(1, 1, 1, 1f);
                graphStatusLabelStyle.fontSize = 12;
                graphStatusLabelStyle.alignment = TextAnchor.UpperLeft;
            }
        }
        
        private void DrawGraphInfo()
        {
            Rect gameObjectRect = new Rect(8, layerRect.yMax - 63, 1000, 20);
            GUI.Label(gameObjectRect, "GameObject:    " + Context.GraphOwner.name, graphFsmLabelStyle);

            Rect componentRect = new Rect(8, layerRect.yMax - 43, 1000, 20);
            GUI.Label(componentRect, "Component:      " + Context.GraphOwner.GetType().Name, graphFsmLabelStyle);

            Rect graphFieldRect = new Rect(8, layerRect.yMax - 23, 1000, 20);
            GUI.Label(graphFieldRect, "Graph:                " + Context.GraphSp.name, graphFsmLabelStyle);

        }

        private void DrawGraphStatusInfo()
        {
            if(EditorApplication.isPlaying == false)
                return;
            
            Rect gameObjectRect = new Rect(8, 25, 1000, 20);

            Color oldTextColor = GUI.contentColor;
            var graphStatus = Context.GraphObject.GraphStatus;
            if (graphStatus == FsmGraphStatus.Running)
            {
                GUI.contentColor = Color.green;
                GUI.Label(gameObjectRect, "Running", graphStatusLabelStyle);
            }
            else if (graphStatus == FsmGraphStatus.Pause)
            {
                GUI.contentColor = new Color(0.9882f, 0.6901f, 0.2352f);
                GUI.Label(gameObjectRect, "Pausing", graphStatusLabelStyle);
            }
            else if (graphStatus == FsmGraphStatus.Stop)
            {
                GUI.contentColor = Color.red;
                GUI.Label(gameObjectRect, "Stopping", graphStatusLabelStyle);
            }
            else
            {
                Debug.LogError("Not code for graph status: " + graphStatus);
            }

            GUI.contentColor = oldTextColor;
        }
    }
}