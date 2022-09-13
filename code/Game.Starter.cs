//
// You don't need to put things in a namespace, but it doesn't hurt.
//

using FantasyTest;
using Sandbox.DeggCommon.Util;
using System.Linq;

namespace Sandbox;

/// <summary>
/// This is your game class. This is an entity that is created serverside when
/// the game starts, and is replicated to the client. 
/// 
/// You can use this to create things like HUDs and declare which player class
/// to use for spawned players.
/// </summary>
/// 

public enum GameStateEnum
{
	WAITING,
	INITIALISING,
	ALMOST_READY,
	READY
}

public partial class MyGame
{

	public static Map GameMap { get; set; }

	public static WaitingMap GameWaitingMap { get; set; }

	[Net]
	public static GameStateEnum GameState { get; set; }

	[Net]
	public static bool FirstPlayerJoined { get; set; }

	public static void Restart()
	{
		Cleanup();
		Start();
		ReadyUp();
	}

	public static void Start()
	{

	}

	public static void Cleanup()
	{
		ModelStore.ModelTasks?.Clear();
		var game = MyGame.GetCurrent<MyGame>();
		var isServer = game.IsServer;
		var allEntities = All.ToList();
		foreach ( var i in allEntities )
		{
			if ( !(i is Client) && !(i is MyGame) )
			{
				if ( i?.IsValid() ?? false )
				{
					if ( isServer || i.IsClientOnly )
					{
						i.Delete();
					}
				}
			};
		}

		if ( game.IsServer )
		{
			GameWaitingMap?.Delete();
			GameMap?.Delete();
			FirstPlayerJoined = false;
			GameState = GameStateEnum.WAITING;
			foreach ( var client in Clients )
			{
				var p = new GameLoadingPawn();
				client.Pawn = p;
			}
		}

	}


	[ConCmd.Server( "game.client.ready" )]
	public static void ReadyUp()
	{
		var game = MyGame.GetCurrent<MyGame>();
		if ( game.IsServer )
		{
			if ( FirstPlayerJoined == false )
			{
				FirstPlayerJoined = true;
			}
		}

	}


	[Event.Tick.Server]
	public void StartCheck()
	{
		if ( GameState == GameStateEnum.WAITING && FirstPlayerJoined )
		{
			Initialise();
		}

		if ( GameState == GameStateEnum.INITIALISING )
		{
			if ( GameWaitingMap.IsSetup && GameWaitingMap.IsChildrenSetup )
			{
				Ready();
			}
		}
	}


	public void Initialise()
	{
		if ( GameState == GameStateEnum.WAITING )
		{
			GameState = GameStateEnum.INITIALISING;
			SetupWaitingRoom();
		}
	}


	public void Ready()
	{
		if ( GameState == GameStateEnum.INITIALISING )
		{
			GameState = GameStateEnum.ALMOST_READY;

			foreach ( var player in GameLoadingPawn.GetAllPlayers<GameLoadingPawn>() )
			{
				player.OnJoin();
			}

			GameState = GameStateEnum.READY;
		}
	}


}
