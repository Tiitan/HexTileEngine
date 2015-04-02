using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class HexagonUtils 
{
	public static Vector3 ConvertHexaSpaceToOrthonormal(Vector2i coordinate)
	{
		return new Vector3(coordinate.x * Hexagon.Width + coordinate.y * 0.5f * Hexagon.Width,
		                   0,
		                   coordinate.y * 0.75f * Hexagon.Lentgh);
	}
	
	static Vector2i ConvertOrthonormalToAproxHexaSpace(Vector3 position)
	{
		return new Vector2i(Mathf.FloorToInt((position.x / Hexagon.Width - position.z * 2 / 3 / Hexagon.Lentgh) + 0.5f),
		                    Mathf.FloorToInt((position.z * 4 / 3 / Hexagon.Lentgh) + 0.5f));
	}
	
	public static Vector2i ConvertOrthonormalToHexaSpace(Vector3 position)
	{
		// TODO: Use math to make this function cleaner.
		
		Vector2i AproxGridCoord = ConvertOrthonormalToAproxHexaSpace(position);
		
		Vector2i[] nerbyHex = new Vector2i[5];
		nerbyHex[0].x = AproxGridCoord.x;		nerbyHex[0].y = AproxGridCoord.y;
		nerbyHex[1].x = AproxGridCoord.x - 1;	nerbyHex[1].y = AproxGridCoord.y;
		nerbyHex[2].x = AproxGridCoord.x + 1;	nerbyHex[2].y = AproxGridCoord.y;
		nerbyHex[3].x = AproxGridCoord.x;		nerbyHex[3].y = AproxGridCoord.y - 1;
		nerbyHex[4].x = AproxGridCoord.x;		nerbyHex[4].y = AproxGridCoord.y + 1;
		
		Vector3 orthoCoordHexPos = ConvertHexaSpaceToOrthonormal(nerbyHex[0]);
		double shortestDist = Vector3.Distance(orthoCoordHexPos, position);
		int idxShortestDist = 0;
		for (int i = 1; i < 5; i++)
		{
			orthoCoordHexPos = ConvertHexaSpaceToOrthonormal(nerbyHex[i]);
			
			double currentDist = Vector3.Distance(orthoCoordHexPos, position);
			if (currentDist < shortestDist)
			{
				shortestDist = currentDist;
				idxShortestDist = i;
			}
		}
		
		return nerbyHex[idxShortestDist];
	}

	/// <summary>
	/// Gets hexagon neighbours. starting from the top left corner, in  inverse trigo direction.
	/// </summary>
	/// <returns>Array of 6 neighbours. Some can be null!</returns>
	/// <param name="coordinate">Coordinate of the hexagon</param>
	/// <param name="hexas">array of hexagon used to get the neighbours from</param>
	public static Hexagon[] GetNeighbours(Vector2i coordinate, HexTerrainData hexaData)
	{
		Hexagon[] neighbours = new Hexagon[6];

		if (hexaData.Contains(coordinate.x - 1, coordinate.y + 1))
			neighbours[0] = hexaData[coordinate.y + 1, coordinate.x - 1];
	    if (hexaData.Contains(coordinate.x    , coordinate.y + 1))
			neighbours[1] = hexaData[coordinate.y + 1, coordinate.x];
		if (hexaData.Contains(coordinate.x + 1, coordinate.y    ))
			neighbours[2] = hexaData[coordinate.y, coordinate.x + 1];
		if (hexaData.Contains(coordinate.x + 1, coordinate.y - 1))
			neighbours[3] = hexaData[coordinate.y - 1, coordinate.x + 1];
		if (hexaData.Contains(coordinate.x    , coordinate.y - 1))
			neighbours[4] = hexaData[coordinate.y - 1, coordinate.x];
		if (hexaData.Contains(coordinate.x - 1, coordinate.y    ))
			neighbours[5] = hexaData[coordinate.y, coordinate.x - 1];

		return neighbours;
	}

	public static IEnumerable<Vector2i> GetLine (Vector2i initialGridCoordinate, Vector2i endGridCoordinate)
	{
		List<Vector2i> line = new List<Vector2i>();
		int distance = initialGridCoordinate.Distance(endGridCoordinate) * 2;
		// Debug.Log(initialGridCoordinate + " " + endGridCoordinate + " " + distance);
		// TODO: Reduce complexity. (now 1 iteration in GetLine then an interation in the calling function).
		for(int i = 0; i < distance; i++)
			line.Add(Vector2i.Lerp(initialGridCoordinate, endGridCoordinate, (float)i / distance));
		return line;
	}
}
