using UnityEditor;

namespace ZeroX.FsmSystem.Editors
{
    public static class FsmSystemMenuItem
    {
        [MenuItem("Tools/ZeroX/Fsm System/Fsm Graph Window")]
        public static void OpenFsmGraphWindow()
        {
            StateMachineGraphWindow.OpenWindow();
        }
        
        [MenuItem("Tools/ZeroX/Fsm System/About")]
        public static void OpenAboutWindow()
        {
            PopupAbout.OpenWindow();
        }
    }
}