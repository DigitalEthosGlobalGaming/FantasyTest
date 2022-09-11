using Degg.Core;
using Sandbox;
using Sandbox.Gamemode.Player;

namespace FantasyTest
{
	public partial class GameBasePlayer : DeggPlayer
	{
		public ClientInventory ClientInventory { get; set; }
		public PlayerWeaponBase MainWeapon { get; set; }

		public float TimeSinceDropped { get; set; }

		public GameBasePlayer()
		{
			Inventory = new PlayerInventory( this );
			ClientInventory = new ClientInventory( this.Client );
		}
		public override void Respawn()
		{
			base.Respawn();
			SetModel( "models/citizen/citizen.vmdl" );
			SetCamera<PlayerCamera>();

			Controller = new PCPlayerController();
			EnableDrawing = true;
			EnableHideInFirstPerson = true;
			EnableShadowInFirstPerson = true;

			if ( MainWeapon?.IsValid() == null )
			{
				MainWeapon = new Builder();
				Inventory.Add( MainWeapon, true );
			}
		}

		public override void BuildInput( InputBuilder input )
		{
			base.BuildInput( input );
			Controller?.BuildInput( input );
		}


		public override void Simulate( Client cl )
		{
			base.Simulate( cl );
			base.SimulateActiveChild( cl, ActiveChild );

			foreach ( var child in Children )
			{
				if ( ActiveChild != child )
				{
					child.Simulate( cl );
				}
			}

			if ( IsServer )
			{
				if ( Input.Pressed( InputButton.Slot1 ) )
				{
					MyGame.Restart();
				}
			}
		}

	}
}
