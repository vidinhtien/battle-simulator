using System;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace ZeroX.FsmSystem.Editors
{
    public class PopupGenerateCode : EditorWindow
    {
        public enum GenerateMode
        {
            Full, OnlyRegister
        }
        
        public enum MethodNamingConvention
        {
            PascalCase, under_score
        }


        private const float editorGUILabelWidth = 240;
        [System.NonSerialized] private StateMachineGraphContext Context;

        private GenerateMode generateMode;
        private bool includeRegister = true;
        private bool includeGraphFieldName = false;
        private MethodNamingConvention registerMethodNamingConvention = MethodNamingConvention.PascalCase;
        private bool replaceWithActionState = true;
        private bool groupTransitionInRegion = true;
        private string codeGenerated;
        
        Vector2 textAreaScrollPos = Vector2.zero;

        [System.NonSerialized] private GUIStyle hasNoContextLabelStyle;

        public static void Show(StateMachineGraphContext context)
        {
            var instance = EditorWindow.GetWindow<PopupGenerateCode>(); //Sử dụng điều này kết hợp với ShowUtility để hiển thị popup ở dạng ko bị hide khi lost focus
            
            instance.titleContent = new GUIContent("Fsm Graph Code Generator");
            instance.Show();
            
            instance.Context = context;
            instance.codeGenerated = "";
        }

        private void OnGUI()
        {
            GUILayout.BeginVertical();
            
            if (Context == null)
            {
                DrawHasNoContext();
                this.Close();
                GUILayout.EndVertical();
                return;
            }
            
            GUILayout.Space(3);
            DrawOptions();
            GUILayout.Space(5);
            DrawButtonGenerateCode();
            DrawButtonGenerateCodeMethod();
            GUILayout.Space(10);
            DrawTextAreaCodeGenerated();
            
            GUILayout.EndVertical();
        }

        void DrawHasNoContext()
        {
            if (hasNoContextLabelStyle == null)
            {
                hasNoContextLabelStyle = new GUIStyle(EditorStyles.label);
                hasNoContextLabelStyle.fontSize = 15;
                hasNoContextLabelStyle.alignment = TextAnchor.MiddleCenter;
            }

            Rect labelRect = new Rect(0, 0, position.width, 100);
            GUI.Label(labelRect, "Has no context, close this popup and reopen from the FsmGraph window", hasNoContextLabelStyle);
        }

        void DrawOptions()
        {
            float oldLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = editorGUILabelWidth;
            
            
            
            generateMode = (GenerateMode)EditorGUILayout.EnumPopup("Generate Mode", generateMode);
            
            if(generateMode == GenerateMode.Full)
                DrawOptions_FullMode();
            else if (generateMode == GenerateMode.OnlyRegister)
                DrawOptions_OnlyRegister();
            
            
            
            EditorGUIUtility.labelWidth = oldLabelWidth;
        }

        void DrawOptions_FullMode()
        {
            bool enableGroup = includeRegister;
            if (enableGroup)
            {
                GUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUIUtility.labelWidth -= 4;
            }
                
            
            //Draw Include Register
            includeRegister = EditorGUILayout.Toggle("Include Register", includeRegister, GUILayout.ExpandWidth(false));
            
            //Draw sub option of include register
            int oldIndentLevel = EditorGUI.indentLevel;
            if (includeRegister)
            {
                EditorGUI.indentLevel += 1;
                EditorGUIUtility.labelWidth += 8;
                
                includeGraphFieldName = EditorGUILayout.Toggle(new GUIContent("Include Graph Field Name"), includeGraphFieldName);
                registerMethodNamingConvention = (MethodNamingConvention)EditorGUILayout.EnumPopup("State Naming Convention", registerMethodNamingConvention);
                
                EditorGUIUtility.labelWidth -= 8;
            }
            EditorGUI.indentLevel = oldIndentLevel;

            if (enableGroup)
            {
                GUILayout.EndVertical();
                EditorGUIUtility.labelWidth += 4;
            }
                
            
            replaceWithActionState = EditorGUILayout.Toggle("Replace UnityEventState by ActionState", replaceWithActionState);
            groupTransitionInRegion = EditorGUILayout.Toggle("Group Transition In Region", groupTransitionInRegion);
        }

        void DrawOptions_OnlyRegister()
        {
            includeGraphFieldName = EditorGUILayout.Toggle(new GUIContent("Include Graph Field Name"), includeGraphFieldName);
            registerMethodNamingConvention = (MethodNamingConvention)EditorGUILayout.EnumPopup("State Naming Convention", registerMethodNamingConvention);
            groupTransitionInRegion = EditorGUILayout.Toggle("Group Transition In Region", groupTransitionInRegion);
        }

        void DrawButtonGenerateCode()
        {
            if (GUILayout.Button("Generate Code", GUILayout.Height(25)))
            {
                if (generateMode == GenerateMode.Full)
                    codeGenerated = GenerateCode_Full();
                else
                    codeGenerated = GenerateCode_OnlyRegister();
                
                UnFocusControl();
            }
        }

        void DrawButtonGenerateCodeMethod()
        {
            if (GUILayout.Button("Generate Method"))
            {
                codeGenerated = GenerateMethodCode();
                UnFocusControl();
            }
        }
        
        void DrawTextAreaCodeGenerated()
        {
            textAreaScrollPos = EditorGUILayout.BeginScrollView(textAreaScrollPos, GUILayout.ExpandHeight(true));
            codeGenerated = EditorGUILayout.TextArea(codeGenerated, GUILayout.ExpandHeight(true));
            EditorGUILayout.EndScrollView();
        }

        #region Utility

        private string GetStateTypeName(SerializedProperty stateSp)
        {
            string s = stateSp.type; //managedReference<UnityEventStateLite>
            s = s.Substring(17);
            s = s.Substring(0, s.Length - 1);
            
            return s;
        }
        
        static string UpperCaseFirstChar(string s)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < s.Length; i++)
            {
                if (i == 0)
                {
                    string c = s[i].ToString().ToUpperInvariant();
                    sb.Append(c);
                }
                else
                {
                    sb.Append(s[i]);
                }
            }

            return sb.ToString();
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

        static void UnFocusControl()
        {
            EditorGUI.FocusTextInControl(null);
            GUI.FocusControl(null);
        }

        #endregion
        
        #region Generate
        
        string GenerateCode_Full()
        {
            StringBuilder sb = new StringBuilder();

            string addNormalState = GenerateAddListNormalState();
            if (string.IsNullOrEmpty(addNormalState) == false)
            {
                sb.Append("//Add Normal State\n");
                sb.Append(addNormalState);
            }

            string addAnyState = GenerateAddListAnyState();
            if (string.IsNullOrEmpty(addAnyState) == false)
            {
                sb.Append("\n\n//Add Any State\n");
                sb.Append(addAnyState);
            }
            
            string addParallelState = GenerateAddListParallelState();
            if (string.IsNullOrEmpty(addParallelState) == false)
            {
                sb.Append("\n\n//Add Parallel State\n");
                sb.Append(addParallelState);
            }

            string transition = GenerateAddListTransition();
            if (string.IsNullOrEmpty(transition) == false)
            {
                sb.AppendLine("\n\n//Add Transition");
                
                if (groupTransitionInRegion)
                    sb.AppendLine("#region Add Transition");
                
                sb.AppendLine(transition);

                if (groupTransitionInRegion)
                    sb.AppendLine("#endregion");
            }

            string setEntryState = GenerateSetEntryState();
            if (string.IsNullOrEmpty(setEntryState) == false)
            {
                sb.Append("\n");
                sb.Append(setEntryState);
            }
            
            return sb.ToString().Trim('\n');
        }
        
        string GenerateAddListNormalState()
        {
            StringBuilder sb = new StringBuilder();

            var listStateSp = Context.ListNormalStateSp;
            int listStateCount = listStateSp.arraySize;
            string addStateMethod = "AddNormalState";

            string nextCode = null;
            for (int i = 0; i < listStateCount; i++)
            {
                string currentCode;
                if (string.IsNullOrEmpty(nextCode))
                {
                    currentCode = GenerateAddState(listStateSp.GetArrayElementAtIndex(i), addStateMethod);
                    sb.AppendLine(currentCode);
                }
                else
                {
                    currentCode = nextCode;
                    sb.AppendLine(currentCode);
                }
                    

                if (i < listStateCount - 1)
                {
                    nextCode = GenerateAddState(listStateSp.GetArrayElementAtIndex(i + 1), addStateMethod);
                    if (nextCode.Contains("\n") || currentCode.Contains("\n"))
                        sb.Append("\n");
                }
                else
                {
                    nextCode = null;
                }
            }

            return sb.ToString().TrimEnd('\n');
        }

        string GenerateAddListAnyState()
        {
            StringBuilder sb = new StringBuilder();

            var listStateSp = Context.ListAnyStateSp;
            int listStateCount = listStateSp.arraySize;

            for (int i = 0; i < listStateCount; i++)
            {
                sb.AppendLine(GenerateAddAnyState(listStateSp.GetArrayElementAtIndex(i)));
            }

            return sb.ToString();
        }

        string GenerateAddListParallelState()
        {
            StringBuilder sb = new StringBuilder();

            var listStateSp = Context.ListParallelStateSp;
            int listStateCount = listStateSp.arraySize;
            string addStateMethod = "AddParallelState";

            string nextCode = null;
            for (int i = 0; i < listStateCount; i++)
            {
                if (string.IsNullOrEmpty(nextCode))
                    sb.AppendLine(GenerateAddState(listStateSp.GetArrayElementAtIndex(i), addStateMethod));
                else
                    sb.AppendLine(nextCode);

                if (i < listStateCount - 1)
                {
                    nextCode = GenerateAddState(listStateSp.GetArrayElementAtIndex(i + 1), addStateMethod);
                    if (nextCode.Contains("\n"))
                        sb.Append("\n");
                }
                else
                {
                    nextCode = null;
                }
            }

            return sb.ToString().TrimEnd('\n');
        }
        
        private string GenerateAddState(SerializedProperty stateSp, string addStateMethod)
        {
            StringBuilder sb = new StringBuilder();
            
            string fsmGraphFieldName = Context.GraphSp.name;
            
            string stateTypeName = GetStateTypeName(stateSp);
            if (replaceWithActionState)
            {
                if (stateTypeName.StartsWith("UnityEventState"))
                    stateTypeName = nameof(ActionState);
            }
            
            string stateName = stateSp.AsState().Name;
            Vector2 statePos = stateSp.AsState().Position;
            
            sb.AppendFormat("{0}.{1}<{2}>(\"{3}\", {4}, {5})", fsmGraphFieldName, addStateMethod, stateTypeName, stateName, (int)statePos.x, (int)statePos.y);

            List<string> listRegister = null;
            if (includeRegister)
            {
                listRegister = GenerateListRegister(stateSp);
            
                foreach (var register in listRegister)
                {
                    sb.Append("\n    ." + register);
                }
            }

            sb.Append(";");
            
            return sb.ToString();
        }

        private string GenerateAddAnyState(SerializedProperty stateSp)
        {
            StringBuilder sb = new StringBuilder();
            
            string fsmGraphFieldName = Context.GraphSp.name;
            string stateName = stateSp.AsState().Name;
            Vector2 statePos = stateSp.AsState().Position;

            bool canTransitionToSelf = stateSp.FindPropertyRelative(AnyState.fn_canTransitionToSelf).boolValue;
            
            if(canTransitionToSelf)
                sb.AppendFormat("{0}.{1}(\"{2}\", {3}, {4});", fsmGraphFieldName, "AddAnyState", stateName, (int)statePos.x, (int)statePos.y);
            else
                sb.AppendFormat("{0}.{1}(\"{2}\", {3}, {4}, {5});", fsmGraphFieldName, "AddAnyState", stateName, "false", (int)statePos.x, (int)statePos.y);

            return sb.ToString();
        }
        
        private List<string> GenerateListRegister(SerializedProperty stateSp)
        {
            List<string> listRegister = new List<string>();
            State state = (State)stateSp.GetObject();

            if (state is UnityEventStateLite ||
                state is UnityEventStateOneUpdate ||
                state is UnityEventStateMultiUpdate ||
                state is UnityEventState)
            {
                foreach (var stateEventFieldName in StateEventFieldNameDefine.list)
                {
                    var unityEventSp = stateSp.FindPropertyRelative(stateEventFieldName.ToString());
                    if(unityEventSp == null)
                        continue;
                    
                    listRegister.Add(GenerateRegisterUnityEvent(stateSp, unityEventSp));
                }
            }

            listRegister.RemoveAll(r => string.IsNullOrEmpty(r));
            return listRegister;
        }

        
        string GenerateMethodNameForRegister(string graphFieldName, string stateName, string stateMethodName)
        {
            graphFieldName = UpperCaseFirstChar(graphFieldName);
            stateMethodName = UpperCaseFirstChar(stateMethodName);

            if (includeGraphFieldName)
            {
                if (registerMethodNamingConvention == MethodNamingConvention.PascalCase)
                    stateName = PascalCase(stateName);
                else
                    stateName = UnderScoreCase(stateName);
                return string.Format("{0}_{1}_{2}", graphFieldName, stateName, stateMethodName);
            }
            else
            {
                // if (stateName.Any(char.IsUpper))
                //     stateName += " State";
                // else
                //     stateName += " state";
                
                if (registerMethodNamingConvention == MethodNamingConvention.PascalCase)
                    stateName = PascalCase(stateName);
                else
                    stateName = UnderScoreCase(stateName);
                
                return string.Format("{0}_{1}", stateName, stateMethodName);
            }
        }

        string GenerateRegisterUnityEvent(SerializedProperty stateSp, SerializedProperty unityEventSp)
        {
            string unityEventFieldName = unityEventSp.name;
            UnityEvent unityEvent = (UnityEvent)unityEventSp.GetObject();

            int count = unityEvent.GetPersistentEventCount();
            if (count == 0)
                return null;


            string graphFieldName = UpperCaseFirstChar(Context.GraphSp.name);
            string stateName = UpperCaseFirstChar(stateSp.AsState().Name.Replace(" ", ""));
            string stateMethodName = UpperCaseFirstChar(unityEventFieldName);
            
            string fullMethodName = GenerateMethodNameForRegister(Context.GraphSp.name, stateSp.AsState().Name, unityEventFieldName);
            // if (includeGraphFieldName)
            // {
            //     fullMethodName = string.Format("{0}_{1}_{2}", graphFieldName, stateName, stateMethodName);
            // }
            // else
            // {
            //     fullMethodName = string.Format("{0}State_{1}", stateName, stateMethodName);
            // }
            
            return string.Format("Register{0}({1})", stateMethodName, fullMethodName);
        }

        private Dictionary<long, string> GetDictStateName()
        {
            Dictionary<long, string> dict = new Dictionary<long, string>();

            var listListState = Context.GraphSp.AsStateMachineGraph().GetListListState(Context);
            foreach (var listStateSp in listListState)
            {
                for (int i = 0; i < listStateSp.arraySize; i++)
                {
                    var state = listStateSp.GetArrayElementAtIndex(i);
                    dict.Add(state.AsState().Id, state.AsState().Name);
                }
            }

            return dict;
        }
        
        string GenerateAddListTransition()
        {
            Dictionary<long, string> dictStateName = GetDictStateName();

            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < Context.ListNormalStateSp.arraySize; i++)
            {
                var stateSp = Context.ListNormalStateSp.GetArrayElementAtIndex(i);
                long stateId = stateSp.AsState().Id;
                
                for (int j = 0; j < Context.ListTransitionSp.arraySize; j++)
                {
                    var transitionSp = Context.ListTransitionSp.GetArrayElementAtIndex(j);
                    
                    if(transitionSp.AsTransition().OriginId == stateId)
                        sb.AppendLine(GenerateAddTransition(transitionSp, dictStateName));
                }
            }
            
            for (int i = 0; i < Context.ListAnyStateSp.arraySize; i++)
            {
                var stateSp = Context.ListAnyStateSp.GetArrayElementAtIndex(i);
                long stateId = stateSp.AsState().Id;
                
                for (int j = 0; j < Context.ListTransitionSp.arraySize; j++)
                {
                    var transitionSp = Context.ListTransitionSp.GetArrayElementAtIndex(j);
                    
                    if(transitionSp.AsTransition().OriginId == stateId)
                        sb.AppendLine(GenerateAddTransition(transitionSp, dictStateName));
                }
            }

            return sb.ToString().Trim('\n');
        }

        string GenerateAddTransition(SerializedProperty transitionSp, Dictionary<long, string> dictStateName)
        {
            string graphFieldName = Context.GraphSp.name;
            string transitionName = transitionSp.AsTransition().Name;
            long originId = transitionSp.AsTransition().OriginId;
            long targetId = transitionSp.AsTransition().TargetId;
            
            dictStateName.TryGetValue(originId, out var originStateName);
            dictStateName.TryGetValue(targetId, out var targetStateName);

            if (string.IsNullOrEmpty(targetStateName))
            {
                return string.Format("{0}.AddTransition(\"{1}\", \"{2}\");", graphFieldName, originStateName, transitionName);
            }
            else
            {
                return string.Format("{0}.AddTransition(\"{1}\", \"{2}\", \"{3}\");", graphFieldName, originStateName, transitionName, targetStateName);
            }
        }

        string GenerateSetEntryState()
        {
            var entryStateId = Context.GraphSp.AsStateMachineGraph().EntryStateId;

            for (int i = 0; i < Context.ListNormalStateSp.arraySize; i++)
            {
                var state = Context.ListNormalStateSp.GetArrayElementAtIndex(i);
                if (state.AsState().Id == entryStateId)
                {
                    string graphFieldName = Context.GraphSp.name;
                    return string.Format("{0}.SetEntryState(\"{1}\");", graphFieldName, state.AsState().Name);
                }
            }

            return "";
        }
        
        #endregion

        #region Generate Only Register

        private string GenerateGetStateAndRegister(SerializedProperty stateSp, string getStateMethod)
        {
            StringBuilder sb = new StringBuilder();
            
            string fsmGraphFieldName = Context.GraphSp.name;
            string stateTypeName = GetStateTypeName(stateSp);
            string stateName = stateSp.AsState().Name;

            sb.AppendFormat("{0}.{1}<{2}>(\"{3}\")", fsmGraphFieldName, getStateMethod, stateTypeName, stateName);

            List<string> listRegister = GenerateListRegister(stateSp);

            if (listRegister.Count == 0)
            {
                return "";
            }
            
            foreach (var register in listRegister)
            {
                sb.Append("\n    ." + register);
            }

            sb.Append(";");
            return sb.ToString();
        }
        
        string GenerateGetListStateAndRegister(SerializedProperty listStateSp)
        {
            StringBuilder sb = new StringBuilder();
            
            int listStateCount = listStateSp.arraySize;
            string getStateMethod = "GetState";

            string nextCode = null;
            for (int i = 0; i < listStateCount; i++)
            {
                string currentCode;
                if (string.IsNullOrEmpty(nextCode))
                {
                    currentCode = GenerateGetStateAndRegister(listStateSp.GetArrayElementAtIndex(i), getStateMethod);
                }
                else
                {
                    currentCode = nextCode;
                }


                if (string.IsNullOrEmpty(currentCode) == false)
                {
                    sb.AppendLine(currentCode);
                    
                    if (i < listStateCount - 1)
                    {
                        nextCode = GenerateGetStateAndRegister(listStateSp.GetArrayElementAtIndex(i + 1), getStateMethod);
                        if (nextCode.Contains("\n") || currentCode.Contains("\n"))
                            sb.Append("\n");
                    }
                    else
                    {
                        nextCode = null;
                    }
                }
                else
                {
                    nextCode = null;
                }
            }

            return sb.ToString().TrimEnd('\n');
        }

        string GenerateCode_OnlyRegister()
        {
            StringBuilder sb = new StringBuilder();

            string registerNormalState = GenerateGetListStateAndRegister(Context.ListNormalStateSp);
            if (string.IsNullOrEmpty(registerNormalState) == false)
            {
                sb.Append("//Register Normal State\n");
                sb.Append(registerNormalState);
            }

            string registerParallelState = GenerateGetListStateAndRegister(Context.ListParallelStateSp);
            if (string.IsNullOrEmpty(registerParallelState) == false)
            {
                sb.Append("\n\n//Register Parallel State\n");
                sb.Append(registerParallelState);
            }

            return sb.ToString().Trim('\n');
        }

        #endregion

        #region Generate Method Code

        string GenerateMethodCode()
        {
            StringBuilder sb = new StringBuilder();
            
            //Normal State
            string methodNormalState = GenerateMethodCodeOfListState(Context.ListNormalStateSp);
            sb.AppendLine(methodNormalState);

            //Parallel State
            string methodParallelState = GenerateMethodCodeOfListState(Context.ListParallelStateSp);
            sb.Append("\n");
            sb.Append(methodParallelState); //Cuối rồi nên ko cần AppendLine để xuống dòng nữa
            
            return sb.ToString();
        }

        string GenerateMethodCodeOfListState(SerializedProperty listStateSp)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < listStateSp.arraySize; i++)
            {
                var stateSp = listStateSp.GetArrayElementAtIndex(i);
                var listMethodCodeOfState = GenerateListMethodCodeOfState(stateSp);
                if(listMethodCodeOfState.Count == 0)
                    continue;

                foreach (var methodCode in listMethodCodeOfState)
                {
                    sb.AppendLine(methodCode);
                    sb.Append("\n");
                }
            }

            return sb.ToString().Trim('\n');
        }

        List<string> GenerateListMethodCodeOfState(SerializedProperty stateSp)
        {
            List<string> listRegister = GenerateListRegister(stateSp);
            List<string> listMethod = new List<string>();
            foreach (var registerCode in listRegister)
            {
                listMethod.Add(RegisterCodeToMethodCode(registerCode)); 
            }

            return listMethod;
        }

        string RegisterCodeToMethodCode(string registerCode)
        {
            int startIndex = registerCode.IndexOf("(", StringComparison.Ordinal) + 1;
            int endIndex = registerCode.IndexOf(")", StringComparison.Ordinal);

            string methodName = registerCode.Substring(startIndex, endIndex - startIndex);
            return string.Format("private void {0}()\n{{\n    \n}}", methodName);
        }

        #endregion
    }
}