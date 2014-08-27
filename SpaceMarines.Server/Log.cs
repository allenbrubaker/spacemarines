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
namespace SpaceMarines.Server
{

    public class Log
    {
		MainWindow Window;
		public Log(MainWindow window)
		{
			Window = window;
		}

        public  void WriteLine(string line)
        {
            Write(line + Environment.NewLine);
        }
	

        public void Write(string text)
        {
			Window.Dispatcher.BeginInvoke(new Action(() => 
			{ 
				Window.tbLog.Text += TimeStamp + ":  " + text; 
				Window.tbLog.ScrollToEnd(); 
			}));
        }

		private  string TimeStamp { get { return DateTime.Now.ToShortTimeString(); } } 
        public  void Clear()
        {
			Window.tbLog.Text = String.Empty;
        }

    }
}
