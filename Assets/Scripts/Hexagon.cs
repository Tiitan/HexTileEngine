using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[Serializable]
public class Hexagon
{
	public const float Lentgh = 10.0f;
	public const float Width = 10.0f;

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
	                       Hexagon[] neighbours, HexagonTypeData types)
	{
		//TODO: compute top vertex position of this hexagon at start to avoid multiple recomputation.

		// Compute hexagon position.
		Vector3 coordinateOffset = HexagonUtils.ConvertHexaSpaceToOrthonormal(coordinate) + chunkOffSet;

		GenerateTop(ref meshData, coordinateOffset, types, neighbours);
		for (int i = 0; i < 6; i++)
		{
			if (IsSideVisible(neighbours[i], types))
				GenerateSide(ref meshData, coordinateOffset, i, neighbours, types);
		}
	}

	#endregion

	#region Private methods

	void GenerateTop(ref Chunk.MeshData meshData, Vector3 coordinateOffset, HexagonTypeData types, Hexagon[] neighbours)
	{
		// Get referential vertice.
		int verticesOffset = meshData.vertices.Count;

		for (int i = 0; i < positionsLookup.GetLength(0); i++)
		{
			Vector3 vertexRelativePosition;
			if (i != 0 && types[_typeID].SizeMultiplier < 1 - HexagonUtils.FloatEpsilon)
				vertexRelativePosition = ComputeVertexRelativePosition(types, neighbours, i - 1, Height);
			else
				vertexRelativePosition = new Vector3(positionsLookup[i, 0], Height, positionsLookup[i, 1]);

			AddVertex(ref meshData,
			          new Vector3(vertexRelativePosition.x * Width + coordinateOffset.x, 
			            		  vertexRelativePosition.y + coordinateOffset.y, 
			            		  vertexRelativePosition.z * Lentgh + coordinateOffset.z),
			          new Vector2(topUVsLookup[i, 0], topUVsLookup[i, 1]));
		}
		
		// triangles
		for (int i = 0; i < topTrianglesLookup.GetLength(0); i++)
		{
			meshData.triangles[types[_typeID].TopMaterialIndex].Add(verticesOffset + topTrianglesLookup[i, 0]);
			meshData.triangles[types[_typeID].TopMaterialIndex].Add(verticesOffset + topTrianglesLookup[i, 1]);
			meshData.triangles[types[_typeID].TopMaterialIndex].Add(verticesOffset + topTrianglesLookup[i, 2]);
		}
	}

	void GenerateSide(ref Chunk.MeshData meshData, Vector3 coordinateOffset, 
	                  int faceIndex, Hexagon[] neighbours, HexagonTypeData types)
	{

		float bottomEdgeHeight = Height - types[_typeID].EdgeHeight;
		if (neighbours[faceIndex] != null && bottomEdgeHeight < neighbours[faceIndex].Height)
		{
			// If the bottomEdge go lower than the neighbour hexagon, make them match.
			//bottomEdgeHeight = neighbours[faceIndex].Height;
		}

		// Make Edge
		if (types[_typeID].EdgeHeight > HexagonUtils.FloatEpsilon  ||
		    types[_typeID].SizeMultiplier < 1 - HexagonUtils.FloatEpsilon)
		{
			Vector3[] topEdgePosition = ComputeEdgeRelativePosition(types, neighbours, faceIndex, Height);
			Vector3[] bottomEdgePosition = ComputeEdgeRelativePositionBase(faceIndex, bottomEdgeHeight);
			AddSideQuad(ref meshData, coordinateOffset, topEdgePosition, bottomEdgePosition,
			            types[_typeID].EdgeMaterialIndex, 0f);
		}

		// Make Side
		float baseLevel = neighbours[faceIndex] == null ? 
			-10 : neighbours[faceIndex].Height - types[neighbours[faceIndex].TypeID].EdgeHeight;
		Vector3[] topSidePosition = ComputeEdgeRelativePositionBase(faceIndex, bottomEdgeHeight);
		Vector3[] baseEdgePosition = ComputeEdgeRelativePositionBase(faceIndex, baseLevel);
		// TODO: remove unneeded underneath geometry when a corner don't make the side visible.
		if (baseLevel < bottomEdgeHeight - HexagonUtils.FloatEpsilon)
			AddSideQuad(ref meshData, coordinateOffset, topSidePosition, baseEdgePosition,
			            types[_typeID].SideMaterialIndex,
			            1 - (bottomEdgeHeight - baseLevel) * types[_typeID].SideLoopFrequency);
	}

	Hexagon[] GetNeighboursOfNeighbour(Hexagon[] neighbours, int faceIndex)
	{
		Hexagon[] neighboursOfNeighbour = new Hexagon[6];
		neighboursOfNeighbour[(faceIndex + 1) % 6] = neighbours[(faceIndex + 2) % 6];
		neighboursOfNeighbour[(faceIndex + 5) % 6] = neighbours[(faceIndex + 4) % 6];
		neighboursOfNeighbour[(faceIndex + 3) % 6] = this;
		return neighboursOfNeighbour;
	}

	bool IsSideVisible(Hexagon neighbour, HexagonTypeData types)
	{
		return neighbour == null || 
			   neighbour.Height < this.Height - HexagonUtils.FloatEpsilon;
	}

	Vector3[] ComputeEdgeRelativePositionBase(int faceIndex, float height)
	{
		int vertexIndex = faceIndex;
		Vector3[] vect =  new Vector3[2];
		vect[0] = new Vector3(positionsLookup[vertexIndex + 1, 0], height, positionsLookup[vertexIndex + 1, 1]);
		vertexIndex = (vertexIndex + 1) % 6;
		vect[1] = new Vector3(positionsLookup[vertexIndex + 1, 0], height, positionsLookup[vertexIndex + 1, 1]);
		return vect;
	}

	Vector3 ComputeVertexRelativePosition(HexagonTypeData types, Hexagon[] neighbours, int vertexIndex, float height)
	{
		bool firstNeighbourVisibility = IsSideVisible(neighbours[(vertexIndex + 5) % 6], types);
		bool secondNeighbourVisibility = IsSideVisible(neighbours[vertexIndex], types);

		if (types[_typeID].SizeMultiplier > 1 - HexagonUtils.FloatEpsilon || 
		    (!firstNeighbourVisibility && !secondNeighbourVisibility))
		{
			return new Vector3(positionsLookup[vertexIndex + 1, 0], height, positionsLookup[vertexIndex + 1, 1]);
		}
		else if (firstNeighbourVisibility && secondNeighbourVisibility)
		{
			return new Vector3(positionsLookup[vertexIndex + 1, 0] * types[_typeID].SizeMultiplier, 
			                   height, 
			                   positionsLookup[vertexIndex + 1, 1] * types[_typeID].SizeMultiplier);
		}
		else if (firstNeighbourVisibility && !secondNeighbourVisibility)
		{
			return new Vector3(neighbourRelativePosition[(vertexIndex + 5) % 6, 0] + positionsLookup[(vertexIndex + 2) % 6 + 1, 0] *  (2 - types[_typeID].SizeMultiplier), 
			                   height, 
			                   neighbourRelativePosition[(vertexIndex + 5) % 6, 1] + positionsLookup[(vertexIndex + 2) % 6 + 1, 1] *  (2 - types[_typeID].SizeMultiplier));
		}
		else if (!firstNeighbourVisibility && secondNeighbourVisibility)
		{
			return new Vector3(neighbourRelativePosition[(vertexIndex + 0) % 6, 0] + positionsLookup[(vertexIndex + 4) % 6 + 1, 0] *  (2 - types[_typeID].SizeMultiplier), 
			                   height, 
			                   neighbourRelativePosition[(vertexIndex + 0) % 6, 1] + positionsLookup[(vertexIndex + 4) % 6 + 1, 1] *  (2 - types[_typeID].SizeMultiplier));
		}
		throw new Exception();
	}

	Vector3[] ComputeEdgeRelativePosition(HexagonTypeData types, Hexagon[] neighbours, int faceIndex, float height)
	{
		Vector3[] edge = new Vector3[2];
		edge[0] = ComputeVertexRelativePosition(types, neighbours, faceIndex, height);
		edge[1] = ComputeVertexRelativePosition(types, neighbours, (faceIndex + 1) % 6, height);
		return edge;
	}

	#endregion
	
	#region Private static Function

	static void AddSideQuad(ref Chunk.MeshData meshData, 
	                        Vector3 coordinateOffset, 
	                        Vector3[] topEdgePosition,
	                        Vector3[] bottomEdgePosition,
	                        int triangleIndex, 
	                        float uvBottomPosition)
	{
		int verticesOffset = meshData.vertices.Count;

		Vector3 scale = new Vector3(Width, 1, Lentgh);

		AddVertex(ref meshData,
		          new Vector3(topEdgePosition[0].x * Width + coordinateOffset.x, 
		            topEdgePosition[0].y + coordinateOffset.y, 
		            topEdgePosition[0].z * Lentgh + coordinateOffset.z),
		          new Vector2(0, 1));
		
		AddVertex(ref meshData,
		          new Vector3(topEdgePosition[1].x * Width + coordinateOffset.x, 
		            topEdgePosition[1].y + coordinateOffset.y, 
		            topEdgePosition[1].z * Lentgh + coordinateOffset.z),
		          new Vector2(1, 1));
		
		AddVertex(ref meshData,
		          new Vector3(bottomEdgePosition[0].x * Width + coordinateOffset.x, 
		            bottomEdgePosition[0].y + coordinateOffset.y, 
		            bottomEdgePosition[0].z * Lentgh + coordinateOffset.z),
		          new Vector2(0, uvBottomPosition));
		
		AddVertex(ref meshData,
		          new Vector3(bottomEdgePosition[1].x * Width + coordinateOffset.x, 
		            bottomEdgePosition[1].y + coordinateOffset.y, 
		            bottomEdgePosition[1].z * Lentgh + coordinateOffset.z),
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
