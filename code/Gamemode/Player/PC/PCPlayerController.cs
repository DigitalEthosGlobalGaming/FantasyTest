using Sandbox;

namespace FantasyTest
{
	public partial class PCPlayerController : FantasyPlayerController
	{
		public override Vector3 GetWishVelocity()
		{
			var forward = 0;
			var left = 0;
			if ( Input.Down( InputButton.Forward ) )
			{
				forward += 1;
			}
			if ( Input.Down( InputButton.Back ) )
			{
				forward -= 1;
			}
			if ( Input.Down( InputButton.Left ) )
			{
				left += 1;
			}
			if ( Input.Down( InputButton.Right ) )
			{
				left -= 1;
			}
			return new Vector3( forward, left, 0 );
		}


		public override void PreSimulate()
		{
			IsSprinting = Input.Down( InputButton.Run );
		}
	}
}
