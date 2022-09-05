using Degg.Util;
using Sandbox;
using Sandbox.Degg.Util;
using System;
using System.Linq;

namespace Degg.Cameras {
	public partial class TopdownCamera : CameraMode
	{

		// should only need TargetRotation but I'm shit
		public Angles TargetAngles;
		Rotation TargetRotation;

		public float Angle { get; set; }

		public float CameraRotationSpeed { get; set; } = 1f;

		public bool IsBuilt = false;

		private float Distance = 150.0f;
		private float TargetDistance = 150.0f;
		public float MinDistance => 20.0f;
		public float MaxDistance => 150.0f;
		public float DistanceStep => 10.0f;

		public float ShakeAmount { get; set; }

		public float NextShake { get; set; }

		public Vector3 TargetPosition { get; set; }

		public bool MatchRotation { get; set; }

		public override void Build( ref CameraSetup camSetup )
		{
			base.Build( ref camSetup );
			if ( !IsBuilt )
			{
				IsBuilt = true;
				LookAt( 0 );
				Rotation = Rotation.FromPitch( 90 );
			}

			camSetup.Position = Position;
			camSetup.Rotation = Rotation;
		}

		public void LookAt(Vector3 position)
		{
			var target = new Vector2( position.x, position.y );
			LookAt( target );
		}
		public void LookAt(Vector2 position)
		{
			var me = new Vector3( Position.x, Position.y, 0 );

			var angle = Vector2Helper.Angle( me, position );
			LookAt( angle );

		}
		public void LookAt(float degrees)
		{
			Angle = degrees;
		}


		public void Shake( float amount )
		{
			ShakeAmount = amount;
			Position = Position.WithX( Position.x + amount );
		}

		public Vector3 GetTargetPosition()
		{
			var p = Vector3.Zero;
			if ( Entity?.IsValid() ?? false )
			{
				p = Entity.Position;
			}

			p = p + Vector3.Up * Distance;
			return p;
		}
		public override void Update()
		{

			TargetPosition = GetTargetPosition();
			Position = Position.LerpTo( TargetPosition, 15f * Time.Delta );

			TargetRotation = Rotation.FromPitch( 90 ).RotateAroundAxis( Vector3.Forward, Angle );

			Rotation = Rotation.Slerp( Rotation, TargetRotation, RealTime.Delta * CameraRotationSpeed );
			TargetDistance = TargetDistance.LerpTo( Distance, RealTime.Delta * 5.0f );

			FieldOfView = 80.0f;
		}

		public override void BuildInput( InputBuilder input )
		{
			Distance = Math.Clamp( Distance + (-input.MouseWheel * DistanceStep), MinDistance, MaxDistance );
		}

	}
}
