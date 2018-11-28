using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(QuestData), true)]
public class QuestBuilderEditor : Editor
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		QuestData myScript = (QuestData)target;
		if (GUILayout.Button("Set Quest id to next available id"))
		{
			myScript.SetQuestId();
		}
	}
}