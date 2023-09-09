using UnityEditor;
using UnityEngine;

namespace ZeroX.FsmSystem.Editors
{
    public class TransitionContextMenu
    {
        private StateMachineGraphWindow EditorWindow { get; set; }
        private StateMachineGraphWindowContext Context => EditorWindow.Context;
        
        public TransitionContextMenu(StateMachineGraphWindow editorWindow)
        {
            this.EditorWindow = editorWindow;
        }

        public void Show()
        {
            long selectedTransitionId = Context.FirstSelectedTransitionId;
            GenericMenu genericMenu = new GenericMenu();
            genericMenu.AddItem(new GUIContent("Delete"), false, () => DeleteTransition(selectedTransitionId));
            
            genericMenu.ShowAsContext();
        }

        void DeleteTransition(long transitionId)
        {
            EditorWindow.InspectEmpty();
            
            Context.UnSelectAllTransition();
            Context.EnqueueEditAction(() =>
            {
                Context.GraphSp.AsStateMachineGraph().DeleteTransition(Context, transitionId);
            });
        }
    }
}