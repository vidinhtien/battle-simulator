using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ZeroX.FsmSystem.Editors
{
    public static class StateSpExtension
    {
        private static readonly StateSpHandler handler = new StateSpHandler();
        public static StateSpHandler AsState(this SerializedProperty serializedProperty)
        {
            handler.serializedProperty = serializedProperty;
            return handler;
        }
    }
    
    public class StateSpHandler
    {
        public SerializedProperty serializedProperty;
        
        private const int NodeWidth = 120;
        private const int NodeHeight = 23;
        
        private const float transitionSpace = 0;
        private const float transitionHeightRelativeNodeHeight = 0.85f;

        
        
        public SerializedProperty IdSp => serializedProperty.FindPropertyRelative(State.fn_id);
        public long Id
        {
            get => IdSp.longValue;
            set => IdSp.longValue = value;
        }

        public SerializedProperty NameSp => serializedProperty.FindPropertyRelative(State.fn_name);
        public string Name
        {
            get => NameSp.stringValue;
            set => NameSp.stringValue = value;
        }
        
        public SerializedProperty PositionSp => serializedProperty.FindPropertyRelative(State.fn_position);
        public Vector2 Position
        {
            get => PositionSp.vector2Value;
            set => PositionSp.vector2Value = value;
        }
        
        
        
        

        public int CountTransition(StateMachineGraphContext context)
        {
            int total = 0;
            long stateId = this.Id;
            
            var listTransitionSp = context.ListTransitionSp;
            for (int i = 0; i < listTransitionSp.arraySize; i++)
            {
                var transitionSp = listTransitionSp.GetArrayElementAtIndex(i);
                if (transitionSp.AsTransition().OriginId == stateId)
                    total++;
            }
            
            return total;
        }

        public IEnumerable<SerializedProperty> GetListTransition(StateMachineGraphContext context)
        {
            long stateId = this.Id;

            var listTransitionSp = context.ListTransitionSp;
            for (int i = 0; i < listTransitionSp.arraySize; i++)
            {
                var transitionSp = listTransitionSp.GetArrayElementAtIndex(i);
                if (transitionSp.AsTransition().OriginId == stateId)
                    yield return transitionSp;
            }
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

        public Rect CalculateTitleRect()
        {
            //Cần tính toán độ dài id và các transition để co dãn theo
            Vector2 position = Position;
            return new Rect()
            {
                x = position.x - NodeWidth / 2.0f,
                y = position.y - NodeHeight / 2.0f,
                width = NodeWidth,
                height = NodeHeight
            };
        }

        public Rect CalculateFullRectOutline(StateMachineGraphContext context)
        {
            int totalTransition = CountTransition(context);

            Rect nodeRect = CalculateTitleRect();
            nodeRect.height += nodeRect.height * transitionHeightRelativeNodeHeight * totalTransition;
            return TransformToOutlineRect(nodeRect);
        }
        
        public Rect CalculateFullRect(StateMachineGraphContext context)
        {
            int totalTransition = CountTransition(context);

            Rect nodeRect = CalculateTitleRect();
            nodeRect.height += nodeRect.height * transitionHeightRelativeNodeHeight * totalTransition;
            return nodeRect;
        }
        
        /// <summary>
        /// Dùng trong trường hợp đã có listTransition của state rồi
        /// </summary>
        public Rect CalculateTransitionRectWithIndex(int transitionIndex)
        {
            Rect titleRect = CalculateTitleRect();
            
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

        public bool IsNormalState(StateMachineGraphContext context)
        {
            return context.GraphSp.AsStateMachineGraph().IsNormalState(context, Id);
        }
        
        public bool IsAnyState(StateMachineGraphContext context)
        {
            return context.GraphSp.AsStateMachineGraph().IsAnyState(context, Id);
        }
        
        public bool IsParallelState(StateMachineGraphContext context)
        {
            return context.GraphSp.AsStateMachineGraph().IsParallelState(context, Id);
        }
    }
}