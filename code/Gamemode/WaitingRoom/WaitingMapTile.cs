using Sandbox;

namespace FantasyTest
{
	public partial class WaitingMapTile : MapTile
	{
		public Client PlayerOwner { get; set; }
		public bool IsEmpty = true;
		public void CreatePlayerRoom()
		{
			if ( IsClient )
			{
				return;
			}
			if ( TileRoom?.IsValid() ?? false )
			{
				return;
			}
			TileRoom = new PlayerRoom();
			TileRoom.Create( this );
			IsEmpty = false;
		}

		public void CreateWaitingRoom()
		{
			if ( IsClient )
			{
				return;
			}
			if ( TileRoom?.IsValid() ?? false )
			{
				return;
			}
			TileRoom = new WaitingRoom();
			TileRoom.Create( this );
			IsEmpty = false;
		}

		public override bool HasRoom()
		{
			return true;
		}
	}
}

