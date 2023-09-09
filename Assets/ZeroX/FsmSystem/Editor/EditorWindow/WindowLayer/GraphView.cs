using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ZeroX.FsmSystem.Editors
{
    //Là 1 windowLayer, nhưng nó chứa các graphLayer khác
    public class GraphView : StateMachineGraphWindowLayer
    {
        private Rect graphRect;
        private List<GraphLayer> Layers { get; } = new List<GraphLayer>();
        private GraphBackgroundLayer graphBackgroundLayer;
        
        public GraphView(StateMachineGraphWindow editorWindow) : base(editorWindow)
        {
            //Add layers to the graph view
            graphBackgroundLayer = new GraphBackgroundLayer(editorWindow);
            this.Layers.Add(graphBackgroundLayer);
            // this.Layers.Add(new GraphTransitionLayer(editorWindow));
            this.Layers.Add(new GraphStateLayer_V2(editorWindow));
            this.Layers.Add(new GraphInfoLayer(editorWindow));
            // this.Layers.Add(new GraphMessageBoxLayer(editorWindow));
        }
        

        public override void Draw(Rect rect)
        {
            this.graphRect = rect;
            
            if (this.Context.HasGraph)
            {
                for (int i = 0; i < this.Layers.Count; i++)
                {
                    this.Layers[i].Draw(rect);
                }
            }
            else
            {
                graphBackgroundLayer.Draw(rect);
                DrawNoGraphSelected();
            }
        }

        public override void ProcessEvents()
        {
            if (this.Context.HasGraph)
            {
                for (int i = this.Layers.Count - 1; i >= 0; i--)
                {
                    this.Layers[i].ProcessEvents();
                }
            }
        }

        void DrawNoGraphSelected()
        {
            Rect rect = new Rect(0,0, 270, 100);
            GUIStyle style = new GUIStyle(EditorStyles.boldLabel);
            style.normal.textColor = new Color(1, 1, 1, 0.5f);
            style.alignment = TextAnchor.MiddleCenter;
            style.fontSize = 25;
            //style.normal.textColor = new Color(1, 1, 1, 0.5f);
            
            GUI.Label(rect, "No Graph Selected", style);
        }
    }
}