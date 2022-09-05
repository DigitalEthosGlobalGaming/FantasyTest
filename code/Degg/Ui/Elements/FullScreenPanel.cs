using Sandbox.UI;

namespace Degg.UI.Elements
{
	public partial class FullScreenPanel : DeggPanel {

		public Panel Inner { get; set; }

		public FullScreenPanel()
		{
			AddClass( "degg-fullscreen" );
			Inner = AddChild<Panel>( "degg-fullscreen-inner" );
			StyleSheet.Load( "/Degg/Ui/Styles/base.scss" );
		}

		public void SetCursorActive(bool val)
		{
			SetClass( "pointer-events-all", val );
		}
	}
}
