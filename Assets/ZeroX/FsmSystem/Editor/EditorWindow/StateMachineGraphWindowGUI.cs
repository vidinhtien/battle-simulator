using UnityEngine;

namespace ZeroX.FsmSystem.Editors
{
    public class StateMachineGraphWindowGUI
    {
        [System.NonSerialized] private readonly StateMachineGraphWindow editorWindow;
        
        private GraphView Graph { get; set; }
        public Toolbar Toolbar { get; private set; }
        public GraphInspector Inspector { get; private set; }
        
        
        public StateMachineGraphWindowGUI(StateMachineGraphWindow editorWindow)
        {
            this.editorWindow = editorWindow;
            this.Graph = new GraphView(this.editorWindow);
            this.Inspector = new GraphInspector(this.editorWindow);
            this.Toolbar = new Toolbar(this.editorWindow);
        }

        public void OnGUI()
        {
            var editorWindowRect = editorWindow.Rect;

            if (Event.current.type == EventType.Repaint)
            {
                Graph.Draw(editorWindowRect);
            }
            
            //Những layout dưới đây sử dụng các control của unity(button, slider).
            //Nên nếu chỉ draw trong Repaint thì các control đó sẽ ko nhận được sự kiện khác (mouseDown, mouseUp...)
            Inspector.Draw(editorWindowRect);
            Toolbar.Draw(editorWindowRect);
            
            if(Event.current.type != EventType.Repaint && Event.current.type != EventType.Layout)
            {
                Toolbar.ProcessEvents();
                Inspector.ProcessEvents();
                Graph.ProcessEvents();
            }
        }
    }
}