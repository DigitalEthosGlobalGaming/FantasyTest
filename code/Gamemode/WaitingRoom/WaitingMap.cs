using Degg.Util;
using Sandbox;

namespace FantasyTest
{
	public partial class WaitingMap : Map
	{
		public SpawnPoint MainSpawn { get; set; }

		public PointLightEntity Sun { get; set; }

		public override void BuildRooms()
		{
			IsChildrenSetup = false;
			var roomsCount = 3;
			if ( IsServer )
			{
				var spawnPosition = Vector3.Up;
				var gridSize = roomsCount * Room.FloorSize;
				Init<WaitingMapTile>( spawnPosition, new Vector2( gridSize, gridSize ), roomsCount, roomsCount );
			}
		}

		public override void SetupRooms()
		{
			var centre = GetSpace<WaitingMapTile>( 1, 1 );
			var neighbours = centre.GetNeighbours<WaitingMapTile>();
			foreach ( var neighbour in neighbours )
			{
				neighbour.CreatePlayerRoom();
			}

			centre.CreateWaitingRoom();
			MainSpawn?.Delete();
			MainSpawn = new SpawnPoint();
			MainSpawn.Position = centre.Position + Vector3.Up * Metrics.TerryHeight;

			var tiles = GetGridAsList<WaitingMapTile>();
			foreach ( var tile in tiles )
			{

				if ( tile.IsEmpty )
				{
					tile.ForceSetup = true;
				}
			}
		}

		public bool IsAllSetup()
		{
			return IsSetup && IsChildrenSetup;
		}
	}
}

