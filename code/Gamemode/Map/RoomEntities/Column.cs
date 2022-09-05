﻿using Sandbox;

namespace FantasyTest.MapEntities
{
	public partial class Column : MapEntity
	{
		public override void Spawn()
		{
			base.Spawn();
			SetupPhysicsFromModel( PhysicsMotionType.Static );
			SetModel( "models/fantasy/column_square.vmdl" );
		}

		public void CreateLights()
		{
			if ( IsClient )
			{
				return;
			}
			var attachments = new string[] { "top_left", "top_right", "top_forward", "top_back" };

			if ( Rand.Float() > 0.8 )
			{
				var attachment = Rand.FromArray( attachments );

				var lightModelLightPosition = GetAttachment( attachment )?.Position ?? Vector3.Zero;
				if ( lightModelLightPosition != Vector3.Zero )
				{
					var light = new Torch();
					light.Position = lightModelLightPosition;
					light.MountPosition = Position;
					light.TurnOn();
					OtherEntities.Add( light );
					DebugOverlay.Sphere( lightModelLightPosition, 5f, Color.Blue, 5f );
				}
				else
				{
					Log.Info( "No Light" );
				}
			}
		}

	}
}