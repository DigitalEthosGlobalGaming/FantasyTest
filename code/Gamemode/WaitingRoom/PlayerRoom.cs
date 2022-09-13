using Sandbox;
using System.Linq;

namespace FantasyTest
{
	public partial class PlayerRoom : WaitingRoom
	{
		public Client OwnerClient { get; set; }
		public ModelEntity RoomCollider { get; set; }
		public bool IsOpen { get; set; }

		public static PlayerRoom GetFreePlayerRoom()
		{
			var freePlayerRooms = Entity.All.Where( ( item ) =>
			{
				if ( item is PlayerRoom room )
				{
					if ( room.OwnerClient?.IsValid() ?? false )
					{
						return false;
					}
					return true;
				}
				return false;
			} );

			if ( freePlayerRooms.Count() == 0 )
			{
				return null;
			}
			return (PlayerRoom)freePlayerRooms.First();
		}

		public void Assign( Client owner )
		{
			OwnerClient = owner;
			IsOpen = true;

			RoomCollider?.Delete();

			RoomCollider = new ModelEntity();
			var height = 5000f;

			var size = GetMapSize();
			size = size.WithZ( height );
			var min = -size / 2;
			var max = size / 2;

			RoomCollider.Position = Position;
			RoomCollider.Tags.Remove( "solid" );
			RoomCollider.Tags.Add( "player-room" );

			// RoomCollider.SetupPhysicsFromAABB( PhysicsMotionType.Static, min, max );
			foreach ( var item in RoomCollider.Tags.List )
			{
				Log.Info( item );
			}
			if ( owner.Pawn is GameBasePlayer player )
			{
				player.MyRoom = this;
			}
		}
	}
}
