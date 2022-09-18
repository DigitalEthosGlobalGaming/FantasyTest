using FantasyTest;
using Sandbox.DeggCommon.Util;
using Sandbox.Gamemode.Entities;

namespace Sandbox.Gamemode.Player.PC
{
	public partial class GamePlayerPC : GameBasePlayer
	{
		[Net]
		public PhysicalContainer Backpack { get; set; }

		public bool IsViewing { get; set; }
		public override void Respawn()
		{
			SetModel( "models/citizen/citizen.vmdl" );

			Log.Info( "Respawn PC Player?" );

			SetModel( "models/citizen/citizen.vmdl" );
			SetCamera<PlayerCamera>();

			Controller = new PCPlayerController();
			EnableDrawing = true;
			EnableHideInFirstPerson = true;
			EnableShadowInFirstPerson = true;

			Backpack?.Delete();
			Backpack = PhysicalContainer.LoadFromModel( ModelStore.GetModel( "player_backpack" ) );

			base.Respawn();
		}

		public PCPlayerController GetController()
		{
			if ( Controller is PCPlayerController con )
			{
				return con;
			}
			return null;
		}

		public override void Simulate( Client cl )
		{
			base.Simulate( cl );

			if ( IsServer )
			{
				if ( Input.Pressed( InputButton.Use ) )
				{
					IsViewing = !IsViewing;
					if ( Backpack != null )
					{
						if ( IsViewing )
						{
							Backpack.Position = Position + Rotation.Forward * 100f;
						}
						else
						{
							Backpack.Position = Position - Rotation.Forward * 50f;
						}
						Backpack.Position = Backpack.Position.WithZ( Position.z );
						Backpack.Rotation = Rotation.From( (Position - Backpack.Position).EulerAngles );

						Backpack.Parent = this;
					}
				}
			}
		}
	}
}
