//
// You don't need to put things in a namespace, but it doesn't hurt.
//

using FantasyTest;

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

	public Map GameMap { get; set; }

	public static WaitingRoom GameWaitingRoom { get; set; }

	[Net]
	public GameStateEnum GameState { get; set; }

	[Net]
	public bool FirstPlayerJoined { get; set; }


	[ConCmd.Server( "game.client.ready" )]
	public static void ReadyUp()
	{
		var game = MyGame.GetCurrent<MyGame>();

		if ( game.FirstPlayerJoined == false )
		{
			game.FirstPlayerJoined = true;
		}

	}


	[Event.Tick.Server]
	public void StartCheck()
	{
		if ( GameState == GameStateEnum.WAITING && FirstPlayerJoined )
		{
			Initialise();
		}
	}


	public void Initialise()
	{
		if ( GameState == GameStateEnum.WAITING )
		{
			GameState = GameStateEnum.INITIALISING;
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
