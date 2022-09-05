using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;

namespace Degg.UI.Elements
{
	public class ModelPreview : Panel
	{
		readonly ScenePanel scenePanel;

		Angles CamAngles = new( 25.0f, 0.0f, 0.0f );
		float CamDistance = 120;
		Vector3 CamPos => Vector3.Up * 10 + CamAngles.Direction * -CamDistance;

		SceneParticles particleObj;

		public ModelPreview()
		{
			Style.FlexWrap = Wrap.Wrap;
			Style.JustifyContent = Justify.Center;
			Style.AlignItems = Align.Center;
			Style.AlignContent = Align.Center;
			Style.Padding = 0;
			Style.SetBackgroundImage( "/ui/tests/background.png" );

			var world = new SceneWorld();
			scenePanel = Add.ScenePanel( world, CamPos, Rotation.From( CamAngles ), 70 );
			scenePanel.Style.Width = Length.Percent( 100 );
			scenePanel.Style.Height = Length.Percent( 100 );

			new SceneModel( world, "models/citizen_props/roadcone01.vmdl", Transform.Zero );
			new SceneModel( world, "models/room.vmdl", Transform.Zero );

			new SceneLight( world, Vector3.Up * 150.0f, 200.0f, Color.Red * 5.0f );
			new SceneLight( world, Vector3.Up * 10.0f + Vector3.Forward * 100.0f, 200, Color.White * 15.0f );
			new SceneLight( world, Vector3.Up * 10.0f + Vector3.Backward * 100.0f, 200, Color.Magenta * 15f );
			new SceneLight( world, Vector3.Up * 10.0f + Vector3.Right * 100.0f, 200, Color.Blue * 15.0f );
			new SceneLight( world, Vector3.Up * 10.0f + Vector3.Left * 100.0f, 200, Color.Green * 15.0f );

			particleObj = new SceneParticles( world, "particles/example/int_from_model_example/int_from_model_example.vpcf" );

		}

		public override void OnMouseWheel( float value )
		{
			CamDistance += value * 10.0f;
			CamDistance = CamDistance.Clamp( 10, 700 );

			base.OnMouseWheel( value );
		}

		public override void OnButtonEvent( ButtonEvent e )
		{
			if ( e.Button == "mouseleft" )
			{
				SetMouseCapture( e.Pressed );
			}

			base.OnButtonEvent( e );
		}

		public override void Tick()
		{
			base.Tick();

			if ( HasMouseCapture )
			{
				CamAngles.pitch += Mouse.Delta.y;
				CamAngles.yaw -= Mouse.Delta.x;
				CamAngles.pitch = CamAngles.pitch.Clamp( 0, 90 );
			}

			scenePanel.CameraPosition = CamPos;
			scenePanel.CameraRotation = Rotation.From( CamAngles );

			// it feels dumb that particle scene objects need pumping manually
			// so we should look at allowing them to 
			particleObj?.Simulate( RealTime.Delta );
			particleObj?.SetControlPoint( 0, Vector3.Up * 30.0f );

			float f = 1;
			foreach ( var obj in scenePanel.World.SceneObjects )
			{
				if ( obj is not SceneLight ) continue;

				obj.Position += new Vector3( MathF.Sin( f + Now * 2.0f ), MathF.Cos( f + Now * 2.0f ), 0 ) * RealTime.Delta * 100.0f;
				f += 1.0f;
			}
		}
	}
}
