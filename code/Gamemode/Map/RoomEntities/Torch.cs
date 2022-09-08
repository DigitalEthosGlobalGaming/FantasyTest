using Sandbox;

namespace FantasyTest.MapEntities
{
	public partial class Torch : MapEntity
	{
		public ModelDocLightEntity Lights { get; set; }

		public Vector3 MountPosition { get; set; }

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
			if ( Lights?.IsValid() ?? false )
			{
				Lights.Delete();
			}

			Lights = new ModelDocLightEntity();
			Lights.Setup( this );
		}
	}
}
