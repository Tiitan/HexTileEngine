using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class HexagonTypeData : ScriptableObject, IEnumerable<HexagonType>
{
	public delegate void MaterialModifiedEventHandler(object sender, EventArgs e);

	#region Fields

	private MaterialModifiedEventHandler _materialModified;

	[SerializeField]
	private HexagonType[] _hexagonTypes = new HexagonType[0];

	#endregion

	#region Properties

	/// <summary>
	/// Event triggered when a material is modified.
	/// Allow all chunk to update their renderers.
	/// Protected against mutliple register.
	/// </summary>
	public event MaterialModifiedEventHandler MaterialModified
	{
		add
		{
			if (_materialModified == null || _materialModified.GetInvocationList().Contains(value))
				_materialModified += value;
		}
		remove
		{
			_materialModified -= value;
		}
	}

	#endregion

	#region Array overload

	public HexagonType this[int i]
	{
		get { return _hexagonTypes[i]; }
	}

	public IEnumerator<HexagonType> GetEnumerator()
	{
		foreach (HexagonType type in _hexagonTypes)
			yield return type;
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return this.GetEnumerator();
	}

	public int Count { get { return _hexagonTypes.Length; } }

	#endregion

	#region public Methods

	/// <summary>
	/// Build and return an array containing the Materials of all Types without duplicate.
	/// Corresponding index is set inside hexagonType for future lookup.
	/// TODO: move building to TriggerMaterialModified to optimize and make sure the is no serialization problems.
	/// </summary>
	/// <returns>The names.</returns>
	public Material[] GetMaterials()
	{
		List<Material> materials = new List<Material>(_hexagonTypes.Length * 2);
		int index = 0;
		foreach (HexagonType hexagonType in _hexagonTypes)
		{
			// TODO factorize this mess.
			// Top
			Material currentMaterial = hexagonType.TopMaterial != null ? hexagonType.TopMaterial : DefaultMaterial;
			int containedMaterialIndex = materials.FindIndex(x => x == currentMaterial);
			if (containedMaterialIndex == -1)
			{
				materials.Add(currentMaterial);
				hexagonType.TopMaterialIndex = index;
				index++;
			}
			else
				hexagonType.TopMaterialIndex = containedMaterialIndex;

			// Edge
			currentMaterial = hexagonType.EdgeMaterial != null ? hexagonType.EdgeMaterial : DefaultMaterial;
			containedMaterialIndex = materials.FindIndex(x => x == currentMaterial);
			if (containedMaterialIndex == -1)
			{
				materials.Add(currentMaterial);
				hexagonType.EdgeMaterialIndex = index;
				index++;
			}
			else
				hexagonType.EdgeMaterialIndex = containedMaterialIndex;

			// Side
			currentMaterial = hexagonType.SideMaterial != null ? hexagonType.SideMaterial : DefaultMaterial;
			containedMaterialIndex = materials.FindIndex(x => x == currentMaterial);
			if (containedMaterialIndex == -1)
			{
				materials.Add(currentMaterial);
				hexagonType.SideMaterialIndex = index;
				index++;
			}
			else
				hexagonType.SideMaterialIndex = containedMaterialIndex;
		}
		return materials.ToArray();
	}

	/// <summary>
	/// Build and return an array containing the names of all Types.
	/// </summary>
	/// <returns>The names.</returns>
	public string[] GetNames()
	{
		string[] nameArray = new string[_hexagonTypes.Length];
		int i = 0;
		foreach (HexagonType HexagonType in _hexagonTypes)
		{
			nameArray[i++] = HexagonType.Name;
		}
		return nameArray;
	}

	/// <summary>
	/// Add a new HexagonType to this container.
	/// </summary>
	public void AddNewElement()
	{
		Array.Resize(ref _hexagonTypes, _hexagonTypes.Length + 1);
		_hexagonTypes[_hexagonTypes.Length - 1] = new HexagonType(this);

		TriggerMaterialModified();
		EditorUtility.SetDirty(this);
		AssetDatabase.SaveAssets();
	}

	/// <summary>
	/// trigger MaterialModified when a material is changed to update the chunks renderer.
	/// </summary>
	public void TriggerMaterialModified()
	{
		if (_materialModified != null)
			_materialModified(this, new EventArgs());
	}

	#endregion

	#region Helpers

	static Material _defaultMaterial;
	static Material DefaultMaterial 
	{ 
		get 
		{ 
			if (_defaultMaterial == null)
				_defaultMaterial = AssetDatabase.GetBuiltinExtraResource<Material>("Default-Diffuse.mat");
			return _defaultMaterial;
		} 
	}

	#endregion
}


































