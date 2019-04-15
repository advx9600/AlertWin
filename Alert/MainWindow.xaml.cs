using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Alert
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private Brush originalBg;
        System.Windows.Forms.NotifyIcon notify;
        private int setTime, remainMinute;
        DataStore dataStore = new DataStore();

        enum AlertType
        {
            NotStart,
            Start,
            Rest
        }
        AlertType alertType = AlertType.NotStart;
        private int resetTime;

        public MainWindow()
        {
            InitializeComponent();
            SetData();

            ShowInCenter();
            SetOriginalUI();
            SetSystemTrayNotify();

            StartTickThread();
        }

        private void SetOriginalUI()
        {
            labelRemainTime.Content = "";
            labelRestTime.Content = "";
            textSetMinute.Text = "" + setTime;
            originalBg = btnSetTime.Background;
            btnOk.Focus();
        }

        private void SetData()
        {
            setTime = remainMinute = dataStore.ReadMinute();
        }

        private void StartTickThread()
        {
            DispatcherTimer dispatcher = new DispatcherTimer();
            dispatcher.Tick += Dispatcher_Tick;
            dispatcher.Interval = new TimeSpan(0, 1, 0);
            dispatcher.Start();
        }

        private void Dispatcher_Tick(object sender, EventArgs e)
        {
            if (alertType == AlertType.Start)
            {
                if (remainMinute < 1)
                {
                    Show();
                    remainMinute = setTime;
                    alertType = AlertType.Rest;
                    resetTime = 0;
                }
                else
                {
                    if (remainMinute == setTime)
                    {
                        notify.BalloonTipText = "remain " + remainMinute + " minutes";
                        notify.ShowBalloonTip(2000);
                    }
                    remainMinute -= 1;
                    SetRemainText( "remain " + remainMinute + " minutes");
                }
            }
            else if (alertType == AlertType.Rest)
            {
                SetRestTimeText("rest " + resetTime +" minutes");
                resetTime += 1;
            }
        }

        private void SetRemainText(string text)
        {
            if (labelRestTime.IsVisible)
            {
                labelRestTime.Visibility = Visibility.Hidden ;
            }

            if (!labelRemainTime.IsVisible)
            {
                labelRemainTime.Visibility = Visibility.Visible;
            }

            labelRemainTime.Content = notify.Text = text;
        }

        private void SetRestTimeText(string text)
        {
            if (labelRemainTime.IsVisible)
            {
                labelRemainTime.Visibility = Visibility.Hidden;
            }

            if (!labelRestTime.IsVisible)
            {
                labelRestTime.Visibility = Visibility.Visible;
            }

            labelRestTime.Content = text;
        }

        private void SetSystemTrayNotify()
        {
            notify = new System.Windows.Forms.NotifyIcon();
            notify.Visible = true;
            notify.Icon = new System.Drawing.Icon("alert.ico");
            notify.Text = "Wait for second";
            notify.MouseClick += Notify_MouseClick;
        }

        private void Notify_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            Show();
        }

        private void ShowInCenter()
        {
            var screenWidth = SystemParameters.PrimaryScreenWidth;
            var screenHeight = SystemParameters.PrimaryScreenHeight;
            this.Left = (screenWidth - this.Width) / 2;
            this.Top = (screenHeight - this.Height) / 2;
        }

        private void ButtonSetTime_Click(object sender, RoutedEventArgs e)
        {
            var time = textSetMinute.Text.ToString();
            dataStore.WriteMinute(int.Parse(time));
            SetData();
            MessageBox.Show("set time ok");
        }


        private void ButtonOk_Click(object sender, RoutedEventArgs e)
        {
            Hide();
            alertType = AlertType.Start;
            Dispatcher_Tick(null, null);
        }


        private void Btn_LostFocus(object sender, RoutedEventArgs e)
        {
            ((Button)sender).Background = originalBg;
        }

        private void Btn_GotFocus(object sender, RoutedEventArgs e)
        {
            ((Button)sender).Background = Brushes.Green;
        }
    }
}
