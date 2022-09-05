using Sandbox;

namespace Degg.GridSystem.GridSpaces
{
	public partial class RoadGridSpace: GridSpace
	{
		public RoadTypeEnum RoadType { get; set; } = RoadTypeEnum.StreetEmpty;

		public enum RoadTypeEnum
		{
			StreetEmpty = 14,
			Straight = 0,
			Corner = 1,
			ThreeWay = 2,
			FourWay = 4,
			DeadEnd = 11,
			WaterEmpty = 15,
		}

		public override void OnAddToMap()
		{
			base.OnAddToMap();
			UpdateNeighbours();
			UpdateModel();
		}

		public override float GetMovementWeight( GridSpace a, NavPoint n )
		{
			if (a is RoadGridSpace)
			{
				return 10;
			}
			return -1;
		}

		public void UpdateNeighbours()
		{
			RoadGridSpace[] neighbours = this.GetNeighbours<RoadGridSpace>();

			foreach(var neighbour in neighbours)
			{
				neighbour?.UpdateModel();
			}
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			if ( IsServer )
			{
				UpdateNeighbours();
			}
		}

		public virtual string GetModelForRoadType(RoadTypeEnum roadType )
		{
			switch ( roadType )
			{
				case RoadTypeEnum.StreetEmpty:
					return "models/tiles/tile.vmdl";
				case RoadTypeEnum.Straight:
					return "models/tiles/tile_straight.vmdl";
				case RoadTypeEnum.Corner:
					return "models/tiles/tile_corner.vmdl";
				case RoadTypeEnum.ThreeWay:
					return "models/tiles/tile_split.vmdl";
				case RoadTypeEnum.FourWay:
					return "models/tiles/tile_crossing.vmdl";
				case RoadTypeEnum.DeadEnd:
					return "models/tiles/tile_end.vmdl";
				case RoadTypeEnum.WaterEmpty:
					return "models/tiles/tile.vmdl";
				default:
					Log.Warning( $"No valid road type for {roadType}" );
					return "models/roads/street_empty.vmdl";
			}
		}


		public virtual void OnNeighbourTileControllerChange( GridSpace tile )
		{
			UpdateModel();
		}

		public virtual void UpdateModel()
		{
			float rotation = 0;


			RoadGridSpace[] neighbours = this.GetNeighbours<RoadGridSpace>();
			bool up = neighbours[0] != null;
			bool right = neighbours[1] != null;
			bool down = neighbours[2] != null;
			bool left = neighbours[3] != null;

			RoadTypeEnum newRoadType = RoadTypeEnum.StreetEmpty;

			// We are a road.
			int totalCount = 0;

			if ( up )
			{
				totalCount++;
			}
			if ( down )
			{
				totalCount++;
			}
			if ( left )
			{
				totalCount++;
			}
			if ( right )
			{
				totalCount++;
			}

			if ( totalCount == 3 ) // THREE WAYS
			{
				newRoadType = RoadTypeEnum.ThreeWay;
				if ( !up )
				{
					rotation = 180;
				}
				else if ( !right )
				{
					rotation = 270;
				}
				else if ( !down )
				{
					rotation = 0;
				}
				else if ( !left )
				{
					rotation = 90;
				}
			}
			else if ( totalCount == 2 ) // STRAIGHT OR CURVES
			{
				if ( left && right )
				{
					newRoadType = RoadTypeEnum.Straight;
					rotation = 90;
				}
				else if ( up && down )
				{
					newRoadType = RoadTypeEnum.Straight;
				}
				else
				{
					newRoadType = RoadTypeEnum.Corner;
					if ( up )
					{
						if ( left )
						{
							rotation = 180;
						}
						else
						{
							rotation = 270;
						}
					}
					if ( down )
					{
						if ( left )
						{
							rotation = 90;
						}
						else
						{
							rotation = 0;
						}
					}
				}
			}
			else if ( totalCount == 1 ) // DEAD ENDS
			{
				if ( up )
				{
					rotation = 0;
				}
				else if ( right )
				{
					rotation = 90;
				}
				else if ( down )
				{
					rotation = 180;
				}
				else if ( left )
				{
					rotation = 270;
				}
				newRoadType = RoadTypeEnum.DeadEnd;
			}
			else
			{
				newRoadType = RoadTypeEnum.FourWay;
			}

			RoadType = newRoadType;

			var TargetRotation = Rotation.FromAxis( Vector3.Up, rotation );

			var model = GetModelForRoadType( newRoadType );

			SetModel( model );
			SetupPhysicsFromModel( PhysicsMotionType.Static );
			this.Rotation = TargetRotation;
		}
	}
}
