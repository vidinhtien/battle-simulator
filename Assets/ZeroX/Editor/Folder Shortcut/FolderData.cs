using UnityEngine;

namespace ZeroX.FolderShortcut
{
    [System.Serializable]
    public class FolderData
    {
        public string displayName = "";
        public string folderName = "";
        public Object folderObject;
        public Color buttonColor = new Color(0, 0, 0, 1);
        public int treeIndex = 0;
        public bool foldOut = false;

        public FolderData(Object folderObject)
        {
            this.folderObject = folderObject;
        }

        public string GetName()
        {
            if (folderObject == null)
                return "Missing Reference";
            
            if (string.IsNullOrEmpty(displayName) == false)
                return displayName;
            
            return folderObject.name;
        }
        
        /// <summary>
        /// Kiểm tra nếu folderName đã lưu không giống với folderObjectName thì xóa DisplayName và set lại
        /// </summary>
        public void CheckAndRefreshName()
        {
            if (folderObject == null)
                return;

            if (folderObject.name != folderName)
            {
                folderName = folderObject.name;
                displayName = folderName;
            }
        }

        public bool IsDefaultColor()
        {
            return buttonColor.r == 0 && buttonColor.g == 0 && buttonColor.b == 0 && buttonColor.a == 1;
        }
    }
}