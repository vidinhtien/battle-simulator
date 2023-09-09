using UnityEditor;
using UnityEngine;

namespace ZeroX.FsmSystem.Editors
{
    public static class LineDrawer
    {
        public static void DrawBezier(Rect rectStart, Vector2 endPos, Color lineColor, float zoomFactor)
        {
            Vector2 startPos = new Vector2();
            startPos.y = rectStart.position.y + rectStart.height / 2;
            
            Vector2 startTangent;
            Vector2 endTangent;
            float tangentMagnitude = 50 * zoomFactor;
            Texture arrowTexture;
            Rect arrowRect = new Rect();
            arrowRect.size = new Vector2(12, 10);
            
            float rectStartRight = rectStart.position.x + rectStart.width;
            float rectStartLeft = rectStart.position.x;

            if (Mathf.Abs(rectStartRight - endPos.x) < Mathf.Abs(rectStartLeft - endPos.x))
            {
                if (rectStartRight < endPos.x)
                {
                    startPos.x = rectStartRight;
                    startTangent = startPos + Vector2.right * tangentMagnitude;
                    endTangent = endPos + Vector2.left *tangentMagnitude;
                
                    //Vẽ arrow
                    endPos.x -= arrowRect.width;
                    arrowTexture = TextureLibrary.RightArrow.Value;
                    arrowRect.position = new Vector2(endPos.x, endPos.y - arrowRect.height / 2);
                }
                else
                {
                    startPos.x = rectStartRight;
                    startTangent = startPos + Vector2.right * tangentMagnitude;
                    endTangent = endPos + Vector2.right * tangentMagnitude;

                    //Vẽ arrow
                    arrowTexture = TextureLibrary.LeftArrow.Value;
                    arrowRect.position = new Vector2(endPos.x, endPos.y - arrowRect.height / 2);
                    endPos.x += arrowRect.width;
                }
            }
            else
            {
                if (rectStartLeft > endPos.x)
                {
                    startPos.x = rectStart.position.x;
                    startTangent = startPos + Vector2.left * tangentMagnitude;
                    endTangent = endPos + Vector2.right * tangentMagnitude;
                
                    //Vẽ arrow
                    arrowTexture = TextureLibrary.LeftArrow.Value;
                    arrowRect.position = new Vector2(endPos.x, endPos.y - arrowRect.height / 2);
                    endPos.x += arrowRect.width;
                }
                else
                {
                    startPos.x = rectStart.position.x;
                    startTangent = startPos + Vector2.left * tangentMagnitude;
                    endTangent = endPos + Vector2.left * tangentMagnitude;

                    //Vẽ arrow
                    endPos.x -= arrowRect.width;
                    arrowTexture = TextureLibrary.RightArrow.Value;
                    arrowRect.position = new Vector2(endPos.x, endPos.y - arrowRect.height / 2);
                }
            }
            
            if (arrowTexture == TextureLibrary.RightArrow.Value)
                ApplyZoomFactoForLineArrowRight(ref endPos, ref arrowRect, zoomFactor);
            else
                ApplyZoomFactoForLineArrowLeft(ref endPos, ref arrowRect, zoomFactor);
            
            Handles.DrawBezier(startPos, endPos, startTangent, endTangent, lineColor, null, 3 * zoomFactor);
            GUI.DrawTexture(arrowRect, arrowTexture, ScaleMode.StretchToFill, true, 1, lineColor, Vector4.zero, Vector4.zero);
        }
        
        public static void DrawBezier(Rect rectStart, Rect rectEnd, Color lineColor, float zoomFactor)
        {
            Vector2 startPos = new Vector2();
            startPos.y = rectStart.position.y + rectStart.height / 2;

            Vector2 endPos = new Vector2();
            endPos.y = rectEnd.position.y + rectEnd.height / 2;

            Vector2 startTangent;
            Vector2 endTangent;
            float tangentMagnitude = 50 * zoomFactor;
            Texture arrowTexture;
            Rect arrowRect = new Rect();
            arrowRect.size = new Vector2(12, 10);

            float rectStartRight = rectStart.position.x + rectStart.width;
            if (rectStartRight < rectEnd.position.x)
            {
                startPos.x = rectStartRight;
                endPos.x = rectEnd.position.x;
                startTangent = startPos + Vector2.right * tangentMagnitude;
                endTangent = endPos + Vector2.left *tangentMagnitude;
                
                //Vẽ arrow
                endPos.x -= arrowRect.width;
                arrowTexture = TextureLibrary.RightArrow.Value;
                arrowRect.position = new Vector2(endPos.x, endPos.y - arrowRect.height / 2);
            }
            else if (rectStartRight <= rectEnd.position.x + rectEnd.width)
            {
                startPos.x = rectStartRight;
                endPos.x = rectEnd.position.x + rectEnd.width;
                startTangent = startPos + Vector2.right * tangentMagnitude;
                endTangent = endPos + Vector2.right * tangentMagnitude;

                //Vẽ arrow
                arrowTexture = TextureLibrary.LeftArrow.Value;
                arrowRect.position = new Vector2(endPos.x, endPos.y - arrowRect.height / 2);
                endPos.x += arrowRect.width;
            }
            else if (rectStart.position.x > rectEnd.position.x + rectEnd.width)
            {
                startPos.x = rectStart.position.x;
                endPos.x = rectEnd.position.x + rectEnd.width;
                startTangent = startPos + Vector2.left * tangentMagnitude;
                endTangent = endPos + Vector2.right * tangentMagnitude;
                
                //Vẽ arrow
                arrowTexture = TextureLibrary.LeftArrow.Value;
                arrowRect.position = new Vector2(endPos.x, endPos.y - arrowRect.height / 2);
                endPos.x += arrowRect.width;
            }
            else
            {
                startPos.x = rectStart.position.x;
                endPos.x = rectEnd.position.x;
                startTangent = startPos + Vector2.left * tangentMagnitude;
                endTangent = endPos + Vector2.left * tangentMagnitude;

                //Vẽ arrow
                endPos.x -= arrowRect.width;
                arrowTexture = TextureLibrary.RightArrow.Value;
                arrowRect.position = new Vector2(endPos.x, endPos.y - arrowRect.height / 2);
            }

            if (arrowTexture == TextureLibrary.RightArrow.Value)
                ApplyZoomFactoForLineArrowRight(ref endPos, ref arrowRect, zoomFactor);
            else
                ApplyZoomFactoForLineArrowLeft(ref endPos, ref arrowRect, zoomFactor);
            
            Handles.DrawBezier(startPos, endPos, startTangent, endTangent, lineColor, null, 3 * zoomFactor);
            GUI.DrawTexture(arrowRect, arrowTexture, ScaleMode.StretchToFill, true, 1, lineColor, Vector4.zero, Vector4.zero);
        }

        static void ApplyZoomFactoForLineArrowRight(ref Vector2 lineEndPos, ref Rect arrowRect, float zoomFactor)
        {
            Vector2 middleRight = new Vector2(arrowRect.xMax, arrowRect.yMin + arrowRect.height / 2f);
            
            arrowRect.width *= zoomFactor;
            arrowRect.height *= zoomFactor;
            arrowRect.position = new Vector2(middleRight.x - arrowRect.width, middleRight.y - arrowRect.height / 2f);

            lineEndPos.x = arrowRect.x;
            lineEndPos.y = arrowRect.y + arrowRect.height / 2;
        }
        
        static void ApplyZoomFactoForLineArrowLeft(ref Vector2 lineEndPos, ref Rect arrowRect, float zoomFactor)
        {
            Vector2 middleLeft = new Vector2(arrowRect.x, arrowRect.yMin + arrowRect.height / 2f);
            
            arrowRect.width *= zoomFactor;
            arrowRect.height *= zoomFactor;
            arrowRect.position = new Vector2(middleLeft.x, middleLeft.y - arrowRect.height / 2f);

            lineEndPos.x = arrowRect.xMax;
            lineEndPos.y = arrowRect.y + arrowRect.height / 2;
        }



        private static void DrawParabola_Internal(Rect rectStart, Rect rectEnd, Color lineColor, float lineWidth, float arrowWidth, float arrowHeight, float zoomFactor)
        {
            float rectStart_yMiddle = rectStart.position.y + rectStart.height / 2f;
            float rectStart_xMiddle = rectStart.position.x + rectStart.width / 2f;
            
            float rectEnd_yMiddle = rectEnd.position.y + rectEnd.height / 2f;
            float rectEnd_xMiddle = rectEnd.position.x + rectEnd.width / 2f;
            
            
            Vector2 startTangent = new Vector2();
            Vector2 endTangent = new Vector2();
            float tangleRatio = 1f;
            Texture arrowTexture;

            Rect arrowRect = new Rect();
            arrowRect.size = new Vector2(arrowWidth * zoomFactor, arrowHeight * zoomFactor);
            Vector2 arrowPos = Vector2.zero;


            Vector2 startPos = new Vector2();
            startPos.y = rectStart_yMiddle;

            Vector2 endPos = new Vector2();
            if (rectStart_yMiddle < rectEnd_yMiddle) // Sử dụng Down Arrow
            {
                endPos.y = rectEnd.yMin - arrowRect.height;
                endTangent.y = endPos.y - Mathf.Abs(endPos.y - startPos.y) * tangleRatio; //Trong Rect thì y tăng theo chiều xuống
                
                //Arrow
                arrowTexture = TextureLibrary.DownArrow.Value;
                arrowPos.y = endPos.y;
            }
            else
            {
                endPos.y = rectEnd.yMax + arrowRect.height;
                endTangent.y = endPos.y + Mathf.Abs(endPos.y - startPos.y) * tangleRatio; //Trong Rect thì y tăng theo chiều xuống
                
                //Arrow
                arrowTexture = TextureLibrary.UpArrow.Value;
                arrowPos.y = endPos.y - arrowRect.height;
            }
                
            
            
            if (rectStart_xMiddle < rectEnd_xMiddle) //Start bên phải
            {
                startPos.x = rectStart.xMax;
                endPos.x = rectEnd_xMiddle - 15f * zoomFactor;
                startTangent = startPos + Vector2.right * Mathf.Abs(endPos.x - startPos.x) * tangleRatio;
                endTangent.x = endPos.x;
            }
            else
            {
                startPos.x = rectStart.xMin;
                endPos.x = rectEnd_xMiddle + 15f * zoomFactor;
                startTangent = startPos - Vector2.right * Mathf.Abs(endPos.x - startPos.x) * tangleRatio;
                endTangent.x = endPos.x;
            }

            arrowPos.x = endPos.x - arrowRect.width / 2;
            arrowRect.position = arrowPos;
            
            //Draw
            Handles.DrawBezier(startPos, endPos, startTangent, endTangent, lineColor, null, lineWidth * zoomFactor);
            GUI.DrawTexture(arrowRect, arrowTexture, ScaleMode.StretchToFill, true, 1, lineColor, Vector4.zero, Vector4.zero);
        }

        
        public static void DrawParabola(Rect rectStart, Rect rectEnd, Color lineColor, float zoomFactor)
        {
            DrawParabola_Internal(rectStart, rectEnd, lineColor, 3, 10, 12, zoomFactor);
        }

        public static void DrawParabolaWithOutline(Rect rectStart, Rect rectEnd, Color lineColor, Color outlineColor, float zoomFactor)
        {
            DrawParabola_Internal(rectStart, rectEnd, outlineColor, 7, 12, 14, zoomFactor);
            DrawParabola_Internal(rectStart, rectEnd, lineColor, 3, 10, 12, zoomFactor);
        }

        public static void DrawParabola(Rect rectStart, Vector2 endPos, Color lineColor, float zoomFactor)
        {
            float rectStart_yMiddle = rectStart.position.y + rectStart.height / 2f;
            float rectStart_xMiddle = rectStart.position.x + rectStart.width / 2f;
            
            Vector2 startTangent = new Vector2();
            Vector2 endTangent = new Vector2();
            float tangleRatio = 1f;
            Texture arrowTexture;
            Rect arrowRect = new Rect();
            arrowRect.size = new Vector2(10 * zoomFactor, 12 * zoomFactor);
            Vector2 arrowPos = Vector2.zero;


            Vector2 startPos = new Vector2();
            startPos.y = rectStart_yMiddle;
            
            if (rectStart_yMiddle < endPos.y) // Sử dụng Down Arrow
            {
                endPos.y -= arrowRect.height;
                endTangent.y = endPos.y - Mathf.Abs(endPos.y - startPos.y) * tangleRatio; //Trong Rect thì y tăng theo chiều xuống
                
                //Arrow
                arrowTexture = TextureLibrary.DownArrow.Value;
                arrowPos.y = endPos.y;
            }
            else
            {
                endPos.y += arrowRect.height;
                endTangent.y = endPos.y + Mathf.Abs(endPos.y - startPos.y) * tangleRatio; //Trong Rect thì y tăng theo chiều xuống
                
                //Arrow
                arrowTexture = TextureLibrary.UpArrow.Value;
                arrowPos.y = endPos.y - arrowRect.height;
            }
                
            
            
            if (rectStart_xMiddle < endPos.x) //Start bên phải
            {
                startPos.x = rectStart.xMax;
                startTangent = startPos + Vector2.right * Mathf.Abs(endPos.x - startPos.x) * tangleRatio;
                endTangent.x = endPos.x;
            }
            else
            {
                startPos.x = rectStart.xMin;
                startTangent = startPos - Vector2.right * Mathf.Abs(endPos.x - startPos.x) * tangleRatio;
                endTangent.x = endPos.x;
            }

            arrowPos.x = endPos.x - arrowRect.width / 2;
            arrowRect.position = arrowPos;
            
            //Draw
            Handles.DrawBezier(startPos, endPos, startTangent, endTangent, lineColor, null, 3 * zoomFactor);
            GUI.DrawTexture(arrowRect, arrowTexture, ScaleMode.StretchToFill, true, 1, lineColor, Vector4.zero, Vector4.zero);
        }
    }
}