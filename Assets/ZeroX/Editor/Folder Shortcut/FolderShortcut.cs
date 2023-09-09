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
    public class FolderShortcut : EditorWindow
    {
        public string pathToDatabase = "Assets/ZeroX/Editor/Folder Shortcut/FolderShortcutDatabase.asset";
        private FolderShortcutDatabase m_database;

        private Vector2 m_scrollPos = Vector2.zero;
        private string m_folderNameToAdd;

        private bool editMode = false;

        private FolderViewMode folderViewMode => m_database.folderViewMode;
        
        void LoadDatabase()
        {
            m_database = AssetDatabase.LoadAssetAtPath<FolderShortcutDatabase>(pathToDatabase);
        }
        
        void RefreshDatabase()
        {
            foreach (var folderData in m_database.listFolderData)
            {
                folderData.CheckAndRefreshName();
            }
        }

        void CreateDatabase()
        {
            var asset = ScriptableObject.CreateInstance<FolderShortcutDatabase>();
            AssetDatabase.CreateAsset(asset, pathToDatabase);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        
        [MenuItem("Tools/ZeroX/Editor/Folder Shortcut", priority = 0)]
        private static void ShowWindow()
        {
            var window = GetWindow<FolderShortcut>();
            window.titleContent = new UnityEngine.GUIContent("Folder Shortcut");
            window.minSize = new Vector2(20, 20);
            window.Show();
            window.LoadDatabase();
        }
        
        void SaveDatabase()
        {
            EditorUtility.SetDirty(m_database);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        void SetDirtyDatabase()
        {
            EditorUtility.SetDirty(m_database);
        }
        
        void RecordUndo(string name)
        {
            Undo.RecordObject(m_database, name);
        }

        private void Awake()
        {
            LoadDatabase();
        }

        private void OnGUI()
        {
            if (m_database == null)
            {
                LoadDatabase();
                if (m_database == null)
                {
                    EditorStyles.label.wordWrap = true;
                    GUILayout.Label("Cannot found Database", EditorStyles.label);
                    if (GUILayout.Button("Create Database"))
                    {
                        CreateDatabase();
                    }
                    return;
                }
            }
            
            RefreshDatabase();
            
            GUILayout.Label("Database", EditorStyles.boldLabel);
            DrawButtonAddFolderShortcut();
            
            GUILayout.BeginHorizontal();
            DrawPopupFolderViewMode();
            DrawButtonOnOffEditMode();
            GUILayout.EndHorizontal();
            
            DrawListFolder();
        }

        void DrawPopupFolderViewMode()
        {
            var newFolderView = (FolderViewMode)EditorGUILayout.EnumPopup(folderViewMode, GUILayout.MaxWidth((80)));
            if (m_database.folderViewMode != newFolderView)
            {
                RecordUndo("Change folder view mode");
                m_database.folderViewMode = newFolderView;
                SaveDatabase();
            }
        }

        void DrawButtonOnOffEditMode()
        {
            var defaultColor = GUI.backgroundColor;
            if(editMode)
                GUI.backgroundColor = new Color(1f, 0.99f, 0.54f);
            string label = editMode ? "Save" : "Edit Mode";
            if (GUILayout.Button(label))
            {
                editMode = !editMode;

                if (editMode == false) //Nếu vừa tắt edit mode, thì save lại các thay đổi
                {
                    ValidateDatabaseAfterExitEditMode();
                    SaveDatabase();
                }
            }

            GUI.backgroundColor = defaultColor;
        }
        
        void ValidateDatabaseAfterExitEditMode()
        {
            if(m_database == null)
                return;

            foreach (var folderData in m_database.listFolderData)
            {
                if (string.IsNullOrEmpty(folderData.displayName))
                    folderData.displayName = folderData.folderName;
            }
        }
        
        void DrawButtonAddFolderShortcut()
        {
            if (GUILayout.Button("Add Folders Selected", GUILayout.MinWidth(20)) == false)
                return;

            var listObject = Selection.objects;
            
            if (listObject == null || listObject.Length == 0)
            {
                EditorUtility.DisplayDialog("Choose folder", "Please choose a folder in project window", "OK");
                return;
            }

            if (listObject.Length == 1)
            {
                HandleAddOneFolder(listObject[0]);
            }
            else
            {
                HandleAddListFolder(listObject);
            }
            
            m_scrollPos = new Vector2(0, float.MaxValue);
        }

        void HandleAddOneFolder(Object obj)
        {
            int instanceID = obj.GetInstanceID();
            if (ProjectWindowUtil.IsFolder(instanceID) == false)
            {
                EditorUtility.DisplayDialog("Choose folder", "Please choose a folder in project window", "OK");
                return;
            }
                
                
            if (m_database.IsFolderExist(obj))
            {
                string path = AssetDatabase.GetAssetPath(obj);
                EditorUtility.DisplayDialog("Shortcut Exist", "Path selecting: " + path, "OK");
                return;
            }
            
            m_database.AddFolder(obj);
            if(editMode == false)
                SaveDatabase();
        }

        void HandleAddListFolder(Object[] listObject)
        {
            int added = 0;
            int exists = 0;
            foreach (var obj in listObject)
            {
                int instanceID = obj.GetInstanceID();
                if (ProjectWindowUtil.IsFolder(instanceID) == false)
                    continue;

                if (m_database.IsFolderExist(obj))
                {
                    exists++;
                    string path = AssetDatabase.GetAssetPath(obj);
                    Debug.LogFormat("This folder already exists in the database: {0}", path);
                    continue;
                }
                
                m_database.AddFolder(obj);
                added++;
            }
            
            if(added == 0 && exists == 0)
                EditorUtility.DisplayDialog("Choose folder", "Please choose a folder in project window", "OK");
            
            if(added > 0)
                SaveDatabase();
        }

        Color OppositeColor(Color color)
        {
            float h, s, v;
            Color.RGBToHSV(color, out h, out s, out v);
            h = (h + 0.5f) % 1f;
            return Color.HSVToRGB(h, s, v);
        }

        void ChangeFolderIndex(FolderData folderData, int newIndex)
        {
            int index = m_database.listFolderData.IndexOf(folderData);
            FolderData oldFolderData = m_database.listFolderData[newIndex];
            m_database.listFolderData[newIndex] = folderData;
            m_database.listFolderData[index] = oldFolderData;
        }
        
        void DrawListFolder()
        {
            if(folderViewMode == FolderViewMode.FlatView)
                DrawListFolderFlat();
            else if (folderViewMode == FolderViewMode.TreeView)
                DrawListFolderTree();
               
        }

        void DrawListFolderFlat()
        {
            GUILayout.Label("List Shortcut", EditorStyles.boldLabel);
            m_scrollPos = GUILayout.BeginScrollView(m_scrollPos,GUILayout.Width(this.position.width), GUILayout.ExpandHeight(true));

            List<FolderButtonFlatDrawer> listFolderButtonFlatDrawer = new List<FolderButtonFlatDrawer>();
            for (int i = 0; i < m_database.Count; i++)
            {
                var folderData = m_database.GetFolder(i);
                FolderButtonFlatDrawer drawer = new FolderButtonFlatDrawer(folderData);
                listFolderButtonFlatDrawer.Add(drawer);
                //Init các action
                drawer.recordUndoAction = RecordUndo;
                
                //Init các trạng thái
                drawer.isFirst = i == 0;
                drawer.isLast = i == m_database.Count - 1;
                drawer.editMode = editMode;
                
                //Vẽ
                drawer.Draw();
            }
            
            GUILayout.EndScrollView();
            
            HandleFolderButtonFlatDrawerManipulation(listFolderButtonFlatDrawer);
        }

        void HandleFolderButtonFlatDrawerManipulation(List<FolderButtonFlatDrawer> listFolderButtonFlatDrawer)
        {
            for (int i = 0; i < listFolderButtonFlatDrawer.Count; i++)
            {
                FolderButtonFlatDrawer drawer = listFolderButtonFlatDrawer[i];
                
                if (drawer.lastManipulation == FolderButtonFlatDrawer.LastManipulation.RemoveInNormalMode)
                {
                    RecordUndo("remove folder shortcut in normal mode - flat");
                    m_database.RemoveFolder(drawer.folderData.folderObject);
                    SaveDatabase();
                    return;
                }
                
                if (drawer.lastManipulation == FolderButtonFlatDrawer.LastManipulation.RemoveInEditMode)
                {
                    RecordUndo("remove folder shortcut in edit mode - flat");
                    m_database.RemoveFolder(drawer.folderData.folderObject);
                    return;
                }
                
                if(drawer.lastManipulation == FolderButtonFlatDrawer.LastManipulation.MoveUp)
                {
                    RecordUndo("move up folder shortcut - flat");
                    
                    int index = m_database.listFolderData.IndexOf(drawer.folderData);
                    ChangeFolderIndex(drawer.folderData, index - 1);
                    return;
                }
                
                if(drawer.lastManipulation == FolderButtonFlatDrawer.LastManipulation.MoveDown)
                {
                    RecordUndo("move down folder shortcut - flat");
                    
                    int index = m_database.listFolderData.IndexOf(drawer.folderData);
                    ChangeFolderIndex(drawer.folderData, index + 1);
                    return;
                }
            }
        }
        
        
        void DrawListFolderTree()
        {
            GUILayout.Label("List Shortcut", EditorStyles.boldLabel);
            m_scrollPos = GUILayout.BeginScrollView(m_scrollPos,GUILayout.Width(this.position.width), GUILayout.ExpandHeight(true));
            
            DrawListFolderTree(m_database.listFolderData);

            GUILayout.EndScrollView();
        }
        
        
        void DrawListFolderTree(List<FolderData> listFolderData)
        {
            var listFolderDataNoHasParentInList = FolderShortcutUtility.GetListFolderNoHasParentInList(listFolderData);
            listFolderDataNoHasParentInList = FolderShortcutUtility.OrderByTreeIndexAndReIndex(listFolderDataNoHasParentInList);
            
            List<FolderButtonTreeDrawer> listFolderButtonTreeDrawer = new List<FolderButtonTreeDrawer>();

            for (int i = 0; i < listFolderDataNoHasParentInList.Count; i++)
            {
                var folderData = listFolderDataNoHasParentInList[i];
                FolderButtonTreeDrawer drawer = new FolderButtonTreeDrawer(folderData);
                listFolderButtonTreeDrawer.Add(drawer);
                //Init các action
                drawer.recordUndoAction = RecordUndo;
                drawer.setDirtyDatabaseAction = SetDirtyDatabase;
                
                //Init các trạng thái
                drawer.isFirst = i == 0;
                drawer.isLast = i == listFolderDataNoHasParentInList.Count - 1;
                drawer.editMode = editMode;
                
                var listChild = FolderShortcutUtility.GetListChildFolder(listFolderData, folderData);
                drawer.hasChild = listChild.Count > 0;
                
                //Vẽ
                drawer.Draw();
                
                if (drawer.showChild)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(23);
                    GUILayout.BeginVertical();
                    DrawListFolderTree(listChild);
                    //GUILayout.Space(10);
                    GUILayout.EndVertical();
                    GUILayout.EndHorizontal();
                }
            }
            
            HandleFolderButtonTreeDrawerManipulation(listFolderButtonTreeDrawer);
        }

        void ChangeTreeIndex(List<FolderData> listFolderData, FolderData folderData, int newIndex)
        {
            int oldIndex = folderData.treeIndex;
            FolderData folderDataOld = listFolderData.FirstOrDefault(fd => fd.treeIndex == newIndex);
            if (folderDataOld != null)
                folderDataOld.treeIndex = oldIndex;
            
            folderData.treeIndex = newIndex;
        }
        
        void HandleFolderButtonTreeDrawerManipulation(List<FolderButtonTreeDrawer> listFolderButtonTreeDrawer)
        {
            for (int i = 0; i < listFolderButtonTreeDrawer.Count; i++)
            {
                FolderButtonTreeDrawer drawer = listFolderButtonTreeDrawer[i];
                
                if (drawer.lastManipulation == FolderButtonTreeDrawer.LastManipulation.RemoveInNormalMode)
                {
                    RecordUndo("remove folder shortcut in normal mode");
                    m_database.RemoveFolder(drawer.folderData.folderObject);
                    return;
                }
                
                if (drawer.lastManipulation == FolderButtonTreeDrawer.LastManipulation.RemoveInEditMode)
                {
                    RecordUndo("remove folder shortcut in edit mode");
                    m_database.RemoveFolder(drawer.folderData.folderObject);
                    return;
                }

                List<FolderData> listFolderData = listFolderButtonTreeDrawer.Select(fb => fb.folderData).ToList();
                
                if(drawer.lastManipulation == FolderButtonTreeDrawer.LastManipulation.MoveUp)
                {
                    RecordUndo("move up folder shortcut - tree");
                    ChangeTreeIndex(listFolderData, drawer.folderData, drawer.folderData.treeIndex - 1);
                    return;
                }
                
                if(drawer.lastManipulation == FolderButtonTreeDrawer.LastManipulation.MoveDown)
                {
                    RecordUndo("move down folder shortcut - tree");
                    ChangeTreeIndex(listFolderData, drawer.folderData, drawer.folderData.treeIndex + 1);
                    return;
                }
            }
        }
    }
}