using Sandbox.DeggCommon.Util;

namespace FantasyTest.MapEntities
{
	public partial class Door : MapEntity
	{
		public override void Spawn()
		{
			base.Spawn();
			SetModel( ModelStore.GetModel( "door" ) );
		}
	}
}
