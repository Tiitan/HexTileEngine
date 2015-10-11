using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class HexTerrainData : ScriptableObject 
{
	public Hexagon[] hexagons;

	[SerializeField]
	private int _length;
	[SerializeField]
	private int _width;

	public int Length { get { return _length; } }
	public int Width  { get { return _width; } }

	public int Size { get { return (Length + 1) / 2; } }

	public Hexagon this[int y, int x]
	{
		get { return hexagons[y * Width + x]; }
		set { hexagons[y * Width + x] = value; }
	}

    public Hexagon this[Vector2i location]
    {
        get { return hexagons[location.y * Width + location.x]; }
        set { hexagons[location.y * Width + location.x] = value; }
    }


    public bool IsInitialized
	{
		get { return hexagons != null; }
	}

	#region Public methods

	/// <summary>Used on asset creation, but not if instanciated by Unity's serialization system</summary>
	public void Initialize(int size)
	{
		// The _size field define one border of the hexagonal map surface inside a square, so the array size is "_size * 2 - 1"
		_length = size * 2 - 1;
		_width = size * 2 - 1;
		
		hexagons = new Hexagon[Length * Width];
		
		for (int y = 0; y < Length; y++)
			for (int x = 0; x < Width; x++)
				if (Contains(x, y))
					this[y, x] = new Hexagon(0);
	}

	public bool Contains(int x, int y)
	{
		if (x < 0 || y < 0 || x >= Width || y >= Length)
			return false;
		int center = Length / 2;
		return (y < center && center - y <= x) || (y >= center && Width - (y - center) > x);
	}
	
	public bool Contains(Vector2i v)
	{
		return Contains(v.x, v.y);
	}

	#endregion	
}