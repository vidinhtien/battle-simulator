using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace ZeroX.FsmSystem.Editors
{
    public static class EditorStateUtility
    {
        private const int NodeWidth = 120;
        private const int NodeHeight = 23;
        
        private const float transitionSpace = 0;
        private const float transitionHeightRelativeNodeHeight = 0.85f;
        
        private static bool isInitializedReflection = false;
        private static BindingFlags bindingFlags = BindingFlags.Default | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        private static FieldInfo idFi;
        private static FieldInfo nameFi;
        private static FieldInfo positionFi;
        
        public static void InitReflectionIfNot()
        {
            if(isInitializedReflection)
                return;
            isInitializedReflection = true;

            Type stateType = typeof(State);
            
            idFi = stateType.GetField(State.fn_id, bindingFlags);
            nameFi = stateType.GetField(State.fn_name, bindingFlags);
            positionFi = stateType.GetField(State.fn_position, bindingFlags);
        }

        

        #region Dict State Type

        //Exclude Any State
        public static Dictionary<string, Type> dictStateTypeBuiltIn = new Dictionary<string, Type>()
        {
            {"Unity Event State/Lite", typeof(UnityEventStateLite)},
            {"Unity Event State/One Update", typeof(UnityEventStateOneUpdate)},
            {"Unity Event State/Multi Update", typeof(UnityEventStateMultiUpdate)},
            {"Unity Event State/Full", typeof(UnityEventState)},
            
            {"Action State", typeof(ActionState)},
            {"Mono Script State", typeof(MonoScriptState)},
            {"Script State", typeof(ScriptState)},
        };

        #endregion
        
        
        
        
        public static int CountTransition(StateMachineGraphContext context, State state)
        {
            if (context.dictStateTransition.TryGetValue(state.Id, out var listTransitionOfState))
                return listTransitionOfState.Count;

            return 0;
        }
        
        
        #region Rect

        public static Rect TransformToOutlineRect(Rect rect)
        {
            rect.width += 4;
            rect.height += 4;
            
            Vector2 pos = rect.position;
            pos.x -= 2;
            pos.y -= 2;
            rect.position = pos;
            
            return rect;
        }

        public static Rect CalculateTitleRect(State state)
        {
            //Cần tính toán độ dài id và các transition để co dãn theo
            Vector2 position = state.Position;
            return new Rect()
            {
                x = position.x - NodeWidth / 2.0f,
                y = position.y - NodeHeight / 2.0f,
                width = NodeWidth,
                height = NodeHeight
            };
        }

        public static Rect CalculateFullRectOutline(StateMachineGraphContext context, State state)
        {
            int totalTransition = CountTransition(context, state);

            Rect nodeRect = CalculateTitleRect(state);
            nodeRect.height += nodeRect.height * transitionHeightRelativeNodeHeight * totalTransition;
            return TransformToOutlineRect(nodeRect);
        }
        
        public static Rect CalculateFullRect(StateMachineGraphContext context, State state)
        {
            int totalTransition = CountTransition(context, state);

            Rect nodeRect = CalculateTitleRect(state);
            nodeRect.height += nodeRect.height * transitionHeightRelativeNodeHeight * totalTransition;
            return nodeRect;
        }
        
        /// <summary>
        /// Dùng trong trường hợp đã có listTransition của state rồi
        /// </summary>
        public static Rect CalculateTransitionRectWithIndex(State state, int transitionIndex)
        {
            Rect titleRect = CalculateTitleRect(state);
            
            Rect transitionRect = new Rect();
            transitionRect.width = titleRect.width * 1f;
            transitionRect.height = titleRect.height * transitionHeightRelativeNodeHeight;
            
            Vector2 pos = Vector2.zero;
            pos.x = titleRect.x + titleRect.width * 0f;
            pos.y = titleRect.y + titleRect.height + transitionRect.height * transitionIndex;
            pos.y += transitionSpace * (transitionIndex + 1);
            transitionRect.position = pos;
            
            return transitionRect;
        }

        #endregion
        
        public static bool IsNormalState(StateMachineGraphContext context, State state)
        {
            return context.dictNormalState.ContainsKey(state.Id);
        }
        
        public static bool IsAnyState(StateMachineGraphContext context, State state)
        {
            return context.dictAnyState.ContainsKey(state.Id);
        }
        
        public static bool IsParallelState(StateMachineGraphContext context, State state)
        {
            return context.dictParallelState.ContainsKey(state.Id);
        }

        public static void SetId(State state, long id)
        {
            InitReflectionIfNot();
            
            idFi.SetValue(state, id);
        }
        
        public static void SetName(State state, string name)
        {
            InitReflectionIfNot();
            
            nameFi.SetValue(state, name);
        }
        
        public static void SetPosition(State state, Vector2 position)
        {
            InitReflectionIfNot();
            
            positionFi.SetValue(state, position);
        }

        public static void CopyOldStateToNewState(State oldState, State newState)
        {
            SetId(newState, oldState.Id);
            SetName(newState, oldState.Name);
            SetPosition(newState, oldState.Position);
        }
    }
}