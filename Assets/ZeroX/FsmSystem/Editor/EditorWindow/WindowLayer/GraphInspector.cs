using System;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace ZeroX.FsmSystem.Editors
{
    public class GraphInspector : StateMachineGraphWindowLayer
    {
        enum InspectType
        {
            Empty, State, Transition, MultiState
        }

        enum NameConvention
        {
            Free, UnderScore, Pascal
        }
        
        private StateMachineGraphWindow EditorWindow { get; set; }
        
        private float inspectorWidth = 250;
        public Rect inspectorRect;
        private Rect resizeBoxRect;

        
        private bool downedMouseOnResizeBoxRect = false;
        private GUIStyle titleStyle;
        private GUIStyle subTitleStyle;
        private Vector2 scrollPos = Vector2.zero;
        private InspectType inspectType = InspectType.Empty;
        private NameConvention nameConvention = NameConvention.Free;
        
        
        private SerializedProperty stateSp = null;
        private SerializedProperty transitionSp = null;
        private string currentSerializedPropertyName;
        //private List<SerializedProperty> listStateSp = new List<SerializedProperty>();
        private int multiStateCount = 0;
        
        
        public Rect InspectorRect => inspectorRect;

        public GraphInspector(StateMachineGraphWindow editorWindow) : base(editorWindow)
        {
            this.EditorWindow = editorWindow;
        }

        

        public override void Draw(Rect rect)
        {
            UpdateInspectorRect(rect);
            DrawInspectorBackground();
            DrawResizeBoxRect();

            //Validate Data
            if (IsCurrentInspectValid() == false)
            {
                InspectEmpty();
                return;
            }

            //Draw Data
            GUILayout.BeginArea(inspectorRect);
            DrawTitle();
            DrawSubTitle();
            DrawScrollViewInspect();
            GUILayout.EndArea();
        }

        void UpdateInspectorRect(Rect rect)
        {
            inspectorRect = new Rect(0, 0, inspectorWidth, rect.height - EditorStyles.toolbar.fixedHeight);
            inspectorRect.width = inspectorWidth;
            inspectorRect.height = rect.height - EditorStyles.toolbar.fixedHeight;
            inspectorRect.x = rect.xMax - inspectorRect.width;
            inspectorRect.y = rect.yMax - inspectorRect.height;
        }

        void DrawInspectorBackground()
        {
            Color backgroundColor = EditorGUIUtility.isProSkin
                ? new Color32(56, 56, 56, 255)
                : new Color32(194, 194, 194, 255);
            
            EditorGUI.DrawRect(inspectorRect, backgroundColor);
        }

        void DrawResizeBoxRect()
        {
            resizeBoxRect = inspectorRect;
            resizeBoxRect.width = 7;
            //resizeBoxRect.x -= resizeBoxRect.width / 2f;

            Rect resizeBoxRectVisible = inspectorRect;
            resizeBoxRectVisible.width = 1.5f;
            //resizeBoxRectVisible.x -= resizeBoxRectVisible.width / 2f;

            EditorGUI.DrawRect(resizeBoxRectVisible, Color.black); 
            EditorGUIUtility.AddCursorRect(resizeBoxRect, MouseCursor.SplitResizeLeftRight);

            //Process Event
            Event e = Event.current;

            switch (e.type)
            {
                case EventType.MouseDown:
                {
                    if ((MouseButton) e.button == MouseButton.LeftMouse)
                    {
                        downedMouseOnResizeBoxRect = resizeBoxRect.Contains(e.mousePosition);
                    }

                    break;
                }
                case EventType.MouseUp:
                {
                    if ((MouseButton) e.button == MouseButton.LeftMouse)
                        downedMouseOnResizeBoxRect = false;

                    break;
                }
                case EventType.MouseDrag:
                {
                    if (downedMouseOnResizeBoxRect)
                    {
                        inspectorWidth -= e.delta.x;
                        inspectorWidth = Mathf.Clamp(inspectorWidth, 50, EditorWindow.Rect.width * 0.8f);
                    }

                    break;
                }
            }
        }

        void UnFocusControl()
        {
            EditorWindow.UnFocusControl();
        }
        
        
        #region Process Event

        protected override void OnLeftMouseDown(Vector2 position)
        {
            if(inspectorRect.Contains(position))
                Event.current.Use();
        }

        protected override void OnRightMouseUp(Vector2 position)
        {
            if(inspectorRect.Contains(position))
                Event.current.Use();
        }

        protected override void OnScrollWheel(Vector2 position)
        {
            if(inspectorRect.Contains(position))
                Event.current.Use();
        }

        #endregion

        #region Utility

        static string GetStateTypeName(SerializedProperty stateSp)
        {
            string s = stateSp.type; //managedReference<UnityEventStateLite>
            s = s.Substring(17);
            return s.Substring(0, s.Length - 1);
        }

        private bool IsCurrentInspectValid()
        {
            if (inspectType == InspectType.Empty)
                return true;
            
            if (EditorWindow.Context.HasGraph == false)
                return false;

            try
            {
                switch (inspectType)
                {
                    case InspectType.State:
                        return stateSp.name == currentSerializedPropertyName;
                    case InspectType.Transition:
                        return transitionSp.name == currentSerializedPropertyName;
                    case InspectType.MultiState:
                        return true;
                    default:
                    {
                        Debug.LogError("Not code for inspect type: " + inspectType);
                        return true;
                    }
                }
            }
            catch (Exception e)
            {
                return false;
            }
        }

        static string PascalCase(string s)
        {
            if (s == null)
                return null;
            
            if (s.Length == 0)
                return s;
            
            StringBuilder sb = new StringBuilder();

            bool nextUpper = true;
            for (int i = 0; i < s.Length; i++)
            {
                char c = s[i];
                
                if (c == ' ' || c == '_')
                {
                    nextUpper = true;
                    continue;
                }
                
                if (nextUpper)
                {
                    nextUpper = false;
                    sb.Append(char.ToUpper(c));
                }
                else
                {
                    sb.Append(c);
                }
            }

            return sb.ToString();
        }

        static string UnderScoreCase(string s)
        {
            if (s == null)
                return null;
            
            if (s.Length == 0)
                return s;

            return s.Replace(" ", "_");
        }

        private void ApplyFieldNameValueConvention(SerializedProperty serializedProperty, string fieldName)
        {
            var nameSp = serializedProperty.FindPropertyRelative(fieldName);
            string oldName = nameSp.stringValue;
            string newName = null;

            if (nameConvention == NameConvention.Free)
            {
                newName = oldName;
            }
            else if (nameConvention == NameConvention.Pascal)
            {
                newName = PascalCase(oldName);
            }
            else if (nameConvention == NameConvention.UnderScore)
            {
                newName = UnderScoreCase(oldName);
            }
            else
            {
                newName = oldName;
                Debug.LogError("Not code for name convention: " + nameConvention);
            }

            if (newName != oldName)
                nameSp.stringValue = newName;
        }

        #endregion

        #region Layout

        void DrawTitle()
        {
            //Draw Title
            if (titleStyle == null)
            {
                titleStyle = new GUIStyle(EditorStyles.label);
                titleStyle.fontSize = 15;
                titleStyle.alignment = TextAnchor.MiddleCenter;
            }
            
            GUILayout.Space(5);
            GUILayout.Label("Graph Inspector", titleStyle);
        }

        void DrawSubTitle()
        {
            //Draw SubTitle
            if (subTitleStyle == null)
            {
                subTitleStyle = new GUIStyle(EditorStyles.label);
                subTitleStyle.fontSize = 12;
                subTitleStyle.alignment = TextAnchor.MiddleCenter;
            }

            if (inspectType == InspectType.State)
            {
                string stateTypeName = GetStateTypeName(stateSp);
                
                if(stateTypeName.Contains("State"))
                {
                    GUILayout.Label("(" + stateTypeName + ")", subTitleStyle);
                }
                else
                {
                    GUILayout.Label("(" + stateTypeName + " State)", subTitleStyle);
                }
            }
                
            
            if(inspectType == InspectType.Transition)
                GUILayout.Label("(Transition)", subTitleStyle);
            
            if(inspectType == InspectType.MultiState)
                GUILayout.Label("(States)", subTitleStyle);
        }

        void DrawOptionNameConvention()
        {
            nameConvention = (NameConvention)EditorGUILayout.EnumPopup("Name Convention", nameConvention);
        }

        void DrawScrollViewInspect()
        {
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            
            DrawInspect();
            
            EditorGUILayout.EndScrollView();
        }

        void DrawInspect()
        {
            switch (inspectType)
            {
                case InspectType.Empty:
                {
                    DrawInspectEmpty();
                    break;
                }
                case InspectType.State:
                {
                    DrawInspectState();
                    break;
                }
                case InspectType.Transition:
                {
                    DrawInspectTransition();
                    break;
                }
                case InspectType.MultiState:
                {
                    DrawInspectMultiState();
                    break;
                }
            }
        }

        void DrawInspectEmpty()
        {
            
        }
        
        void DrawInspectState()
        {
            if (stateSp == null)
            {
                Color oldColor = GUI.color;
                GUI.color = Color.red;
                GUILayout.Label("Cannot found state to inspect");
                GUI.color = oldColor;
                return;
            }

            //string stateTypeName = GetStateTypeName(stateSp);
            //string fieldDisplayName = stateSp.AsState().Name + "            (" + stateTypeName + ")";

            GUILayout.Space(10);
            DrawOptionNameConvention();
            
            EditorGUILayout.PropertyField(stateSp, new GUIContent(stateSp.AsState().Name), true);
            ApplyFieldNameValueConvention(stateSp, State.fn_name);
        }

        void DrawInspectTransition()
        {
            if (transitionSp == null)
            {
                Color oldColor = GUI.color;
                GUI.color = Color.red;
                GUILayout.Label("Cannot found transition to inspect");
                GUI.color = oldColor;
                return;
            }
            

            GUILayout.Space(10);
            DrawOptionNameConvention();
            
            if(transitionSp.isExpanded)
                DrawButtonSetTransitionNameToFinished();
            
            EditorGUILayout.PropertyField(transitionSp, new GUIContent(transitionSp.AsTransition().Name), true);
            ApplyFieldNameValueConvention(transitionSp, Transition.fn_name);
        }

        void DrawButtonSetTransitionNameToFinished()
        {
            Rect rect = new Rect(134, 50, 18, 18);
            if (GUI.Button(rect, "F"))
            {
                transitionSp.AsTransition().Name = "finished";
                UnFocusControl();
            }
        }
        
        /*void DrawInspectMultiState()
        {
            foreach (var stateSp in listStateSp)
            {
                if (stateSp == null)
                {
                    Color oldColor = GUI.color;
                    GUI.color = Color.red;
                    GUILayout.Label("Cannot found state to inspect");
                    GUI.color = oldColor;
                    return;
                }
            
                EditorGUILayout.PropertyField(stateSp, new GUIContent(stateSp.AsState().Name), true);
            }
        }*/

        void DrawInspectMultiState()
        {
            EditorGUILayout.LabelField("   Selecting " + multiStateCount + " states");
        }

        #endregion


        #region Control
        
        public void InspectEmpty()
        {
            UnFocusControl();
            inspectType = InspectType.Empty;
        }

        public void InspectState(long stateId)
        {
            UnFocusControl();
            inspectType = InspectType.State;
            this.stateSp = Context.GraphSp.AsStateMachineGraph().GetStateById(Context, stateId);

            if (this.stateSp == null)
            {
                Debug.LogWarning("Cannot inspect null state");
                InspectEmpty();
                return;
            }
            
            this.stateSp.isExpanded = true;
            this.currentSerializedPropertyName = stateSp.name;
        }

        public void InspectTransition(long transitionId)
        {
            UnFocusControl();
            inspectType = InspectType.Transition;
            this.transitionSp = Context.GraphSp.AsStateMachineGraph().GetTransitionById(Context, transitionId);
            
            if (this.transitionSp == null)
            {
                Debug.LogWarning("Cannot inspect null transition");
                InspectEmpty();
                return;
            }
            
            this.transitionSp.isExpanded = true;
            this.currentSerializedPropertyName = transitionSp.name;
        }

        /*public void InspectMultiState(HashSet<long> listStateId)
        {
            UnFocusControl();
            inspectType = InspectType.MultiState;
            
            this.listStateSp.Clear();

            var listAllState = Context.GraphSp.AsStateMachineGraph().GetAllState(Context);
            foreach (var stateSp in listAllState)
            {
                if (listStateId.Contains(stateSp.AsState().Id))
                {
                    this.listStateSp.Add(stateSp);
                    stateSp.isExpanded = false;
                }
                    
            }
        }*/
        
        public void InspectMultiState(HashSet<long> listStateId)
        {
            UnFocusControl();
            inspectType = InspectType.MultiState;

            this.multiStateCount = listStateId.Count;
        }
        
        #endregion
        
    }
}