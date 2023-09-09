using System.Collections.Generic;

namespace ZeroX.FsmSystem.Editors
{
    public enum StateEventFieldName
    {
        onStateAwake,
        onStateEnter,
        onStateExit,
            
        onStateUpdate,
        onStateLateUpdate,
        onStateFixedUpdate,
        onStateLateFixedUpdate,
            
        onGraphPause,
        onGraphResume,
    }
    
    public static class StateEventFieldNameDefine
    {
        public static readonly List<StateEventFieldName> list = new List<StateEventFieldName>()
        {
            StateEventFieldName.onStateAwake,
            StateEventFieldName.onStateEnter,
            StateEventFieldName.onStateExit,
            
            StateEventFieldName.onStateUpdate,
            StateEventFieldName.onStateLateUpdate,
            StateEventFieldName.onStateFixedUpdate,
            StateEventFieldName.onStateLateFixedUpdate,
            
            StateEventFieldName.onGraphPause,
            StateEventFieldName.onGraphResume,
        };
    }
}