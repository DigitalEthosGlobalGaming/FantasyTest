using Sandbox;
using System.Collections.Generic;

namespace FantasyTest.MapEntities
{
	public partial class MapProp : MapEntity
	{
		public bool IsGhost { get; set; }
		public PropResource PropData { get; set; }
		public List<Entity> OtherEntities { get; set; }

		public Room Room { get; set; }

		public override void Spawn()
		{
			base.Spawn();
			OtherEntities = new List<Entity>();
			Tags.Add( "prop" );
			Tags.Add( "solid" );
		}

		public void AddToRoom( Room room )
		{
			Room = room;
			Room.Props.Add( this );

		}

		public void SetPropData( PropResource prop )
		{
			PropData = prop;

			SetModel( prop.Model );

			Scale = prop.Scale;

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

		public static MapProp Create( PropResource res )
		{
			var className = res.ClassName;
			if ( className.Trim() == "" )
			{
				className = "GenericProp";
			}
			var newEntity = Entity.CreateByName<MapProp>( className );
			newEntity.SetPropData( res );

			return newEntity;
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
