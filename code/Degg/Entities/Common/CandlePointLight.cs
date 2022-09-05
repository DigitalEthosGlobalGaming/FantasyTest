using Sandbox;
using System;

namespace Degg.Entities.Common
{
	public partial class CandlePointLight : PointLightEntity
	{
		public bool DisableFlicker { get; set; } = true;
		[Event.Tick]
		internal void ProcessLightAnimation()
		{
			if ( !DisableFlicker )
			{
				return;
			}
			if ( (!base.IsClient || base.IsClientOnly) && Enabled )
			{
				float id = (GetHashCode() % 1000 + Time.Now % 1000);
				float brightness = (float)(Math.Sin( id * 5 ));
				brightness = brightness / 4;
				brightness = brightness + 1;


				BrightnessMultiplier = brightness;
			}
		}
	}
}
