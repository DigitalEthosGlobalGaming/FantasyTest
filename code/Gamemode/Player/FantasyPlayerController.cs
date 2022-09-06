namespace FantasyTest
{
	public partial class FantasyPlayerController : Degg.Controllers.WalkController
	{

		public bool IsSprinting = false;
		public bool IsCrouching = false;
		public FantasyPlayerController()
		{
			WalkSpeed = 140;
			SprintSpeed = 220;
			DefaultSpeed = 160;
			AirAcceleration = 10;
		}


		public override void Simulate()
		{
			PreSimulate();
			base.Simulate();
			PostSimulate();
		}

		public virtual void PreSimulate()
		{

		}
		public virtual void PostSimulate()
		{

		}

		public override float GetWishSpeed()
		{
			var speedMultiplier = 1f;
			if ( IsCrouching )
			{
				speedMultiplier = 0.5f;
			}
			else if ( IsSprinting )
			{
				speedMultiplier = 1.5f;
			}
			var speed = base.GetWishSpeed();
			return speed * speedMultiplier;
		}



	}
}
