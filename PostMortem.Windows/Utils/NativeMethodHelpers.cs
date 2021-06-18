using System;
using System.Runtime.InteropServices;

namespace PostMortem.Windows.Utils
{
    static public class NativeMethodHelpers
    {
        [DllImport("user32.dll")]
        static private extern int SetWindowLong(IntPtr hwnd, int index, int newStyle);

        private const int GWL_HWNDPARENT = -8;

        static public void SetOwnerWindow(IntPtr windowHandle, IntPtr ownerHandle)
        {
            SetWindowLong(windowHandle, GWL_HWNDPARENT, ownerHandle.ToInt32());
        }
    }
}