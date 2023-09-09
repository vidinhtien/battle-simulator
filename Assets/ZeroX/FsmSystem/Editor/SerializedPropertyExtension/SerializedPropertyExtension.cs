using System.Reflection;
using UnityEditor;
using System;
using System.Collections;
using UnityEngine;

namespace ZeroX.FsmSystem.Editors
{
    public static class SerializedPropertyExtension
    {
        static BindingFlags bindingFlags = BindingFlags.Default | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        private static FieldInfo cachedLocalizedDisplayNameFi;
        private static MethodInfo getNameInternalMi;
        private static MethodInfo syncSerializedObjectVersionMi;
        private static object[] emptyParameters = new object[]{};
        
        public static object GetObject(this SerializedProperty serializeProperty)
        {
            var listField = serializeProperty.propertyPath.Split('.');
            Type targetObjectType = serializeProperty.serializedObject.targetObject.GetType();
            
            Type parentType = targetObjectType;
            object parentObject = serializeProperty.serializedObject.targetObject;

            for (int i = 0; i < listField.Length; i++)
            {
                string fieldName = listField[i];
                bool isList = i + 1 < listField.Length && listField[i] == "Array" && listField[i + 1].StartsWith("data[");
                
                if (isList) //có nghĩa fieldInfo hiện tại là list
                {
                    var data = listField[i + 1]; //data[xxx];
                    int index = int.Parse(data.Substring(5, data.Length - 6));

                    IList list = (IList)parentObject;

                    parentObject = list[index];

                    if (parentObject == null)
                        return null;
                    
                    parentType = parentObject.GetType();
                    i++;
                }
                else
                {
                    var fi = parentType.GetField(fieldName, bindingFlags);
                    var obj = fi.GetValue(parentObject);

                    parentType = fi.FieldType;
                    parentObject = obj;
                }
            }

            return parentObject;
        }

        public static string GetCachedLocalizedDisplayName(this SerializedProperty serializeProperty)
        {
            if (cachedLocalizedDisplayNameFi == null)
            {
                var type = typeof(SerializedProperty);
                cachedLocalizedDisplayNameFi = type.GetField("m_CachedLocalizedDisplayName", bindingFlags);
            }

            return (string)cachedLocalizedDisplayNameFi.GetValue(serializeProperty);
        }


        private static void InvokeSyncSerializedObjectVersion(this SerializedProperty serializeProperty)
        {
            if (syncSerializedObjectVersionMi == null)
            {
                var type = typeof(SerializedProperty);
                syncSerializedObjectVersionMi = type.GetMethod("SyncSerializedObjectVersion", bindingFlags);
            }

            try
            {
                syncSerializedObjectVersionMi.Invoke(serializeProperty, emptyParameters);
            }
            catch (Exception e)
            {
                Debug.LogFormat(e.Message);
            }
            
        }
        
        public static string InvokeGetName(this SerializedProperty serializeProperty)
        {
            if (getNameInternalMi == null)
            {
                var type = typeof(SerializedProperty);
                getNameInternalMi = type.GetMethod("GetNameInternal", bindingFlags);
            }

            InvokeSyncSerializedObjectVersion(serializeProperty);
            return (string)getNameInternalMi.Invoke(serializeProperty, emptyParameters);
        }
    }
}