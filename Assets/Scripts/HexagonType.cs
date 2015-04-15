using UnityEngine;
using System;
using System.Collections;

[Serializable]
public class HexagonType
{
	/// <summary>parent container.</summary>
	[SerializeField]
	private HexagonTypeData	_container;

	/// <summary>Name only used in editor.</summary>
	[SerializeField]
	private string 		    _name;

	/// <summary>Material.</summary>
	[SerializeField]
	private	Material	 	_material;

	/// <summary>Color.</summary>
	[SerializeField]
	private	Color	 		_color;

	/// <summary>Constructor used by unity's serialization system.</summary>
	public HexagonType() { }

	/// <summary>Constructor used when the asset is first created.</summary>
	/// <param name="container">HexagonTypeData holding this type.</param>
	public HexagonType(HexagonTypeData container) 
	{
		_container = container; 
	}

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

	public Material TopMaterial
	{
		get
		{
			return _material;
		}
		set
		{
			_material = value;
			_container.TriggerMaterialModified();
		}
	}

	public Color TopColor
	{
		get
		{
			return _color;
		}
		set
		{
			_color = value;
		}
	}
}
