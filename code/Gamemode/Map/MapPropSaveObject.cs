namespace FantasyTest.MapEntities
{
	public struct MapPropSaveObjectData
	{
		public Vector3 Position { get; set; }
		public Rotation Rotation { get; set; }
		public string PropDataResourcePath { get; set; }
	}
	public class MapPropSaveObject
	{
		MapProp Prop { get; set; }
		public MapPropSaveObject()
		{

		}
		public MapPropSaveObject( MapProp prop )
		{
			SetMapProp( prop );
		}
		public void SetMapProp( MapProp prop )
		{
			Prop = prop;
		}

		public static MapPropSaveObjectData Serialise( MapProp obj )
		{
			var data = new MapPropSaveObjectData()
			{
				Position = obj.Position,
				Rotation = obj.Rotation,
				PropDataResourcePath = obj.PropData?.ResourcePath
			};
			Log.Info( data );

			return data;
		}


		public static MapProp Create( MapPropSaveObjectData obj )
		{
			var newProp = MapProp.Create( PropResource.Get<PropResource>( obj.PropDataResourcePath ) );

			if ( newProp?.IsValid ?? false )
			{
				newProp.Position = obj.Position;
				newProp.Rotation = obj.Rotation;
			}

			return newProp;
		}
	}
}
