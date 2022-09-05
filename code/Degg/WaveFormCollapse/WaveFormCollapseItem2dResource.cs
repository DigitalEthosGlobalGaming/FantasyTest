
using Sandbox;

namespace Degg.WaveFormCollapse
{
	[GameResource( "Wave Form Collapse 2D", "wfctwod", "" )]
	public partial class WaveFormCollapseItem2dResource : DeggGameResource
	{
		public string Code { get; set; }
		public string LeftCodes { get; set; }
		public string RightCodes { get; set; }
		public string TopCodes { get; set; }
		public string BottomCodes { get; set; }

		public bool IsValidLeftConnection( WaveFormCollapseItem2dResource item)
		{
			return LeftCodes.Contains( item.Code );
		}
		public bool IsValidRightConnection( WaveFormCollapseItem2dResource item )
		{
			return RightCodes.Contains( item.Code );
		}
		public bool IsValidTopConnection( WaveFormCollapseItem2dResource item )
		{
			return TopCodes.Contains( item.Code );
		}
		public bool IsValidBottomConnection( WaveFormCollapseItem2dResource item )
		{
			return BottomCodes.Contains( item.Code );
		}

		public bool IsValidLeftConnection( string item )
		{
			return LeftCodes.Contains( item );
		}
		public bool IsValidRightConnection( string item )
		{
			return RightCodes.Contains( item );
		}
		public bool IsValidTopConnection( string item )
		{
			return TopCodes.Contains( item );
		}
		public bool IsValidBottomConnection( string item )
		{
			return BottomCodes.Contains( item );
		}

		public bool SharesCodes( string mine, string theirs)
		{
			var mineArray = mine.Split( "," );
			foreach ( var i in mineArray )
			{
				if (theirs.Contains( i ))
				{
					return true;
				}
			}
			return false;
		}
	}
}
