using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
[RequireComponent (typeof (MeshFilter), typeof (MeshRenderer))]
public class Chunk : MonoBehaviour 
{
	[SerializeField][HideInInspector]
	int 				_height;

	[SerializeField][HideInInspector]
	int 				_width;

	[SerializeField][HideInInspector]
	HexTerrain			_hexTerrain;

	// Referential offset of this chunk inside the HexTerrainData array
	[SerializeField]
	Vector2i			_chunkGridOffset;

	[SerializeField]
	Mesh 				_mesh;

	MeshFilter 			_meshFilter;

	public bool 	IsDirty { get; set; }
    
	// mesh arrays
	public struct MeshData
	{
		public List<Vector3>  	vertices;
		//public List<Vector3>  	normals;
		public List<int>[]  	triangles;
		public List<Color32> 	colors;
		public List<Vector2> 	uvs;

		public void Init(int meshCount)
		{
			vertices = 		new List<Vector3>();
			// normals = 		new List<Vector3>(); 	// now using _mesh.RecalculateNormals
			triangles = 	new List<int>[meshCount];
			for (int i = 0; i < meshCount; i++)
				triangles[i] = 	new List<int>();
			colors = 		new List<Color32>();
			uvs =			new List<Vector2>();
		}
	}

	void Awake () 
	{
	}

	void OnEnable()
	{
		_meshFilter = GetComponent<MeshFilter>();
		
		if (_mesh == null)
		{
			_mesh = new Mesh();
			_mesh.name = "mesh" + _chunkGridOffset;
			_meshFilter.sharedMesh = _mesh;
		}


		// event is bound in BindMap on asset creation and on Awake after serialization.
		if (_hexTerrain != null)
			_hexTerrain.Types.MaterialModified += OnMaterialModified;
	}

	void OnDestroy()
	{
		_hexTerrain.Types.MaterialModified -= OnMaterialModified;

		DestroyImmediate(_mesh);
	}

	void GenerateGeometry()
	{
		//Debug.Log("Generate geometry " + gameObject.name + ".");

		MeshData meshData = new MeshData();
		meshData.Init(_hexTerrain.Types.Count * 2);

		for (int yi = 0; yi < _height; yi++)
		{
			for (int xi = 0; xi < _width; xi++)
			{
				if (_hexTerrain.HexData.Contains(xi + _chunkGridOffset.x, yi + _chunkGridOffset.y))
				{
					if (_hexTerrain.HexData[yi + _chunkGridOffset.y, xi + _chunkGridOffset.x] != null)
					{
						Vector2i coordinate = new Vector2i(xi, yi);
						Hexagon hexa = _hexTerrain.HexData[yi + _chunkGridOffset.y, xi + _chunkGridOffset.x];
						hexa.AddToChunk(ref meshData, Vector3.zero, coordinate, 
							           	HexagonUtils.GetNeighbours(coordinate + _chunkGridOffset, _hexTerrain.HexData),
						                _hexTerrain.Types);
					}
              	}
			}
		}

		// Setup meshFilter.
		_mesh.Clear();
		_mesh.subMeshCount = meshData.triangles.Length;
		_mesh.vertices = meshData.vertices.ToArray();
		//_mesh.normals = _meshData.normals.ToArray();
		_mesh.uv = meshData.uvs.ToArray();
		for (int i = 0; i < meshData.triangles.Length; i++)
			_mesh.SetTriangles(meshData.triangles[i].ToArray(), i);
		_mesh.colors32 = meshData.colors.ToArray();
		_mesh.RecalculateNormals();
		IsDirty = false;
	}

	public void ForceRebuild()
	{
		GenerateGeometry();
	}

	public void RebuildIfDirty()
	{
		if (IsDirty)
			GenerateGeometry();
	}

	public void BindMap(HexTerrain hexTerrain, Vector2i chunkGridOffset, Vector2i size)
	{
		_hexTerrain = hexTerrain;
		_chunkGridOffset = chunkGridOffset;
		_width = size.x;
		_height = size.y;

		// event is bound in BindMap on asset creation and on Awake after serialization.
		_hexTerrain.Types.MaterialModified += OnMaterialModified;
	}

	private void OnMaterialModified(object sender, EventArgs e)
	{
		GetComponent<Renderer>().sharedMaterials = _hexTerrain.Types.GetMaterials();
	}
}


