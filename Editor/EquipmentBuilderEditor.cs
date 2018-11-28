using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(Equipment), true)]
public class EquipmentBuilderEditor : Editor
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		Item myScript = (Item)target;
		if (GUILayout.Button("Set Item id to next available id"))
		{
			myScript.SetItemId();
		}

		Equipment myScript2 = (Equipment)target;
		if (GUILayout.Button("Randomize Equipment Stats"))
		{
			myScript2.RandomizeStats();
		}
	}
}