using System;
using System.Reflection;

namespace ZeroX.FsmSystem.Editors
{
    public static class EditorTransitionUtility
    {
        private static bool isInitializedReflection = false;
        private static BindingFlags bindingFlags = BindingFlags.Default | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        private static FieldInfo idFi;
        private static FieldInfo originIdFi;
        private static FieldInfo targetIdFi;
        
        public static void InitReflectionIfNot()
        {
            if(isInitializedReflection)
                return;
            isInitializedReflection = true;

            Type transitionType = typeof(Transition);
            
            idFi = transitionType.GetField(Transition.fn_id, bindingFlags);
            originIdFi = transitionType.GetField(Transition.fn_originId, bindingFlags);
            targetIdFi = transitionType.GetField(Transition.fn_targetId, bindingFlags);
        }

        public static void SetId(Transition transition, long id)
        {
            InitReflectionIfNot();
            idFi.SetValue(transition, id);
        }
        
        public static void SetOriginId(Transition transition, long originId)
        {
            InitReflectionIfNot();
            originIdFi.SetValue(transition, originId);
        }
        
        public static void SetTargetId(Transition transition, long targetId)
        {
            InitReflectionIfNot();
            targetIdFi.SetValue(transition, targetId);
        }
    }
}