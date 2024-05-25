namespace PrimalEditor.Utilities
{
	public static class ID
	{
		public static int INVALID_ID => -1;
		public static bool IsValid(int id) => id != INVALID_ID;
	}

	public static class MathUtil
	{
		public static float Epsilon => 0.00001f;

		public static bool IsTheSameAs(this float value, float other)
		{
			return Math.Abs(value - other) < Epsilon;
		}

		public static bool IsTheSameAs(this float? value, float? other)
		{
			if (!value.HasValue || !other.HasValue) return false;
			return Math.Abs(value.Value - other.Value) < Epsilon;
		}
	}
}