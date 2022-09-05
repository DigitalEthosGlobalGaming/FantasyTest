using Degg.GridSystem;
using FantasyTest.MapEntities;
using Sandbox;
using System.Collections.Generic;
using System.Linq;

namespace FantasyTest
{
	public partial class Room : GridMap
	{
		public const float FloorSize = 190f;

		public static int RoomsCount = 10;
		PointLightEntity WorldLight { get; set; }
		public MapTile ParentMapTile { get; set; }

		public List<RoomConnection> RoomConnections { get; set; }

		public RoomConnection TopConnection { get; set; }
		public RoomConnection DownConnection { get; set; }
		public RoomConnection LeftConnection { get; set; }

		public RoomConnection RightConnection { get; set; }

		public override void OnSetup()
		{
			base.OnSetup();
			if ( IsClient )
			{
				return;
			}
			SetupLights();
		}

		public override void ServerTick()
		{
			base.ServerTick();
			DebugOverlay.Text( "      [[[[ " + ParentMapTile.GridPosition.x + "," + ParentMapTile.GridPosition.y + " ]]]]", Position );
		}


		public void OnAllChildrenSetup()
		{
			SetupConnections();
		}

		public void SetupConnections()
		{
			var n = ParentMapTile.GetNeighbours<MapTile>();
			ConnectToRoom( n[0] );
			ConnectToRoom( n[1] );
			ConnectToRoom( n[2] );
			ConnectToRoom( n[3] );
		}

		public void ConnectToRoom( MapTile other )
		{
			if ( other == null )
			{
				return;
			}

			if ( other?.HasRoom ?? false )
			{
				if ( other?.TileRoom?.IsValid() ?? false )
				{
					ConnectToRoom( other?.TileRoom );
				}
			}
		}
		public void ConnectToRoom( Room other )
		{
			if ( !(other?.IsValid() ?? false) )
			{
				return;
			}
			var existing = GetConnection( other );
			if ( existing != null )
			{
				return;
			}
			if ( RoomConnections == null )
			{
				RoomConnections = new List<RoomConnection>();
			}

			if ( other.RoomConnections == null )
			{
				other.RoomConnections = new List<RoomConnection>();
			}

			var connection = RoomConnection.Create( this, other );
			RoomConnections.Add( connection );
			other.RoomConnections.Add( connection );
		}
		public RoomConnection GetConnection( Room other )
		{
			if ( RoomConnections == null )
			{
				return null;
			}
			return RoomConnections.FirstOrDefault( ( item ) => item?.IsConnection( this, other ) ?? false );
		}

		[Event.Hotload]
		public void OnHotload()
		{
			if ( IsServer )
			{
				SetupLights();
			}
		}

		public void Create( MapTile tile )
		{
			if ( IsClient )
			{
				return;
			}
			ParentMapTile = tile;
			Init<RoomTile>( tile.GetWorldPosition(), new Vector2( FloorSize, FloorSize ), Room.RoomsCount, Room.RoomsCount );
		}


		public void SetupLights()
		{
			if ( IsServer )
			{
				return;
				if ( !(WorldLight?.IsValid() ?? false) )
				{
					WorldLight = new PointLightEntity();
				}

				WorldLight.Brightness = 50f;
				WorldLight.FadeDistanceMin = 0f;
				WorldLight.FadeDistanceMax = 0f;
				WorldLight.Position = Position + Vector3.Up * 500f;
				WorldLight.Color = Color.FromBytes( 250, 249, 227 );
				WorldLight.Falloff = 99999f;
			}
		}



		protected override void OnDestroy()
		{
			base.OnDestroy();
			WorldLight?.Delete();
			if ( RoomConnections != null )
			{
				foreach ( var item in RoomConnections )
				{
					item?.Delete();
				}
			}
		}
	}
}
