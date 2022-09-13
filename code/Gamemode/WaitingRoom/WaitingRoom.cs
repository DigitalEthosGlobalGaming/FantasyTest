using Degg.Entities.Common;
using Sandbox;
using System.Collections.Generic;

namespace FantasyTest
{
	public partial class WaitingRoom : Room
	{
		public List<Entity> WatchedEntities { get; set; }

		public override void Create( MapTile tile )
		{
			if ( IsClient )
			{
				return;
			}
			ParentMapTile = tile;
			Init<RoomTile>( tile.GetWorldPosition(), new Vector2( FloorSize, FloorSize ), 3, 3 );
			WatchedEntities = new List<Entity>();

			var light = new CandlePointLight()
			{

			};
			WatchedEntities.Add( light );
			light.Position = Position + Vector3.Up * 250f;

		}



		public void PlayerJoin()
		{

		}


		public override void OnSetup()
		{
			base.OnSetup();
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			if ( IsServer )
			{
				if ( WatchedEntities != null )
				{
					foreach ( var i in WatchedEntities )
					{
						i.Delete();
					}
				}
				WatchedEntities = new List<Entity>(); ;
			}
		}

		public void SpawnAllLoadingPlayers()
		{
			var players = GameLoadingPawn.GetAllPlayers<GameLoadingPawn>();
			foreach ( var player in players )
			{
				player.OnJoin();
			}

			MyGame.TeleportAllPlayersToWaitingRoom();
		}
	}
}

