using Sandbox.UI;

namespace Degg.UI.Elements
{
	public partial class DeggCardImage : Panel
	{		
		public DeggCardImage()
		{

		}

		public void SetImage(string src)
		{
			RemoveClass( "card-image-container" );
			if ( src != null && src.Length > 0 )
			{
				AddClass( "card-image-container" );
				DeleteChildren();
				var Image = AddChild<Image>();
				Image.SetTexture( src );
				Image.AddClass( "card-image" );
			}
		}
	}
}
