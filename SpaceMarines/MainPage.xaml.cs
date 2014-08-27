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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net.Sockets;
using System.Windows.Media.Imaging;
using System.Text;
using System.Reflection;
namespace SpaceMarines
{
    public partial class MainPage : UserControl
    {

        public MainPage()
        {
            InitializeComponent();
            SelectRandomBackgroundImage();
            this.Loaded += new RoutedEventHandler(Page_Loaded);
            this.KeyDown += new KeyEventHandler(MainPage_KeyDown);
        }

        private void SelectRandomBackgroundImage()
        {
            var imagePaths = Assembly.GetCallingAssembly().GetManifestResourceNames().Where(e => e.Contains(".jpg")).ToList();
            string name = imagePaths[new Random().Next(imagePaths.Count())];
            var stream = Assembly.GetCallingAssembly().GetManifestResourceStream(name);
            BitmapImage image = new BitmapImage();
            image.SetSource(stream);
            BackgroundImage.Source = image;
        }

        void MainPage_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (tbChat.Visibility == Visibility.Collapsed)
                {
                    KeyHandler.Detach();
                    tbChat.Text = string.Empty;
                    tbChat.Visibility = System.Windows.Visibility.Visible;
                    tbChat.Focus();
                }
                else
                {
                    tbChat.Visibility = System.Windows.Visibility.Collapsed;
                    string text = tbChat.Text.Trim();
                    if (text != string.Empty)
                    {
                        Log.WriteLine(text);
                        Remote.Send(text);
                    }
                    tbChat.Text = string.Empty;
                    KeyHandler.Attach();
                }
            }
        }


        // After the Frame navigates, ensure the HyperlinkButton representing the current page is selected
        private void ContentFrame_Navigated(object sender, NavigationEventArgs e)
        {
            foreach (UIElement child in LinksStackPanel.Children)
            {
                HyperlinkButton hb = child as HyperlinkButton;
                if (hb != null && hb.NavigateUri != null)
                {
                    if (ContentFrame.UriMapper.MapUri(e.Uri).ToString().Equals(ContentFrame.UriMapper.MapUri(hb.NavigateUri).ToString()))
                    {
                        VisualStateManager.GoToState(hb, "ActiveLink", true);
                    }
                    else
                    {
                        VisualStateManager.GoToState(hb, "InactiveLink", true);
                    }
                }
            }
        }

        // If an error occurs during navigation, show an error window
        private void ContentFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            e.Handled = true;
            ChildWindow errorWin = new ErrorWindow(e.Uri);
            errorWin.Show();
        }

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double x = Math.Max(0, .2 * (this.ActualWidth - 500));
            LayoutRoot.Margin = new Thickness(x, 0, x, 0);

        }

        void Page_Loaded(object sender, RoutedEventArgs e)
        {
        }

    }
}