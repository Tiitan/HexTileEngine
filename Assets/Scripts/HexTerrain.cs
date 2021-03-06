﻿using UnityEngine;
using System.Collections.Generic;
using System;

[ExecuteInEditMode]
public class HexTerrain : MonoBehaviour 
{
	const int chunkLength = 16;
	const int chunkWidth = 16;

	/*
	[SerializeField]
	Material[] 		_materials;
	*/

	[SerializeField]
	private HexTerrainData 	_hexData;

	[SerializeField]
    private Chunk[] 		_chunks;

	[SerializeField]
    private HexagonTypeData _types;

    static HexTerrain       _instance;

	#region Properties

    public static HexTerrain Instance
    {
        get { return _instance; }
    }

    /*
	public Material[] Materials 
	{ 
		get 
		{ 
			if (_materials == null)
				_materials = new Material[1];
			return _materials; 
		} 
		set 
		{ 
			_materials = value;
			if (_materials != null)
			{
				foreach (Chunk chunk in _chunks)
					chunk.GetComponent<MeshRenderer>().sharedMaterials = _materials;
			}
			else
			{
				foreach (Chunk chunk in _chunks)
					chunk.GetComponent<MeshRenderer>().materials = null;
			}
		}
	}
	*/

    public HexTerrainData HexData 
	{ 
		get { return _hexData; } 
		set { _hexData = value; }
	}

	public HexagonTypeData Types 
	{ 
		get { return _types; } 
		set { _types = value; }
	}

	public bool IsValid
	{
		get 
		{ 
			return HexData != null && 
				   _types != null &&
				   HexData.IsInitialized && 
				   _chunks != null;
		}
	}

	public int Length { get { return HexData.Length; } }
	public int Width  { get { return HexData.Width;  } }
	public int ChunkRow { get { return Width / chunkWidth + 1;   } }
	public int ChunkCol { get { return Length / chunkLength + 1; } }

	#endregion

	#region Unity events

    void OnEnable()
    {
        _instance = this;
    }

	void OnDestroy()
	{
		Clean();
	}

	#endregion

	#region Private Methods

	Chunk MakeNewChunk(Vector3 chunkWorldOffset, Vector2i chunkGridOffset)
	{
		GameObject chunkGameObject = new GameObject("Chunk [" + chunkGridOffset.x + "," + chunkGridOffset.y + "]");
		Chunk chunk = chunkGameObject.AddComponent<Chunk>();
		//chunkGameObject.hideFlags = HideFlags.;
		chunkGameObject.transform.SetParent(transform);
		chunkGameObject.GetComponent<MeshRenderer>().sharedMaterials = _types.GetMaterials();
		chunkGameObject.transform.position = chunkWorldOffset;
		//chunkGameObject.isStatic = true; FIXME
		chunk.BindMap(this, chunkGridOffset, new Vector2i(chunkWidth, chunkLength));
        chunk.ForceRebuild();
        return chunk;
    }

	bool EditHexagon(Vector2i gridCoordinate, int typeID, float height, PaintLayer paintLayer)
	{
		//Debug.Log("gridCoordinate: " + gridCoordinate);
		bool isDirty = false;

		if (HexData.Contains(gridCoordinate))
		{
			if (paintLayer.Contain(PaintLayer.Type) && HexData[gridCoordinate].TypeID != typeID)
			{
				HexData[gridCoordinate].TypeID = typeID;
				isDirty |= true;

	        }
			if (paintLayer.Contain(PaintLayer.Height) && HexData[gridCoordinate].Height != height)
			{
				HexData[gridCoordinate].Height = height;
				isDirty |= true;
			}
			_chunks[gridCoordinate.y / chunkLength * ChunkRow + gridCoordinate.x / chunkWidth].IsDirty |= isDirty;
		}
		return isDirty;
    }

	void Clean()
	{
		if (_chunks != null)
		{
			for (int i = 0; i < _chunks.Length; i++)
				DestroyImmediate(_chunks[i].gameObject);
		}
	}

	#endregion

    #region Public methods

	public void BuildChunks()
	{
		Debug.Log("Build chunk array");

		// Create Chunk array
		_chunks = new Chunk[ChunkCol * ChunkRow];
		for (int i = 0; i < ChunkCol; i++)
			for (int j = 0; j < ChunkRow; j++)
				_chunks[i * ChunkRow + j] = MakeNewChunk(HexagonUtils.ConvertHexaSpaceToOrthonormal(new Vector2i(j * chunkWidth, i * chunkLength)), 
				                             new Vector2i(j * chunkWidth, i * chunkLength));
	}

	public bool EditHexagon(Vector3 worldCoordinate, int typeID, float height, PaintLayer paintLayer)
	{
		Vector2i gridCoordinate = HexagonUtils.ConvertOrthonormalToHexaSpace(worldCoordinate);
		return EditHexagon(gridCoordinate, typeID, height, paintLayer);
	}

	public bool EditHexagon(Vector3 initialWorldCoordinate, Vector3 endWorldCoordinate,
	                        int typeID, float height, PaintLayer paintLayer)
	{
		bool 	 isDirty = false;
		Vector2i initialGridCoordinate = HexagonUtils.ConvertOrthonormalToHexaSpace(initialWorldCoordinate);
		Vector2i endGridCoordinate = HexagonUtils.ConvertOrthonormalToHexaSpace(endWorldCoordinate);

		IEnumerable<Vector2i> line = HexagonUtils.GetLine(initialGridCoordinate, endGridCoordinate);
		foreach (Vector2i gridCoordinate in line)
			isDirty |= EditHexagon(gridCoordinate, typeID, height, paintLayer);
		return isDirty;
    }
    
	public void ForceRebuild()
	{
		if (_chunks == null)
			BuildChunks();
		else
		{
			foreach (Chunk chunk in _chunks)
				chunk.ForceRebuild();
		}
	}

    public void RebuildDirtyChunks()
	{
		if (_chunks == null)
			BuildChunks();
		else
		{
			foreach (Chunk chunk in _chunks)
				chunk.RebuildIfDirty();
		}
	}

	public bool Contains(Vector2i location)
	{
		return HexData.Contains(location);
	}

    public Vector2i? RegisterPawn(PawnControler pawnControler)
    {
        Vector2i gridCoordinate = HexagonUtils.ConvertOrthonormalToHexaSpace(pawnControler.transform.position);
        if (HexData.Contains(gridCoordinate))
        {
            HexData[gridCoordinate].LocalPawns.Add(pawnControler);
            return gridCoordinate;
        }

        return null;
    }

    #endregion
}


/*
	 * Now using scriptableObject
	 * 

    void Serialize()
	{
		try
		{
			FileStream fs = new FileStream(_mapFilePath, FileMode.Create);
			try 
			{
				BinaryFormatter formatter = new BinaryFormatter();
				formatter.Serialize(fs, Data.Hexagons);
			}
			catch (SerializationException e) 
			{
				Debug.LogError("Failed to serialize. Reason: " + e.Message);
			}
			finally 
			{
				fs.Close();
			}
		}
		catch (Exception e) 
		{
			Debug.LogError(e.Message);
        }
		Debug.Log("Map saved in " + _mapFilePath + ".");
    }
	
	void Deserialize()
	{
		try
        {
			FileStream fs = new FileStream(_mapFilePath, FileMode.Open);
			try 
			{
				BinaryFormatter formatter = new BinaryFormatter();
				Data.Hexagons = (Hexagon[,]) formatter.Deserialize(fs);
			}
			catch (Exception e) 
	        {
	            Debug.LogError("Failed to deserialize. Reason: " + e.Message);
	        }
	        finally 
	        {
	            fs.Close();
	        }
		}
		catch (Exception e) 
		{
			Debug.LogError(e.Message);
        }
		Debug.Log(_mapFilePath + " loaded.");
    }

	string _mapFilePath = "Assets/test.hex";
	public string MapFilePath
	{
		get { return _mapFilePath; }
		set { _mapFilePath = value; }
	}
    */