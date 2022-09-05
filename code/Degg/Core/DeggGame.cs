
using Sandbox;
using Sandbox.UI.Construct;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

//
// You don't need to put things in a namespace, but it doesn't hurt.
//
namespace Degg.Core
{
	/// <summary>
	/// This is your game class. This is an entity that is created serverside when
	/// the game starts, and is replicated to the client. 
	/// 
	/// You can use this to create things like HUDs and declare which player class
	/// to use for spawned players.
	/// </summary>
	public partial class DeggGame : Sandbox.Game
	{
		public static WorldEntity MainWorld { get; set; }
		public DeggGame()
		{
		}

		public static T GetCurrent<T>() where T: DeggGame
		{
			var game = Current;
			if ( game is T t)
			{
				return t;
			}
			return null;
		}

		public static DeggGame GetCurrent()
		{
			return GetCurrent<DeggGame>();
		}


		[Net]
		public bool DevelopmentMode { get; set; }

		public static bool IsDevelopment()
		{
			return GetCurrent().DevelopmentMode;
		}


		[ConCmd.Server( "degg.set_developer_mode" )]
		public static void SetDeveloperMode( string status )
		{
			
			var isAdmin = DeggConsoleSystem.IsCallingAdmin();
			if ( !isAdmin)
			{
				return;
			}
			var isDevelopmentMode = false;
			status = status.ToLower().Trim();
			if (status == "true" || status == "t" || status == "1")
			{
				isDevelopmentMode = true;
			}
			Log.Info( "Development mode set to " + isDevelopmentMode );
			GetCurrent().DevelopmentMode = isDevelopmentMode;
		}

	}

}
