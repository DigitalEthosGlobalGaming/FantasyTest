
using Degg.Util;
using Sandbox;
using System;

namespace Degg.Cameras {
	public partial class CinematicCamera : CameraMode
	{
		Rotation TargetRotation;

		[Net]
		public Vector3 FocusPosition { get; set; }

		[Net]
		public Vector3 TargetPosition { get; set; }



		public override void Build( ref CameraSetup camSetup )
		{
			base.Build( ref camSetup );
			camSetup.Position = Position;
			camSetup.Rotation = Rotation;
		}

		public override void Update()
		{
			Position = TargetPosition;
			var t = Rotation.From( (FocusPosition - TargetPosition).EulerAngles );
			TargetRotation = t;
			Rotation = Rotation.Slerp( Rotation, TargetRotation, RealTime.Delta * 10.0f );
			FieldOfView = 80.0f;
		}
	}

}
