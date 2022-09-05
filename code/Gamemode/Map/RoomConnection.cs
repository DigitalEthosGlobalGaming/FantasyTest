using Sandbox;

namespace FantasyTest.MapEntities
{
	public partial class RoomConnection : MapEntity
	{

		public RoomTile From { get; set; }
		public RoomTile To { get; set; }

		public static RoomConnection Create( Room from, Room to )
		{
			var existingConnection = from.GetConnection( to );
			if ( existingConnection != null )
			{
				return existingConnection;
			}
			var connection = new RoomConnection();

			if ( from.ParentMapTile.GridPosition.y != to.ParentMapTile.GridPosition.y )
			{
				if ( from.Position.y > to.Position.y )
				{
					var temp = from;
					from = to;
					to = temp;
				}

				var xPosition = Rand.Int( from.XSize - 3 ) + 1;
				var fromPosition = new Vector2( xPosition, from.YSize - 1 );
				var toPosition = new Vector2( xPosition, 0 );
				connection.From = from.GetSpace<RoomTile>( fromPosition.x, fromPosition.y );
				connection.To = to.GetSpace<RoomTile>( toPosition.x, toPosition.y );
			}
			else
			{
				if ( from.Position.x > to.Position.x )
				{
					var temp = from;
					from = to;
					to = temp;
				}

				var yPosition = Rand.Int( from.YSize - 3 ) + 1;
				var fromPosition = new Vector2( from.XSize - 1, yPosition );
				var toPosition = new Vector2( 0, yPosition );
				connection.From = from.GetSpace<RoomTile>( fromPosition.x, fromPosition.y );
				connection.To = to.GetSpace<RoomTile>( toPosition.x, toPosition.y );
			}

			if ( connection.From?.IsValid() ?? false )
			{
				if ( connection.To?.IsValid() ?? false )
				{
					connection.From.SetConnection( connection );
					connection.To.SetConnection( connection );
				}
			}

			return connection;
		}

		public bool IsConnection( Room a, Room b )
		{
			if ( a == From?.Map && b == To?.Map )
			{
				return true;
			}

			if ( b == From?.Map && a == To?.Map )
			{
				return true;
			}

			return false;
		}

		[Event.Tick.Server]
		public void ServerTick()
		{
			DebugOverlay.Sphere( Position, 5f, Color.Green, 0, true );
			if ( From?.IsValid() ?? false )
			{
				if ( To?.IsValid() ?? false )
				{
					DebugOverlay.Line( From.Position, To.Position, 0, false );
				}
			}
		}

		public override void Spawn()
		{
			base.Spawn();
		}
	}
}
