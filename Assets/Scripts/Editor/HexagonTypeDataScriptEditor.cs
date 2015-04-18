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
			hexType.TopMaterial = EditorGUILayout.ObjectField("Top Material: ", hexType.TopMaterial, 
			                                               		typeof(Material), false) as Material;
			hexType.SideMaterial = EditorGUILayout.ObjectField("Side Material: ", hexType.SideMaterial, 
			                                                  	typeof(Material), false) as Material;
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
