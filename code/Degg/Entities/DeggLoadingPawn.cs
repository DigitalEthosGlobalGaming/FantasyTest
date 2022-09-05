using Degg.Cameras;
using Degg.Core;
using Degg.Ui;
using Sandbox;
using System;

namespace Degg.Entities
{
	public partial class DeggLoadingPawn: DeggPlayer
	{

		public string EntityName { get; set; }
		public override void Spawn()
		{
			base.Spawn();
		}

		public virtual Entity OnJoin()
		{
			Delete();
			Client.Pawn = CreateByName( EntityName );
			return Client.Pawn;
		}

		public override void HudSetup()
		{
			base.HudSetup();
		}

		
	}
}
