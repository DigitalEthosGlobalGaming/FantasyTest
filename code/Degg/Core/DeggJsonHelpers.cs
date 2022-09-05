using System.Text.Json;

namespace Degg.Core
{
	public class DeggJsonHelpers
	{

		public static object GetJsonElementValue( object obj )
		{
			if ( obj is JsonElement element)
			{
				switch ( element.ValueKind )
				{
					case JsonValueKind.False:
						return false;
					case JsonValueKind.Null:
						return null;
					case JsonValueKind.Number:
						return element.GetSingle();
					case JsonValueKind.Object:
						return element;
					case JsonValueKind.String:
						return element.ToString();
					case JsonValueKind.True:
						return true;
					case JsonValueKind.Undefined:
						return null;
					case JsonValueKind.Array:
						return element;
					default:
						return null;
				}
			}
			return obj;
		}
	}
}
