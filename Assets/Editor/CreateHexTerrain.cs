using UnityEngine;
using UnityEditor;
using System.Collections;

public class CreateHexTerrain : EditorWindow 
{
	int		_size = 8;
	string	_name = "HexMap";

	// Add menu named "My Window" to the Window menu
	[MenuItem ("Window/Create Hexagonal terrain")]
	static void Init () 
	{
		// Get existing open window or if none, make a new one:
		CreateHexTerrain window = (CreateHexTerrain)EditorWindow.GetWindow (typeof (CreateHexTerrain));
		window.Show();
	}

	void OnGUI()
	{
		_name = EditorGUILayout.TextArea(_name);
		_size = EditorGUILayout.IntField("Size: ", _size);

		EditorGUILayout.Space();


		if (GUILayout.Button("Create new map"))
	    {
			HexTerrainData terrainData = MakeHexTerrainData();
			HexTerrain hexTerrain = MakeHexTerrain();

			// Initialize hexTerrain
			hexTerrain.HexData = terrainData;
			hexTerrain.BuildChunks();
		}

		EditorGUILayout.Space();

		if (GUILayout.Button("Create data only"))
		{
			MakeHexTerrainData();
		}
		if (GUILayout.Button("Create gameObject only"))
		{
			MakeHexTerrain();
		}
	}

	HexTerrainData MakeHexTerrainData()
	{
		// Create the data as ScriptableObject
		HexTerrainData terrainData = ScriptableObject.CreateInstance<HexTerrainData>();
		terrainData.Initialize(_size);
		AssetDatabase.CreateAsset(terrainData, AssetDatabase.GenerateUniqueAssetPath("Assets/" + _name + ".asset"));
		EditorUtility.FocusProjectWindow();
		Selection.activeObject = terrainData;
		AssetDatabase.SaveAssets();

		return terrainData;
	}

	HexTerrain MakeHexTerrain()
	{
		// Create the gameobject
		GameObject gameObject = new GameObject(_name);
		HexTerrain hexTerrain = gameObject.AddComponent<HexTerrain>();
		return hexTerrain;
	}
}
