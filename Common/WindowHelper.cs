using System;
using System.Runtime.InteropServices;

namespace Common
{
    public class WindowHelper
    {
        public static string Class = typeof(WindowHelper).ToString();

        private const int SW_SHOWNOMAL = 1;

        public const int HWND_TOP = 0;

        public const int HWND_BOTTOM = 1;

        public const int SWP_NOSIZE = 0x1;

        public const int SWP_NOMOVE = 0x2;

        /// <summary>
        /// 设置窗口在桌面上的位置
        /// </summary>
        /// <param name="hwnd"></param>
        /// <param name="hWndInsertAfter"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="cx"></param>
        /// <param name="cy"></param>
        /// <param name="wFlags"></param>
        /// <returns></returns>
        [DllImport("User32.dll")]
        public static extern int SetWindowPos(IntPtr hwnd, int hWndInsertAfter, int x, int y, int cx, int cy, int wFlags);

        ///<summary>
        /// 该函数设置由不同线程产生的窗口的显示状态
        /// </summary>
        /// <param name="hWnd">窗口句柄</param>
        /// <param name="cmdShow">指定窗口如何显示。查看允许值列表，请查阅ShowWlndow函数的说明部分</param>
        /// <returns>如果函数原来可见，返回值为非零；如果函数原来被隐藏，返回值为零</returns>
        [DllImport("User32.dll")]
        private static extern bool ShowWindowAsync(IntPtr hWnd, int cmdShow);

        /// <summary>
        ///  该函数将创建指定窗口的线程设置到前台，并且激活该窗口。键盘输入转向该窗口，并为用户改各种可视的记号。
        ///  系统给创建前台窗口的线程分配的权限稍高于其他线程。 
        /// </summary>
        /// <param name="hWnd">将被激活并被调入前台的窗口句柄</param>
        /// <returns>如果窗口设入了前台，返回值为非零；如果窗口未被设入前台，返回值为零</returns>
        [DllImport("User32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        public static void ShowWindowAsyncX(IntPtr hWnd)
        {
            ShowWindowAsync(hWnd, SW_SHOWNOMAL);
        }

        public static void SetForegroundWindowX(IntPtr hWnd)
        {
            SetForegroundWindow(hWnd);
        }
    }
}
