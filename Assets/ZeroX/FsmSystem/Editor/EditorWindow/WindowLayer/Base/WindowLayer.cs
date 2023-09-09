using UnityEngine;
using UnityEngine.UIElements;

namespace ZeroX.FsmSystem.Editors
{
    //Chủ yếu handle sẵn phần sự kiện. Dự định class này ko dependency class nào khác thuộc về tool này
    public abstract class WindowLayer
    {
        protected MouseButton? downedMouseInLayerRect = null;

        /// <summary>
        /// Please always call before ProcessEvents
        /// </summary>
        public abstract void Draw(Rect rect);

        #region Process Events
        
        public virtual void ProcessEvents()
        {
            Event currentEvent = Event.current;

            if (currentEvent.rawType == EventType.MouseUp)
            {
                ProcessEvent_MouseUp(currentEvent);
            }
            else
            {
                switch (currentEvent.type)
                {
                    case EventType.MouseDown:
                    {
                        ProcessEvent_MouseDown(currentEvent);
                        break;
                    }
                    case EventType.MouseDrag:
                    {
                        ProcessEvent_MouseDrag(currentEvent);
                        break;
                    }
                    case EventType.ScrollWheel:
                    {
                        ProcessEvent_ScrollWheel(currentEvent);
                        break;
                    }
                    case EventType.KeyDown:
                    {
                        OnKeyDown(currentEvent.keyCode);
                        break;
                    }
                    case EventType.KeyUp:
                    {
                        OnKeyUp(currentEvent.keyCode);
                        break;
                    }
                }
            }
        }


        private void ProcessEvent_MouseDown(Event currentEvent)
        {
            MouseButton mouseButton = (MouseButton) currentEvent.button;
            switch (mouseButton)
            {
                case MouseButton.LeftMouse:
                {
                    OnLeftMouseDown(currentEvent.mousePosition);
                    break;
                }
                case MouseButton.RightMouse:
                {
                    OnRightMouseDown(currentEvent.mousePosition);
                    break;
                }
                case MouseButton.MiddleMouse:
                {
                    OnMiddleMouseDown(currentEvent.mousePosition);
                    break;
                }
            }
        }
        
        private void ProcessEvent_MouseDrag(Event currentEvent)
        {
            MouseButton mouseButton = (MouseButton) currentEvent.button;
            
            switch (mouseButton)
            {
                case MouseButton.LeftMouse:
                {
                    OnLeftMouseDrag(currentEvent.mousePosition);
                    break;
                }
                case MouseButton.RightMouse:
                {
                    OnRightMouseDrag(currentEvent.mousePosition);
                    break;
                }
                case MouseButton.MiddleMouse:
                {
                    OnMiddleMouseDrag(currentEvent.mousePosition);
                    break;
                }
            }
        }
        
        private void ProcessEvent_MouseUp(Event currentEvent)
        {
            MouseButton mouseButton = (MouseButton) currentEvent.button;
            switch (mouseButton)
            {
                case MouseButton.LeftMouse:
                {
                    OnLeftMouseUp(currentEvent.mousePosition);
                    break;
                }
                case MouseButton.RightMouse:
                {
                    OnRightMouseUp(currentEvent.mousePosition);
                    break;
                }
                case MouseButton.MiddleMouse:
                {
                    OnMiddleMouseUp(currentEvent.mousePosition);
                    break;
                }
            }
        }
        
        private void ProcessEvent_ScrollWheel(Event currentEvent)
        {
            OnScrollWheel(currentEvent.mousePosition);
        }
        
        #endregion

        protected virtual void OnLeftMouseDown(Vector2 position)
        {
        }
        
        protected virtual void OnLeftMouseDrag(Vector2 position)
        {
        }
        
        protected virtual void OnLeftMouseUp(Vector2 position)
        {
        }
        
        protected virtual void OnRightMouseDown(Vector2 position)
        {
        }
        
        protected virtual void OnRightMouseDrag(Vector2 position)
        {
        }
        
        protected virtual void OnRightMouseUp(Vector2 position)
        {
        }
        
        protected virtual void OnMiddleMouseDown(Vector2 position)
        {
        }
        
        protected virtual void OnMiddleMouseDrag(Vector2 position)
        {
        }
        
        protected virtual void OnMiddleMouseUp(Vector2 position)
        {
        }
        
        protected virtual void OnScrollWheel(Vector2 position)
        {
        }

        protected virtual void OnKeyDown(KeyCode keyCode)
        {
        }
        
        protected virtual void OnKeyUp(KeyCode keyCode)
        {
        }
    }
}