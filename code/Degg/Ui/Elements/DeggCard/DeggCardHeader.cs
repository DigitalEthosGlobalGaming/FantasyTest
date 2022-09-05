using Sandbox.UI;

namespace Degg.UI.Elements
{
	public partial class DeggCardHeader : Panel
	{
		public Label Text { get; set; }

		public Image HeaderImage {get;set;}		
		public string Image { get; set; }
		public DeggCardHeader()
		{
			AddClass( "card-header" );
			Text = AddChild<Label>();
		}
		public void SetText(string t)
		{
			Text.SetText( t );
		}
	}
}
