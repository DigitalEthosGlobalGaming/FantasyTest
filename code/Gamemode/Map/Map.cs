using Degg.GridSystem;
using Sandbox;
using System.Collections.Generic;

namespace FantasyTest
{
	public partial class Map : GridMap
	{
		public bool IsChildrenSetup { get; set; }
		public List<MapTile> MainPath { get; set; }
		public virtual void BuildRooms()
		{
			IsChildrenSetup = false;
			var roomsCount = 5;
			if ( IsServer )
			{
				var gridSize = 3 * Room.FloorSize;
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
			SetupRooms();


		}

		public virtual void SetupRooms()
		{
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
			var tiles = GetGridAsList<MapTile>();
			foreach ( var tile in tiles )
			{
				tile?.OnAllChildrenSetup();
			}
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
