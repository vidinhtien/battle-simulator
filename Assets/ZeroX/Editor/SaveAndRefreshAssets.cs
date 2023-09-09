using UnityEditor;

namespace ZeroX.Editors
{
    public static class SaveAndRefreshAssets
    {
        //[MenuItem("Tools/ZeroX/Editor/Save And Refresh Assets", priority = 1)]
        [MenuItem("Assets/Save And Refresh Assets", priority = 1)]
        public static void MenuCheckAndUpdateDefine()
        {
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}