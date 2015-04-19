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
	private string 		    _name = "Unamed type";

	/// <summary>Material of the top part of an hexagon</summary>
	[SerializeField]
	private	Material	 	_topMaterial;

	/// <summary>
	/// Material of the side part of an hexagon
	/// Can be left null if hexagon is flat.
	/// </summary>
	[SerializeField]
	private	Material 		_sideMaterial;

	/// <summary>
	/// Material of the edge part of an hexagon
	/// Can be left null if hexagon is flat or edge if edge height is 0.
	/// </summary>
	[SerializeField]
	private	Material 		_edgeMaterial;

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

	/// <summary>
	/// Index in the material array of the chunk for edge material.
	/// Initialized by the type container and used at generation to avoid multiple mesh with identical material.
	/// </summary>
	[SerializeField]
	private	int 			_edgeMaterialIndex;

	/// <summary>
	/// The edge height.
	/// the edge is "the top of the side",  this top ring can have its own texture to make a transition 
	/// between the side looping texture and the top.
	/// </summary>
	[SerializeField]
	private	float 			_edgeHeight = 0f;

	/// <summary>
	/// The edge Width.
	/// Horizontal shift fot the edge
	/// </summary>
	[SerializeField]
	private	float 			_sizeMultiplier = 1f;

	/// <summary>
	/// The side distance before the texture loop.
	/// Use 2f for undeformed square texture with the default hexagon size (of 1 Unity unit)
	/// </summary>
	[SerializeField]
	private	float 			_sideLoopFrequency = 2f;

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

	/// <summary>
	/// Material of the edge part of an hexagon
	/// Can be left null if hexagon is flat or edge if edge height is 0.
	/// </summary>
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
	/// Material of the edge part of an hexagon
	/// Can be left null if hexagon is flat or edge if edge height is 0.
	/// </summary>
	public Material EdgeMaterial
	{
		get
		{
			return _edgeMaterial;
		}
		set
		{
			if (_edgeMaterial != value)
			{
				_edgeMaterial = value;
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

	/// <summary>
	/// Index in the material array of the chunk for edge material.
	/// Initialized by the type container and used at generation to avoid multiple mesh with identical material.
	/// </summary>
	public int EdgeMaterialIndex
	{
		get
		{
			return _edgeMaterialIndex;
		}
		set
		{
			_edgeMaterialIndex = value;
		}
	}

	
	/// <summary>
	/// The edge height.
	/// the edge is "the top of the side",  this top ring can have its own texture to make a transition 
	/// between the side looping texture and the top.
	/// </summary>
	public float EdgeHeight
	{
		get
		{
			return _edgeHeight;
		}
		set
		{
			_edgeHeight = value;
		}
	}

	/// <summary>
	/// size of the hexagon inside his allocated space (range 0 => 1).
	/// Horizontal shift fot the edge
	/// </summary>
	public float SizeMultiplier
	{
		get
		{
			return _sizeMultiplier;
		}
		set
		{
			_sizeMultiplier = value;
		}
	}

	/// <summary>
	/// The side distance before the texture loop.
	/// Match hexagon size (default = 1 unity unit) for underformed square texture
	/// </summary>
	public float SideLoopFrequency
	{
		get
		{
			return _sideLoopFrequency;
		}
		set
		{
			_sideLoopFrequency = value;
		}
	}
	#endregion
}
