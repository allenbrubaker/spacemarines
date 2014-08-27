using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Navigation;
using SpaceMarines.Utility;
namespace SpaceMarines
{
    public partial class Home : Page
    {
        public Home()
        {
            InitializeComponent();
            GameCanvas.Width = Constants.BoardWidth;
            GameCanvas.Height = Constants.BoardHeight;
        }

        // Executes when the user navigates to this page.
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

		private void Page_Loaded(object sender, RoutedEventArgs e)
		{
            GameManager manager = new GameManager(GameCanvas);	
			manager.Run();	
		}

        private void GameCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            RectangleGeometry rect = new RectangleGeometry();
            rect.Rect = new Rect(0, 0, GameCanvas.ActualWidth, GameCanvas.ActualHeight);
            GameCanvas.Clip = rect;
        }

    }
}
