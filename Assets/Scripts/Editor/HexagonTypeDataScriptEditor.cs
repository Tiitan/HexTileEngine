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
			hexType.EdgeHeight = EditorGUILayout.FloatField("Edge height: ", hexType.EdgeHeight);
			hexType.SizeMultiplier = EditorGUILayout.Slider("Size multiplier: ", hexType.SizeMultiplier, 0, 1);
			hexType.EdgeMaterial = EditorGUILayout.ObjectField("Edge Material: ", hexType.EdgeMaterial, 
			                                                  	typeof(Material), false) as Material;
			hexType.SideLoopFrequency = EditorGUILayout.FloatField("Side loop frequency: ", hexType.SideLoopFrequency);
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
