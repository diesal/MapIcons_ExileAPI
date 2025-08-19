using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace DieselTools_ExileAPI
{
    public class Keyboard
    {
        private const int KEYEVENTF_EXTENDEDKEY = 0x0001;
        private const int KEYEVENTF_KEYUP = 0x0002;

        private static HashSet<Keys> heldKeys = new HashSet<Keys>();

        [DllImport("user32.dll")]
        private static extern uint keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);



        private static bool IsExtendedKey(Keys key) {
            // List of extended keys
            return key == Keys.Up || key == Keys.Down || key == Keys.Left || key == Keys.Right ||
                   key == Keys.Insert || key == Keys.Delete || key == Keys.Home || key == Keys.End ||
                   key == Keys.PageUp || key == Keys.PageDown || key == Keys.NumLock;
        }


        public static void KeyDown(Keys key) {
            if (!heldKeys.Contains(key)) {
                keybd_event((byte)key, 0, KEYEVENTF_EXTENDEDKEY | 0, 0);
                heldKeys.Add(key);
            }
        }

        //public static void KeyUp(Keys key) {
        //    int flags = IsExtendedKey(key) ? KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP : KEYEVENTF_KEYUP;
        //    if (heldKeys.Contains(key)) {
        //        keybd_event((byte)key, 0, flags, 0); // Release key
        //        heldKeys.Remove(key);
        //        Log.Message($"Key up: {key}");
        //    }
        //}

        public static void KeyUp(Keys key) {
            int flags = IsExtendedKey(key) ? KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP : KEYEVENTF_KEYUP;
            if (heldKeys.Contains(key)) {
                keybd_event((byte)key, 0, flags, 0); // Release key
                heldKeys.Remove(key);
                //Log.Message($"Key up: {key}");
            }

            // If the key is a shift key, release all shift keys
            if (key == Keys.LShiftKey || key == Keys.RShiftKey || key == Keys.ShiftKey) {
                keybd_event((byte)Keys.LShiftKey, 0, KEYEVENTF_KEYUP, 0);
                keybd_event((byte)Keys.RShiftKey, 0, KEYEVENTF_KEYUP, 0);
                keybd_event((byte)Keys.ShiftKey, 0, KEYEVENTF_KEYUP, 0);

                heldKeys.Remove(Keys.LShiftKey);
                heldKeys.Remove(Keys.RShiftKey);
                heldKeys.Remove(Keys.ShiftKey);
            }
        }

        public static void ReleaseAllKeys() {
            foreach (var key in new List<Keys>(heldKeys)) // Create a copy to avoid modification during iteration
            {
                KeyUp(key);
            }
            heldKeys.Clear();
        }



    }
}
