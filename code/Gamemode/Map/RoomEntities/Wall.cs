﻿namespace FantasyTest.MapEntities
{
	public partial class Wall : MapEntity
	{
		public bool IsDoor { get; set; }
		public override void Spawn()
		{
			base.Spawn();
			SetModel( "models/fantasy/walls.vmdl" );
			SetupPhysicsFromModel( Sandbox.PhysicsMotionType.Static );
			EnableShadowCasting = true;
		}
	}
}
