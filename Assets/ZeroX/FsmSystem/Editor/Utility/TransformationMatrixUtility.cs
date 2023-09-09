using UnityEngine;

namespace ZeroX.FsmSystem.Editors
{
    public static class TransformationMatrixUtility
    {
        public static Matrix4x4 GetTransformationMatrix(Rect areaRect, Vector2 dragOffset, float zoomFactor)
        {
            var centerMat = Matrix4x4.Translate(-areaRect.size / 2);
            var translationMat = Matrix4x4.Translate(dragOffset);
            var scaleMat = Matrix4x4.Scale(Vector3.one * zoomFactor);

            return centerMat.inverse * scaleMat * translationMat * centerMat;
        }

        public static Rect WorldToArea(Rect areaRect, Vector2 dragOffset, float zoomFactor, Rect worldRect)
        {
            var transformationMatrix = GetTransformationMatrix(areaRect, dragOffset, zoomFactor);
            Rect result = new Rect
            {
                position = transformationMatrix.MultiplyPoint(worldRect.position),
                size = transformationMatrix.MultiplyVector(worldRect.size)
            };

            return result;
        }

        public static Vector2 WorldPointToAreaPoint(Rect areaRect, Vector2 dragOffset, float zoomFactor, Vector2 worldPoint)
        {
            var transformationMatrix = GetTransformationMatrix(areaRect, dragOffset, zoomFactor);
            return transformationMatrix.MultiplyPoint(worldPoint);
        }

        public static Vector2 AreaPointToWorldPoint(Rect areaRect, Vector2 dragOffset, float zoomFactor, Vector2 areaPoint)
        {
            var transformationMatrix = GetTransformationMatrix(areaRect, dragOffset, zoomFactor).inverse;
            return transformationMatrix.MultiplyPoint(areaPoint);
        }
    }
}