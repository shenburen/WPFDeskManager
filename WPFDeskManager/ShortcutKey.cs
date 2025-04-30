using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

namespace WPFDeskManager
{
    internal class ShortcutKey
    {
        #region Win32 API
        // 激活窗口ID
        private const int ACTIVE_MAIN_WINDOW = 9000;

        private const int WM_HOTKEY = 0x0312; // 快捷键
        private const uint MOD_ALT = 0x0001; // alt
        private const uint MOD_CONTROL = 0x0002; // ctrl
        private const uint MOD_SHIFT = 0x0004; // shift
        private const uint MOD_WIN = 0x0008; // win

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);
        #endregion

        /// <summary>
        /// 注册快捷键
        /// </summary>
        public static void CreateShortcutKey()
        {
            IntPtr hwnd = new WindowInteropHelper(Global.MainWindow).Handle;
            HwndSource source = HwndSource.FromHwnd(hwnd);
            source.AddHook(HwndHook);

            if (!RegisterHotKey(hwnd, ACTIVE_MAIN_WINDOW, MOD_CONTROL, (uint)KeyInterop.VirtualKeyFromKey(Key.Space)))
            {
                // 快捷键注册失败，后续我想在注册失败的时候发出通知，现在暂不做处理。
            }
        }

        /// <summary>
        /// 清理资源
        /// </summary>
        public static void Dispose()
        {
            IntPtr hwnd = new WindowInteropHelper(Global.MainWindow).Handle;
            UnregisterHotKey(hwnd, ACTIVE_MAIN_WINDOW);
        }

        /// <summary>
        /// 处理窗口消息
        /// </summary>
        /// <param name="hwnd"></param>
        /// <param name="msg"></param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <param name="handled"></param>
        /// <returns></returns>
        private static IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == WM_HOTKEY)
            {
                int id = wParam.ToInt32();
                if (id == ACTIVE_MAIN_WINDOW && Global.MainWindow != null)
                {
                    // Global.MainWindow.Activate();
                    if (Global.MainWindow.Visibility == Visibility.Visible)
                    {
                        Global.MainWindow.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        Global.MainWindow.Visibility = Visibility.Visible;
                    }
                }
                handled = true;
            }
            return IntPtr.Zero;
        }
    }
}
