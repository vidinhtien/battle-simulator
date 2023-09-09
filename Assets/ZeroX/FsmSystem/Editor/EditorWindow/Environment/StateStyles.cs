using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ZeroX.FsmSystem.Editors
{
    public class StateStyles
    {
        public enum Style
        {
            NormalState = 0,
            AnyState,
            ParallelSate,
            EntryState,
            CurrentState,
            DuplicatedStateName,
            MissingStateName,
            TransitionBox,
            TransitionBoxOn,
            SelectedStateOutline,
        }
        
        [NonSerialized] Dictionary<Style, GUIStyle> styleDictionary = null;

        public StateStyles()
        {
            Init();
        }

        void Init()
        {
            if(styleDictionary == null)
                styleDictionary = new Dictionary<Style, GUIStyle>();
            else
                styleDictionary.Clear();
            
            styleDictionary.Add(Style.NormalState, CreateNormalStateStyle());
            styleDictionary.Add(Style.AnyState, CreateAnyStateStyle());
            styleDictionary.Add(Style.ParallelSate, CreateParallelStateStyle());
            styleDictionary.Add(Style.EntryState, CreateEntryStateStyle());
            styleDictionary.Add(Style.CurrentState, CreateCurrentStateStyle());
            styleDictionary.Add(Style.TransitionBox, CreateTransitionBoxStyle());
            styleDictionary.Add(Style.TransitionBoxOn, CreateTransitionBoxOnStyle());
            styleDictionary.Add(Style.SelectedStateOutline, CreateSelectedStateOutlineStyle());
            styleDictionary.Add(Style.DuplicatedStateName, CreateDuplicatedStateName());
            styleDictionary.Add(Style.MissingStateName, CreateMissingStateName());
        }

        void InitIfNeed()
        {
            if (styleDictionary == null || styleDictionary.Count == 0)
            {
                Init();
                return;
            }

            if (styleDictionary.Values.First().normal.background == null)
            {
                Init();
            }
        }

        public static Texture2D CreateTextureWithColor(Color color)
        {
            Color[] pixel = new Color[1];
            pixel[0] = color;

            Texture2D texture = new Texture2D(1, 1);
            texture.SetPixels(pixel);
            texture.Apply();
            return texture;
        }
        
        public static Texture2D CreateTextureWithBorder(Color color, Color borderColor, int borderSize)
        {
            int size = (borderSize * 2) + 1; //chắc chắn số lẻ
            Color[] pixel = new Color[size * size];
            for (int i = 0; i < pixel.Length; i++)
            {
                if (i == ((size * size) - 1) / 2)
                {
                    pixel[i] = color;
                }
                else
                {
                    pixel[i] = borderColor;
                }
            }

            Texture2D texture = new Texture2D(size, size);
            texture.filterMode = FilterMode.Point;
            texture.SetPixels(pixel);
            texture.Apply();
            return texture;
        }

        GUIStyle CreateStateTitleStyle(Color color, Color borderColor, int borderSize, Color textColor)
        {
            GUIStyle guiStyle = new GUIStyle();
            guiStyle.normal.background = CreateTextureWithBorder(color, borderColor, borderSize);
            guiStyle.border = new RectOffset(borderSize, borderSize, borderSize, borderSize);
            
            guiStyle.alignment = TextAnchor.MiddleCenter;
            guiStyle.fontStyle = FontStyle.Bold;
            guiStyle.fontSize = 12;
            guiStyle.wordWrap = false;
            guiStyle.richText = false;
            guiStyle.normal.textColor = textColor;
            
            int padding = 5;
            guiStyle.padding = new RectOffset(padding, padding, padding, padding);
            
            return guiStyle;
        }
        
        GUIStyle CreateNormalStateStyle()
        {
            Color color = new Color(0.2862f, 0.3019f, 0.3137f, 1);
            Color borderColor = new Color(0, 0, 0, 1);
            int borderSize = 1;
            Color textColor = Color.white;

            return CreateStateTitleStyle(color, borderColor, borderSize, textColor);
        }
        
        GUIStyle CreateAnyStateStyle()
        {
            Color color = new Color(0.3607f, 0.6196f, 0.5490f, 1);
            Color borderColor = new Color(0, 0, 0, 1);
            int borderSize = 1;
            Color textColor = Color.white;

            return CreateStateTitleStyle(color, borderColor, borderSize, textColor);
        }
        
        GUIStyle CreateParallelStateStyle()
        {
            Color color = new Color(0.2f, 0.3372f, 0.5686f, 1);
            Color borderColor = new Color(0, 0, 0, 1);
            int borderSize = 1;
            Color textColor = Color.white;

            return CreateStateTitleStyle(color, borderColor, borderSize, textColor);
        }
        
        GUIStyle CreateEntryStateStyle()
        {
            Color color = new Color(0.1333f, 0.4705f, 0.2235f, 1);
            Color borderColor = new Color(0, 0, 0, 1);
            int borderSize = 1;
            Color textColor = Color.white;

            return CreateStateTitleStyle(color, borderColor, borderSize, textColor);
        }
        
        GUIStyle CreateCurrentStateStyle()
        {
            Color color = new Color(0.6313f, 0.3607f, 0.0980f, 1);
            Color borderColor = new Color(0, 0, 0, 1);
            int borderSize = 1;
            Color textColor = Color.white;

            return CreateStateTitleStyle(color, borderColor, borderSize, textColor);
        }

        GUIStyle CreateDuplicatedStateName()
        {
            Color color = new Color(0.63f, 0.11f, 0.08f);
            Color borderColor = new Color(0, 0, 0, 1);
            int borderSize = 1;
            Color textColor = Color.white;

            return CreateStateTitleStyle(color, borderColor, borderSize, textColor);
        }
        
        GUIStyle CreateMissingStateName()
        {
            Color color = new Color(0.63f, 0.11f, 0.08f);
            Color borderColor = new Color(0, 0, 0, 1);
            int borderSize = 1;
            Color textColor = Color.white;

            return CreateStateTitleStyle(color, borderColor, borderSize, textColor);
        }
        
        GUIStyle CreateTransitionBoxStyle()
        {
            Color color = new Color(0.6313f, 0.6313f, 0.6313f, 1);
            Color borderColor = new Color(0, 0, 0, 1);
            int borderSize = 1;
            
            GUIStyle guiStyle = new GUIStyle();
            guiStyle.normal.background = CreateTextureWithBorder(color, borderColor, borderSize);
            guiStyle.border = new RectOffset(borderSize, borderSize, borderSize, borderSize);
            
            guiStyle.alignment = TextAnchor.MiddleCenter;
            guiStyle.fontStyle = FontStyle.Normal;
            guiStyle.fontSize = 12;
            guiStyle.wordWrap = false;
            guiStyle.richText = false;
            guiStyle.normal.textColor = Color.black;
            
            int padding = 5;
            guiStyle.padding = new RectOffset(padding, padding, padding, padding);
            
            return guiStyle;
        }
        
        GUIStyle CreateTransitionBoxOnStyle()
        {
            Color color = new Color(0.1882f, 0.6980f, 0.9843f, 1);
            Color borderColor = new Color(0, 0, 0, 1);
            int borderSize = 1;
            
            GUIStyle guiStyle = new GUIStyle();
            guiStyle.normal.background = CreateTextureWithBorder(color, borderColor, borderSize);
            guiStyle.border = new RectOffset(borderSize, borderSize, borderSize, borderSize);
            
            guiStyle.alignment = TextAnchor.MiddleCenter;
            guiStyle.fontStyle = FontStyle.Normal;
            guiStyle.fontSize = 12;
            guiStyle.wordWrap = false;
            guiStyle.richText = false;
            guiStyle.normal.textColor = Color.white;
            
            int padding = 5;
            guiStyle.padding = new RectOffset(padding, padding, padding, padding);
            
            return guiStyle;
        }
        
        GUIStyle CreateSelectedStateOutlineStyle()
        {
            Color color = new Color(0.1882f, 0.6980f, 0.9843f, 1);

            GUIStyle guiStyle = new GUIStyle();
            guiStyle.normal.background = CreateTextureWithColor(color);
            guiStyle.border = new RectOffset(0, 0, 0, 0);

            return guiStyle;
        }

        public void ApplyZoomFactor(float zoomFactor)
        {
            foreach(GUIStyle style in styleDictionary.Values)
            {
                style.fontSize = Mathf.RoundToInt(10 * zoomFactor);
                int padding = Mathf.RoundToInt(5 * zoomFactor);
                style.padding = new RectOffset(padding, padding, padding, padding);
            }
        }
        
        public GUIStyle Get(Style style)
        {
            //Return appropriate style
            InitIfNeed();
            return styleDictionary[style];
        }

        public GUIStyle GetForTransitionBox(bool isSelected, bool duplicatedName, bool missingName)
        {
            var style = Get(isSelected ? Style.TransitionBoxOn : Style.TransitionBox);

            if (duplicatedName || missingName)
            {
                style.normal.textColor = Color.red;
            }
            else
            {
                style.normal.textColor = isSelected ? Color.white : Color.black;
            }
            

            return style;
        }
    }
}