using UnityEditor;
using UnityEngine;

namespace ZeroX.FsmSystem.Editors
{
    public class PopupAbout : EditorWindow
    {
        public static void OpenWindow()
        {
            var instance = GetWindow<PopupAbout>(); //Sử dụng điều này kết hợp với ShowUtility để hiển thị popup ở dạng ko bị hide khi lost focus
            
            instance.titleContent = new GUIContent("About Fsm System");
            instance.Show();
        }

        private void OnGUI()
        {
            position = new Rect(position.x, position.y, 250, 170);
            
            GUILayout.BeginVertical();
            
            GUILayout.Space(15);
            DrawBigTitle();
            
            GUILayout.EndVertical();
            DrawInfo();
        }


        void DrawBigTitle()
        {
            GUIStyle style = new GUIStyle(EditorStyles.boldLabel);
            style.fontSize = 20;
            style.alignment = TextAnchor.UpperCenter;
            
            GUILayout.Label("Fsm System", style);
            
        }

        void DrawInfo()
        {
            GUIStyle style = new GUIStyle(EditorStyles.label);
            style.fontSize = 12;
            style.alignment = TextAnchor.UpperCenter;
            
            
            GUILayout.Label("Version 1.1", style);
            
            GUILayout.Space(35);
            GUILayout.Label("A product of ZeroX", style);
            GUILayout.Label("Developer: LuongNM", style);
            
            DrawEmail();
        }

        void DrawEmail()
        {
            GUIStyle mailStyle = new GUIStyle(EditorStyles.linkLabel);
            mailStyle.fontSize = 12;
            mailStyle.alignment = TextAnchor.MiddleCenter;

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Email: minhluongneo@gmail.com", mailStyle, GUILayout.ExpandWidth(true)))
            {
                Application.OpenURL("mailto:minhluongneo@gmail.com");
            }
            GUILayout.EndHorizontal();
        }
    }
}