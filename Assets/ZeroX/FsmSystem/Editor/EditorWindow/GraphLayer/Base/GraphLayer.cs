using UnityEngine;

namespace ZeroX.FsmSystem.Editors
{
    public abstract class GraphLayer : StateMachineGraphWindowLayer
    {
        private Matrix4x4 transformMatrix = Matrix4x4.identity;
        
        
        
        protected GraphLayer(StateMachineGraphWindow editorWindow) : base(editorWindow)
        {
        }
        
        private void UpdateTransformationMatrix(Rect rect)
        {
            var centerMat = Matrix4x4.Translate(-rect.size / 2);
            var translationMat = Matrix4x4.Translate(Context.DragOffset);
            var scaleMat = Matrix4x4.Scale(Vector3.one * Context.ZoomFactor);

            this.transformMatrix = centerMat.inverse * scaleMat * translationMat * centerMat;
        }
        
        /// <summary>
        /// Computes and returns a transformed version of a given rect by applying the current offset and the zoom factor.
        /// </summary>
        public Rect GetTransformedRect(Rect rect)
        {
            Rect result = new Rect
            {
                position = transformMatrix.MultiplyPoint(rect.position),
                size = transformMatrix.MultiplyVector(rect.size)
            };

            return result;
        }

        
        public override void Draw(Rect rect)
        {
            UpdateTransformationMatrix(rect);
            DrawGraphLayer(rect);
        }
        
        protected abstract void DrawGraphLayer(Rect rect);
    }
}