using Sandbox;
using Sandbox.UI;

namespace Degg.UI.Elements
{
	public class Cursor :  Panel
	{
		public float Scale { get; set; } = 1;
		public Image ImageElement { get; set; }

		public Vector3 Position { get; set; }

		public Cursor()
		{
			ImageElement = AddChild<Image>();
		}
		
		public void SetScale(float scale)
		{
			Scale = scale;
		}
		public void SetTexture( string image)
		{
			ImageElement.SetTexture(image);	
		}

		public override void Tick()
		{
			base.Tick();
			var panelPos = Position.ToScreen();

			var left = panelPos.x * 100;
			Style.Left = Length.Percent( left );
			var top = panelPos.y * 100;
			Style.Top = Length.Percent( top );
			Style.Position = PositionMode.Absolute;
		}
		
	}
}
