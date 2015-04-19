using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[Serializable]
public class Hexagon
{
	public const float Lentgh = 1.0f;
	public const float Width = 1.0f;

	[SerializeField]
	private int 	_typeID;

	[SerializeField]
	private float	_height;

	public int 		TypeID 
	{
		get { return _typeID; }
		set { _typeID = value; }
	}

	public float 	Height 
	{
		get { return _height; }
		set { _height = value; }
	}

	#region Constructors

	/// <summary>Constructor used by Unity's serialization system.</summary>
	public Hexagon() {}

	/// <summary>Constructor used on asset creation.</summary>
	public Hexagon(int typeID)
	{
		_typeID = typeID;
	}

	#endregion

	#region Public methods

	/// <summary>
	/// Add the geometry of this Hexagon inside the MeshData structure of the calling Chunk.
	/// </summary>
	/// <param name="meshData">MeshData structure to be filed </param>
	/// <param name="chunkOffSet">world offset of the parent chunk </param>
	/// <param name="coordinate">2d grid coordinate of this hexa relative to its chunk </param>
	/// <param name="neighbours"> Array containing the 6 neighbours of this hex, some can be null</param>
	public void AddToChunk(ref Chunk.MeshData meshData, Vector3 chunkOffSet, Vector2i coordinate, 
	                       Hexagon[] neighbours, HexagonType type)
	{
		// Compute hexagon position.
		Vector3 coordinateOffset = HexagonUtils.ConvertHexaSpaceToOrthonormal(coordinate) + chunkOffSet;

		GenerateTop(ref meshData, coordinateOffset, type, neighbours);
		for (int i = 0; i < 6; i++)
		{
			if (IsSideVisible(neighbours[i]) || type.SizeMultiplier < 1 - HexagonUtils.FloatEpsilon)
				GenerateSide(ref meshData, coordinateOffset, i, neighbours, type);
		}
	}

	#endregion

	#region Private methods

	void GenerateTop(ref Chunk.MeshData meshData, Vector3 coordinateOffset, HexagonType type, Hexagon[] neighbours)
	{
		// Get referential vertice.
		int verticesOffset = meshData.vertices.Count;

		for (int i = 0; i < positionsLookup.GetLength(0); i++)
		{
			// Compute Edge horizontal shifting
			Vector2 multiplier = i != 0 && IsSideVisible(neighbours[i - 1]) && IsSideVisible(neighbours[(i + 4) % 6])? 
						new Vector2(type.SizeMultiplier, type.SizeMultiplier): new Vector2(1f, 1f);

			AddVertex(ref meshData,
			          new Vector3(positionsLookup[i, 0] * Width * multiplier.x + coordinateOffset.x, 
						          Height + coordinateOffset.y, 
			            		  positionsLookup[i, 1] * Lentgh * multiplier.y + coordinateOffset.z),
			          new Vector2(topUVsLookup[i, 0], topUVsLookup[i, 1]));
		}
		
		// triangles
		for (int i = 0; i < topTrianglesLookup.GetLength(0); i++)
		{
			meshData.triangles[type.TopMaterialIndex].Add(verticesOffset + topTrianglesLookup[i, 0]);
			meshData.triangles[type.TopMaterialIndex].Add(verticesOffset + topTrianglesLookup[i, 1]);
			meshData.triangles[type.TopMaterialIndex].Add(verticesOffset + topTrianglesLookup[i, 2]);
		}
	}

	void GenerateSide(ref Chunk.MeshData meshData, Vector3 coordinateOffset, 
	                  int faceIndex, Hexagon[] neighbours, HexagonType type)
	{
		// Make Edge
		if (type.EdgeHeight > HexagonUtils.FloatEpsilon  || type.SizeMultiplier < 1 - HexagonUtils.FloatEpsilon)
		{
			// Compute Edge horizontal shifting
			float[] multiplier = new float[2];
			multiplier[0] = IsSideVisible(neighbours[(faceIndex + 5) % 6]) && IsSideVisible(neighbours[faceIndex])? 
				type.SizeMultiplier : 1f;
			multiplier[1] = IsSideVisible(neighbours[faceIndex]) && IsSideVisible(neighbours[(faceIndex + 1) % 6])? 
				type.SizeMultiplier : 1f;

			AddSideQuad(ref meshData, coordinateOffset, faceIndex, multiplier,
			            type.EdgeMaterialIndex, Height, Height - type.EdgeHeight, 0f);
		}

		// Make Side
		float baseLevel = neighbours[faceIndex] == null ? -1 : neighbours[faceIndex].Height;
		if (baseLevel < Height - type.EdgeHeight - HexagonUtils.FloatEpsilon)
			AddSideQuad(ref meshData, coordinateOffset, faceIndex, new float[] { 1f, 1f} ,
			type.SideMaterialIndex, Height - type.EdgeHeight, baseLevel, 1 - (Height - type.EdgeHeight - baseLevel) * type.SideLoopFrequency);
	}

	#endregion
	
	#region Private static Function

	bool IsSideVisible(Hexagon neighbour)
	{
		return neighbour == null || neighbour.Height < this.Height - HexagonUtils.FloatEpsilon;
	}

	static void AddSideQuad(ref Chunk.MeshData meshData, Vector3 coordinateOffset, int faceIndex, float[] topMultiplier,
	                        int triangleIndex, float top, float bottom, float uvBottomPosition)
	{
		int verticesOffset = meshData.vertices.Count;

		// vertices
		AddVertex(ref meshData,
		          new Vector3(positionsLookup[faceIndex + 1, 0] * Width * topMultiplier[0] + coordinateOffset.x, 
		            top + coordinateOffset.y, 
		            positionsLookup[faceIndex + 1, 1] * Lentgh * topMultiplier[0] + coordinateOffset.z),
		          new Vector2(0, 1));
		
		AddVertex(ref meshData,
		          new Vector3(positionsLookup[(faceIndex + 1) % 6 + 1, 0] * Width * topMultiplier[1] + coordinateOffset.x, 
		            top + coordinateOffset.y, 
		            positionsLookup[(faceIndex + 1) % 6 + 1, 1] * Lentgh  * topMultiplier[1] + coordinateOffset.z),
		          new Vector2(1, 1));
		
		AddVertex(ref meshData,
		          new Vector3(positionsLookup[faceIndex + 1, 0] * Width + coordinateOffset.x, 
		            bottom + coordinateOffset.y, 
		            positionsLookup[faceIndex + 1, 1] * Lentgh + coordinateOffset.z),
		          new Vector2(0, uvBottomPosition));
		
		AddVertex(ref meshData,
		          new Vector3(positionsLookup[(faceIndex + 1) % 6 + 1, 0] * Width + coordinateOffset.x, 
		            bottom + coordinateOffset.y, 
		            positionsLookup[(faceIndex + 1) % 6 + 1, 1] * Lentgh + coordinateOffset.z),
		          new Vector2(1, uvBottomPosition));

		// triangles
		meshData.triangles[triangleIndex].Add(verticesOffset + sideTrianglesLookup[0, 0]);
		meshData.triangles[triangleIndex].Add(verticesOffset + sideTrianglesLookup[0, 1]);
		meshData.triangles[triangleIndex].Add(verticesOffset + sideTrianglesLookup[0, 2]);
		
		meshData.triangles[triangleIndex].Add(verticesOffset + sideTrianglesLookup[1, 0]);
		meshData.triangles[triangleIndex].Add(verticesOffset + sideTrianglesLookup[1, 1]);
		meshData.triangles[triangleIndex].Add(verticesOffset + sideTrianglesLookup[1, 2]);
	}

	static void AddVertex(ref Chunk.MeshData meshData, Vector3 position, Vector2 uvs)
	{
		meshData.vertices.Add(position);
		meshData.uvs.Add(uvs);
		//meshData.normals.Add(new Vector3(0, 1, 0)); // now use recalculateNormal function
		meshData.colors.Add(Color.white);
	}

	#endregion

	#region lookup tables
	
	//  TOP    -0.5 | 0  |  0.5	
	//       ------------------	
	//  0.5  |      | 2  |   	
	//  0.25 |  1   |    |   3	
	//  0    |      | 0  |       
	// -0.25 |  6   |    |   4  
	// -0.5  |      | 5  |    	

	/// <summary>
	/// Hexagon vertex position.
	/// </summary>
	static readonly float[,] positionsLookup = new [,]
	{ 
		{    0f,  0f   }, 
		{ -0.5f,  0.25f}, 
		{    0f,  0.5f }, 
		{  0.5f,  0.25f}, 
		{  0.5f, -0.25f}, 
		{    0f, -0.5f }, 
		{ -0.5f, -0.25f},
	};
	
	static readonly float[,] topUVsLookup = new [,]
	{ 
		// Hexagon top
		{ 0.5f, 0.5f }, 
		{ 0f  , 0.75f}, 
		{ 0.5f, 1f   },
		{ 1f  , 0.75f}, 
		{ 1f  , 0.25f}, 
		{ 0.5f, 0f   }, 
		{ 0f  , 0.25f}
	};

	static readonly int[,] topTrianglesLookup = new [,]
	{ 
		// Hexagon top
		{ 0, 1, 2 },
		{ 0, 2, 3 },
		{ 0, 3, 4 },
		{ 0, 4, 5 },
		{ 0, 5, 6 },
		{ 0, 6, 1 }
	};
	
	static readonly int[,] sideTrianglesLookup = new [,]
	{ 
		// Sides
		{ 0, 2, 1},
		{ 1, 2, 3}
	};
	
	#endregion
}
