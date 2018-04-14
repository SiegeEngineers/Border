using Border.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Linq;
using System.Windows.Media;
using Border.Helpers;
using Border.ViewModel;
using Border.Properties;

namespace Border.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Loaded += OnLoaded;
            BuildOrders = App.BuildOrderList;
            PingTitle(Strings.AppName, 5000);  
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            WindowsServices.Initialize(this);
        }

        protected override void OnClosed(EventArgs e)
        {
            //WindowsServices.Close(this); // windows is pretty good at doing this by itself
            WindowsServices.UnregisterAllKeys(this);
            Settings.X = (long)Left;
            Settings.Y = (long)Top;
            if (Settings.Vertical)
            {
                Settings.VerticalWidth = Width;
                Settings.VerticalHeight = Height;
                Settings.HorizontalWidth = HorizontalWidth;
                Settings.HorizontalHeight = HorizontalHeight;
            }
            else
            {
                Settings.HorizontalHeight = Height;
                Settings.HorizontalWidth = Width;
                Settings.VerticalWidth = VerticalHeight;
                Settings.VerticalHeight = VerticalHeight;
            }

            Settings.ClickThrough = ClickThrough;
            Settings.AlwaysOnTop = AlwaysOnTop;

            Settings.LastBO = BuildOrders.Current?.Title;

            Settings.Vertical = Vertical;
            Settings.Compact = Compact;
            base.OnClosed(e);
        }
        Int32Converter buttonConverter = new Int32Converter();
        protected void OnLoaded(object sender, RoutedEventArgs e)
        {
            Left = Settings.X;
            Top = Settings.Y;
            VerticalWidth = Settings.VerticalWidth;
            VerticalHeight = Settings.VerticalHeight;
            HorizontalWidth = Settings.HorizontalWidth;
            HorizontalHeight = Settings.VerticalHeight;
            if (Settings.Vertical)
            {
                Width = Settings.VerticalWidth;
                Height = Settings.VerticalHeight;
            }
            else
            {
                Width = Settings.HorizontalWidth;
                Height = Settings.HorizontalHeight;
            }
            Vertical = Settings.Vertical;
            ClickThrough = Settings.ClickThrough;
            AlwaysOnTop = Settings.AlwaysOnTop;
            Compact = Settings.Compact;

            PanelWidth = Settings.PanelWidth;
            PanelHeight = Settings.PanelHeight;
            Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(Settings.BackgroundColor));

            RegisterGlobalHotkey(Settings.Buttons.ToggleClickThrough, () =>
            {
                ClickThrough = !ClickThrough;
            });

            RegisterGlobalHotkey(Settings.Buttons.NextBO, () =>
            {
                NextBuildOrder();
            });
            RegisterGlobalHotkey(Settings.Buttons.PreviousBO, () =>
            {
                PreviousBuildOrder();
            });
            RegisterGlobalHotkey(Settings.Buttons.NextTask, () =>
            {
                NextTask();
            });
            RegisterGlobalHotkey(Settings.Buttons.PreviousTask, () =>
            {
                PreviousTask();
            });
        }

        double VerticalWidth,
        VerticalHeight,
        HorizontalWidth,
        HorizontalHeight;

        private WindowsServices.Hotkey GenerateHotkey(ButtonData button)
        {
            try
            {
                int key = (int)buttonConverter.ConvertFromString(button.KeyCode);
                return new WindowsServices.Hotkey(key, button.Alt, button.Shift, button.Control);
            } catch (NotSupportedException)
            {
                throw;
            }

        }
        private bool RegisterGlobalHotkey(ButtonData button, WindowsServices.HotkeyActivated callback)
        {
            try
            {
                var key = GenerateHotkey(button);
                var res = WindowsServices.RegisterKey(key, callback, this) >= 0;
                return res;
            } catch (NotSupportedException)
            {
                return false;
            }
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                // this prevents win7 aerosnap TODO: Prevent aeroresize
                if (this.ResizeMode != System.Windows.ResizeMode.NoResize)
                {
                    // Small hack to disable window edge resizing
                    this.ResizeMode = System.Windows.ResizeMode.NoResize;
                    this.UpdateLayout();
                }

                DragMove();
            }
        }

        private void Window_MouseLeave(object sender, MouseEventArgs e)
        {
            // Small hack to b/c mouse up is not called
            if (this.ResizeMode == System.Windows.ResizeMode.NoResize)
            {
                // restore resize grips
                this.ResizeMode = System.Windows.ResizeMode.CanResizeWithGrip;
                this.UpdateLayout();
            }

        }

        public bool ClickThrough
        {
            get
            {
                return WindowsServices.WindowTransparent;
            }
            private set {
                if (value)
                {
                    Opacity = Settings.ClickThroughOpacity;
                    if (Opacity < 0.1)
                    {
                        Opacity = 0.1;
                    }
                    WindowsServices.SetWindowExTransparent(this);
                }
                else
                {
                    Opacity = Settings.Opacity;
                    if (Opacity < 0.1)
                    {
                        Opacity = 0.1;
                    }
                    WindowsServices.UnsetWindowExTransparent(this);
                }
                base.SetValue(ClickThroughProperty, value);
            }
        }
        public static readonly DependencyProperty ClickThroughProperty =
            DependencyProperty.Register("ClickThrough", typeof(bool), typeof(MainWindow), null);

        private void ClickThrough_Checked(object sender, RoutedEventArgs e)
        {
            if (Settings.WarnClickThrough)
            {
                MessageBoxResult result = MessageBox.Show(this, "The only way to turn this back on is via the shortcut: " + Settings.Buttons.ToggleClickThrough.HumanReadable + "\n\nDo we need to bug you about it in the future?", "Click Through", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);

                // Process message box results
                switch (result)
                {
                    case MessageBoxResult.No:
                        Settings.WarnClickThrough = false;
                        break;
                    case MessageBoxResult.Cancel:
                        (sender as System.Windows.Controls.MenuItem).IsChecked = false;
                        return;
                }
            }
            ClickThrough = true;
        }
        private void ClickThrough_Unchecked(object sender, RoutedEventArgs e)
        {
            ClickThrough = false;
        }

        public bool Compact
        {
            get { return (bool)base.GetValue(CompactProperty); }
            set { base.SetValue(CompactProperty, value); }
        }
        public static readonly DependencyProperty CompactProperty =
          DependencyProperty.Register("Compact", typeof(bool), typeof(MainWindow), null);

        private void Compact_Checked(object sender, RoutedEventArgs e)
        {
            Compact = true;
        }
        private void Compact_Unchecked(object sender, RoutedEventArgs e)
        {
            Compact = false;
        }

        public bool Vertical
        {
            get { return (bool)base.GetValue(VerticalProperty); }
            set { if (value)
                {
                    HorizontalWidth = Width;
                    HorizontalHeight = Height;
                    Width = VerticalWidth;
                    Height = VerticalHeight;
                }
                else {
                    VerticalWidth = Width;
                    VerticalHeight = Height;
                    Width = HorizontalWidth;
                    Height = HorizontalHeight;
                };
                base.SetValue(VerticalProperty, value);
            }
        }
        public static readonly DependencyProperty VerticalProperty =
          DependencyProperty.Register("Vertical", typeof(bool), typeof(MainWindow), null);

        private void Vertical_Checked(object sender, RoutedEventArgs e)
        {
            Vertical = true;
        }
        private void Vertical_Unchecked(object sender, RoutedEventArgs e)
        {
            Vertical = false;
        }

        public bool AlwaysOnTop {
            get { return (bool)base.GetValue(AlwaysOnTopProperty); }
            set { base.SetValue(AlwaysOnTopProperty, value); Topmost = value; }
        }
        public static readonly DependencyProperty AlwaysOnTopProperty =
          DependencyProperty.Register("AlwaysOnTop", typeof(bool), typeof(MainWindow), null);



        public double PanelWidth
        {
            get { return (double)GetValue(PanelWidthProperty); }
            set { SetValue(PanelWidthProperty, value); }
        }

        public static readonly DependencyProperty PanelWidthProperty =
            DependencyProperty.Register("PanelWidth", typeof(double), typeof(MainWindow));

        public double PanelHeight
        {
            get { return (double)GetValue(PanelHeightProperty); }
            set { SetValue(PanelHeightProperty, value); }
        }

        public static readonly DependencyProperty PanelHeightProperty =
            DependencyProperty.Register("PanelHeight", typeof(double), typeof(MainWindow));



        private void AlwaysOnTop_Checked(object sender, RoutedEventArgs e)
        {
            AlwaysOnTop = true;
        }
        private void AlwaysOnTop_Unchecked(object sender, RoutedEventArgs e)
        {
            AlwaysOnTop = false;
        }
        private void NextBuildOrder_Click(object sender, RoutedEventArgs e)
        {
            NextBuildOrder();
        }

        private void NextBuildOrder()
        {

            BuildOrders.Next();
            string message = BuildOrders.Current?.Title;
            if (message == null)
            {
                message = Strings.Title_NoBuildOrder;
            }
            PingTitle(message, 2000);
            UpdateScrollPosition();
        }



        public string TitleMessage
        {
            get { return (string)GetValue(TitleMessageProperty); }
            set { SetValue(TitleMessageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TitleMessage.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TitleMessageProperty =
            DependencyProperty.Register("TitleMessage", typeof(string), typeof(MainWindow), new PropertyMetadata(Strings.AppName));



        object TitleVisibilityLock = new object();
        public Visibility TitleVisibility
        {
            get { lock (TitleVisibilityLock) { return (Visibility)GetValue(ShowTitleProperty); } }
            set { lock (TitleVisibilityLock) { SetValue(ShowTitleProperty, value); } }
        }

        // Using a DependencyProperty as the backing store for ShowTitle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowTitleProperty =
            DependencyProperty.Register("TitleVisibility", typeof(Visibility), typeof(MainWindow), new PropertyMetadata(Visibility.Hidden));

        public DateTime lastTitlePing;

        private async void PingTitle(string message, int time)
        {
            lastTitlePing = DateTime.UtcNow;
            TitleVisibility = Visibility.Visible;
            TitleMessage = message;
            // TODO: Threadsafe? probably not
            await System.Threading.Tasks.Task.Delay(time+1);
            
            if((DateTime.UtcNow - lastTitlePing).TotalMilliseconds >= time)
            {
                TitleVisibility = Visibility.Hidden;
            }
            
        }

        private void PreviousBuildOrder_Click(object sender, RoutedEventArgs e)
        {
            PreviousBuildOrder();
        }

        private void PreviousBuildOrder()
        {
            BuildOrders.Previous();
            string message = BuildOrders.Current?.Title;
            if(message == null)
            {
                message = Strings.Title_NoBuildOrder;
            }
            PingTitle(message, 2000);
            UpdateScrollPosition();
        }

        private void NextStep_Click(object sender, RoutedEventArgs e)
        {
            NextTask();
        }

        private void NextTask()
        {
            BuildOrders.Current?.Next();
            UpdateScrollPosition();
        }

        private void PreviousStep_Click(object sender, RoutedEventArgs e)
        {
            PreviousTask();
        }

        private void PreviousTask()
        {
            BuildOrders.Current?.Previous();
            UpdateScrollPosition();
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            App.ShowAbout();
        }
        private void Quit_Click(object sender, RoutedEventArgs e)
        {
            App.Shutdown();
        }
        private void Update_Click(object sender, RoutedEventArgs e)
        {
            App.CheckUpdates(true);
        }

        private App _App;
        public App App
        {
            get
            {
                if (_App == null)
                {
                    _App = ((App)Application.Current);
                }
                return _App;
            }
        }

        public SettingsData Settings {
            get { return App.Settings; }
        }

        public BuildOrderList BuildOrders
        {
            get { return (BuildOrderList)GetValue(BuildOrderListProperty); }
            set { SetValue(BuildOrderListProperty, value); }
        }

        public static readonly DependencyProperty BuildOrderListProperty =
            DependencyProperty.Register("BuildOrders", typeof(BuildOrderList), typeof(MainWindow));

        public SelectBuildOrderCommand SelectBuildOrder
        {
            get { return new SelectBuildOrderCommand(this); }
        }



        public double ScrollLeft
        {
            get { return (double)GetValue(ScrollLeftProperty); }
            set { SetValue(ScrollLeftProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ScrollLeft.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ScrollLeftProperty =
            DependencyProperty.Register("ScrollLeft", typeof(double), typeof(MainWindow), new PropertyMetadata(0.0));



        public double ScrollTop
        {
            get { return (double)GetValue(ScrollTopProperty); }
            set { SetValue(ScrollTopProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ScrollTop.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ScrollTopProperty =
            DependencyProperty.Register("ScrollTop", typeof(double), typeof(MainWindow), new PropertyMetadata(0.0));


        public void UpdateScrollPosition()
        {
            if (Vertical) {
                double taskPos = GetActiveTaskPosition();
                double windowSize = ActualHeight;
                double containerSize = TaskContainer.ActualHeight;
                if(windowSize > containerSize)
                {
                    ScrollTop = 0;
                    ScrollLeft = 0;
                    return;
                }
                //if(taskPos > windowSize / 2)
                //{
                //    ScrollTop = -(taskPos-(windowSize / 2));
                //}
                //else
                //{
                //    ScrollTop = 0;
                //}
                double frac = taskPos / (containerSize - PanelHeight);
                ScrollTop = -(frac * (containerSize - windowSize));
                ScrollLeft = 0;
            }
            else
            {
                double taskPos = GetActiveTaskPosition();
                double windowSize = ActualWidth;
                double containerSize = TaskContainer.ActualWidth;
                if (windowSize > containerSize)
                {
                    ScrollLeft = 0;
                    ScrollTop = 0;
                    return;
                }
                //if (taskPos > windowSize / 2)
                //{
                //    ScrollLeft = -(taskPos - (windowSize / 2));
                //}
                //else
                //{
                //    ScrollLeft = 0;
                //}
                double frac = taskPos / (containerSize-PanelWidth);
                ScrollLeft = -(frac * (containerSize - windowSize));
                ScrollTop = 0;
            }
        }

        public double GetActiveTaskPosition()
        {
            if(BuildOrders.Current != null && BuildOrders.Current.Current != null)
            {
                int pos = BuildOrders.Current.GetCurrentId();
                if (pos < 0) { return 0; }
                if (Vertical)
                {
                    return pos * PanelHeight;
                }
                else
                {
                    return pos * PanelWidth;
                }
            }
            return 0;
        }

        public Task GetTask(int index)
        {
            return VisualTreeHelper.GetChild(TaskContainer, index) as Task;
        }

        public async void SetDefaultBO()
        {
            if(Settings.LastBO == null)
            {
                BuildOrders.SetCurrentBuildOrder(BuildOrders.List[0]);
            }
            else
            {
                var bo = BuildOrders.List.FirstOrDefault((x) => x.Title == Settings.LastBO);
                if (bo == null)
                {
                    BuildOrders.SetCurrentBuildOrder(BuildOrders.List[0]);
                }
                else
                {
                    BuildOrders.SetCurrentBuildOrder(bo);
                }
            }

            if (BuildOrders.Current != null)
            {
                PingTitle(BuildOrders.Current.Title, 2000);
            }
        }
    }
}
