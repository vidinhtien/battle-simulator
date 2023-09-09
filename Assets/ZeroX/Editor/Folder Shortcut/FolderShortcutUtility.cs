using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ZeroX.FolderShortcut
{
    public static class FolderShortcutUtility
    {
        #region Core

        public static void ShowFolderContents(int folderInstanceID)
        {
            // Find the internal ProjectBrowser class in the editor assembly.
            Assembly editorAssembly = typeof(Editor).Assembly;
            System.Type projectBrowserType = editorAssembly.GetType("UnityEditor.ProjectBrowser");
         
            // This is the internal method, which performs the desired action.
            // Should only be called if the project window is in two column mode.
            MethodInfo showFolderContents = projectBrowserType.GetMethod(
                "ShowFolderContents", BindingFlags.Instance | BindingFlags.NonPublic);
         
            // Find any open project browser windows.
            Object[] projectBrowserInstances = Resources.FindObjectsOfTypeAll(projectBrowserType);
         
            //if (projectBrowserInstances.Length > 0)
            //{
                for (int i = 0; i < projectBrowserInstances.Length; i++)
                    ShowFolderContentsInternal(projectBrowserInstances[i], showFolderContents, folderInstanceID);
            //}
            //else
            //{
                //EditorWindow projectBrowser = OpenNewProjectBrowser(projectBrowserType);
                //ShowFolderContentsInternal(projectBrowser, showFolderContents, folderInstanceID);
            //}
        }

        private static bool IsEditorWindowLocked(Object editorWindow)
        {
            var type = editorWindow.GetType();
            BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            var propertyInfo = type.GetProperty("isLocked", bindingFlags);
            bool isLocked = (bool)propertyInfo.GetValue(editorWindow);

            return isLocked;
        }
 
        private static void ShowFolderContentsInternal(Object projectBrowser, MethodInfo showFolderContents, int folderInstanceID)
        {
            if(IsEditorWindowLocked(projectBrowser))
                return;
            
            // Sadly, there is no method to check for the view mode.
            // We can use the serialized object to find the private property.
            SerializedObject serializedObject = new SerializedObject(projectBrowser);
            bool inTwoColumnMode = serializedObject.FindProperty("m_ViewMode").enumValueIndex == 1;
         
            if (!inTwoColumnMode)
            {
                // If the browser is not in two column mode, we must set it to show the folder contents.
                MethodInfo setTwoColumns = projectBrowser.GetType().GetMethod("SetTwoColumns", BindingFlags.Instance | BindingFlags.NonPublic);
                setTwoColumns.Invoke(projectBrowser, null);
            }

            bool revealAndFrameInFolderTree = true;
            try
            {
                showFolderContents.Invoke(projectBrowser, new object[] { folderInstanceID, revealAndFrameInFolderTree });
            }
            catch (Exception e)
            {
                if (e is NullReferenceException == false)
                {
                    Debug.LogException(e);
                }
            }
        }
 
        private static EditorWindow OpenNewProjectBrowser(System.Type projectBrowserType)
        {
            EditorWindow projectBrowser = EditorWindow.GetWindow(projectBrowserType);
            projectBrowser.Show();
         
            // Unity does some special initialization logic, which we must call,
            // before we can use the ShowFolderContents method (else we get a NullReferenceException).
            MethodInfo init = projectBrowserType.GetMethod("Init", BindingFlags.Instance | BindingFlags.Public);
            init.Invoke(projectBrowser, null);
         
            return projectBrowser;
        }

        #endregion
        
        
        
        
        
        static void ReFormatListPath(List<string> listPath)
        {
            for (int i = 0; i < listPath.Count; i++)
            {
                listPath[i] = listPath[i].ToLower().Replace('\\', '/');
            }
        }

        static string ReFormatPath(string path)
        {
            return path.ToLower().Replace('\\', '/');
        }

        static string GetParentPath(string pathFormatted)
        {
            int index = pathFormatted.LastIndexOf("/");
            if (index == -1)
                return null;

            return pathFormatted.Substring(0, index);
        }
        
        public static bool HasParentFolderInList(List<string> listFolderPath, string childFolderPath)
        {
            if (listFolderPath.Count == 0)
                return false;
            
            ReFormatListPath(listFolderPath);
            childFolderPath = ReFormatPath(childFolderPath);
            
            HashSet<string> allParent = new HashSet<string>();
            string parentPath = GetParentPath(childFolderPath);
            while (string.IsNullOrEmpty(parentPath) == false)
            {
                allParent.Add(parentPath);
                parentPath = GetParentPath(parentPath);
            }

            if (allParent.Count == 0)
                return false;

            return listFolderPath.Any(fp => allParent.Contains(fp));
        }

        public static bool HasParentFolderInList(List<FolderData> listFolderData, FolderData childFolderData)
        {
            List<string> listFolderPath = listFolderData.Select(fd => AssetDatabase.GetAssetPath(fd.folderObject)).ToList();
            string childFolderPath = AssetDatabase.GetAssetPath(childFolderData.folderObject);
            return HasParentFolderInList(listFolderPath, childFolderPath);
        }

        public static List<FolderData> GetListFolderNoHasParentInList(List<FolderData> listFolderData)
        {
            List<FolderData> list = new List<FolderData>();
            foreach (var fd in listFolderData)
            {
                if(HasParentFolderInList(listFolderData, fd) == false)
                    list.Add(fd);
            }

            
            return list;
        }
        
        static bool IsParentFolder(string childFolderPathFormatted, string parentFolderPathFormatted)
        {
            string parentPath = GetParentPath(childFolderPathFormatted);
            while (string.IsNullOrEmpty(parentPath) == false)
            {
                if (parentFolderPathFormatted == parentPath)
                    return true;
                
                parentPath = GetParentPath(parentPath);
            }

            return false;
        }
        
        public static List<string> GetListChildFolder(List<string> listFolderPath, string parentFolderPath)
        {
            if(listFolderPath.Count == 0)
                return new List<string>();
            
            ReFormatListPath(listFolderPath);
            parentFolderPath = ReFormatPath(parentFolderPath);

            List<string> listChildFolder = new List<string>();
            foreach (var folderPath in listFolderPath)
            {
                if(folderPath == parentFolderPath)
                    continue;
                
                if(IsParentFolder(folderPath, parentFolderPath))
                    listChildFolder.Add(folderPath);
            }

            return listChildFolder;
        }
        
        public static List<FolderData> GetListChildFolder(List<FolderData> listFolderData, FolderData parentFolderData)
        {
            if(listFolderData.Count == 0)
                return new List<FolderData>();

            string parentFolderPath = AssetDatabase.GetAssetPath(parentFolderData.folderObject);
            parentFolderPath = ReFormatPath(parentFolderPath);
            
            List<FolderData> listChildFolder = new List<FolderData>();
            foreach (var folderData in listFolderData)
            {
                if(folderData == parentFolderData)
                    continue;

                string folderPath = AssetDatabase.GetAssetPath(folderData.folderObject);
                folderPath = ReFormatPath(folderPath);
                
                if(IsParentFolder(folderPath, parentFolderPath))
                    listChildFolder.Add(folderData);
            }

            return listChildFolder;
        }

        public static List<FolderData> OrderByTreeIndexAndReIndex(List<FolderData> listFolderData)
        {
            var list = listFolderData.OrderBy(fd => fd.treeIndex).ToList();
            for (int i = 0; i < list.Count; i++)
            {
                list[i].treeIndex = i;
            }

            return list;
        }
    }
}