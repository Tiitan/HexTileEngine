using UnityEngine;
using System;
using System.Collections;

[Serializable]
public class HexagonType
{
	#region Fields

	/// <summary>
	/// parent container.
	/// Used to Trigger MaterialModified.
	/// </summary>
	[SerializeField]
	private HexagonTypeData	_container;

	/// <summary>
	/// Name.
	/// Only used in editor.
	/// </summary>
	[SerializeField]
	private string 		    _name;

	/// <summary>Material of the top part of an hexagon</summary>
	[SerializeField]
	private	Material	 	_topMaterial;

	/// <summary>
	/// Material of the Side part of an hexagon
	/// Can be left null if hexagon is flat.
	/// </summary>
	[SerializeField]
	private	Material 		_sideMaterial;

	/// <summary>
	/// Index in the material array of the chunk for top material.
	/// Initialized by the type container and used at generation to avoid multiple mesh with identical material.
	/// </summary>
	[SerializeField]
	private	int		 		_topMaterialIndex;

	/// <summary>
	/// Index in the material array of the chunk for side material.
	/// Initialized by the type container and used at generation to avoid multiple mesh with identical material.
	/// </summary>
	[SerializeField]
	private	int 			_sideMaterialIndex;

	#endregion

	#region Constructors

	/// <summary>Constructor used by unity's serialization system.</summary>
	public HexagonType() { }

	/// <summary>Constructor used when the asset is first created.</summary>
	/// <param name="container">HexagonTypeData holding this type.</param>
	public HexagonType(HexagonTypeData container) 
	{
		_container = container; 
	}

	#endregion

	#region public Properties

	/// <summary>
	/// Name.
	/// Only used in editor.
	/// </summary>
	public string Name
	{
		get
		{
			return _name;
		}
		set
		{
			_name = value;
		}
	}

	/// <summary>Material of the top part of an hexagon</summary>
	public Material TopMaterial
	{
		get
		{
			return _topMaterial;
		}
		set
		{
			if (_topMaterial != value)
			{
				_topMaterial = value;
				_container.TriggerMaterialModified();
			}
		}
	}

	public Material SideMaterial
	{
		get
		{
			return _sideMaterial;
		}
		set
		{
			if (_sideMaterial != value)
			{
				_sideMaterial = value;
                _container.TriggerMaterialModified();
            }
        }
	}

	/// <summary>
	/// Index in the material array of the chunk for top material.
	/// Initialized by the type container and used at generation to avoid multiple mesh with identical material.
	/// </summary>
	public int TopMaterialIndex
	{
		get
		{
			return _topMaterialIndex;
		}
		set
		{
			_topMaterialIndex = value;
		}
	}

	/// <summary>
	/// Index in the material array of the chunk for side material.
	/// Initialized by the type container and used at generation to avoid multiple mesh with identical material.
	/// </summary>
	public int SideMaterialIndex
	{
		get
		{
			return _sideMaterialIndex;
		}
		set
		{
			_sideMaterialIndex = value;
		}
	}

	#endregion
}
