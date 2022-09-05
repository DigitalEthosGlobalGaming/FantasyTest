
using Degg.Util;
using Sandbox;
using System;

namespace Degg.Cameras {
	public partial class OrbitCamera : CameraMode
	{
		// should only need TargetRotation but I'm shit
		public Angles TargetAngles;
		Rotation TargetRotation;

		[Net, Local]
		public Vector3 TargetPosition { get; set; }
		public Vector3 TargetDistance { get; set; }
		[Net, Local]
		public float Height { get; set; }

		[Net, Local]
		public float Distance { get; set; }


		[Net, Local]
		public float OrbitSpeed { get; set; }


		public override void Build( ref CameraSetup camSetup )
		{
			base.Build( ref camSetup );
			camSetup.Position = Position;
			camSetup.Rotation = Rotation;
		}

		public override void Update()
		{
			var time = Time.Now;
			float x =(float) Math.Cos( time * OrbitSpeed ) * Distance;
			float y = (float) Math.Sin( time * OrbitSpeed ) * Distance;
			Position = TargetPosition + (Vector3.Left * x) + (Vector3.Forward* y) + (Vector3.Up * Height);
			TargetRotation = Rotation.From( (TargetPosition - Position).EulerAngles );

			Rotation = Rotation.Slerp( Rotation, TargetRotation, RealTime.Delta * 10.0f );
			TargetDistance = TargetDistance.LerpTo( Distance, RealTime.Delta * 5.0f );

			Position += Rotation.Backward * TargetDistance;

			FieldOfView = 80.0f;
		}
	}

}
