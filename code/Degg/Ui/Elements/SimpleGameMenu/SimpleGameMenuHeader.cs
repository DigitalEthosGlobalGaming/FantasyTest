
using Sandbox.UI;

namespace Degg.UI.Elements.SimpleGameMenu
{
	public class SimpleGameMenuHeader: Button
	{
		public Label Header { get; set; }

		public SimpleGameMenuHeader()
		{
			AddClass( "simple-game-menu-header" );
		}
		public virtual void SetTitle(string title)
		{
			if ( Header == null )
			{
				Header = AddChild<Label>();
			}
			Header.SetText( title );
		}
	}
}
