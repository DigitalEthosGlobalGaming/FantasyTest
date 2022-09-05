using Degg.GridSystem;
using Sandbox;

namespace FantasyTest
{
	public enum RoomTypes
	{
		None,
		Main,
		Trap,
	}
	public partial class MapTile : GridSpace
	{
		public Room TileRoom { get; set; }
		public override float GetMovementWeight( GridSpace a, NavPoint n )
		{
			return this.Difficulty;
		}
		public float Difficulty { get; set; }
		public RoomTypes RoomType { get; set; }
		public bool HasRoom { get => RoomType != RoomTypes.None; }
		public override void OnAddToMap()
		{
			base.OnAddToMap();
			Difficulty = Rand.Float();
		}

		public bool IsSetup()
		{
			if ( !HasRoom )
			{
				return true;
			}
			return TileRoom?.IsSetup ?? false;
		}
		public override void ServerTick( float delta, float currentTick )
		{
			base.ServerTick( delta, currentTick );
			if ( HasRoom )
			{
				DebugOverlay.Sphere( GetWorldPosition(), 50f, Color.Green, 0f, false );
			}
			else
			{
				DebugOverlay.Sphere( GetWorldPosition(), 50f, Color.Red, 0f, false );
			}
		}

		public void OnAllChildrenSetup()
		{
			TileRoom?.OnAllChildrenSetup();
		}

		public void BuildRoom()
		{
			if ( IsClient )
			{
				return;
			}
			if ( HasRoom )
			{
				if ( TileRoom?.IsValid() ?? false )
				{
					return;
				}
				TileRoom = new Room();
				TileRoom.Create( this );
			}
		}

		public override void OnMapSetup()
		{
			base.OnMapSetup();
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			TileRoom?.Delete();
		}
	}
}
