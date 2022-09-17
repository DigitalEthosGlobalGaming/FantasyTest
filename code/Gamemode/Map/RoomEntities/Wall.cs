using Sandbox.DeggCommon.Util;

namespace FantasyTest.MapEntities
{
	public partial class Wall : MapEntity
	{
		public bool IsDoor { get; set; }
		public override void Spawn()
		{
			base.Spawn();
			SetModel( ModelStore.GetModel( "wall" ) );
			Tags.Add( "solid" );
			SetupPhysicsFromModel( Sandbox.PhysicsMotionType.Static );
			EnableShadowCasting = true;
		}
	}
}
