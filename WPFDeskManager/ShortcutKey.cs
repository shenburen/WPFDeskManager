using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media.Animation;

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
                Debug.WriteLine("注册快捷键失败！");
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
                    if (Global.MainWindow.Visibility == Visibility.Visible)
                    {
                        WindowTransition(1, 0, () =>
                        {
                            Global.MainWindow.Visibility = Visibility.Collapsed;
                        });
                    }
                    else
                    {
                        Global.MainWindow.Visibility = Visibility.Visible;
                        WindowTransition(0, 1);
                    }
                }
                handled = true;
            }
            return IntPtr.Zero;
        }

        /// <summary>
        /// 窗体淡入淡出动画
        /// </summary>
        /// <param name="from">起点</param>
        /// <param name="to">终点</param>
        /// <param name="action">如果有想执行的其它动作</param>
        private static void WindowTransition(double from, double to, Action? action = null)
        {
            DoubleAnimation fade = new DoubleAnimation
            {
                From = from,
                To = to,
                Duration = TimeSpan.FromMilliseconds(200),
            };
            fade.Completed += (object? sender, EventArgs e) =>
            {
                action?.Invoke();
            };

            Global.MainWindow?.BeginAnimation(Window.OpacityProperty, fade);
        }
    }
}
