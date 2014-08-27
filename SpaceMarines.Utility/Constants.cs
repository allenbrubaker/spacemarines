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

namespace SpaceMarines.Utility
{
	public class Constants
	{
		public static readonly Random Random = new Random();

        // Networking constants
		public static readonly int GamePort = 4530;  // 4502-4534 is the range
        public static readonly int PolicyPort = 943;
        public static readonly string ServerAddress = "localhost";

        // Game board constants
        public static readonly double BoardAspectRatio = 1.6;
        public static readonly double BoardWidth = 800;
		public static readonly double BoardHeight = BoardWidth / BoardAspectRatio ;
        public static double BoardDiagonal = Math.Sqrt(Math.Pow(BoardHeight, 2) + Math.Pow(BoardWidth, 2));
        public static readonly double BoardBound = 1.3; // Remove entities outside of BoardBound * (boardwidth, boardheight)
        public static readonly double BoardMarginWidth = (BoardBound - 1) / 2 * BoardWidth;
        public static readonly double BoardMarginHeight = (BoardBound - 1) / 2 * BoardHeight; 

        // Game entity constants
        public static readonly double SplineTime = .1; // Time in seconds needed to traverse an interpolating spline between two positions. 
        public static readonly double SplineIntermediateControlInfluence = 0; // (Between [0,1]. Controls how much the second and third control points influence the movement. 
        public static readonly double PlayerVelocity = .20; // move x% of the diagonal of the board per second.
		public static readonly int PlayerHealthBalls = 10; // Number of rotating health balls around player.
		public static readonly int PlayerHealthBallsRotationSpeed = 10; // x seconds for 1 full rotation.
        public static readonly double PlayerArmorIncreaseRatePerLevel = .02; 
		public static readonly double MachineGunVelocity = .20;
        public static readonly TimeSpan MachineGunFireRate = TimeSpan.FromMilliseconds(250);
        public static readonly double MachineGunDamage = .10;
        public static readonly double SniperVelocity = .6;
        public static readonly TimeSpan SniperFireRate = TimeSpan.FromMilliseconds(3000);
        public static readonly double SniperDamage = .20;
		public static readonly double BomberVelocity = .6;
		public static readonly TimeSpan BomberFireRate = TimeSpan.FromMilliseconds(1000);
		public static readonly double BomberDamage = 1.0;
		public static readonly TimeSpan InvincibleDuration = TimeSpan.FromSeconds(10);
		public static readonly TimeSpan StaminaDuration = TimeSpan.FromSeconds(10);
		public static readonly double StaminaVelocityMultiplier = 1.3;
		public static readonly TimeSpan InvisibleDuration = TimeSpan.FromSeconds(10);
		public static readonly TimeSpan PickupDropRate = TimeSpan.FromSeconds(30);
		public static readonly double PickupDropRateRandomizer = .20; // +-x% from specified PickupDropRate.  
		
		
        
        // Entity color constants
        public static readonly Tuple<int,int> DamageColorRange = Tuple.Create(0, 329); // Start at red and end at purple - max is 360!
        public static readonly double Brightness = 1;
        public static readonly double Saturation = .80;

        // Deadreckoning
        public static readonly double DeadReckoningThreshhold = 1; //PlayerVelocity * BoardDiagonal/20.0;

	}


	public enum Command
	{
        EntityCreateMany,
		EntityCreate,
        EntityUpdate,
        EntityDestroy,
        PlayerResurrect,
        ClientDisconnect,
        ClientConnect,
		ClientElected,
        LogMessage,
        ServerDisconnect
	}
}
