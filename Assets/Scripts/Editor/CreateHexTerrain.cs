using UnityEngine;
using UnityEditor;
using System.Collections;

public class CreateHexTerrain : EditorWindow 
{
	int				_size = 8;
	string			_terrainName = "HexTerrainData";
	HexTerrainData  _terrainData;
	string			_typeName = "HexTypeData";
	HexagonTypeData	_typeData;
	string 			_gameObjectName = "HexTerrain";
	
	[MenuItem ("Window/Create Hexagonal terrain")]
	static void Init () 
	{
		// Get existing open window or if none, make a new one:
		CreateHexTerrain window = (CreateHexTerrain)EditorWindow.GetWindow (typeof (CreateHexTerrain));
		window.Show();
	}

	void OnGUI()
	{
		// Create HexTerrainData
		EditorGUILayout.LabelField("Terrain data");
		EditorGUI.indentLevel++;
		_terrainData = EditorGUILayout.ObjectField("object:", _terrainData, typeof(HexTerrainData), false) as HexTerrainData;
		if (_terrainData != null)
		{
			_terrainName = _terrainData.name;
			_size = _terrainData.Size;
		}
		EditorGUILayout.Separator();
		GUI.enabled = _terrainData == null;
		_terrainName = EditorGUILayout.TextField("name: ",_terrainName);
		_size = EditorGUILayout.IntField("Size: ", _size);
		if (GUILayout.Button("Create Terrain data"))
			_terrainData = MakeHexTerrainData(_terrainName, _size);
		GUI.enabled = true;
		EditorGUI.indentLevel--;

		EditorGUILayout.Space();

		// Create HexagonTypeData
		EditorGUILayout.LabelField("Type data");
		EditorGUI.indentLevel++;
		_typeData = EditorGUILayout.ObjectField("object:", _typeData, typeof(HexagonTypeData), false) as HexagonTypeData;
		if (_typeData != null)
			_typeName = _typeData.name;
		EditorGUILayout.Separator();
		GUI.enabled = _typeData == null;
		_typeName = EditorGUILayout.TextField("name: ", _typeName);
		if (GUILayout.Button("Create Type data"))
			_typeData = MakeHexType(_typeName);
		GUI.enabled = true;
		EditorGUI.indentLevel--;

		EditorGUILayout.Space();

		// Create Map
		_gameObjectName = EditorGUILayout.TextField("new gameObject name: ", _gameObjectName);
		GUI.enabled = _typeData != null && _terrainData != null;
		if (GUILayout.Button("Instantiate gameObject"))
		{
			if (MakeHexTerrain(_gameObjectName, _terrainData, _typeData))
				this.Close ();
		}
		GUI.enabled = true;
	}

	#region Private static functions

	static HexTerrainData MakeHexTerrainData(string name, int size)
	{
		HexTerrainData terrainData = ScriptableObject.CreateInstance<HexTerrainData>();
		terrainData.Initialize(size);
		AssetDatabase.CreateAsset(terrainData, AssetDatabase.GenerateUniqueAssetPath("Assets/" + name + ".asset"));
		EditorUtility.FocusProjectWindow();
		Selection.activeObject = terrainData;
		AssetDatabase.SaveAssets();
		return terrainData;
	}

	static HexagonTypeData MakeHexType(string name)
	{
		HexagonTypeData types = ScriptableObject.CreateInstance<HexagonTypeData>();
		AssetDatabase.CreateAsset(types, AssetDatabase.GenerateUniqueAssetPath("Assets/" + name + ".asset"));
		EditorUtility.FocusProjectWindow();
		Selection.activeObject = types;
		AssetDatabase.SaveAssets();
		return types;
	}

	static bool MakeHexTerrain(string name, HexTerrainData terrainData, HexagonTypeData typeData)
	{
		GameObject gameObject = new GameObject(name);
		HexTerrain hexTerrain = gameObject.AddComponent<HexTerrain>();
		hexTerrain.HexData = terrainData;
		hexTerrain.Types = typeData;
		hexTerrain.BuildChunks();
		return true;
	}

	#endregion
}
