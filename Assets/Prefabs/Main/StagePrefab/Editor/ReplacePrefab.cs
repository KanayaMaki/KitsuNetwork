using UnityEngine;
using UnityEditor;
using System.Collections;

public class ReplacePrefab : EditorWindow
{
	private GameObject prefab;

	[MenuItem("GameObject/Create Other/Replace Prefab")]
	static void Init()
	{
		GetWindow<ReplacePrefab>(true, "Replace Prefab");
	}


	void OnGUI()
	{
		try
		{
			prefab = EditorGUILayout.ObjectField("Prefab", prefab, typeof(GameObject), true) as GameObject;

			GUILayout.Label("", EditorStyles.boldLabel);
			if (GUILayout.Button("Replace")) Create();
		}
		catch (System.FormatException) { }
	}

	private void Create()
	{
		if (prefab == null) return;

		foreach(var g in Selection.gameObjects)
		{
			GameObject obj = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
			obj.transform.parent = g.transform.parent;
			obj.transform.position = g.transform.position;
			obj.transform.rotation = g.transform.rotation;
			obj.transform.localScale = g.transform.localScale;
			obj.transform.SetSiblingIndex(g.transform.GetSiblingIndex());
			Undo.RegisterCreatedObjectUndo(obj, "Replace Prefab");

			Undo.DestroyObjectImmediate(g);
		}
	}
}