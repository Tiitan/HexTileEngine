using System;

/// <summary>
/// flag used when editing the map to set wich data should be modified.
/// </summary>
[Flags]
public enum PaintLayer
{
	None = 0,
	Type = 1,
	Height = 2,
	All = 3
}

public static class EnumHelper
{
	public static bool Contain<T>(this Enum type, T value)
	{
		return (((int)(object)type & (int)(object)value) == (int)(object)value);
	}

	public static T Include<T> (this Enum type, T value) where T : struct
	{
		return (T)(ValueType)(((int)(ValueType) type | (int)(ValueType) value));
	}
	
	public static T Remove<T> (this Enum type, T value) where T : struct
	{
		return (T)(ValueType)(((int)(ValueType)type & ~(int)(ValueType)value));
	}
}