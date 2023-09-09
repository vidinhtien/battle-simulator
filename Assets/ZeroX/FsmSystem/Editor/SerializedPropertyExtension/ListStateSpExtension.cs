using UnityEditor;

namespace ZeroX.FsmSystem.Editors
{
    public static class ListStateSpExtension
    {
        private static readonly ListStateSpHandler handler = new ListStateSpHandler();
        public static ListStateSpHandler AsListState(this SerializedProperty serializedProperty)
        {
            handler.serializedProperty = serializedProperty;
            return handler;
        }
    }
    
    public class ListStateSpHandler
    {
        public SerializedProperty serializedProperty;
        
        public int Count => serializedProperty.arraySize;

        public SerializedProperty this[int index] => serializedProperty.GetArrayElementAtIndex(index);

        public SerializedProperty AddState()
        {
            int newIndex = serializedProperty.arraySize;
            serializedProperty.InsertArrayElementAtIndex(newIndex);
            return serializedProperty.GetArrayElementAtIndex(newIndex);
        }

        public bool DeleteStateById(long stateId)
        {
            for (int i = 0; i < serializedProperty.arraySize; i++)
            {
                var stateSp = serializedProperty.GetArrayElementAtIndex(i);
                if (stateSp.AsState().Id == stateId)
                {
                    serializedProperty.DeleteArrayElementAtIndex(i);
                    return true;
                }
            }

            return false;
        }

        public bool DeleteState(SerializedProperty stateSp)
        {
            return DeleteStateById(stateSp.AsState().Id);
        }

        public SerializedProperty GetStateSpById(long stateId)
        {
            for (int i = 0; i < serializedProperty.arraySize; i++)
            {
                var stateSp = serializedProperty.GetArrayElementAtIndex(i);
                if (stateSp.AsState().Id == stateId)
                    return stateSp;
            }

            return null;
        }
    }
}