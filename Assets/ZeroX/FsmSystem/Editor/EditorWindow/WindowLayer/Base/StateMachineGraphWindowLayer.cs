namespace ZeroX.FsmSystem.Editors
{
    public abstract class StateMachineGraphWindowLayer : WindowLayer
    {
        public StateMachineGraphWindow EditorWindow { get; private set; }
        public StateMachineGraphWindowContext Context => EditorWindow.Context;

        public StateMachineGraphWindowLayer(StateMachineGraphWindow editorWindow)
        {
            this.EditorWindow = editorWindow;
        }
    }
}