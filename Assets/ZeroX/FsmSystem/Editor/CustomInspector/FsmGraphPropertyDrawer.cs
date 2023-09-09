using UnityEditor;
using UnityEngine;

namespace ZeroX.FsmSystem.Editors
{
    [CustomPropertyDrawer(typeof(FsmGraph))]
    public class FsmGraphPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Rect labelRect = new Rect();
            labelRect.position = position.position;
            labelRect.height = position.height;
            labelRect.width = position.width / 2;
            GUIStyle labelStyle = property.prefabOverride ? EditorStyles.boldLabel : EditorStyles.label;
            GUI.Label(labelRect, property.displayName, labelStyle);


            Rect buttonRect = new Rect();
            buttonRect.position = new Vector2(position.x + position.width / 2f, position.y);
            buttonRect.width = position.width / 2;
            buttonRect.height = position.height;
            
            if (GUI.Button(buttonRect, "Edit Graph"))
            {
                StateMachineGraphWindow.OpenWindow(property);
            }
        }
    }
}