using Degg;
using Degg.Entities;
using Sandbox;

namespace FantasyTest
{
	public partial class GameLoadingPawn: DeggLoadingPawn
	{

		public override void HudSetup()
		{
			base.HudSetup();
		}

		[ConCmd.Server( "ss.client.loaded" )]
		public static void OnLoad()
		{
			var player = ClientUtil.GetCallingPawn<GameLoadingPawn>();
			player.EntityName = "GameBasePlayer";
			player.OnJoin();
		}

		public override Entity OnJoin()
		{
			var result = base.OnJoin();
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
					OnJoin();
				}
			}
		}

	}
}
