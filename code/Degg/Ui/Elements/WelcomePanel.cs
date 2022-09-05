using Sandbox.UI;

namespace Degg.Ui
{
	public partial class WelcomePanel : Panel {

		public Panel Inner { get; set; }
		public WelcomePanel()
		{
			AddClass( "degg-welcome-pane" );
			Inner = AddChild<Panel>( "degg-welcome-panel-inner" );
			StyleSheet.Load( "/Degg/Ui/Elements/WelcomePanel.scss" );
			SetupWelcomeElements();

		}

		public virtual void SetupWelcomeElements()
		{
			var button = Inner.AddChild<Button>( "start-button" );
			button.SetText( "Join Game" );
			button.AddEventListener( "OnClick", () =>
			{
				Sandbox.ConsoleSystem.Run( "degg.client.loaded" );
			} );
		}
	}
}
