using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace ZeroX.FsmSystem.Editors
{
    public class StateMachineGraphWindow : EditorWindow
    {
        [SerializeField] public StateMachineGraphWindowContext context = new StateMachineGraphWindowContext();
        public StateMachineGraphWindowContext Context
        {
            get => context;
            set => context = value;
        }

        [System.NonSerialized] private StateMachineGraphWindowGUI editorWindowGUI;
        
        
        public Rect Rect
        {
            get { return new Rect(0, 0, this.position.width, this.position.height); }
        }
        
        private bool stopApplyModifiedProperties = false;
        private int controlId;
        private bool wantUnFocusControl;

        void InitIfNeed()
        {
            if(editorWindowGUI != null)
                return;

            editorWindowGUI = new StateMachineGraphWindowGUI(this);
            Context.ReloadIfNeed();
        }
        
        public static void OpenWindow(SerializedProperty graphSp)
        {
            StateMachineGraphWindow window = GetWindow<StateMachineGraphWindow>(false, "Fsm Graph", true);
            window.EditStateMachine(graphSp);
        }
        
        public static void OpenWindow()
        {
            StateMachineGraphWindow window = GetWindow<StateMachineGraphWindow>(false, "Fsm Graph", true);
        }

        public void EditStateMachine(SerializedProperty graphSp)
        {
            Context.SetGraphSp(graphSp);
            editorWindowGUI.Inspector.InspectEmpty();
            ViewToCenter();
        }

        #region Unity Method

        private void OnEnable()
        {
            InitIfNeed();
        }
        
        private void Update()
        {
            if (EditorApplication.isPlaying)
            {
                Repaint();
            }
        }
        
        private void OnInspectorUpdate()
        {
            Repaint();
        }

        private void OnGUI()
        {
            InitIfNeed();
            Context.ReloadIfNeed();
            
            UpdateStopApplyModifiedProperties();
            UpdateGraphOwnerSerializedObject();

            bool graphHasError = false;
            if(Context.HasGraph)
                graphHasError = EditorFsmGraphUtility.CheckAndFixIfGraphHasError(context);
            
            if(graphHasError == false)
                this.editorWindowGUI.OnGUI();
                
            ExecuteEditActionNeedExecute();
            
            ApplyModifiedProperties();
            UnFocusControlIfHasRequest();
        }

        #endregion

        void UpdateStopApplyModifiedProperties()
        {
            if (Event.current.type == EventType.MouseDown && (MouseButton)Event.current.button == MouseButton.LeftMouse)
            {
                Vector2 mousePos = Event.current.mousePosition;

                bool insideToolbar = editorWindowGUI.Toolbar.ToolbarRect.Contains(mousePos);
                bool insideInspector = editorWindowGUI.Inspector.InspectorRect.Contains(mousePos);
                bool insideWindow = Rect.Contains(mousePos);
                if (insideWindow && !insideToolbar && !insideInspector)
                {
                    controlId = GUIUtility.GetControlID(FocusType.Passive);
                    GUIUtility.hotControl = controlId;
                    
                    stopApplyModifiedProperties = true;
                    
                    if(Context.HasGraph)
                        Context.GraphSp.serializedObject.Update();
                    //Debug.Log("Stop Apply");
                }
            }
            
            if (Event.current.rawType == EventType.MouseUp && (MouseButton)Event.current.button == MouseButton.LeftMouse)
            {
                if(GUIUtility.hotControl == controlId)
                    GUIUtility.hotControl = 0;
                
                stopApplyModifiedProperties = false;
                
                if(Context.HasGraph)
                    Context.GraphSp.serializedObject.ApplyModifiedProperties();
                //Debug.Log("UnStop Apply");
            }

            // if (Event.current.type == EventType.MouseDrag)
            //     stopApplyModifiedProperties = true;
            // else if (Event.current.type == EventType.MouseUp)
            //     stopApplyModifiedProperties = false;
        }

        void UpdateGraphOwnerSerializedObject()
        {
            if(Context.HasGraph == false)
                return;
            
            if (stopApplyModifiedProperties) //Moving khiến serializeProperty thay đổi liên tục sẽ khiến Apply liên tục, nên sẽ tạm ngưng Apply
                return;
            
            Context.GraphSp.serializedObject.Update();
        }

        void ApplyModifiedProperties()
        {
            if(Context.HasGraph == false)
                return;
            
            if (stopApplyModifiedProperties) //Moving khiến serializeProperty thay đổi liên tục sẽ khiến Apply liên tục, nên sẽ tạm ngưng Apply
                return;
            
            Context.GraphSp.serializedObject.ApplyModifiedProperties();
            //Context.GraphSp.serializedObject.Update(); //Vì một lý do nào đó mà ko thể gọi Update trước OnGUI. Nên gọi Upate sau chắc cũng đc
        }

        void ExecuteEditActionNeedExecute()
        {
            if(Event.current.rawType != EventType.Repaint)
                return;

            while (Context.CountQueueEditAction > 0)
            {
                Action editAction = Context.DequeueEditAction();
                try
                {
                    editAction.Invoke();
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }
        }

        public void InspectSelected()
        {
            if (Context.CountSelectedTransition() == 1)
            {
                editorWindowGUI.Inspector.InspectTransition(Context.FirstSelectedTransitionId);
                return;
            }

            if (Context.CountSelectedState() == 0)
            {
                editorWindowGUI.Inspector.InspectEmpty();
                return;
            }
            
            if (Context.CountSelectedState() == 1)
            {
                editorWindowGUI.Inspector.InspectState(Context.FirstSelectedStateId);
                return;
            }
            
            editorWindowGUI.Inspector.InspectMultiState(Context.HashSetSelectedStateId);
            return;
        }

        public void InspectEmpty()
        {
            editorWindowGUI.Inspector.InspectEmpty();
        }
        
        public void ViewToCenter()
        {
            if(Context.HasGraph == false)
                return;
            
            Vector2 centerPos = Vector2.zero;
            int numberState = 0;

            var listListState = Context.GraphSp.AsStateMachineGraph().GetListListState(context);
            foreach (var listStateSp in listListState)
            {
                for (int i = 0; i < listStateSp.arraySize; i++)
                {
                    var stateSp = listStateSp.GetArrayElementAtIndex(i);

                    centerPos += stateSp.AsState().Position;
                    numberState++;
                }
            }

            if(numberState > 0)
                centerPos /= numberState;

            Rect graphViewRect = Rect;
            graphViewRect.width -= editorWindowGUI.Inspector.InspectorRect.width / Context.ZoomFactor;
            graphViewRect.height -= editorWindowGUI.Toolbar.ToolbarRect.height / Context.ZoomFactor;
            
            centerPos.x -= graphViewRect.width / 2;
            centerPos.y -= graphViewRect.height / 2;
            
            Context.DragOffset = -centerPos; 
        }

        public void UnFocusControl()
        {
            wantUnFocusControl = true;
        }

        void UnFocusControlIfHasRequest()
        {
            if (wantUnFocusControl)
            {
                wantUnFocusControl = false;
                EditorGUI.FocusTextInControl(null);
                GUI.FocusControl(null);
            }
        }
    }
}