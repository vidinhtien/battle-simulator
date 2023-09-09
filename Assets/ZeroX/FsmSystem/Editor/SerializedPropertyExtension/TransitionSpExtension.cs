using UnityEditor;

namespace ZeroX.FsmSystem.Editors
{
    public static class TransitionSpExtension
    {
        private static readonly TransitionSpHandler handler = new TransitionSpHandler();
        public static TransitionSpHandler AsTransition(this SerializedProperty serializedProperty)
        {
            handler.serializedProperty = serializedProperty;
            return handler;
        }
    }
    
    public class TransitionSpHandler
    {
        public SerializedProperty serializedProperty;
        
        public SerializedProperty IdSp => serializedProperty.FindPropertyRelative(Transition.fn_id);
        public long Id
        {
            get => IdSp.longValue;
            set => IdSp.longValue = value;
        }
        
        public SerializedProperty OriginIdSp => serializedProperty.FindPropertyRelative(Transition.fn_originId);
        public long OriginId
        {
            get => OriginIdSp.longValue;
            set => OriginIdSp.longValue = value;
        }
        
        public SerializedProperty TargetIdSp => serializedProperty.FindPropertyRelative(Transition.fn_targetId);
        public long TargetId
        {
            get => TargetIdSp.longValue;
            set => TargetIdSp.longValue = value;
        }
        
        public SerializedProperty NameSp => serializedProperty.FindPropertyRelative(Transition.fn_name);
        public string Name
        {
            get => NameSp.stringValue;
            set => NameSp.stringValue = value;
        }
    }
}