using Degg.Core;
using Degg.Util;
using Sandbox;

namespace FantasyTest
{
	public partial class GameBasePlayer : DeggPlayer
	{
		public override void Respawn()
		{
			base.Respawn();

			SetModel( "models/citizen/citizen.vmdl" );

			Controller = new PlayerController();

			EnableDrawing = true;
			EnableHideInFirstPerson = true;
			EnableShadowInFirstPerson = true;
		}

		public override void BuildInput( InputBuilder input )
		{
			base.BuildInput( input );
		}




		public override void Simulate( Client cl )
		{
			base.Simulate( cl );

			foreach ( var child in Children )
			{
				child.Simulate( cl );
			}

			if ( IsServer )
			{
				var w = Metrics.TerryWidth / 2;
				var h = Metrics.TerryHeight / 2;
				DebugOverlay.Box( Position, new Vector3( -w, -w, -h ), new Vector3( w, w, h ) );
				if ( Input.Pressed( InputButton.Slot1 ) )
				{
					MyGame.RestartGame();
				}
			}
		}

	}
}
