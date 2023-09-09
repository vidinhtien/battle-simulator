using UnityEditor;
using UnityEngine;

namespace ZeroX.FolderShortcut
{
    public static class AssetLibrary
    {
        private static Texture m_upArrowIcon, m_downArrowIcon;
        private static Texture m_rightArrowIcon;
        
        private static Texture m_upArrowOrderIcon, m_downArrowOrderIcon;

        public static Texture UpArrowIcon
        {
            get
            {
                if (m_upArrowIcon == null)
                    m_upArrowIcon = AssetDatabase.LoadAssetAtPath<Texture>("Assets/ZeroX/Editor/Folder Shortcut/Icon/up arrow.png");

                return m_upArrowIcon;
            }
        }
        
        public static Texture DownArrowIcon
        {
            get
            {
                if (m_downArrowIcon == null)
                    m_downArrowIcon = AssetDatabase.LoadAssetAtPath<Texture>("Assets/ZeroX/Editor/Folder Shortcut/Icon/down arrow.png");

                return m_downArrowIcon;
            }
        }
        
        public static Texture RightArrowIcon
        {
            get
            {
                if (m_rightArrowIcon == null)
                    m_rightArrowIcon = AssetDatabase.LoadAssetAtPath<Texture>("Assets/ZeroX/Editor/Folder Shortcut/Icon/right arrow.png");

                return m_rightArrowIcon;
            }
        }
        
        public static Texture UpArrowOrderIcon
        {
            get
            {
                if (m_upArrowOrderIcon == null)
                    m_upArrowOrderIcon = AssetDatabase.LoadAssetAtPath<Texture>("Assets/ZeroX/Editor/Folder Shortcut/Icon/up arrow order.png");

                return m_upArrowOrderIcon;
            }
        }
        
        public static Texture DownArrowOrderIcon
        {
            get
            {
                if (m_downArrowOrderIcon == null)
                    m_downArrowOrderIcon = AssetDatabase.LoadAssetAtPath<Texture>("Assets/ZeroX/Editor/Folder Shortcut/Icon/down arrow order.png");

                return m_downArrowOrderIcon;
            }
        }
    }
}