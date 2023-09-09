using UnityEditor;

namespace ZeroX.FsmSystem.Editors
{
    public static class SerializedObjectExtension
    {
        public static SerializedProperty FindPropertyDeep(this SerializedObject so, string propertyPath)
        {
            //data.listGraph.Array.data[1]
            //data.arrayGraph.Array.data[10]
            //data.a.b

            string[] listPath = propertyPath.Split('.');
            if (listPath.Length == 1)
                return so.FindProperty(propertyPath);
            
            SerializedProperty sp = so.FindProperty(listPath[0]);
            if (sp == null)
                return null;
            
            

            for (int i = 1; i < listPath.Length; i++)
            {
                //bool isList = i + 1 < listPath.Length && listPath[i] == "Array" && listPath[i + 1].StartsWith("data[");
                if (sp.isArray == false)
                {
                    sp = sp.FindPropertyRelative(listPath[i]);
                }
                else
                {
                    string dataPath = listPath[i + 1];
                    int index = int.Parse(dataPath.Substring(5, dataPath.Length - 6));
                    
                    sp = sp.GetArrayElementAtIndex(index);
                    i += 1;
                }
            }

            return sp;
        }
    }
}