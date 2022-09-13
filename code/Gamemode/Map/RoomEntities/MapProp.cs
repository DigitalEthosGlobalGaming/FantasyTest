using Sandbox;
using System.Collections.Generic;

namespace FantasyTest.MapEntities
{
	public partial class MapProp : MapEntity
	{
		public bool IsGhost { get; set; }
		public PropResource PropData { get; set; }
		public List<Entity> OtherEntities { get; set; }

		public override void Spawn()
		{
			base.Spawn();
			OtherEntities = new List<Entity>();
			Tags.Add( "prop" );
			Tags.Add( "solid" );
		}

		public void SetPropData( PropResource prop )
		{
			PropData = prop;

			SetModel( prop.Model );



			if ( !IsClientOnly )
			{
				SetupPhysicsFromModel( PhysicsMotionType.Dynamic );

				if ( prop.Mass < 0 )
				{
					// Todo, figure out how to ovveride the mass of the object.
				}

				CanPickup = prop.CanPickup;
			}
			else
			{
				RenderColor = Color.Green.WithAlpha( 0.5f );
			}
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
