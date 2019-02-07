using UnityEngine;
using System.Collections;
using UnityEditor;
using System;

[CustomEditor(typeof(HexTerrain))]
public class HexTerrainScriptEditor : Editor 
{
	const int 		editMapButtonID = 0;
	
	Vector3			_lastMousePosition;
	
	bool 			_showMaterialsActive;

	int 			_typeIdBrush;
	float 			_heightBrush;
	
	bool 			_isEditModeEnabled;

	bool 			_isDragging;

	PaintLayer		_paintLayer = PaintLayer.All;

	HexTerrain		TargetMap
	{
		get { return target as HexTerrain; }
	}
	
	/// <summary>
	/// property defining if the edit mode is enabled
	/// A button in OnSceneGUI() set it
	/// </summary>
	/// <value>true if this instance is edit mode enabled; otherwise, false.</value>
	bool IsEditModeEnabled
	{
		get { return _isEditModeEnabled; }
		set
		{
			_isEditModeEnabled = value;
			Repaint();
		}
	}

	string EditModeButtonString
	{
		get 
		{
			return _isEditModeEnabled ? "Edit mode enable " : "Edit mode disable ";
		}
	}

	void OnSceneGUI()
	{
		Event e = Event.current;

		//Debug.Log("type " + e.type + ", ismouse " + e.isMouse + ", button " + e.button);

		// Is edit mode enabled  ?
		if (IsEditModeEnabled)
		{
			// Catch window focus.
			int controlID = GUIUtility.GetControlID(FocusType.Passive);
			if (e.type == EventType.Layout)
			{
				HandleUtility.AddDefaultControl(controlID);
				return;
			}

			// Release mouse
			if (e.type == EventType.MouseUp && e.button == editMapButtonID)
				_isDragging = false;

			// paint.
			if ((e.type == EventType.MouseDrag || e.type == EventType.MouseDown) && e.button == editMapButtonID)
			{
				float rayDistance;
				Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
				Plane plane = new Plane(new Vector3(0, 1, 0), 0);
				if (plane.Raycast(ray, out rayDistance))
				{
					Vector3 point = ray.GetPoint(rayDistance);

					//Debug.Log("type " + e.type + ", ismouse " + e.isMouse + ", button " + e.button);

					if (e.type == EventType.MouseDrag && _isDragging)
					{
						if (TargetMap.EditHexagon (_lastMousePosition, point, _typeIdBrush, _heightBrush, _paintLayer))
							MapModified();
					}
					else
					{
						if (TargetMap.EditHexagon (point, _typeIdBrush, _heightBrush, _paintLayer))
							MapModified();
						_isDragging = true;
					}

					_lastMousePosition = point;
				}
				e.Use();
			}
		}
	}

	private void MapModified()
	{
		TargetMap.RebuildDirtyChunks();
		EditorUtility.SetDirty(TargetMap.HexData);
		//AssetDatabase.SaveAssets();
	}
	
	public override void OnInspectorGUI() 
	{


		GUILayout.Space(15.0f);

		GUI.enabled = TargetMap.IsValid;
		_paintLayer = (PaintLayer)EditorGUILayout.MaskField ("Painted layer: ", (int)_paintLayer, new[] {"Type", "Height"});

		// HexaType picker
		GUI.enabled = TargetMap.IsValid && _paintLayer.Contain(PaintLayer.Type);
		_typeIdBrush = EditorGUILayout.Popup("Brush type: ", _typeIdBrush, GUI.enabled ? TargetMap.Types.GetNames() : new[] {""} );

		// Height picker
		GUI.enabled = TargetMap.IsValid && _paintLayer.Contain(PaintLayer.Height);
		_heightBrush = EditorGUILayout.Slider("Brush height: ", _heightBrush, 0, 20);

		// Space
		GUILayout.Space(5.0f);

		// Enable edit mode buton
		GUI.enabled = TargetMap.IsValid;
		GUI.color = IsEditModeEnabled ? Color.green : Color.yellow;
		if (GUILayout.Button(EditModeButtonString))
			IsEditModeEnabled = !IsEditModeEnabled;
		GUI.color = Color.white;
		GUILayout.Label(IsEditModeEnabled ? "Edit mode hide scene view handlers!" : "");

		// Rebuild button.
		if (GUILayout.Button("Force rebuild"))
			TargetMap.ForceRebuild();

		GUILayout.Space(15.0f);

		// Set Data
		GUI.enabled = true;
		TargetMap.HexData = EditorGUILayout.ObjectField("HexTerrain data:", TargetMap.HexData, 
		                                                typeof(HexTerrainData), false) as HexTerrainData;
		TargetMap.Types = EditorGUILayout.ObjectField("Types data:", TargetMap.Types, 
		                                              typeof(HexagonTypeData), false) as HexagonTypeData;
	} 
}

// Materials
/*
		_showMaterialsActive = EditorGUILayout.Foldout(_showMaterialsActive, "Materials");
		if(_showMaterialsActive) 
		{
			EditorGUI.indentLevel++;
			EditorGUI.BeginChangeCheck();
			int sizeMaterials = EditorGUILayout.IntField("Size:", TargetMap.Materials.Length);
			Material[] newMaterial = new Material[sizeMaterials];
			for(int i = 0; i < sizeMaterials && i < TargetMap.Materials.Length; i++)
				newMaterial[i] = EditorGUILayout.ObjectField("Element " + i, TargetMap.Materials[i], 
				                                             typeof(Material), false) as Material;
			if(EditorGUI.EndChangeCheck())
				TargetMap.Materials = newMaterial;

			EditorGUI.indentLevel--;
		}
*/

// Save button
//if (GUILayout.Button("Save"))
//	TargetMap.Save();
// Reload button
//if (GUILayout.Button("Reload") && EditorUtility.DisplayDialog("Confirmation", "Are you sure ? you will loose any unsaved change", "Yes"))
//	TargetMap.Reload();