
using Sandbox.UI;
using System;

namespace Degg.UI.Elements.SimpleGameMenu
{
	public class SimpleGameMenuButtonItem : SimpleGameMenuItem
	{
		public Button PanelButton {get;set;}

		public SimpleGameMenuButtonItem()
		{
			AddClass("simple-game-menu-item-button");
			var me = this;
			AddEventListener("onclick", () =>
			 {
				 if ( me.OnSelect != null)
				 {
					 me.OnSelect();
				 }
				 Menu.OnMenuItemSelected( me );
			 } );
		}


		public override void SetText(string text)
		{
			if ( PanelButton == null )
			{
				PanelButton = AddChild<Button>();
			}
			PanelButton.Text = text;
		}
	}
}
