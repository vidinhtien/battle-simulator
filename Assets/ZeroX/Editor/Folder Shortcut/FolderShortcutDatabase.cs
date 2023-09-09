using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ZeroX.FolderShortcut
{
    //[CreateAssetMenu(fileName = "Folder Shortcut Database", menuName = "ZeroX/Folder Shortcut Database", order = 0)]
    public class FolderShortcutDatabase : ScriptableObject
    {
        public FolderViewMode folderViewMode = FolderViewMode.FlatView;
        public List<FolderData> listFolderData = new List<FolderData>();

        public bool IsFolderExist(Object folderObject)
        {
            return listFolderData.Any(d => d.folderObject == folderObject);
        }

        public void AddFolder(Object folderObject)
        {
            if(IsFolderExist(folderObject))
                return;
            var data = new FolderData(folderObject);
            data.CheckAndRefreshName();

            if (listFolderData.Count > 0)
            {
                int maxTreeIndex = listFolderData.Max(fd => fd.treeIndex);
                data.treeIndex = maxTreeIndex + 1;
            }
            
            listFolderData.Add(data);
        }

        public void RemoveFolder(Object folderObject)
        {
            for (int i = 0; i < listFolderData.Count; i++)
            {
                if (listFolderData[i].folderObject == folderObject)
                {
                    listFolderData.RemoveAt(i);
                    i--;
                }
            }
        }

        public FolderData GetFolder(int index)
        {
            return listFolderData[index];
        }

        public int Count
        {
            get
            { 
                return listFolderData.Count;
            }
        }
    }
}