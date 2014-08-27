using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace SpaceMarines
{

    public static class Log
    {
        public static MainPage Window;

        public static void GuiThread(Action a)
        {
            Window.Dispatcher.BeginInvoke(a);
        }

        public static void ToggleConnectStatus(bool isConnected)
        {
            GuiThread(()=>Window.tbServerOffline.Visibility = isConnected ? Visibility.Collapsed : Visibility.Visible);
        }

		public static void ToggleLog(bool isVisible)
		{
			Window.tbLog.Visibility = isVisible ? Visibility.Visible : Visibility.Collapsed;
		}

		public static void ToggleStatusBar(bool isVisible)
		{
			Window.tbFramesPerSecond.Visibility = isVisible ? Visibility.Visible : Visibility.Collapsed;
			Window.tbLatency.Visibility = isVisible ? Visibility.Visible : Visibility.Collapsed;
		}

		public static void UpdateFPS(double fps)
		{
			GuiThread(() =>Window.tbFramesPerSecond.Text = String.Format("{0:0} fps", fps));
		}

		public static void UpdateLatency(double latency)
		{
			GuiThread(()=>Window.tbLatency.Text = String.Format("{0:0} ms", latency));
		}

        public static void WriteLine(string line, int remoteClientID = -2)
        {
            GuiThread(()=>Window.tbLog.Text = TimeStamp + "  [" + (remoteClientID == -2 ? Game.ClientID.ToString() : remoteClientID == -1 ? "S" : remoteClientID.ToString()) + "] " + line + Environment.NewLine + Window.tbLog.Text);
        }

        private static string TimeStamp { get { return DateTime.Now.ToShortTimeString(); } }
        public static void Clear()
        {
            Window.tbLog.Text = String.Empty;
        }

    }
}
