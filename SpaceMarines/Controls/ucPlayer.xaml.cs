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
using SpaceMarines.Utility;
namespace SpaceMarines
{
	public partial class ucPlayer
	{
        public Storyboard HealthAnimation;
		public ucPlayer()
		{
			InitializeComponent();
            HealthAnimation = CreateHealthAnimation(Constants.PlayerHealthBalls);
		}
		public List<Ellipse> Ellipses = new List<Ellipse>();
		private Storyboard CreateHealthAnimation(int count)
		{
			Storyboard story = new Storyboard();
			double lastAngle = 0;
			double angleDelta = 360.0 / count;
			for (int i=0; i<count; ++i)
			{
				Ellipse e = new Ellipse();
				e.Style = Resources["healthEllipseStyle"] as Style;
				e.RenderTransform = new RotateTransform() { CenterX = 12, CenterY = 12 };
				(e.RenderTransform as RotateTransform).Angle = lastAngle;
				Ellipses.Add(e);
				(Content as Grid).Children.Add(e);
				DoubleAnimation a = new DoubleAnimation();
				a.From = lastAngle;
				a.To = lastAngle - 360;
				a.Duration = new Duration(TimeSpan.FromSeconds(Constants.PlayerHealthBallsRotationSpeed));
				a.RepeatBehavior = RepeatBehavior.Forever;
				Storyboard.SetTargetProperty(a, new PropertyPath("(Ellipse.RenderTransform).(RotateTransform.Angle)"));
				Storyboard.SetTarget(a, e);
				story.Children.Add(a);
				lastAngle += angleDelta;
			}
			story.Begin();
            return story;
		}

        double health = 1.0;
		public void SetHealthPercent(double percent)
		{
            health = percent;
            int visibleBalls = (int)Math.Round(percent * Ellipses.Count);
            for (int i = 0; i < Ellipses.Count; ++i)
                Ellipses[i].Visibility = i < visibleBalls ? Visibility.Visible : Visibility.Collapsed;
		}

        public void ToggleInvisibility(bool isInvisible, bool isOwner)
        {
            foreach (var c in (this.Content as Grid).Children)
                c.Visibility = !isInvisible || isOwner && c != Body ? Visibility.Visible : Visibility.Collapsed;
            Border.Visibility = isInvisible && isOwner ? Visibility.Visible : Visibility.Collapsed;
            if (!isInvisible || isInvisible && isOwner)
                SetHealthPercent(health); //Preserve visibility of balls based on health percent! (Don't sent them all visible if isInvisible = false!)
        }
	}
}
