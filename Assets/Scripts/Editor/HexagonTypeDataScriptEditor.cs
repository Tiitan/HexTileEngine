using UnityEngine;
using UnityEditor;
using System.Collections;
using System;

[CustomEditor(typeof(HexagonTypeData))]
public class HexagonTypeDataScriptEditor : Editor  
{
	HexagonTypeData		TargetData
	{
		get { return target as HexagonTypeData; }
	}

	public override void OnInspectorGUI()
	{
		if (GUILayout.Button("Add new type"))
			TargetData.AddNewElement();

		EditorGUILayout.Space();

		int i = 0;
		foreach (HexagonType hexType in TargetData)
        {
			EditorGUILayout.LabelField("Type ID: " + i);
			EditorGUI.indentLevel++;
			hexType.Name = EditorGUILayout.TextField("Name: ", hexType.Name);
			hexType.TopMaterial = EditorGUILayout.ObjectField("Material: ", hexType.TopMaterial, 
			                                               typeof(Material), false) as Material;
			hexType.TopColor = EditorGUILayout.ColorField("Color: ", hexType.TopColor);
			EditorGUI.indentLevel--;
			i++;
			EditorGUILayout.Space();
		}

		if (GUI.changed)
		{
			EditorUtility.SetDirty(TargetData);
			AssetDatabase.SaveAssets();
		}
	}
}
