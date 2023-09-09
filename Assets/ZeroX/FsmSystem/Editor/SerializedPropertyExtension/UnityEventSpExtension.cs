using UnityEditor;
using UnityEngine.Events;

namespace ZeroX.FsmSystem.Editors
{
    public static class UnityEventSpExtension
    {
        private static readonly UnityEventSpHandler handler = new UnityEventSpHandler();
        public static UnityEventSpHandler AsUnityEvent(this SerializedProperty serializedProperty)
        {
            handler.serializedProperty = serializedProperty;
            return handler;
        }
    }
    
    public class UnityEventSpHandler
    {
        public SerializedProperty serializedProperty;

        public void AddPersistentListener()
        {
            var m_PersistentCallsSp = serializedProperty.FindPropertyRelative("m_PersistentCalls");
            var m_Calls = m_PersistentCallsSp.FindPropertyRelative("m_Calls");

            int newIndex = m_Calls.arraySize;
            m_Calls.InsertArrayElementAtIndex(newIndex);

            var persistentCallSp = m_Calls.GetArrayElementAtIndex(newIndex);
            
            var m_ModeSp = persistentCallSp.FindPropertyRelative("m_Mode");
            m_ModeSp.enumValueIndex = (int)PersistentListenerMode.Void; //1

            var m_CallStateSp = persistentCallSp.FindPropertyRelative("m_CallState");
            m_CallStateSp.enumValueIndex = (int)UnityEventCallState.RuntimeOnly; //2
        }

        public int CountPersistentListener()
        {
            var m_PersistentCallsSp = serializedProperty.FindPropertyRelative("m_PersistentCalls");
            var m_Calls = m_PersistentCallsSp.FindPropertyRelative("m_Calls");

            return m_Calls.arraySize;
        }
    }
}