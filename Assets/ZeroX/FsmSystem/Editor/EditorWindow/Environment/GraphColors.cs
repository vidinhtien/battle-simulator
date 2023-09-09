using UnityEngine;

namespace ZeroX.FsmSystem.Editors
{
    public static class GraphColors
    {
        public static readonly Color BackgroundColor = new Color(0.1647f, 0.1647f, 0.1647f, 1);

        public static readonly Color SelectionRectColor = new Color(0.3921f, 0.7843f, 1, 0.1254f);

        public static readonly Color InnerGridColor = new Color(0.1333f, 0.1333f, 0.1333f);
        public static readonly Color OuterGridColor = new Color(0.0941f, 0.0941f, 0.0941f);
        
        public static readonly Color InnerGridColor_PlayMode = new Color(0.1133f, 0.1133f, 0.1133f);
        public static readonly Color OuterGridColor_PlayMode = new Color(0.0741f, 0.0741f, 0.0741f);

        //public static readonly Color SelectionColor = new Color(0.3921f, 0.7843f, 1, 1);

        public static readonly Color TransitionLineColor = new Color(1, 1, 1, 1);
        public static readonly Color TransitionLineColor_Last = new Color(0.6313f, 0.3607f, 0.0980f, 1);
        public static readonly Color TransitionLineColor_Selected = new Color(0.1882f, 0.6980f, 0.9843f, 1);
    }
}