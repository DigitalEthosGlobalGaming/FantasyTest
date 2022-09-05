using Degg.Core;
using Degg.UI.Elements;
using Sandbox;
using Sandbox.UI;
using System.Collections.Generic;

namespace Degg.Ui
{
	public partial class PlayerHud : HudEntity<RootPanel>
	{
		public static PlayerHud Current { get; set; }
		public bool HasCursor { get; set; }
		public List<Panel> Panels { get; set; }
		public DeggPlayer Player { get; set; }
		public PlayerHud( DeggPlayer player )
		{
			Current = this;
			HasCursor = false;
			Player = player;
			Panels = new List<Panel>();
			Log.Info( "HUD" );
			Setup();
		}

		[Event.Hotload]
		public void Setup()
		{
			if ( Panels != null )
			{
				foreach ( var i in Panels )
				{
					i.Delete( true );
				}
			}

			OnSetup();
		}

		[Event.Tick]
		public void Tick()
		{
			if ( RootPanel != null )
			{
				RootPanel.SetClass( "degg-enable-cursor", HasCursor );
			}
		}
		public virtual void OnSetup()
		{
			if ( RootPanel != null )
			{
				RootPanel.StyleSheet.Load( "/Degg/Ui/Styles/base.scss" );
				RootPanel.AddClass( "degg-root" );
				RootPanel.AddChild<ChatBox>();
			}
		}

		public T AddPanel<T>() where T : Panel, new()
		{
			var newElement = RootPanel.AddChild<T>();
			if ( newElement is PlayerPanel<DeggPlayer> playerPanel )
			{
				playerPanel.SetPlayer( Player );
			}
			Panels.Add( newElement );
			return newElement;
		}


		protected override void OnDestroy()
		{
			base.OnDestroy();
			foreach ( var item in Children )
			{
				if ( item.IsValid() )
				{
					item.Delete();
				}
			}
		}
	}
}
