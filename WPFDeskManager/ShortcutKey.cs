using System.Runtime.InteropServices;
using System.Windows.Input;
using System.Windows.Interop;

namespace WPFDeskManager
{
    internal class ShortcutKey
    {
        #region Win32 API
        // 激活窗口ID
        private const int ACTIVE_MAIN_WINDOW = 9000;

        private const int WINDOWS_HOTKEY = 0x0312;
        private const uint WINDOWS_HOTKEY_ALT = 0x0001;
        private const uint WINDOWS_HOTKEY_CONTROL = 0x0002;
        private const uint WINDOWS_HOTKEY_SHIFT = 0x0004;
        private const uint WINDOWS_HOTKEY_WIN = 0x0008;

        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);
        #endregion

        /// <summary>
        /// 构造函数
        /// </summary>
        public ShortcutKey()
        {
            WindowInteropHelper helper = new WindowInteropHelper(Global.MainWindow);
            HwndSource source = HwndSource.FromHwnd(helper.Handle);
            source.AddHook(HwndHook);

            if (!RegisterHotKey(helper.Handle, ACTIVE_MAIN_WINDOW, WINDOWS_HOTKEY_CONTROL, (uint)KeyInterop.VirtualKeyFromKey(Key.Space)))
            {
                // 快捷键注册失败，后续我想在注册失败的时候发出通知，现在暂不做处理。
            }
        }

        /// <summary>
        /// 清理资源
        /// </summary>
        public void Dispose()
        {
            WindowInteropHelper helper = new WindowInteropHelper(Global.MainWindow);
            UnregisterHotKey(helper.Handle, ACTIVE_MAIN_WINDOW);
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
        private IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == WINDOWS_HOTKEY)
            {
                int id = wParam.ToInt32();
                if (id == ACTIVE_MAIN_WINDOW && Global.MainWindow != null)
                {
                    Global.MainWindow.Activate();
                }
                handled = true;
            }
            return IntPtr.Zero;
        }
    }
}
