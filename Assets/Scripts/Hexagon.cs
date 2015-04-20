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
		//TODO: compute top vertex position of this hexagon at start to avoid multiple recomputation.

		// Compute hexagon position.
		Vector3 coordinateOffset = HexagonUtils.ConvertHexaSpaceToOrthonormal(coordinate) + chunkOffSet;

		GenerateTop(ref meshData, coordinateOffset, type, neighbours);
		for (int i = 0; i < 6; i++)
		{
			if (IsSideVisible(neighbours[i]))
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
			Vector3 vertexRelativePosition;
			if (i != 0 && type.SizeMultiplier < 1 - HexagonUtils.FloatEpsilon)
				vertexRelativePosition = ComputeVertexRelativePosition(type.SizeMultiplier, neighbours, i - 1);
			else
				vertexRelativePosition = new Vector3(positionsLookup[i, 0], positionsLookup[i, 0]);

			AddVertex(ref meshData,
			          new Vector3(vertexRelativePosition.x * Width + coordinateOffset.x, 
						          Height + coordinateOffset.y, 
			            		  vertexRelativePosition.z * Lentgh + coordinateOffset.z),
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
		Vector3[] edgePosition;
		if (type.EdgeHeight > HexagonUtils.FloatEpsilon  || type.SizeMultiplier < 1 - HexagonUtils.FloatEpsilon)
		{
			edgePosition = ComputeEdgeRelativePosition(type.SizeMultiplier, neighbours, faceIndex);

			AddSideQuad(ref meshData, coordinateOffset, faceIndex, edgePosition,
			            type.EdgeMaterialIndex, Height, Height - type.EdgeHeight, 0f);
		}

		// Make Side
		edgePosition = ComputeEdgeRelativePosition(1, neighbours, faceIndex);
		float baseLevel = neighbours[faceIndex] == null ? -1 : neighbours[faceIndex].Height;
		if (baseLevel < Height - type.EdgeHeight - HexagonUtils.FloatEpsilon)
			AddSideQuad(ref meshData, coordinateOffset, faceIndex, edgePosition ,
			type.SideMaterialIndex, Height - type.EdgeHeight, baseLevel, 1 - (Height - type.EdgeHeight - baseLevel) * type.SideLoopFrequency);
	}

	bool IsSideVisible(Hexagon neighbour)
	{
		return neighbour == null || neighbour.Height < this.Height - HexagonUtils.FloatEpsilon;
	}

	Vector3 ComputeVertexRelativePosition(float sizeMultiplier, Hexagon[] neighbours, int vertexIndex)
	{
		bool firstNeighbourVisibility = IsSideVisible(neighbours[(vertexIndex + 5) % 6]);
		bool secondNeighbourVisibility = IsSideVisible(neighbours[vertexIndex]);

		if (sizeMultiplier > 1 - HexagonUtils.FloatEpsilon || 
		    (!firstNeighbourVisibility && !secondNeighbourVisibility))
		{
			return new Vector3(positionsLookup[vertexIndex + 1, 0], 0, positionsLookup[vertexIndex + 1, 1]);
		}
		else if (firstNeighbourVisibility && secondNeighbourVisibility)
		{
			return new Vector3(positionsLookup[vertexIndex + 1, 0] * sizeMultiplier, 0, 
			                   positionsLookup[vertexIndex + 1, 1] * sizeMultiplier);
		}
		else if (firstNeighbourVisibility && !secondNeighbourVisibility)
		{
			return new Vector3(neighbourRelativePosition[(vertexIndex + 5) % 6, 0] + positionsLookup[(vertexIndex + 2) % 6 + 1, 0] *  (2 - sizeMultiplier), 0, 
			                   neighbourRelativePosition[(vertexIndex + 5) % 6, 1] + positionsLookup[(vertexIndex + 2) % 6 + 1, 1] *  (2 - sizeMultiplier));
		}
		else if (!firstNeighbourVisibility && secondNeighbourVisibility)
		{
			return new Vector3(neighbourRelativePosition[(vertexIndex + 0) % 6, 0] + positionsLookup[(vertexIndex + 4) % 6 + 1, 0] *  (2 - sizeMultiplier), 0, 
			                   neighbourRelativePosition[(vertexIndex + 0) % 6, 1] + positionsLookup[(vertexIndex + 4) % 6 + 1, 1] *  (2 - sizeMultiplier));
		}
		throw new Exception();
	}

	Vector3[] ComputeEdgeRelativePosition(float sizeMultiplier, Hexagon[] neighbours, int faceIndex)
	{
		Vector3[] edge = new Vector3[2];
		edge[0] = ComputeVertexRelativePosition(sizeMultiplier, neighbours, faceIndex);
		edge[1] = ComputeVertexRelativePosition(sizeMultiplier, neighbours, (faceIndex + 1) % 6);
		return edge;
	}

	#endregion
	
	#region Private static Function

	static void AddSideQuad(ref Chunk.MeshData meshData, Vector3 coordinateOffset, int faceIndex, Vector3[] edgeRelativePosition,
	                        int triangleIndex, float top, float bottom, float uvBottomPosition)
	{
		int verticesOffset = meshData.vertices.Count;

		AddVertex(ref meshData,
		          new Vector3(edgeRelativePosition[0].x * Width + coordinateOffset.x, 
		            top + coordinateOffset.y, 
		            edgeRelativePosition[0].z * Lentgh + coordinateOffset.z),
		          new Vector2(0, 1));
		
		AddVertex(ref meshData,
		          new Vector3(edgeRelativePosition[1].x * Width + coordinateOffset.x, 
		            top + coordinateOffset.y, 
		            edgeRelativePosition[1].z * Lentgh + coordinateOffset.z),
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

	static readonly float[,] neighbourRelativePosition = new [,]
	{ 
		{ -0.5f,  0.75f},
		{  0.5f,  0.75f}, 
		{    1f,  0f   }, 
		{  0.5f, -0.75f}, 
		{ -0.5f, -0.75f},
		{   -1f,  0f   }
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
