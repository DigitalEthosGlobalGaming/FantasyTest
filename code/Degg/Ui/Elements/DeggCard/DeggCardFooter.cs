using Sandbox.UI;

namespace Degg.UI.Elements
{
	public partial class DeggCardFooter : Panel
	{
		public Label Description { get; set; }

		public DeggCardFooter()
		{
			AddClass( "card-footer" );	
		}
		public void SetText( string str )
		{
			if ( Description  != null)
			{
				Description.Delete();
			}

			if ( str != null )
			{
				Description = AddChild<Label>();
				Description.Text = str;
			}
		}
	}
}
