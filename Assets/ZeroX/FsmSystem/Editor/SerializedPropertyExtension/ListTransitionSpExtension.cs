using UnityEditor;

namespace ZeroX.FsmSystem.Editors
{
    public static class ListTransitionSpExtension
    {
        private static readonly ListTransitionSpHandler handler = new ListTransitionSpHandler();
        public static ListTransitionSpHandler AsListTransition(this SerializedProperty serializedProperty)
        {
            handler.serializedProperty = serializedProperty;
            return handler;
        }
    }
    
    public class ListTransitionSpHandler
    {
        public SerializedProperty serializedProperty;

        public int Count => serializedProperty.arraySize;

        public SerializedProperty this[int index] => serializedProperty.GetArrayElementAtIndex(index);

        public SerializedProperty AddNewTransition()
        {
            int newIndex = serializedProperty.arraySize;
            serializedProperty.InsertArrayElementAtIndex(newIndex);
            return serializedProperty.GetArrayElementAtIndex(newIndex);
        }

        public void DeleteTransitionById(long transitionId)
        {
            for (int i = 0; i < serializedProperty.arraySize; i++)
            {
                var transitionSp = serializedProperty.GetArrayElementAtIndex(i);
                if (transitionSp.AsTransition().Id == transitionId)
                {
                    serializedProperty.DeleteArrayElementAtIndex(i);
                    return;
                }
            }
        }

        public void DeleteTransition(SerializedProperty transitionSp)
        {
            DeleteTransitionById(transitionSp.AsTransition().Id);
        }
    }
}