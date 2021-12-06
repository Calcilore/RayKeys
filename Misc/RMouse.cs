using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using RayKeys.Render;

namespace RayKeys.Misc {
    public static class RMouse {
        public static int X;
        public static int Y;
        public static bool LeftButton;
        public static bool RightButton;
        public static bool MiddleButton;
        public static bool XButton1;
        public static bool XButton2;
        public static int ScrollPosition;
        public static int ScrollFrame;
        
        public static int LastX;
        public static int LastY;
        public static bool LastLeftButton;
        public static bool LastRightButton;
        public static bool LastMiddleButton;
        public static bool LastXButton1;
        public static bool LastXButton2;
        public static int LastScrollPosition;
        public static int LastScrollFrame;

        public static bool LeftButtonPressed;
        public static bool RightButtonPressed;
        public static bool MiddleButtonPressed;
        public static bool XButton1Pressed;
        public static bool XButton2Pressed;

        public static void Update() {
            LastX = X; LastY = Y;
            LastLeftButton = LeftButton;
            LastMiddleButton = MiddleButton;
            LastRightButton = RightButton;
            LastXButton1 = XButton1;
            LastXButton2 = XButton2;
            LastScrollPosition = ScrollPosition;
            LastScrollFrame = ScrollFrame;

            MouseState ms = Mouse.GetState();
            
            // funny window things
            Point pos = ((ms.Position - Game1.Game.RenderRectangle.Location).ToVector2() / Game1.Game.Scaling).ToPoint();
            pos += RRender.CameraPos.ToPoint();

            X = pos.X; Y = pos.Y;
            
            LeftButton   = ms.LeftButton   == ButtonState.Pressed;
            RightButton  = ms.LeftButton   == ButtonState.Pressed; 
            MiddleButton = ms.MiddleButton == ButtonState.Pressed;
            XButton1     = ms.XButton1     == ButtonState.Pressed;
            XButton2     = ms.XButton2     == ButtonState.Pressed;
            
            ScrollPosition = Mouse.GetState().ScrollWheelValue;
            ScrollFrame = ScrollPosition - LastScrollPosition;
            
            LeftButtonPressed = LeftButton && !LastLeftButton;
            RightButtonPressed = RightButton && !LastRightButton;
            MiddleButtonPressed = MiddleButton && !LastMiddleButton;
            XButton1Pressed =  XButton1 && !LastXButton1;
            XButton1Pressed =  XButton1 && !LastXButton1;
        }
    }
}
