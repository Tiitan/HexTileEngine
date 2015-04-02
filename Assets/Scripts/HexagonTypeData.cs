using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class HexagonTypeData
{
	// <summary>ID of this type, not using array position to allow sorting without rebuilding hexagon data.</summary>
	public	int		typeID;

	/// <summary>Material this type will use
	public	int	 	materialID;
}
