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
		public bool ForceSetup { get; set; }
		public Room TileRoom { get; set; }
		public override float GetMovementWeight( GridSpace a, NavPoint n )
		{
			return this.Difficulty;
		}
		public float Difficulty { get; set; }
		public RoomTypes RoomType { get; set; }
		public override void OnAddToMap()
		{
			base.OnAddToMap();
			Difficulty = Rand.Float();
		}

		public bool IsSetup()
		{
			if ( ForceSetup )
			{
				return true;
			}
			if ( !HasRoom() )
			{
				return true;
			}
			return TileRoom?.IsSetup ?? false;
		}

		public virtual bool HasRoom()
		{
			return RoomType != RoomTypes.None;
		}
		public override void ServerTick( float delta, float currentTick )
		{
			base.ServerTick( delta, currentTick );
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
			if ( HasRoom() )
			{
				if ( TileRoom?.IsValid() ?? false )
				{
					return;
				}
				TileRoom = new Room();
				TileRoom.Create( this );
			}
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			TileRoom?.Delete();
		}
	}
}
