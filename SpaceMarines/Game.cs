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
using System.Threading;
using SpaceMarines.Utility;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Threading;


namespace SpaceMarines
{

	public class Game
	{
		Random Random = new Random();
		public static int ClientID;
		public static Canvas Canvas;
		public static List<Entity> Entities = new List<Entity>();
        public static List<Bullet> Bullets { get { lock (Entities) { return Entities.OfType<Bullet>().ToList(); } } }
        public static List<Player> Players { get { lock (Entities) { return Entities.OfType<Player>().ToList(); } } }
		public static Player Player;
		public static bool IsCoordinator;
		GameListener Listener;
		DateTime LastFrameTime = DateTime.Now;
		DateTime LastPickupTime = DateTime.MinValue;
		TimeSpan CurrentPickupDropRate = TimeSpan.Zero;
		public static Point MousePosition;
		public static Dictionary<Key, Gun> Guns = new Dictionary<Key, Gun>();
        public bool IsRunning = false;
		public Game(Canvas canvas)
		{
			Canvas = canvas;
		}

		public void Start(int clientID, bool isCoordinator)
		{
            ClientID = clientID;
            IsCoordinator = isCoordinator;
			(App.Current.RootVisual as MainPage).Name = "Client " + ClientID;
			Player = new Player(); // Don't create any entities until ClientID is set (or else they'll default to an Owner of 0 and not ClientID).
			//Player.RemoteCreate();
			EquipGuns();
			Player.RandomMove();
			Add(Player);
			(Canvas.Parent as Border).MouseMove += new MouseEventHandler(Canvas_MouseMove);
			CompositionTarget.Rendering += new EventHandler(CompositionTarget_Rendering);
            IsRunning = true;
        }

        public void Stop()
        {
            CompositionTarget.Rendering -= new EventHandler(CompositionTarget_Rendering);
            (Canvas.Parent as Border).MouseMove -= new MouseEventHandler(Canvas_MouseMove);
            foreach (Entity e in Entities.ToList())
            {
                Remove(e);
            }
            Guns.Clear();
            IsRunning = false;
        }

		/// <summary>
		/// Per frame calculations
		/// </summary>
		void CompositionTarget_Rendering(object sender, EventArgs e)
		{
			double Delta = (DateTime.Now - LastFrameTime).TotalSeconds;
			Log.UpdateFPS(1 / Delta); 
			LastFrameTime = DateTime.Now;

			Player.Movement.UpdateVector(PlayerMovementVector());
			foreach (Key key in Guns.Keys)
			{
				if (KeyHandler.IsKeyPressed(key) && Guns[key].CanFire)
					Game.Add(Guns[key].Fire().OfType<Entity>().ToList());
			}
            lock (Entities) 
            {
                foreach (Entity entity in Entities)
                    entity.Move();
            }
			SpawnPickups();
			TrimCanvas();
			HandleCollisions();

		}

		private void SpawnPickups()
		{
			lock (Entities)
			{
				if (!IsCoordinator) return; // IsCoordinator needs to be locked for thread-safe access.
				if (DateTime.Now.Subtract(LastPickupTime) > CurrentPickupDropRate)
				{
					LastPickupTime = DateTime.Now;
					Pickup p = Pickup.RandomPickup;
					CurrentPickupDropRate = p.DropRate;
					p.RandomMove();
					Add(p);
                    p.RemoteCreate();
				}
			}
		}

		private Vector PlayerMovementVector()
		{
			Vector vector = KeyHandler.PlayerMovementVector(Constants.PlayerVelocity * Constants.BoardDiagonal * (Player.IsHyper ? Constants.StaminaVelocityMultiplier : 1));

			if (Player.Position.X - Player.Width / 2 < 0)
				vector.X = Math.Max(0, vector.X);
			if (Player.Position.X + Player.Width / 2 > Constants.BoardWidth)
				vector.X = Math.Min(0, vector.X);
			if (Player.Position.Y - Player.Height / 2 < 0)
				vector.Y = Math.Max(0, vector.Y);
			if (Player.Position.Y + Player.Height / 2 > Constants.BoardHeight)
				vector.Y = Math.Min(0, vector.Y);
			return vector;
		}

		private void TrimCanvas()
		{
			List<Entity> remove = new List<Entity>();
            lock (Entities)
            {
                foreach (Entity e in Entities)
                    if (e.Position.X < -Constants.BoardMarginWidth || e.Position.X > Constants.BoardWidth + Constants.BoardMarginWidth ||
                        e.Position.Y < -Constants.BoardMarginHeight || e.Position.Y > Constants.BoardHeight + Constants.BoardMarginHeight)
                        remove.Add(e);
            }
			foreach (var e in remove)
			{
				Remove(e);
			}
		}

		void EquipGuns()
		{
			Guns.Add(KeyHandler.LeftMouseButton, new MachineGun());
			Guns.Add(KeyHandler.RightMouseButton, new Sniper());
			Guns.Add(Key.E, new Bomber());
		}

		void Canvas_MouseMove(object sender, MouseEventArgs e)
		{
			MousePosition = e.GetPosition(Canvas);
		}

		public void HandleCollisions()
		{
			double health = Player.Health;
            foreach (Player p in Players)
            {
                foreach (Entity e in Collisions(p))
                {
                    Remove(e);
					if (p == Player)
					{
						if (e is Bullet) Player.Hit(e as Bullet);
						if (e is Pickup) (e as Pickup).Apply(Player);
					}
                }
            }

			if (Player.IsDead)
                Player.Resurrect();
			else if (health != Player.Health)
				Player.RemoteUpdate();
		}

		/// <summary>
		/// Returns the set of entities currently colliding with the given entity.
		/// </summary>
		public List<Entity> Collisions(Entity e)
		{
            lock (Entities)
                return Entities.Where(x => !(x is Player) && !x.Equals(e) && (!(x is Bullet) || x.Owner != e.Owner) && e.Collide(x)).ToList();
		}

		public static void Add(Entity e)
		{
			lock (Entities)
				Entities.Add(e);
            Log.GuiThread(() => Canvas.Children.Add(e.Control));
		}

        public static void Add(List<Entity> list)
        {
            lock (Entities)
                Entities.AddRange(list);
            Log.GuiThread(() => list.ForEach(e=>Canvas.Children.Add(e.Control)));
        }

		/// <summary>
		/// Threadsafe
		/// </summary>
		public static Entity Remove(Entity entity)
		{
            lock (Entities)
            {
                Entity e = Game.Entities.FirstOrDefault(x => x.Equals(entity));
                if (e != null)
                {
                    Entities.Remove(e);
                    Log.GuiThread(() => Canvas.Children.Remove(e.Control));
                }
                return e;
            }
		}
	}
}
