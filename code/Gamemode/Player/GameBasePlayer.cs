using Degg.Core;
using Degg.Util;

namespace Sandbox.Gamemode.Player
{
	public partial class GameBasePlayer : DeggPlayer
	{
		public override void BuildInput( InputBuilder input )
		{
			base.BuildInput( input );
		}

		public override void Simulate( Client cl )
		{
			base.Simulate( cl );
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
