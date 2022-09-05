using Sandbox.UI;

namespace Degg.UI.Elements
{


	public partial class DeggCard: DeggPanel
	{
		public DeggCardHeader Header { get; set; }
		public DeggCardImage Image { get; set; }
		public DeggCardFooter Footer { get; set; }
		public DeggCardBody Body { get; set; }
		public Panel Inner { get; set; }

		public DeggCard()
		{
			AddClass( "card" );
			CreateSubElements();
		}

		protected override void OnClick( MousePanelEvent e )
		{
			base.OnClick( e );
			if (e.Target != this)
			{
				OnClickInner( e );
			}
		}

		public virtual void OnClickInner(MousePanelEvent e)
		{

		}



		public virtual DeggCardHeader CreateHeaderElement()
		{
			return Inner.AddChild<DeggCardHeader>();
		}
		public virtual DeggCardImage CreateImageElement()
		{
			return Inner.AddChild<DeggCardImage>();
		}
		public virtual DeggCardBody CreateBodyElement()
		{
			return Inner.AddChild<DeggCardBody>();
		}
		public virtual DeggCardFooter CreateFooterElement()
		{
			return Inner.AddChild<DeggCardFooter>();
		}

		public virtual void CleanupChildren()
		{
			this.DeleteChildren();
			Inner = null;
			Header = null;
			Image = null;
			Body = null;
			Footer = null;
		}

		public virtual void CreateSubElements()
		{
			CleanupChildren();
			Inner = AddChild<Panel>();
			Inner.AddClass( "card-inner" );
			Header = CreateHeaderElement();
			Image = CreateImageElement();
			Body = CreateBodyElement();
			Footer = CreateFooterElement();
		}
	}
}
