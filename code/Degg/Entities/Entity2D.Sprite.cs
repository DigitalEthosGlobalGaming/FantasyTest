using Sandbox;

namespace Degg.Entities
{
	public partial class Entity2D
	{

		[Net]
		public SpriteSheetResource Sprite { get; set; }
		[Net]
		public string SpriteCode { get; set; }

		public bool Physics2DEnabled { get; set; }

		public void SetSprite( string code )
		{
			SpriteCode = code;
			SetMaterialGroup( code );
			if ( Physics2DEnabled )
			{
				SetupPhysics();
			}
		}

		public string GetSprite()
		{
			return SpriteCode;
		}

		public Sprite? GetSpritesheetSprite()
		{
			if ( Sprite?.Sprites.Count == 0 )
			{
				return null;
			}
			Sprite? firstSprite = null;

			foreach ( var sprite in Sprite?.Sprites )
			{
				if ( firstSprite == null )
				{
					firstSprite = sprite;
				}
				if ( sprite.Code?.Trim()?.ToLower() == SpriteCode?.Trim()?.ToLower() )
				{
					return sprite;
				}
			}
			return firstSprite;
		}

		public void SetupPhysics()
		{
			Physics2DEnabled = true;
			var nullableSprite = GetSpritesheetSprite();
			if ( nullableSprite.HasValue )
			{
				var sprite = nullableSprite.Value;

				if ( sprite.Shape == SpriteShapes.Square )
				{
					SetShape( sprite.Width, sprite.Height, Scale );
				}
				else
				{
					var largest = sprite.Width;
					if ( sprite.Height > largest )
					{
						largest = sprite.Height;
					}
					SetShape( largest, Scale );
				}
			}
		}

		public void SetSpritesheet( string path )
		{
			if ( IsClient )
			{
				return;
			}
			var sprite = ResourceLibrary.Get<SpriteSheetResource>( path );
			Sprite = sprite;
			if ( sprite != null )
			{
				SetModel( sprite.Model );
				if ( Physics2DEnabled )
				{
					SetupPhysics();
				}
			}
			else
			{
				Log.Warning( "No sprite found for " + path );
			}
		}
	}
}
