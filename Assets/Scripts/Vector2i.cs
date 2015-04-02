using UnityEngine;
using System.Collections;
using System;

[Serializable]
public struct Vector2i
{
	public int x;
	public int y;

	#region Public methods

	public Vector2i(int x, int y)
	{
		this.x = x;
		this.y = y;
	}

	public int Distance(Vector2i other)
	{
		return Mathf.FloorToInt(Mathf.Sqrt(SquaredDistance(other))) + 1;
	}

	public int SquaredDistance(Vector2i other)
	{
		return Mathf.Abs(x - other.x) * Mathf.Abs(y - other.y);
	}

	public static Vector2i Lerp(Vector2i a, Vector2i b, float ratio)
	{
		return new Vector2i(Mathf.FloorToInt(Mathf.Lerp(a.x, b.x, ratio)), 
		                    Mathf.FloorToInt(Mathf.Lerp(a.y, b.y, ratio)));
	}

	public override string ToString()
	{
		return "(" + x + ", " + y + ")";
	}

	public override bool Equals(System.Object obj) 
	{
		return obj is Vector2i && this == (Vector2i)obj;
	}
	public override int GetHashCode() 
	{
		return x.GetHashCode() ^ y.GetHashCode();
	}

	#endregion

	#region Static methods

	public static Vector2i Convert(ref Vector2 floatVector)
	{
		return new Vector2i(Mathf.FloorToInt(floatVector.x), Mathf.FloorToInt(floatVector.y));
	}

	public static bool operator ==(Vector2i a, Vector2i b) 
	{
		return a.x == b.x && a.y == b.y;
	}
	public static bool operator !=(Vector2i x, Vector2i y) 
	{
		return !(x == y);
	}

	public static Vector2i operator+(Vector2i v1, Vector2i v2)
	{
		return new Vector2i(v1.x + v2.x, v1.y + v2.y);
	}

	public static Vector2i operator-(Vector2i v1, Vector2i v2)
	{
		return new Vector2i(v1.x - v2.x, v1.y - v2.y);
	}

	public static Vector2i operator*(Vector2i v1, Vector2i v2)
	{
		return new Vector2i(v1.x * v2.x, v1.y * v2.y);
	}

	public static Vector2i operator/(Vector2i v1, Vector2i v2)
	{
		return new Vector2i(v1.x / v2.x, v1.y / v2.y);
	}

	public static Vector2i operator%(Vector2i v1, Vector2i v2)
	{
		return new Vector2i(v1.x % v2.x, v1.y % v2.y);
	}

	#endregion
}
