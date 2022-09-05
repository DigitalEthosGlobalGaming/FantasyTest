using Sandbox.UI;

namespace Degg.UI.Elements
{
	public partial class DeggCardBody : Panel
	{		
		public Label Description { get; set; }
		public DeggCardBody()
		{
			AddClass( "card-body" );
			Description = AddChild<Label>();
		}

		public void SetText(string str)
		{
			Description.SetText( str );
		}
	}
}
