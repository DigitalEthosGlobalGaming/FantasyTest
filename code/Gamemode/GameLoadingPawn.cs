using Degg;
using Degg.Core;
using Degg.Entities;
using Sandbox;

namespace FantasyTest
{
	public partial class GameLoadingPawn : DeggLoadingPawn
	{

		public override void HudSetup()
		{
			base.HudSetup();
		}

		[ConCmd.Server( "ss.client.loaded" )]
		public static void OnLoad()
		{
			if ( MyGame.GameWaitingRoom == null )
			{
				MyGame.SetupWaitingRoom();
				Log.Info( "SETUP" );
				return;
			}
			if ( MyGame.GameWaitingRoom.IsSetup )
			{
				var player = ClientUtil.GetCallingPawn<GameLoadingPawn>();
				if ( ConsoleSystem.Caller.IsUsingVr )
				{
					player.EntityName = "GamePlayerVR";
				}
				else
				{
					player.EntityName = "GameBasePlayer";
				}
			}
		}

		public void GameStart()
		{

		}

		public override Entity OnJoin()
		{
			var result = base.OnJoin();
			if ( result is DeggPlayer player )
			{
				player.Respawn();
				Log.Info( "HERE" );
			}

			return result;
		}

		public override void Simulate( Client cl )
		{
			base.Simulate( cl );
			if ( IsServer )
			{
				if ( Input.Pressed( InputButton.Jump ) )
				{
					MyGame.StartGame();
					EntityName = "GameBasePlayer";
					GameStart();
				}
			}
		}

	}
}
