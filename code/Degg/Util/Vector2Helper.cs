using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sandbox.Degg.Util
{
	public static class Vector2Helper
	{
		public static float Angle(Vector2 a, Vector2 b)
		{
			float xDiff = b.x - a.x;
			float yDiff = b.y - a.y;
			return (float)(Math.Atan2( yDiff, xDiff ) * 180.0 / Math.PI);
		}
	}
}
