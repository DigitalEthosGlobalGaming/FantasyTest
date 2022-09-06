using Sandbox;

namespace FantasyTest
{
	public partial class PlayerController : Degg.Controllers.WalkController
	{

		public bool IsSprinting = false;
		public PlayerController()
		{
			WalkSpeed = 140;
			SprintSpeed = 220;
			DefaultSpeed = 160;
			AirAcceleration = 10;
		}

		public override float GetWishSpeed()
		{
			IsSprinting = false;
			var speedMultiplier = 1f;

			var ws = Duck.GetWishSpeed();
			if ( ws >= 0 ) return ws;

			if ( Input.Down( InputButton.Run ) && Input.Forward > 0 )
			{

			}
			if ( Input.Down( InputButton.Walk ) ) return WalkSpeed * speedMultiplier;

			return DefaultSpeed * speedMultiplier;
		}

		public override void Simulate()
		{
			base.Simulate();
			Log.Info( "PLyaer controller simulate" );
		}
	}
}
