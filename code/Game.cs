using Degg.Core;
using Degg.Util;
using FantasyTest;
using System.Collections.Generic;

//
// You don't need to put things in a namespace, but it doesn't hurt.
//
namespace Sandbox;

/// <summary>
/// This is your game class. This is an entity that is created serverside when
/// the game starts, and is replicated to the client. 
/// 
/// You can use this to create things like HUDs and declare which player class
/// to use for spawned players.
/// </summary>
public partial class MyGame : DeggGame
{

	public static List<Client> Clients { get; set; }

	public MyGame()
	{
		Clients = new List<Client>();
		IsStarted = false;
	}

	/// <summary>
	/// A client has joined the server. Make them a pawn to play with
	/// </summary>
	public override void ClientJoined( Client client )
	{
		base.ClientJoined( client );
		Clients.Add( client );

		// Create a pawn for this client to play with
		var pawn = new GameLoadingPawn();
		client.Pawn = pawn;
	}

	public override void ClientDisconnect( Client cl, NetworkDisconnectionReason reason )
	{
		base.ClientDisconnect( cl, reason );
		Clients.Remove( cl );
	}

	public bool IsStarted { get; set; }

	public static void StartGame()
	{
		var game = MyGame.GetCurrent<MyGame>();
		if ( GameMap?.IsValid() ?? false )
		{
			GameMap.Delete();
		}
		GameMap = new Map();

		GameMap.BuildRooms();
	}
	[Event.Hotload]
	public void OnHotload()
	{
		Restart();
	}

	public static void RestartGame()
	{
		var game = MyGame.GetCurrent<MyGame>();
		if ( !GameMap?.IsSetup ?? true )
		{
			return;
		}
		if ( GameMap?.IsValid() ?? false )
		{
			GameMap.Delete();
			GameMap = null;
		}
		StartGame();
	}

	public static void TeleportAllPlayersToWaitingRoom()
	{
		if ( GameWaitingRoom?.IsValid() ?? false )
		{
			var spawnPosition = GameWaitingRoom.Position + (Vector3.Up * Metrics.TerryHeight);
			var players = DeggPlayer.GetAllPlayers<GameBasePlayer>();
			foreach ( var player in players )
			{
				player.Position = spawnPosition;
				player.Respawn();
			}
		}
	}

	public static void SetupWaitingRoom()
	{
		if ( GameWaitingRoom?.IsValid() ?? false )
		{
			GameWaitingRoom.Delete();
		}
		GameWaitingRoom = new WaitingRoom();
		GameWaitingRoom.Create();
	}


}
