using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace ZeroX.FsmSystem.Editors
{
    public static class EditorActionStateUtility
    {
        private static BindingFlags bindingFlags = BindingFlags.Default | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        public static Action GetAction(ActionState actionState, string fieldName)
        {
            var type = typeof(ActionState);
            var fieldInfo = type.GetField(fieldName, bindingFlags);
            if (fieldInfo == null)
            {
                Debug.LogError("ActionState chưa khai báo field này: " + fieldName);
                return null;
            }

            return (Action) fieldInfo.GetValue(actionState);
        }
        
        public static List<Action> GetListAction(ActionState actionState)
        {
            List<Action> list = new List<Action>();

            var type = typeof(ActionState);

            foreach (var fieldName in StateEventFieldNameDefine.list)
            {
                var fieldInfo = type.GetField(fieldName.ToString(), bindingFlags);
                if (fieldInfo == null)
                {
                    Debug.LogError("ActionState chưa khai báo field này: " + fieldName);
                    continue;
                }

                list.Add((Action)fieldInfo.GetValue(actionState));
            }

            return list;
        }
    }
}