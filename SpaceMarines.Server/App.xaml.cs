using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using SpaceMarines.Utility;

namespace SpaceMarines.Server
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{

		public App()
		{
			RunServers();
		}

		static void RunServers()
		{
			Server policyServer = new Server(new PolicyHandler(), Constants.PolicyPort);
			policyServer.Start(false);

			Server gameServer = new Server(new GameHandler(), Constants.GamePort);
			gameServer.Start();
		}
	}



}
