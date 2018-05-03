using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Interop;
using System.Windows.Media;

namespace SkyViewer
{
    public class CommonWindow : Window
    {
        private static string Class = typeof(CommonWindow).ToString();

        #region private filelds

        private double normalWidth = 0;

        private double normalHeight = 0;

        private Button minButton;

        private Button maxButton;

        private Button regainButton;

        private Border TitleBorder;

        private Border ContentBorder;

        private ContentPresenter MainContent;

        private Image titleIcon;

        private IEnumerable<Thumb> resizeThumbs;

        #endregion

        public CommonWindow()
        {
            this.Loaded += CommonWindow_Loaded;
            this.SourceInitialized += CommonWindow_SourceInitialized;

            this.StateChanged += new EventHandler(CommonWindow_StateChanged);
            Style style = System.Windows.Application.Current.FindResource("CommonWindow") as Style;
            if (style != null)
            {
                this.Style = style;
            }
        }

        void CommonWindow_StateChanged(object sender, EventArgs e)
        {
            SetSizeByState(WindowState);
        }

        void CommonWindow_Loaded(object sender, RoutedEventArgs e)
        {

            if (WindowState == System.Windows.WindowState.Maximized)
            {
                this.normalWidth = this.Width * 0.8;
                this.normalHeight = this.Height * 0.8;
            }
            else
            {
                this.normalWidth = this.Width;
                this.normalHeight = this.Height;
            }

            InitializeEvent();

            SetSizeByState(WindowState);
        }

        static CommonWindow()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CommonWindow), new FrameworkPropertyMetadata(typeof(CommonWindow)));
        }

        private void SetSizeByState(WindowState winState)
        {
            if (winState == System.Windows.WindowState.Maximized)
            {
                this.Width = SystemParameters.WorkArea.Width;
                this.Height = SystemParameters.WorkArea.Height;
            }
            else
            {
                this.Width = this.normalWidth;
                this.Height = this.normalHeight;
            }
        }

        #region ===依赖属性===
        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register("CornerRadius", typeof(double), typeof(CommonWindow));

        public double CornerRadius
        {
            get
            {
                return (double)this.GetValue(CornerRadiusProperty);
            }
            set
            {
                this.SetValue(CornerRadiusProperty, value);
            }
        }

        public static readonly DependencyProperty TitleHorizontalAlignmentProperty = DependencyProperty.Register("TitleHorizontalAlignment",
            typeof(HorizontalAlignment), typeof(CommonWindow), new PropertyMetadata(HorizontalAlignment.Left));

        public HorizontalAlignment TitleHorizontalAlignment
        {
            get
            {
                return (HorizontalAlignment)this.GetValue(TitleHorizontalAlignmentProperty);
            }
            set
            {
                this.SetValue(TitleHorizontalAlignmentProperty, value);
            }
        }

        public static readonly DependencyProperty TitleBackgroundProperty = DependencyProperty.Register("TitleBackground", typeof(Brush), typeof(CommonWindow));

        public Brush TitleBackground
        {
            get
            {
                return (Brush)this.GetValue(TitleBackgroundProperty);
            }
            set
            {
                this.SetValue(TitleBackgroundProperty, value);
            }
        }

        public static readonly DependencyProperty TitleForgroundProperty = DependencyProperty.Register("TitleForground", typeof(Brush), typeof(CommonWindow));

        public Brush TitleForground
        {
            get
            {
                return (Brush)this.GetValue(TitleForgroundProperty);
            }
            set
            {
                this.SetValue(TitleForgroundProperty, value);
            }
        }

        public static readonly DependencyProperty TitleHeightProperty = DependencyProperty.Register("TitleHeight", typeof(double), typeof(CommonWindow));

        public double TitleHeight
        {
            get
            {
                return (double)this.GetValue(TitleHeightProperty);
            }
            set
            {
                this.SetValue(TitleHeightProperty, value);
            }
        }

        public static readonly DependencyProperty TitleDockProperty = DependencyProperty.Register("TitleDock", typeof(Dock), typeof(CommonWindow), new PropertyMetadata(Dock.Top));
        /// <summary>
        /// 标题栏停靠位置
        /// </summary>
        public Dock TitleDock
        {
            get
            {
                return (Dock)this.GetValue(TitleDockProperty);
            }
            set
            {
                this.SetValue(TitleDockProperty, value);
            }
        }
        #endregion

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            Thickness borderThickness = this.BorderThickness;

            TitleBorder = (Border)base.Template.FindName("PART_TitleBorder", this);
            ContentBorder = (Border)base.Template.FindName("PART_ContentBorder", this);

            titleIcon = (Image)base.Template.FindName("TitleIcon", this);
            if (titleIcon != null && this.Icon == null)
                titleIcon.Visibility = Visibility.Collapsed;
            MainContent = (ContentPresenter)base.Template.FindName("PART_MainContent", this);

            if (TitleBorder != null && TitleDock == Dock.Bottom)
            {
                TitleBorder.BorderThickness = new Thickness(borderThickness.Left, 0, borderThickness.Right, borderThickness.Bottom);
                TitleBorder.CornerRadius = new CornerRadius(0, 0, CornerRadius, CornerRadius);
            }
            else if (TitleBorder != null)
            {
                TitleBorder.BorderThickness = new Thickness(borderThickness.Left, borderThickness.Top, borderThickness.Right, 0);
                TitleBorder.CornerRadius = new CornerRadius(CornerRadius, CornerRadius, 0, 0);
            }
            if (ContentBorder != null && TitleDock == Dock.Bottom)
            {
                ContentBorder.BorderThickness = new Thickness(borderThickness.Left, borderThickness.Top, borderThickness.Right, 0);
                ContentBorder.CornerRadius = new CornerRadius(CornerRadius, CornerRadius, 0, 0);
            }
            else if (ContentBorder != null)
            {
                ContentBorder.BorderThickness = new Thickness(borderThickness.Left, 0, borderThickness.Right, borderThickness.Bottom);
                ContentBorder.CornerRadius = new CornerRadius(0, 0, CornerRadius, CornerRadius);
            }
            //if (MainContent != null && TitleDock == Dock.Bottom)
            //{

            //}
            //else if (MainContent != null)
            //{
            //    MainContent.Margin = new Thickness(0, 0, 0, CornerRadius);
            //}
        }

        private void InitializeEvent()
        {
            try
            {
                minButton = (Button)this.Template.FindName("MinButton", this);
                if (minButton != null)
                {
                    minButton.Click += delegate
                    {
                        WindowState = WindowState.Minimized;
                    };
                }

                maxButton = (Button)base.Template.FindName("MaxButton", this);
                if (maxButton != null)
                {
                    maxButton.Click += delegate
                    {
                        SwitchWindowState();
                    };
                }

                regainButton = (Button)this.Template.FindName("RegainButton", this);
                if (regainButton != null)
                {
                    regainButton.Click += delegate
                    {
                        SwitchWindowState();
                    };
                }

                Button closeBtn = (Button)this.Template.FindName("CloseButton", this);
                if (closeBtn != null)
                {
                    closeBtn.Click += delegate
                    {
                        this.Close();
                    };
                }

                Grid titlePart = (Grid)base.Template.FindName("TitlePart", this);
                if (titlePart != null)
                {
                    titlePart.MouseLeftButtonDown += titlePart_MouseLeftButtonDown;
                }

                Grid themeGrid = (Grid)this.Template.FindName("PART_ThumeGrid", this);
                if (themeGrid != null)
                {
                    resizeThumbs = themeGrid.Children.OfType<Thumb>();
                    if (resizeThumbs == null || resizeThumbs.Count() == 0)
                        return;
                    foreach (Thumb thumb in resizeThumbs)
                    {
                        thumb.DragDelta += thumb_DragDelta;
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        void titlePart_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                if (ResizeMode == ResizeMode.CanResize
                    || ResizeMode == ResizeMode.CanResizeWithGrip)
                {
                    SwitchWindowState();
                }
                return;
            }
            else if (WindowState == WindowState.Maximized)
            {
                return;
            }
            DragMove();
        }

        void thumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            try
            {
                Thumb thumb = sender as Thumb;
                string resizeDirection = Convert.ToString(thumb.Tag);
                switch (resizeDirection)
                {
                    case "Top":
                        if (this.Height - e.VerticalChange > MinHeight && this.Height - e.VerticalChange <= SystemParameters.PrimaryScreenHeight)
                        {
                            this.Top += e.VerticalChange;
                            this.Height -= e.VerticalChange;
                        }
                        break;
                    case "Bottom":
                        if (this.Height + e.VerticalChange > MinHeight && this.Height + e.VerticalChange <= SystemParameters.PrimaryScreenHeight)
                        {
                            this.Height += e.VerticalChange;
                        }
                        break;
                    case "Left":
                        if (this.Width - e.HorizontalChange > MinWidth && this.Width - e.HorizontalChange <= SystemParameters.PrimaryScreenWidth)
                        {
                            this.Left += e.HorizontalChange;
                            this.Width -= e.HorizontalChange;
                        }
                        break;
                    case "Right":
                        if (this.Width + e.HorizontalChange > MinWidth && this.Width + e.HorizontalChange <= SystemParameters.PrimaryScreenWidth)
                        {
                            this.Width += e.HorizontalChange;
                        }
                        break;
                    case "TopLeft":
                        if (this.Height - e.VerticalChange > MinHeight && this.Height - e.VerticalChange <= SystemParameters.PrimaryScreenHeight)
                        {
                            this.Top += e.VerticalChange;
                            this.Height -= e.VerticalChange;
                        }
                        if (this.Width - e.HorizontalChange > MinWidth && this.Width - e.HorizontalChange <= SystemParameters.PrimaryScreenWidth)
                        {
                            this.Left += e.HorizontalChange;
                            this.Width -= e.HorizontalChange;
                        }
                        break;
                    case "TopRight":
                        if (this.Height - e.VerticalChange > MinHeight && this.Height - e.VerticalChange <= SystemParameters.PrimaryScreenHeight)
                        {
                            this.Top += e.VerticalChange;
                            this.Height -= e.VerticalChange;
                        }
                        if (this.Width + e.HorizontalChange > MinWidth && this.Width + e.HorizontalChange <= SystemParameters.PrimaryScreenWidth)
                        {
                            this.Width += e.HorizontalChange;
                        }
                        break;
                    case "BottomLeft":
                        if (this.Height + e.VerticalChange > MinHeight && this.Height + e.VerticalChange <= SystemParameters.PrimaryScreenHeight)
                        {
                            this.Height += e.VerticalChange;
                        }
                        if (this.Width - e.HorizontalChange > MinWidth && this.Width - e.HorizontalChange <= SystemParameters.PrimaryScreenWidth)
                        {
                            this.Left += e.HorizontalChange;
                            this.Width -= e.HorizontalChange;
                        }
                        break;
                    case "BottomRight":
                        if (this.Height + e.VerticalChange > MinHeight && this.Height + e.VerticalChange <= SystemParameters.PrimaryScreenHeight)
                        {
                            this.Height += e.VerticalChange;
                        }
                        if (this.Width + e.HorizontalChange > MinWidth && this.Width + e.HorizontalChange <= SystemParameters.PrimaryScreenWidth)
                        {
                            this.Width += e.HorizontalChange;
                        }
                        break;
                    default:
                        break;
                }
                this.normalWidth = this.Width;
                this.normalHeight = this.Height;
            }
            catch (Exception ex)
            {

            }
        }

        void CommonWindow_SourceInitialized(object sender, EventArgs e)
        {
            IntPtr mWindowHandle = (new WindowInteropHelper(this)).Handle;
            HwndSource.FromHwnd(mWindowHandle).AddHook(new HwndSourceHook(WindowProc));
        }

        private static System.IntPtr WindowProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                case 0x0024:
                    WmGetMinMaxInfo(hwnd, lParam);
                    break;
            }
            return IntPtr.Zero;
        }

        private static void WmGetMinMaxInfo(System.IntPtr hwnd, System.IntPtr lParam)
        {
            try
            {


                POINT lMousePosition;
                GetCursorPos(out lMousePosition);

                IntPtr lPrimaryScreen = MonitorFromPoint(new POINT(0, 0), MonitorOptions.MONITOR_DEFAULTTOPRIMARY);
                MONITORINFO lPrimaryScreenInfo = new MONITORINFO();
                if (GetMonitorInfo(lPrimaryScreen, lPrimaryScreenInfo) == false)
                {
                    return;
                }

                IntPtr lCurrentScreen = MonitorFromPoint(lMousePosition, MonitorOptions.MONITOR_DEFAULTTONEAREST);

                MINMAXINFO lMmi = (MINMAXINFO)Marshal.PtrToStructure(lParam, typeof(MINMAXINFO));

                RECT rcWorkArea = lPrimaryScreenInfo.rcWork;
                RECT rcMonitorArea = lPrimaryScreenInfo.rcMonitor;


                if (lPrimaryScreen.Equals(lCurrentScreen))
                {
                    lMmi.ptMaxPosition.X = Math.Abs(rcWorkArea.Left - rcMonitorArea.Left);
                    lMmi.ptMaxPosition.Y = Math.Abs(rcWorkArea.Top - rcMonitorArea.Top);
                    lMmi.ptMaxSize.X = Math.Abs(rcWorkArea.Right - rcWorkArea.Left);
                    lMmi.ptMaxSize.Y = Math.Abs(rcWorkArea.Bottom - rcWorkArea.Top);
                }
                else
                {
                    lMmi.ptMaxPosition.X = Math.Abs(rcWorkArea.Left - rcMonitorArea.Left);
                    lMmi.ptMaxPosition.Y = Math.Abs(rcWorkArea.Top - rcMonitorArea.Top);
                    lMmi.ptMaxSize.X = Math.Abs(rcWorkArea.Right - rcWorkArea.Left);
                    lMmi.ptMaxSize.Y = Math.Abs(rcWorkArea.Bottom - rcWorkArea.Top);
                }

                Marshal.StructureToPtr(lMmi, lParam, true);
            }
            catch (Exception ex)
            {

            }
        }

        private void SwitchWindowState()
        {
            switch (WindowState)
            {
                case WindowState.Normal:
                    WindowState = WindowState.Maximized;
                    SetSizeByState(WindowState);
                    break;
                case WindowState.Maximized:
                    WindowState = WindowState.Normal;
                    SetSizeByState(WindowState);
                    break;
            }
        }


        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetCursorPos(out POINT lpPoint);

        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr MonitorFromPoint(POINT pt, MonitorOptions dwFlags);

        [DllImport("user32.dll")]
        static extern bool GetMonitorInfo(IntPtr hMonitor, MONITORINFO lpmi);

        enum MonitorOptions : uint
        {
            MONITOR_DEFAULTTONULL = 0x00000000,
            MONITOR_DEFAULTTOPRIMARY = 0x00000001,
            MONITOR_DEFAULTTONEAREST = 0x00000002
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;

            public POINT(int x, int y)
            {
                this.X = x;
                this.Y = y;
            }
        }


        [StructLayout(LayoutKind.Sequential)]
        public struct MINMAXINFO
        {
            public POINT ptReserved;
            public POINT ptMaxSize;
            public POINT ptMaxPosition;
            public POINT ptMinTrackSize;
            public POINT ptMaxTrackSize;
        };


        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public class MONITORINFO
        {
            public int cbSize = Marshal.SizeOf(typeof(MONITORINFO));
            public RECT rcMonitor = new RECT();
            public RECT rcWork = new RECT();
            public int dwFlags = 0;
        }


        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left, Top, Right, Bottom;

            public RECT(int left, int top, int right, int bottom)
            {
                this.Left = left;
                this.Top = top;
                this.Right = right;
                this.Bottom = bottom;
            }
        }
    }
}
