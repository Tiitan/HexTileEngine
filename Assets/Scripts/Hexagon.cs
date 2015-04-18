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

		GenerateTop(ref meshData, coordinateOffset, type);
		for (int i = 0; i < 6; i++)
		{
			if (neighbours[i] == null || neighbours[i].Height < this.Height + 0.05f)
				GenerateSide(ref meshData, coordinateOffset, i, neighbours[i], type);
		}
	}

	#endregion

	#region Private methods

	void GenerateTop(ref Chunk.MeshData meshData, Vector3 coordinateOffset, HexagonType type)
	{
		// Get referential vertice.
		int verticesOffset = meshData.vertices.Count;

		for (int i = 0; i < positionsLookup.GetLength(0); i++)
		{
			AddVertex(meshData,
			          new Vector3(positionsLookup[i, 0] * Width + coordinateOffset.x, 
			            Height + coordinateOffset.y, 
					              positionsLookup[i, 1] * Lentgh + coordinateOffset.z),
			          new Vector2(topUVsLookup[i, 0], topUVsLookup[i, 1]),
			          type);
		}
		
		// triangles
		for (int i = 0; i < topTrianglesLookup.GetLength(0); i++)
		{
			meshData.triangles[type.TopMaterialIndex].Add(verticesOffset + topTrianglesLookup[i, 0]);
			meshData.triangles[type.TopMaterialIndex].Add(verticesOffset + topTrianglesLookup[i, 1]);
			meshData.triangles[type.TopMaterialIndex].Add(verticesOffset + topTrianglesLookup[i, 2]);
		}
	}

	void GenerateSide(ref Chunk.MeshData meshData, Vector3 coordinateOffset, int faceIndex, Hexagon neighbour, HexagonType type)
	{
		// Get referential vertice.
		int verticesOffset = meshData.vertices.Count;

		float baseLevel = neighbour == null ? -1 : neighbour.Height;

		// vertices
		AddVertex(meshData,
		          new Vector3(positionsLookup[faceIndex + 1, 0] * Width + coordinateOffset.x, 
		                      Height + coordinateOffset.y, 
		            		  positionsLookup[faceIndex + 1, 1] * Lentgh + coordinateOffset.z),
		          new Vector2(sideUVsLookup[0, 0], sideUVsLookup[0, 1]),
		          type);

		AddVertex(meshData,
		          new Vector3(positionsLookup[(faceIndex + 1) % 6 + 1, 0] * Width + coordinateOffset.x, 
		            		  Height + coordinateOffset.y, 
		            		  positionsLookup[(faceIndex + 1) % 6 + 1, 1] * Lentgh + coordinateOffset.z),
		          new Vector2(sideUVsLookup[0, 0], sideUVsLookup[0, 1]),
		          type);

		AddVertex(meshData,
		          new Vector3(positionsLookup[faceIndex + 1, 0] * Width + coordinateOffset.x, 
		            		  baseLevel + coordinateOffset.y, 
		           			  positionsLookup[faceIndex + 1, 1] * Lentgh + coordinateOffset.z),
		          new Vector2(sideUVsLookup[0, 0], sideUVsLookup[0, 1]),
		          type);

		AddVertex(meshData,
		          new Vector3(positionsLookup[(faceIndex + 1) % 6 + 1, 0] * Width + coordinateOffset.x, 
		            		  baseLevel + coordinateOffset.y, 
		            		  positionsLookup[(faceIndex + 1) % 6 + 1, 1] * Lentgh + coordinateOffset.z),
		      	  new Vector2(sideUVsLookup[0, 0], sideUVsLookup[0, 1]),
		          type);


		// triangles
		meshData.triangles[type.SideMaterialIndex].Add(verticesOffset + sideTrianglesLookup[0, 0]);
		meshData.triangles[type.SideMaterialIndex].Add(verticesOffset + sideTrianglesLookup[0, 1]);
		meshData.triangles[type.SideMaterialIndex].Add(verticesOffset + sideTrianglesLookup[0, 2]);

		meshData.triangles[type.SideMaterialIndex].Add(verticesOffset + sideTrianglesLookup[1, 0]);
		meshData.triangles[type.SideMaterialIndex].Add(verticesOffset + sideTrianglesLookup[1, 1]);
		meshData.triangles[type.SideMaterialIndex].Add(verticesOffset + sideTrianglesLookup[1, 2]);
	}

	void AddVertex(Chunk.MeshData meshData, Vector3 position, Vector2 uvs, HexagonType type)
	{
		meshData.vertices.Add(position);
		meshData.uvs.Add(uvs);
		//meshData.normals.Add(new Vector3(0, 1, 0));
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
	
	static readonly float[,] positionsLookup = new [,]
	{ 
		// Hexagon, this lookup is called one time by layer 
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
	
	static readonly float[,] sideUVsLookup = new [,]
	{ 
		// Hexagon top
		{ 0f, 1f}, { 1f, 1f}, 
		{ 0f, 0f}, { 1f, 0f}
	};
	
	static readonly int[,] sideTrianglesLookup = new [,]
	{ 
		// Sides
		{ 0, 2, 1},
		{ 1, 2, 3}
	};
	
	#endregion
}
