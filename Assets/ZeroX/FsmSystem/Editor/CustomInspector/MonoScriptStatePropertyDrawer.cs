using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace ZeroX.FsmSystem.Editors
{
    [CustomPropertyDrawer(typeof(MonoScriptState))]
    public class MonoScriptStatePropertyDrawer : PropertyDrawer
    {
        private ReorderableList reorderableListBehaviour;
        private const float lineHeight = 18;
        private const float lineSpace = 2;
        private const float spacingWithListBehaviour = 5;
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            DrawTitle(position, property);
            DrawListBehaviour(position, property);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (reorderableListBehaviour == null)
                return lineHeight;

            return lineHeight + lineSpace + reorderableListBehaviour.GetHeight();
        }

        void DrawTitle(Rect position, SerializedProperty property)
        {
            var stateNameSp = property.FindPropertyRelative(State.fn_name);
            Rect rect = position;
            rect.height = lineHeight;

            EditorGUI.PropertyField(rect, stateNameSp);
        }

        void DrawListBehaviour(Rect propertyRect, SerializedProperty property)
        {
            var listBehaviourSp = property.FindPropertyRelative(MonoScriptState.fn_listBehaviour);

            if (reorderableListBehaviour == null)
            {
                reorderableListBehaviour = new ReorderableList(property.serializedObject, listBehaviourSp, true, true, true, true);
                reorderableListBehaviour.drawElementCallback += DrawElementCallback;
                reorderableListBehaviour.drawHeaderCallback += DrawHeaderCallback;
            }

            reorderableListBehaviour.serializedProperty = listBehaviourSp;

            Rect rectOfList = propertyRect;
            rectOfList.y += lineHeight + lineSpace; //Cách ra 1 đoạn của line stateName
            reorderableListBehaviour.DoList(rectOfList);
        }

        private void DrawHeaderCallback(Rect rect)
        {
            EditorGUI.LabelField(rect, "List Behaviour: " + reorderableListBehaviour.count);
        }

        private void DrawElementCallback(Rect rect, int index, bool isActive, bool isFocused)
        {
            var behaviourSp = reorderableListBehaviour.serializedProperty.GetArrayElementAtIndex(index);
            behaviourSp.objectReferenceValue = EditorGUI.ObjectField(rect, behaviourSp.objectReferenceValue, typeof(MonoStateBehaviour), true);
        }
    }
}