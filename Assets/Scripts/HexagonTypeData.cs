using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class HexagonTypeData : ScriptableObject, IEnumerable<HexagonType>
{
	public delegate void MaterialModifiedEventHandler(object sender, EventArgs e);

	private MaterialModifiedEventHandler _materialModified;
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

	[SerializeField]
	private HexagonType[] _hexagonTypes = new HexagonType[0];

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

	public Material[] GetMaterials()
	{
		Material[] materialArray = new Material[_hexagonTypes.Length];
		int i = 0;
		foreach (HexagonType HexagonType in _hexagonTypes)
		{
			materialArray[i++] = HexagonType.TopMaterial;
		}
		return materialArray;
	}

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

}
