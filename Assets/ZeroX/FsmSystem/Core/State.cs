using System.Collections.Generic;
using UnityEngine;

namespace ZeroX.FsmSystem
{
    [System.Serializable]
    public abstract class State
    {
        [SerializeField] [HideInInspector] private long id;
        [SerializeField] private string name;
        [SerializeField] [HideInInspector] private Vector2 position;

        public const string fn_id = "id";
        public const string fn_name = "name";
        public const string fn_position = "position";

        public const long emptyId = -1;
        public const long undefinedId = 0;

        public long Id
        {
            get => id;
            internal set => id = value;
        }

        public string Name
        {
            get => name;
            internal set => name = value;
        }

        public Vector2 Position
        {
            get => position;
            set => position = value;
        }


        #region Constructor

        protected State()
        {
        }

        protected State(string name, Vector2 position)
        {
            this.name = name;
            this.position = position;
        }

        #endregion
        
        
        [System.NonSerialized] internal bool isAwaken = false;
        [System.NonSerialized] internal Dictionary<string, Transition> dictTransitionByName = new Dictionary<string, Transition>();
        [System.NonSerialized] internal StateType stateType = StateType.Normal;
        [System.NonSerialized] internal FsmGraph fsmGraph;

        [System.NonSerialized] internal long onStateEnterFrameCount = 0;
        [System.NonSerialized] internal long onStateEnterFixedFrameCount = 0;

        public virtual void OnStateEnter()
        {
        }

        public virtual void OnStateExit()
        {
        }
        
        
        
        public virtual void OnStateAwake()
        {
        }
        
        public virtual void OnStateUpdate()
        {
        }
        
        public virtual void OnStateLateUpdate()
        {
        }
        
        public virtual void OnStateFixedUpdate()
        {
        }
        
        public virtual void OnStateLateFixedUpdate()
        {
        }
        
        public virtual void OnGraphPause()
        {
        }
        
        public virtual void OnGraphResume()
        {
        }
    }
}