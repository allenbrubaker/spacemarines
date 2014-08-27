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
using System.Xml.Linq;
using SpaceMarines.Utility;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading;

namespace SpaceMarines
{
    [DataContract]
    public abstract class Entity
    {
        #region Constructor/Fields
        [DataMember] public int ID;
		static int _idCounter = 0;
		[DataMember] public int Owner;
		public bool IsOwner { get { return Owner == Game.ClientID; } }

        // Movement
        [DataMember] public Movement Movement;
        public Point Position { get { return Movement.Position; } }
		public abstract bool UseDeadReckoning { get; }

        [DataMember] public Command Command;

        public UserControl Control;
		public double Width { get { return Control.ActualWidth; } }
		public double Height { get { return Control.ActualHeight; } }
		public double Radius { get { return (Width + Height) / 4; } }
        public Point Center { get { return new Point(Width/2 + Canvas.GetLeft(Control), Height/2 + Canvas.GetTop(Control)); }}
        private Storyboard ColorAnimations;
		private EntityColor _Color;
        [DataMember] public EntityColor Color
		{
			get { return _Color; }
			set
			{
				_Color = value;
                if (value is DefaultColor)
                    ResetControlColor();
                if (value is CustomColor)
                    SetControlColor(((CustomColor)value).Color);
            }
		}
        
		public Entity(Ray ray=null, EntityColor color = null)
		{
            Control = CreateControl();
            ColorAnimations = new Storyboard();
			ID = _idCounter++;
			Owner = Game.ClientID;
            Color = color ?? NullColor;
            Movement = new Movement(ray ?? new Ray(), this);
		}

        [OnDeserializing]
        public void OnDeserializing(StreamingContext context)
        {
            AutoResetEvent reset = new AutoResetEvent(false);
            Log.GuiThread(() =>
            {
                Control = CreateControl();
                ColorAnimations = new Storyboard();
                reset.Set();
            });
            reset.WaitOne();
        }

        [OnDeserialized]
        public void OnDeserialized(StreamingContext context) 
        {
            Ray ray = Movement.Ray;
            if (Command != Command.EntityUpdate)
                Movement = new Movement(ray, this);
            else
            {
                Entity oldVersion;
                lock (Game.Entities)
                    oldVersion = Game.Entities.Where(e => e.Equals(this)).FirstOrDefault();
                if (oldVersion == null)
                    Movement = new Movement(ray, this);
                else
                {
                    Movement = oldVersion.Movement;
                    Movement.Entity = this;
                    Movement.InterpolateTo(ray);
                }
            }
            Log.GuiThread(() => SetCanvasPosition(Position));
        }

        #endregion

        #region Movement

        public bool Collide(Entity e)
        {
            return Math.Pow(Radius + e.Radius, 2) >= Math.Pow(Center.X - e.Center.X, 2) + Math.Pow(Center.Y - e.Center.Y, 2);
        }

		public void RandomMove()
		{
			Move(new Point(Constants.Random.NextDouble() * .95 * Constants.BoardWidth, Constants.Random.NextDouble() * .95 * Constants.BoardHeight));
		}

        public void Move() { Movement.Move(); }
        public void Move(Point to) { Movement.Move(to); }

        #endregion

        #region UIElement

        public void SetCanvasPosition(Point p)
        {
            Point adjusted = new Point(p.X - Width / 2, p.Y - Height / 2);
            Canvas.SetLeft(Control, adjusted.X);
            Canvas.SetTop(Control, adjusted.Y);
        }

        /// <summary>
        /// Default associates entity to its control named uc[Entity] 
        /// </summary>
        private UserControl CreateControl()
        {
            string[] str = GetType().ToString().Split('.');
            str[str.Length - 1] = "uc" + str[str.Length - 1];
            return (UserControl)Activator.CreateInstance(Type.GetType(String.Join(".", str)));
        }

        /// <summary>
        /// Default color to give this entity if null is provided as parameter value for Color.
        /// </summary>
        public virtual EntityColor NullColor { get { return new DefaultColor(); } }

        public void UpdateAutoColor(double progress)
        {
            if (Color is AutoColor)
                SetControlColor((Color as AutoColor).Update(progress));
        }

        private void SetControlColor(Color c)
        {
            ResetControlColor();
            Log.GuiThread(() =>
            {
                foreach (Shape shape in (Control.Content as Grid).Children.OfType<Shape>())
                {
                    if (shape.Fill == null) continue;
                    ColorAnimation a = new ColorAnimation() { Duration = TimeSpan.FromSeconds(0), FillBehavior = FillBehavior.HoldEnd, To = c };
                    Storyboard.SetTargetProperty(a, new PropertyPath("(Shape.Fill).(SolidColorBrush.Color)"));
                    Storyboard.SetTarget(a, shape);
                    ColorAnimations.Children.Add(a);
                }
                ColorAnimations.Begin();
            });
        }

        private void ResetControlColor()
        {
            Log.GuiThread(() =>
            {
                ColorAnimations.Stop();
                ColorAnimations.Children.Clear();
            });
        }
        #endregion 

        #region Network
        public void RemoteDestroy() { ToServer(Command.EntityDestroy); }
        public void RemoteUpdate() { ToServer(Command.EntityUpdate); }
        public void RemoteCreate() { ToServer(Command.EntityCreate); }
        protected void ToServer(Command command)
        {
            if (!IsOwner && command != Command.EntityDestroy) // No updates allowed on the virtual or enties not owned by this client (unless of course its a bullet that just hit you)
                return;
            Command = command;
            Remote.Send(new Packet(command, this));
        }
        #endregion

        #region Equals
        public override bool Equals(object obj)
        {
            if (this == obj) return true;
			if (obj == null || !(obj is Entity)) return false;
            Entity y = obj as Entity;
			return Owner == y.Owner && ID == y.ID;
        }
		public override int GetHashCode()
		{
			return Owner ^ ID;
		}
        #endregion


    }        
}
