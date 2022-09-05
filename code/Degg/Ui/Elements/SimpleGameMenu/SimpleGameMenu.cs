
using Sandbox.UI;
using System;
using System.Collections.Generic;

namespace Degg.UI.Elements.SimpleGameMenu
{
	public  class SimpleGameMenu: FullScreenPanel
	{
		public SimpleGameMenuHeader HeaderPanel { get; set; } 

		public Dictionary<string, SimpleGameMenuItem> MenuItems { get; set; }

		public Panel MenuItemsPanel { get; set; }

		public Button CloseScreenButton { get; set; }

		public SimpleGameMenuScreen SelectedScreen { get; set; }


		public SimpleGameMenu()
		{
			AddClass( "simple-game-menu" );
			SetupMenuItems();
		}


		public virtual void SetupMenuItems()
		{
			CreateHeader();
			CreateMenuItems();
			CloseScreenButton = AddChild<Button>( "hidden" );
			CloseScreenButton.SetText( "Back" );
			CloseScreenButton.AddClass( "simple-game-menu-close-button" );
			CloseScreenButton.AddEventListener( "onclick", () =>
			 {
				 OnMenuItemSelected( null );
			 } );
		}

		public virtual void SetTitle(string title)
		{
			if (HeaderPanel != null)
			{
				HeaderPanel.SetTitle( title );
			}
		}

		public virtual SimpleGameMenuHeader CreateHeader()
		{
			HeaderPanel = AddChild<SimpleGameMenuHeader>();
			HeaderPanel.SetTitle( "My Game" );
			return HeaderPanel;
		}

		public virtual void CreateMenuItems()
		{
			MenuItemsPanel = AddChild<Panel>( "simple-game-menu-items" );
			if ( MenuItems == null )
			{
				MenuItems = new Dictionary<string, SimpleGameMenuItem>();
			}
		}

		public T AddMenuItem<T>( string text, Action onSelect = null ) where T : SimpleGameMenuItem, new()
		{

			if ( MenuItems == null)
			{
				MenuItems = new Dictionary<string, SimpleGameMenuItem>(); 
			}

			T item;
			if (MenuItems.ContainsKey(text))
			{
				item = MenuItems[text] as T;
			} else
			{
				item = MenuItemsPanel.AddChild<T>( "simple-game-menu-item" );
			}

			item.OnSelect = onSelect;
			item.SetText( text );
			item.Menu = this;

			MenuItems[text] = item;

			return item;
		}

		public SimpleGameMenuButtonItem AddMenuItemScreen<T>( string text, Action onSelect = null ) where T : SimpleGameMenuScreen, new()
		{
			var item = AddMenuItem<SimpleGameMenuButtonItem>( text, onSelect );
			var screen = AddChild<T>( "hidden" );
			item.Screen = screen;

			return item;
		}

		public T AddMenuItem<T,S>( string text,Action onSelect = null ) where T: SimpleGameMenuItem, new() where S: SimpleGameMenuScreen, new()
		{			
			var item = AddMenuItem<T>(text, onSelect );
			var screen = AddChild<S>("hidden");
			item.Screen = screen;

			return item;
		}

		public SimpleGameMenuButtonItem AddMenuItem(string text, Action onSelect = null )
		{
			return AddMenuItem<SimpleGameMenuButtonItem>( text, onSelect );
		}

		public virtual void OnMenuItemSelected( SimpleGameMenuItem item )
		{
			SelectedScreen?.AddClass( "hidden" );
			SelectedScreen = item?.Screen;
			var isScreenOpened = SelectedScreen != null;
			HeaderPanel?.SetClass( "hidden", isScreenOpened );
			MenuItemsPanel?.SetClass( "hidden", isScreenOpened );
			SelectedScreen?.SetClass( "hidden", !isScreenOpened );
			CloseScreenButton?.SetClass( "hidden", !isScreenOpened );
		}
	}
}
