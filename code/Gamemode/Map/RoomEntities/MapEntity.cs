using Sandbox;
using System.Collections.Generic;

namespace FantasyTest.MapEntities
{
	public partial class MapEntity : ModelEntity
	{
		public List<Entity> OtherEntities { get; set; }
		public override void Spawn()
		{
			base.Spawn();
			OtherEntities = new List<Entity>();
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			if ( OtherEntities != null )
			{
				foreach ( var item in OtherEntities )
				{
					item?.Delete();
				}
			}
		}
	}
}
