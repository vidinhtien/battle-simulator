using UnityEngine;

namespace ZeroX.FsmSystem.Editors
{
    [System.Serializable]
    public class ZoomSettings 
    {
        public static readonly float MinZoomFactor = 0.3f;
        public static readonly float MaxZoomFactor = 1f;

        [SerializeField]
        private float zoomFactor = 1.0f;

        public float ZoomFactor
        {
            get => this.zoomFactor;
            set
            {
                this.zoomFactor = Mathf.Clamp(value, MinZoomFactor, MaxZoomFactor);
            }
        }
    }
}