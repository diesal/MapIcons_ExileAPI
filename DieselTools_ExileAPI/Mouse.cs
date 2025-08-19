using System.Runtime.InteropServices;
using SharpDX;
using DxPoint = SharpDX.Point;

namespace DieselTools_ExileAPI
{
    public class Mouse
    {
        public enum MouseEvents
        {
            LeftDown = 0x00000002,
            LeftUp = 0x00000004,
            MiddleDown = 0x00000020,
            MiddleUp = 0x00000040,
            Move = 0x00000001,
            Absolute = 0x00008000,
            RightDown = 0x00000008,
            RightUp = 0x00000010
        }

        [DllImport("user32.dll")]
        private static extern bool BlockInput(bool block);


        [DllImport("user32.dll")]
        public static extern bool SetCursorPos(int x, int y);

        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(out DxPoint lpPoint);

        public static DxPoint GetCursorPosition()
        {
            DxPoint lpPoint;
            GetCursorPos(out lpPoint);

            return lpPoint;
        }

        [DllImport("user32.dll")]
        private static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);


        public static void blockInput(bool block) {
            BlockInput(block);
        }

        public static void moveMouse(Vector2 pos)
        {
            SetCursorPos((int)pos.X, (int)pos.Y);
        }

        public static void LeftDown()
        {
            mouse_event((int)MouseEvents.LeftDown, 0, 0, 0, 0);
        }

        public static void LeftUp()
        {
            mouse_event((int)MouseEvents.LeftUp, 0, 0, 0, 0);
        }

        public static void RightDown()
        {
            mouse_event((int)MouseEvents.RightDown, 0, 0, 0, 0);
        }

        public static void RightUp()
        {
            mouse_event((int)MouseEvents.RightUp, 0, 0, 0, 0);
        }

    }
}
