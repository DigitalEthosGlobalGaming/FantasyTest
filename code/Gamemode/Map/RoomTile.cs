using Degg.GridSystem;
using FantasyTest.MapEntities;
using Sandbox;
using Sandbox.DeggCommon.Util;
using System.Collections.Generic;

namespace FantasyTest
{
	public class RoomTile : GridSpace
	{
		public ModelEntity Floor { get; set; }
		public List<Entity> Entities { get; set; }

		public RoomConnection Connection { get; set; }

		public override void OnAddToMap()
		{
			base.OnAddToMap();
			if ( IsClient )
			{
				return;
			}
			Entities = new List<Entity>();
			if ( Floor?.IsValid() ?? false )
			{
				return;
			}

			Floor = new ModelEntity();
			Floor.SetupPhysicsFromModel( PhysicsMotionType.Static );
			Floor.SetModel( ModelStore.GetModel( "floor" ) );
			Floor.Position = Position;
		}

		public override void OnMapSetup()
		{
			SetupWalls();
		}


		public void SetupWalls()
		{
			var n = GetNeighbours<RoomTile>();
			if ( n[0] == null ) // Up
			{
				var wall = new Wall();
				wall.Tags.Add( "edge-wall" );
				wall.Rotation = Rotation.FromAxis( Vector3.Up, 270f );
				wall.Position = GetWorldPosition();
				Entities.Add( wall );
			}
			if ( n[1] == null ) // Right
			{
				var wall = new Wall();
				wall.Tags.Add( "edge-wall" );
				wall.Rotation = Rotation.FromAxis( Vector3.Up, 0f );
				wall.Position = GetWorldPosition();
				Entities.Add( wall );
			}
			if ( n[2] == null ) // Down
			{
				var wall = new Wall();
				wall.Tags.Add( "edge-wall" );
				wall.Rotation = Rotation.FromAxis( Vector3.Up, 90f );
				wall.Position = GetWorldPosition();
				Entities.Add( wall );
			}

			if ( n[3] == null ) // Left
			{
				var wall = new Wall();
				wall.Tags.Add( "edge-wall" );
				wall.Rotation = Rotation.FromAxis( Vector3.Up, 180f );
				wall.Position = GetWorldPosition();
				Entities.Add( wall );
			}
		}

		public void DeleteEdgeWalls()
		{
			foreach ( var e in Entities )
			{
				if ( e.Tags.Has( "edge-wall" ) )
				{
					e.Delete();
				}
			}
		}

		public void SetConnection( RoomConnection c )
		{
			Connection = c;
			this.DeleteEdgeWalls();
			RoomTile other = c.From;
			if ( other == this )
			{
				other = c.To;
			}

			var door = new Door();
			var angles = (-(Position - other.Position)).Normal.EulerAngles;
			var rot = Rotation.From( angles );
			door.Rotation = rot;
			door.Position = GetWorldPosition();
			Entities.Add( door );

			if ( c.From == this )
			{
				Entities.Add( c );
			}
		}
		public override void ServerTick( float delta, float currentTick )
		{
			base.ServerTick( delta, currentTick );
		}
		protected override void OnDestroy()
		{
			base.OnDestroy();
			if ( IsServer )
			{
				if ( Floor?.IsValid() ?? false )
				{
					Floor.Delete();
				}
				if ( Entities != null )
				{
					foreach ( var item in Entities )
					{
						if ( item?.IsValid() ?? false )
						{
							item.Delete();
						}
					}
				}
			}
		}
	}
}
