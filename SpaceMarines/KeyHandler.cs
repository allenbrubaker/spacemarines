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
using System.Collections.Generic;
using SpaceMarines.Utility;

namespace SpaceMarines
{
public sealed class KeyHandler
    {

     //static Point MousePosition;
     //public static Point GetMousePosition(UIElement relativeTo)
     //{
     //    GeneralTransform objGeneralTransform = relativeTo.TransformToVisual(Application.Current.RootVisual as UIElement);
     //    Point point = objGeneralTransform.Transform(new Point(0, 0));
     //    return new Point(MousePosition.X - point.X, MousePosition.Y - point.Y);
     //}
        public static readonly Key LeftMouseButton = Key.Left;
        public static readonly Key MiddleMouseButton = Key.Up;
        public static readonly Key RightMouseButton = Key.Right;

		static List<Key> WASD = new List<Key>();
        static bool[] isPressed = new bool[256];
        public static void ClearKeyPresses()
        {
            for (int i = 0; i < 256; i++)
            {
                isPressed[i] = false;
            }
			WASD.Clear();
        }

		public static Vector PlayerMovementVector(double magnitude)
		{
			Key[] keys = ProcessedMovementKeysPressed();
			Point p = new Point();
			if (keys[0] == Key.W || keys[1] == Key.W) // In a canvas moving down actually yields positive in the y direction.  (0,0 is upper left)
				p.Y = -1;
			if (keys[0] == Key.A || keys[1] == Key.A)
				p.X = -1;
			if (keys[0] == Key.S || keys[1] == Key.S)
				p.Y = 1;
			if (keys[0] == Key.D || keys[1] == Key.D)
				p.X = 1;
            if (IsKeyPressed(Key.Shift))
                magnitude /= 2;
			return new Vector(p, magnitude);
		}
        
		static Key[] ProcessedMovementKeysPressed()
		{
            int[] opposite = new int[4];
            for (int i = WASD.Count-1; i >= 0; --i)
            {
                if (opposite[i] > 0) continue;
                for (int j = i-1; j >= 0; --j)
                {
                    if (WASD[j] == OppositeMovementKey(WASD[i]))
                    {
                        opposite[i] = 1;
                        opposite[j] = 2;
                        break;
                    }
                }
            }

            int k=0;
            Key[] keys = new Key[2];

            for (int i = WASD.Count - 1; i >= 0; --i)
            {
                if (WASD.Count >= 3)
                {
                    if (opposite[i] == 0)
                        keys[k++] = WASD[i];
                }
                else
                {
                    if (opposite[i] <= 1)
                        keys[k++] = WASD[i];
                }
            }
        

			for (;k<2; ++k)
				keys[k] = Key.None;

			return keys;
		}

		static Key OppositeMovementKey(Key k)
		{
			switch (k)
			{
				case Key.A: return Key.D;
				case Key.D: return Key.A;
				case Key.W: return Key.S;
				case Key.S: return Key.W;
				default: return k;
			}
		}
		
	
		public static void ClearKey(Key k)
        {
            isPressed[(int)k] = false;
        }

        public static void Attach()
        {
            var target = App.Current.RootVisual;
            ClearKeyPresses();
            target.KeyDown += new KeyEventHandler(target_KeyDown);
            target.KeyUp += new KeyEventHandler(target_KeyUp);
            target.LostFocus += new RoutedEventHandler(target_LostFocus);
            target.MouseLeftButtonDown += new MouseButtonEventHandler(target_MouseLeftButtonDown);
            target.MouseLeftButtonUp += new MouseButtonEventHandler(target_MouseLeftButtonUp);
            target.MouseRightButtonDown += new MouseButtonEventHandler(target_MouseRightButtonDown);
            target.MouseRightButtonUp += new MouseButtonEventHandler(target_MouseRightButtonUp);
           
        }

        static void target_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            isPressed[(int)RightMouseButton] = false;
        }

        static void target_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            isPressed[(int)RightMouseButton] = true;
        }

        static void target_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            isPressed[(int)LeftMouseButton] = false;
        }

        static void target_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            isPressed[(int)LeftMouseButton] = true;
        }

        public static void Detach()
        {
            var target = App.Current.RootVisual;
            target.KeyDown -= new KeyEventHandler(target_KeyDown);
            target.KeyUp -= new KeyEventHandler(target_KeyUp);
            target.LostFocus -= new RoutedEventHandler(target_LostFocus);
            target.MouseLeftButtonDown -= new MouseButtonEventHandler(target_MouseLeftButtonDown);
            target.MouseLeftButtonUp -= new MouseButtonEventHandler(target_MouseLeftButtonUp);
            ClearKeyPresses();
        }

        static void target_KeyDown(object sender, KeyEventArgs e)
        {
            int key = (int)e.Key;
            isPressed[(int)e.Key] = true;
			if (e.Key == Key.A || e.Key == Key.W || e.Key == Key.S || e.Key == Key.D)
			{
                if (!WASD.Contains(e.Key))
                    WASD.Add(e.Key);// If you hold down W for instance, multiple w's trigger keydown without ever sending a keyup.
			}
        }

        static void target_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Unknown) return;
            isPressed[(int)e.Key] = false;
				WASD.Remove(e.Key);
        }

        static void target_LostFocus(object sender, EventArgs e)
        {
            ClearKeyPresses();
        }

        public static bool IsKeyPressed(Key k)
        {
            int v = (int)k;
            if (v < 0 || v > 82) return false;
            return isPressed[v];
        }

        public static bool IsKeyPressed(Key[] keys)
        {
            foreach (Key k in keys)
            {
                if (IsKeyPressed(k))
                    return true;
            }

            return false;
        }

    }
}
