using System;
using UnityEditor;
using UnityEngine;

namespace ZeroX.FolderShortcut
{
    public class FolderButtonFlatDrawer
    {
        public enum LastManipulation
        {
            None, MoveUp, MoveDown, RemoveInNormalMode, RemoveInEditMode
        }

        public FolderButtonFlatDrawer(FolderData folderData)
        {
            this.folderData = folderData;
        }
        
        private static GUIStyle buttonStyle;
        private static GUIStyle ButtonStyle
        {
            get
            {
                if (buttonStyle == null)
                {
                    buttonStyle = new GUIStyle(EditorStyles.miniButton);
                    buttonStyle.alignment = TextAnchor.MiddleLeft;
                    return buttonStyle;
                }

                return buttonStyle;
            }
        }
        

        public FolderData folderData;
        public bool editMode = false;
        
        public LastManipulation lastManipulation = LastManipulation.None;
        public bool isFirst = false;
        public bool isLast = false;
        
        //Actions
        public Action<string> recordUndoAction;

        void RecordUndo(string name)
        {
            recordUndoAction?.Invoke(name);
        }

        public void Draw()
        {
            if(editMode)
                DrawInEditMode();
            else
                DrawNormal();
        }
        
        void DrawNormal()
        {
            Color defaultColor = GUI.backgroundColor;
            if (folderData.IsDefaultColor() == false)
                GUI.backgroundColor = folderData.buttonColor;

            if (GUILayout.Button(folderData.GetName()))
            {
                if (folderData.folderObject == null)
                {
                    bool confirm = EditorUtility.DisplayDialog("Cannot found shortcut", "Delete this shortcut?", "Yes", "No");
                    if (confirm)
                        lastManipulation = LastManipulation.RemoveInNormalMode;
                }
                else
                {
                    EditorUtility.FocusProjectWindow();
                    int id = folderData.folderObject.GetInstanceID();
                    FolderShortcutUtility.ShowFolderContents(id);
                }
            }
            
            GUI.backgroundColor = defaultColor;
        }

        void DrawInEditMode()
        {
            GUILayout.BeginHorizontal();
                    
            DrawButtonOrder();
            DrawBoxEditFolderDisplayName();
            DrawButtonColorField();
            DrawButtonRemoveFolder();
                    
            GUILayout.EndHorizontal();
        }
        
        bool DrawGUILayoutButtonOrderFolder(Texture icon, bool enabled)
        {
            Color defaultContentColor = GUI.contentColor;
            if(EditorGUIUtility.isProSkin == false)
                GUI.contentColor = Color.black;
                
            Color defaultBgColor = GUI.backgroundColor;
            if (folderData.IsDefaultColor() == false)
                GUI.backgroundColor = folderData.buttonColor;
                
            GUI.enabled = enabled;
            bool clicked = GUILayout.Button(icon, GUILayout.Width(20), GUILayout.Height(18));
            GUI.enabled = true;
                
            //Color
            if (folderData.IsDefaultColor() == false)
                GUI.backgroundColor = defaultBgColor;
                
            if(EditorGUIUtility.isProSkin == false)
                GUI.contentColor = defaultContentColor;

            return clicked;
        }
        
        void DrawButtonOrder()
        {
            //Button move up
            if (isFirst)
            {
                DrawGUILayoutButtonOrderFolder(AssetLibrary.UpArrowOrderIcon, false);
            }
            else
            {
                bool clicked = DrawGUILayoutButtonOrderFolder(AssetLibrary.UpArrowOrderIcon, true);
                if (clicked)
                    lastManipulation = LastManipulation.MoveUp;
            }
            
            //Button move down
            if (isLast)
            {
                DrawGUILayoutButtonOrderFolder(AssetLibrary.DownArrowOrderIcon, false);
            }
            else
            {
                bool clicked = DrawGUILayoutButtonOrderFolder(AssetLibrary.DownArrowOrderIcon, true);
                if (clicked)
                    lastManipulation = LastManipulation.MoveDown;
            }
        }
        
        void DrawBoxEditFolderDisplayName()
        {
            if (folderData.folderObject == null)
            {
                GUILayout.Label("Missing Reference");
                return;
            }
            
            folderData.displayName = GUILayout.TextField(folderData.displayName, GUILayout.MinWidth(20));
        }
        
        void DrawButtonColorField()
        {
            folderData.buttonColor = EditorGUILayout.ColorField(folderData.buttonColor, GUILayout.MaxWidth(40));
        }
        
        void DrawButtonRemoveFolder()
        {
            //Color
            Color defaultColor = GUI.backgroundColor;
            if (folderData.IsDefaultColor() == false)
                GUI.backgroundColor = folderData.buttonColor;
            
            if (GUILayout.Button("-", GUILayout.MaxWidth(25)))
            {
                lastManipulation = LastManipulation.RemoveInEditMode;
            }
            
            //Color
            if (folderData.IsDefaultColor() == false)
                GUI.backgroundColor = defaultColor;
        }
    }
}