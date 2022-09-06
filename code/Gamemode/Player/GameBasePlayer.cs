using Degg.Core;
using Sandbox;
using Sandbox.Gamemode.Player;

namespace FantasyTest
{
	public partial class GameBasePlayer : DeggPlayer
	{
		public PlayerWeaponBase MainWeapon { get; set; }
		public override void Respawn()
		{
			base.Respawn();

			SetModel( "models/citizen/citizen.vmdl" );
			SetCamera<PlayerCamera>();

			Controller = new PCPlayerController();
			EnableDrawing = true;
			EnableHideInFirstPerson = true;
			EnableShadowInFirstPerson = true;
			MainWeapon = new Dagger();
			MainWeapon.ActiveStart( this );

		}

		public override void BuildInput( InputBuilder input )
		{
			base.BuildInput( input );
			Controller?.BuildInput( input );
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
				if ( Input.Pressed( InputButton.Slot1 ) )
				{
					MyGame.RestartGame();
				}
			}
		}

	}
}
