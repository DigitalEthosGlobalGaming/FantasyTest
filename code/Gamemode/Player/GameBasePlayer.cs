using Degg.Core;
using Degg.Util;

namespace Sandbox.Gamemode.Player
{
	public partial class GameBasePlayer : DeggPlayer
	{
		public override void Respawn()
		{
			base.Respawn();

			SetModel( "models/citizen/citizen.vmdl" );

			Controller = new PlayerController()
			{
				AirAcceleration = 1500,
				WalkSpeed = 260,
				SprintSpeed = 260,
				DefaultSpeed = 260,
				AutoJump = true,
				Acceleration = 5,
				GroundFriction = 4 //Do this just for safety if player respawns inside friction volume.
			};

			EnableDrawing = true;
			EnableHideInFirstPerson = true;
			EnableShadowInFirstPerson = true;

		}

		private bool UpdateViewAngle;
		private Angles UpdatedViewAngle;
		private float YawSpeed;
		public InputButton ButtonToSet { get; set; } = InputButton.Slot9;

		public override void BuildInput( InputBuilder input )
		{
			base.BuildInput( input );

			if ( UpdateViewAngle )
			{
				UpdateViewAngle = false;
				input.ViewAngles = UpdatedViewAngle;
			}

			if ( YawSpeed != 0 )
			{
				input.ViewAngles = input.ViewAngles.WithYaw( input.ViewAngles.yaw + YawSpeed * Time.Delta );
			}

			if ( ButtonToSet == InputButton.Slot9 ) return;

			input.SetButton( ButtonToSet, true );
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
