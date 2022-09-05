using Degg.GridSystem;
using Sandbox;
using System.Collections.Generic;
using System.Linq;

namespace FantasyTest
{
	public partial class Map : GridMap
	{
		public bool IsChildrenSetup { get; set; }
		public List<MapTile> MainPath { get; set; }
		public void BuildRooms()
		{
			IsChildrenSetup = false;
			var roomsCount = 5;
			if ( IsServer )
			{
				var gridSize = Room.RoomsCount * Room.FloorSize;
				gridSize = gridSize + Room.FloorSize;
				Init<MapTile>( Vector3.Zero, new Vector2( gridSize, gridSize ), roomsCount, roomsCount );
			}
		}
		public override void OnSetup()
		{
			base.OnSetup();
			if ( IsClient )
			{
				return;
			}
			var startingPosition = Rand.Int( 0, XSize - 1 );
			var endingPosition = Rand.Int( 0, XSize - 1 );

			var startingSpace = GetSpace( startingPosition, 0 );
			var endingSpace = GetSpace( endingPosition, YSize - 1 );
			if ( (startingSpace?.IsValid() ?? false) && (endingSpace?.IsValid() ?? false) )
			{
				var path = CreatePath<MapTile>( startingSpace, endingSpace );
				foreach ( var space in path )
				{
					space.RoomType = RoomTypes.Main;
				}
				MainPath = path;
			}

			foreach ( var item in GetGridAsList<MapTile>() )
			{
				item.BuildRoom();
			}
		}

		public bool AreChildrenSetup()
		{
			if ( IsChildrenSetup )
			{
				return true;
			}
			var tiles = GetGridAsList<MapTile>();
			foreach ( var tile in tiles )
			{
				if ( !(tile.IsSetup()) )
				{
					return false;
				}
			}
			return true;
		}
		public void OnAllChildrenSetup()
		{
			Log.Info( "OnAllChildrenSetup" );
			var tiles = GetGridAsList<MapTile>();
			foreach ( var tile in tiles )
			{
				tile?.OnAllChildrenSetup();
			}
			MyGame.SpawnPlayers( MainPath.First() );
		}
		public override void ServerTick()
		{
			base.ServerTick();
			if ( IsChildrenSetup == false && IsSetup )
			{
				IsChildrenSetup = AreChildrenSetup();
				if ( IsChildrenSetup )
				{
					OnAllChildrenSetup();
				}
			}
		}


	}
}
