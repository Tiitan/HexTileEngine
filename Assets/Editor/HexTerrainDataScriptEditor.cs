using UnityEngine;
using System.Collections;
using UnityEditor;
using System;

[CustomEditor(typeof(HexTerrainData))]
[CanEditMultipleObjects]
public class HexTerrainDataScriptEditor : Editor 
{
	SerializedProperty materialProperty;
	SerializedProperty hexagonsProperty;

	HexTerrainData		TargetData
	{
		get { return target as HexTerrainData; }
	}

	void OnEnable()
	{
		hexagonsProperty = serializedObject.FindProperty("hexagons");
	}
	
	public override void OnInspectorGUI()
	{
		// Materials
		serializedObject.Update();
		if (targets.Length == 1)
		{
			EditorGUILayout.PropertyField(hexagonsProperty, true);
		}

		serializedObject.ApplyModifiedProperties();

		// Display the size of maps
		if (targets.Length == 1)
		{
			EditorGUILayout.LabelField("Size: ", TargetData.Width + " x " + TargetData.Length);
		}
		else
		{
			EditorGUILayout.LabelField("Size:");
			EditorGUI.indentLevel++;
			foreach(HexTerrainData targetData in targets)
			{
				EditorGUILayout.LabelField(targetData.name + ": ", targetData.Width + " x " + targetData.Length);
			}
			EditorGUI.indentLevel--;
		}
	}
}
