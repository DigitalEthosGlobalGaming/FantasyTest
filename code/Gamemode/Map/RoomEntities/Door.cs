namespace FantasyTest.MapEntities
{
	public partial class Door : MapEntity
	{
		public override void Spawn()
		{
			base.Spawn();
			SetModel( "models/fantasy/door.vmdl" );
		}
	}
}
