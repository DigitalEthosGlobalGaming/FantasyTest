namespace DeggExtensions
{
	// Extension methods must be defined in a static class.
	public static class FloatExtensions
	{
		public const float BaseMetric = 0.0254f;

		public static float MM( this float f )
		{
			return (f * BaseMetric) / 1000;
		}
		public static float CM( this float f )
		{
			return (f * BaseMetric) / 100;
		}

		public static float KM( this float f )
		{
			return (f * BaseMetric) * 1000;
		}

		public static float PeopleHeights( this float f )
		{
			return (f * 72);
		}

		public static float M( this float f )
		{
			return f * BaseMetric;
		}

	}
}
