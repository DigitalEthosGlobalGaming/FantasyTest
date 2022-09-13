using Degg.Util;
using Sandbox;
namespace FantasyTest.Gamemode.Player.VR
{
	public partial class GamePlayerVR : GameBasePlayer
	{
		[Net, Predicted] public VRHandLeft LeftHand { get; set; }
		[Net, Predicted] public VRHandRight RightHand { get; set; }

		private void CreateHands()
		{
			DeleteHands();

			AdvLog.Info( "Create Hands" );

			LeftHand = new() { };
			RightHand = new() { };

			LeftHand.Other = RightHand;
			LeftHand.Parent = this;
			RightHand.Other = LeftHand;
			RightHand.Parent = this;
		}

		private void DeleteHands()
		{
			AdvLog.Info( "Delete Hands" );
			LeftHand?.Delete();
			RightHand?.Delete();
		}

		public override void Respawn()
		{
			SetModel( "models/citizen/citizen.vmdl" );

			Log.Info( "Respawn VR Player?" );

			Controller = new PlayerVRController();
			Animator = new PlayerVRAnimator();
			CameraMode = new FirstPersonCamera();

			Log.Info( Controller.GetType() );

			EnableAllCollisions = true;
			EnableDrawing = true;
			EnableHideInFirstPerson = true;
			EnableShadowInFirstPerson = true;

			CreateHands();

			// Hide hands from the First-person view.
			SetBodyGroup( "Hands", 1 );

			base.Respawn();
		}

		public override void Simulate( Client cl )
		{
			base.Simulate( cl );
			SimulateActiveChild( cl, ActiveChild );
			
			if (IsServer)
			{
				CheckRotate();
				SetVrAnimProperties();
			}
		}

		public override void FrameSimulate( Client cl )
		{
			base.FrameSimulate( cl );

		}
		public void SetVrAnimProperties()
		{
			if ( LifeState != LifeState.Alive )
				return;

			if ( !Input.VR.IsActive )
				return;

			if ( !IsServer )
				return;

			SetAnimParameter( "b_vr", true );
			var leftHandLocal = Transform.ToLocal( LeftHand.GetBoneTransform( 0 ) );
			var rightHandLocal = Transform.ToLocal( RightHand.GetBoneTransform( 0 ) );

			var handOffset = Vector3.Zero;
			SetAnimParameter( "left_hand_ik.position", leftHandLocal.Position + (handOffset * leftHandLocal.Rotation) );
			SetAnimParameter( "right_hand_ik.position", rightHandLocal.Position + (handOffset * rightHandLocal.Rotation) );

			SetAnimParameter( "left_hand_ik.rotation", leftHandLocal.Rotation * Rotation.From( 0, 0, 180 ) );
			SetAnimParameter( "right_hand_ik.rotation", rightHandLocal.Rotation );

			float height = Input.VR.Head.Position.z - Position.z;
			SetAnimParameter( "duck", 1.0f - ((height - 32f) / 32f) ); // This will probably need tweaking depending on height
		}

		private TimeSince timeSinceLastRotation;
		private void CheckRotate()
		{
			if ( !IsServer )
				return;

			const float deadzone = 0.2f;
			const float angle = 45f;
			const float delay = 0.25f;

			float rotate = Input.VR.RightHand.Joystick.Value.x;

			if ( timeSinceLastRotation > delay )
			{
				if ( rotate > deadzone )
				{
					Transform = Transform.RotateAround(
						Input.VR.Head.Position.WithZ( Position.z ),
						Rotation.FromAxis( Vector3.Up, -angle )
					);

					timeSinceLastRotation = 0;
				}
				else if ( rotate < -deadzone )
				{
					Transform = Transform.RotateAround(
						Input.VR.Head.Position.WithZ( Position.z ),
						Rotation.FromAxis( Vector3.Up, angle )
					);

					timeSinceLastRotation = 0;
				}
			}

			if ( rotate > -deadzone && rotate < deadzone )
			{
				timeSinceLastRotation = 10;
			}
		}

		public override void OnKilled()
		{
			base.OnKilled();
			EnableDrawing = false;
			DeleteHands();
		}

		public override void PostCameraSetup( ref CameraSetup setup )
		{
			// You will probably need to tweak these depending on your use case
			setup.ZNear = 1;
			setup.ZFar = 25000;

			base.PostCameraSetup( ref setup );
		}
	}
}
