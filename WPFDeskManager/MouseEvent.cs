using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;

namespace WPFDeskManager
{
    internal class MouseEvent
    {
        private static double DevicePixelRatioX = 1;
        private static double DevicePixelRatioY = 1;
        private static Action<Point>? ActionMouseMove;
        private static Action<Point>? ActionMouseLeftUp;

        private static IntPtr HookID = IntPtr.Zero;
        private static LowLevelMouseProc Proc = HookCallback;

        private const int WH_MOUSE_LL = 14;
        private const int WM_MOUSEMOVE = 0x0200;
        private const int WM_LBUTTONUP = 0x0202;
        private const int WM_RBUTTONUP = 0x0205;

        private delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);

        [StructLayout(LayoutKind.Sequential)]
        private struct POINT
        {
            public int x;
            public int y;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MSLLHOOKSTRUCT
        {
            public POINT pt;
            public uint mouseData;
            public uint flags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelMouseProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("user32.dll")]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        public static void SetHook(Action<Point>? mouseMove = null, Action<Point>? mouseLeftUp = null)
        {
            ActionMouseMove = mouseMove;
            ActionMouseLeftUp = mouseLeftUp;

            PresentationSource source = PresentationSource.FromVisual(Application.Current.MainWindow);
            if (source != null)
            {
                DevicePixelRatioX = source.CompositionTarget.TransformToDevice.M11;
                DevicePixelRatioY = source.CompositionTarget.TransformToDevice.M22;
            }

            Process process = Process.GetCurrentProcess();
            ProcessModule? module = process.MainModule;
            if (module == null)
            {
                return;
            }

            HookID = SetWindowsHookEx(WH_MOUSE_LL, Proc, GetModuleHandle(module.ModuleName), 0);
        }

        public static void Unhook()
        {
            UnhookWindowsHookEx(HookID);

            ActionMouseMove = null;
            ActionMouseLeftUp = null;
        }

        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                MSLLHOOKSTRUCT hookStruct = Marshal.PtrToStructure<MSLLHOOKSTRUCT>(lParam);
                double x = hookStruct.pt.x / DevicePixelRatioX;
                double y = hookStruct.pt.y / DevicePixelRatioY;

                if (wParam == (IntPtr)WM_MOUSEMOVE)
                {
                    ActionMouseMove?.Invoke(new Point(x, y));
                }
                else if (wParam == (IntPtr)WM_LBUTTONUP)
                {
                    ActionMouseLeftUp?.Invoke(new Point(x, y));
                }
                else if (wParam == (IntPtr)WM_RBUTTONUP)
                {

                }
            }

            return CallNextHookEx(HookID, nCode, wParam, lParam);
        }
    }
}
