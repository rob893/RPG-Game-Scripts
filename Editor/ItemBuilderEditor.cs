using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(Item), true)]
public class ItemBuilderEditor : Editor
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		Item myScript = (Item)target;
		if (GUILayout.Button("Set Item id to next available id"))
		{
			myScript.SetItemId();
		}
	}
}