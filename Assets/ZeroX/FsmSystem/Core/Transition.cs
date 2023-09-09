using UnityEngine;

namespace ZeroX.FsmSystem
{
    [System.Serializable]
    public class Transition
    {
        [SerializeField] [HideInInspector] private long id;
        [SerializeField] [HideInInspector] private long originId = -1;
        [SerializeField] [HideInInspector] private long targetId = -1;
        [SerializeField] private string name;

        public const string fn_id = "id";
        public const string fn_originId = "originId";
        public const string fn_targetId = "targetId";
        public const string fn_name = "name";

        public const long emptyId = -1;
        public const long undefinedId = 0;
        public const string finishedName = "finished";

        public long Id
        {
            get => id;
            internal set => id = value;
        }

        public long OriginId
        {
            get => originId;
            internal set => originId = value;
        }

        public long TargetId
        {
            get => targetId;
            internal set => targetId = value;
        }

        public string Name
        {
            get => name;
            internal set => name = value;
        }
    }
}