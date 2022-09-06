using Degg.Core;
using Degg.Util;
using FantasyTest;
using System;
using System.Linq;

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
	public Map GameMap { get; set; }
	public MyGame()
	{
		IsStarted = false;
	}

	/// <summary>
	/// A client has joined the server. Make them a pawn to play with
	/// </summary>
	public override void ClientJoined( Client client )
	{
		base.ClientJoined( client );

		// Create a pawn for this client to play with
		var pawn = new GameLoadingPawn();
		client.Pawn = pawn;

		// Get all of the spawnpoints
		var spawnpoints = Entity.All.OfType<SpawnPoint>();

		// chose a random one
		var randomSpawnPoint = spawnpoints.OrderBy( x => Guid.NewGuid() ).FirstOrDefault();

		// if it exists, place the pawn there
		if ( randomSpawnPoint != null )
		{
			var tx = randomSpawnPoint.Transform;
			tx.Position = tx.Position + Vector3.Up * 50.0f; // raise it up
			pawn.Transform = tx;
		}
	}

	public bool IsStarted { get; set; }

	public static void StartGame()
	{
		var game = MyGame.GetCurrent<MyGame>();
		if ( game.GameMap?.IsValid() ?? false )
		{
			game.GameMap.Delete();
		}
		game.GameMap = new Map();

		game.GameMap.BuildRooms();
	}
	[Event.Hotload]
	public void OnHotload()
	{
		if ( IsServer )
		{
			RestartGame();
		}
	}

	public static void RestartGame()
	{
		var game = MyGame.GetCurrent<MyGame>();
		if ( !game?.GameMap?.IsSetup ?? true )
		{
			return;
		}
		if ( game.GameMap?.IsValid() ?? false )
		{
			game.GameMap.Delete();
			game.GameMap = null;
		}
		StartGame();
	}
	public static void SpawnPlayers( MapTile t )
	{
		var spawnPosition = t.Position + (Vector3.Up * Metrics.TerryHeight * 1.5f);
		var players = DeggPlayer.GetAllPlayers<GameBasePlayer>();
		foreach ( var player in players )
		{
			player.Position = spawnPosition;
			player.Respawn();
		}
	}


}
