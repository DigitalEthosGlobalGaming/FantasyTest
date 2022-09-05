
using Sandbox.UI;
using System;

namespace Degg.UI.Elements.SimpleGameMenu
{
	public class SimpleGameMenuItem : Panel
	{
		public SimpleGameMenuScreen Screen { get; set; }
		public SimpleGameMenu Menu { get; set; }

		public Action OnSelect { get; set; }

		public SimpleGameMenuItem()
		{
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


		public virtual void SetText(string text)
		{
			
		}
	}
}
