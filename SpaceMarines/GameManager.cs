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

namespace SpaceMarines
{
    public class GameManager
    {
        Canvas GameCanvas;
        public GameManager(Canvas gameCanvas)
        {
            GameCanvas = gameCanvas;
        }

        public void Run()
        {
            Thread thread = new Thread(new ThreadStart(StayConnected));
            thread.Start();
        }

        public void StayConnected()
        {
            int clientID;
            bool isCoordinator;
            GameListener listener = new GameListener();
            Game game = new Game(GameCanvas);
            AutoResetEvent mutex = new AutoResetEvent(false);
            while (true)
            {
                Log.ToggleConnectStatus(Remote.IsConnected);
                if (!Remote.IsConnected)
                {
                    if (game.IsRunning)
                        Log.GuiThread(()=>game.Stop());
                    if (Remote.Connect(out clientID, out isCoordinator))
                    {
                        listener.Listen();
                        Log.GuiThread(()=>
                            {
                                game.Start(clientID, isCoordinator);// Make sure the game is run on the same thread as ui thread!
                                mutex.Set();
                            });
                        mutex.WaitOne();
                    }
                }
                Thread.Sleep(1000);
            }
        }
    }
}
