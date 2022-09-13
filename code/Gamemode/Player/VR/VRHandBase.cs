using Degg.Util;
using Sandbox;

namespace FantasyTest.Gamemode.Player.VR
{
	public partial class VRHandBase : AnimatedEntity
	{
		protected virtual string ModelPath => "";

		[Net] public VRHandBase Other { get; set; }

		public bool GripPressed => InputHand.Grip > 0.5f;
		public bool TriggerPressed => InputHand.Trigger > 0.5f;

		public virtual Input.VrHand InputHand { get; }

		public override void Spawn()
		{
			SetModel( ModelPath );

			EnableAllCollisions = false;
			EnableSelfCollisions = false;

			PhysicsEnabled = false;

			Predictable = true;

			Transmit = TransmitType.Always;
		}

		public override void FrameSimulate( Client cl )
		{
			base.FrameSimulate( cl );
		}

		public override void Simulate( Client cl )
		{
			base.Simulate( cl );

			if (IsServer)
			{
				Transform = InputHand.Transform;
				Animate();
			}
		}

		private void Animate()
		{
			SetAnimParameter( "Index", InputHand.GetFingerCurl( 1 ) );
			SetAnimParameter( "Middle", InputHand.GetFingerCurl( 2 ) );
			SetAnimParameter( "Ring", InputHand.GetFingerCurl( 3 ) );
			SetAnimParameter( "Thumb", InputHand.GetFingerCurl( 0 ) );
		}
	}
}
