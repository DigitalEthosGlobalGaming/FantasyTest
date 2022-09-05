using Degg.Entities.Common;
using Degg.Util;
using Sandbox;

namespace FantasyTest.MapEntities
{
	public partial class Torch : MapEntity
	{
		public CandlePointLight Light { get; set; }
		public CandlePointLight Light2 { get; set; }

		public Vector3 MountPosition { get; set; }

		public Vector3 LightPosition
		{
			get => Light?.Position ?? Position;
			set
			{
				if ( Light?.IsValid() ?? false )
				{
					Light.Position = value;
				}
				if ( Light2?.IsValid() ?? false )
				{
					Light2.Position = value;
				}
			}
		}
		public override void Spawn()
		{
			base.Spawn();
			SetupPhysicsFromModel( PhysicsMotionType.Static );
			SetModel( "models/fantasy/light_standard.vmdl" );
			EnableShadowCasting = false;
		}


		[Event.Hotload]
		public void Hotload()
		{
			if ( IsClient )
			{
				return;
			}
			TurnOn();
		}

		public void TurnOn()
		{
			AdvLog.Info( "Turn on" );
			if ( !(Light?.IsValid() ?? false) )
			{
				Light = new CandlePointLight();
			}
			if ( !(Light2?.IsValid() ?? false) )
			{
				Light2 = new CandlePointLight();
			}

			Light.SetLightColor( Color.FromBytes( 255, 210, 50 ) );


			var mountPosition = MountPosition.WithZ( Position.z );
			var position = Position - mountPosition;
			var rot = Rotation.From( position.Normal.EulerAngles );
			Rotation = rot;

			var lightModelLightPosition = GetAttachment( "light" )?.Position ?? Vector3.Zero;
			LightPosition = lightModelLightPosition;
			Light.Range = 750f;
			Light.Brightness = 1f;
			Light.FadeDistanceMin = 0f;
			Light.FadeDistanceMax = 3000f;

			Light2.SetLightColor( Color.FromBytes( 255, 210, 150 ) );
			Light2.DisableFlicker = true;
			Light2.Range = 1500f;
			Light2.Brightness = 0.5f;
			Light2.FadeDistanceMin = 0f;
			Light2.FadeDistanceMax = 3000f;

			OtherEntities.Add( Light );
			OtherEntities.Add( Light2 );

		}

		[Event.Tick.Server]
		public void ServerTick()
		{
			DebugOverlay.Sphere( LightPosition, 5f, Color.Yellow );
		}
	}
}
