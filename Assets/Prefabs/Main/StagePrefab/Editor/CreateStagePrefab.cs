using UnityEngine;
using UnityEditor;
using System.Collections;

public class CreateStagePrefab : EditorWindow
{
	private GameObject parent;
	private GameObject prefab;
	private GameObject topPrefab;
	private float intervalX = 1.0f;
	private float intervalY = 1.0f;

	[MenuItem("GameObject/Create Other/Create StagePrefab")]
	static void Init()
	{
		GetWindow<CreateStagePrefab>(true, "Create StagePrefab");
	}

	void OnEnable()
	{
		if (Selection.gameObjects.Length > 0) parent = Selection.gameObjects[0];
	}

	void OnSelectionChange()
	{
		if (Selection.gameObjects.Length > 0) parent = Selection.gameObjects[0];
		Repaint();
	}

	void OnGUI()
	{
		try
		{
			parent = EditorGUILayout.ObjectField("Parent", parent, typeof(GameObject), true) as GameObject;
			topPrefab = EditorGUILayout.ObjectField("TopPrefab", topPrefab, typeof(GameObject), true) as GameObject;
			prefab = EditorGUILayout.ObjectField("Prefab", prefab, typeof(GameObject), true) as GameObject;

			GUILayout.Label("IntervalX : ", EditorStyles.boldLabel);
			intervalX = float.Parse(EditorGUILayout.TextField(intervalX.ToString()));

			GUILayout.Label("IntervalY : ", EditorStyles.boldLabel);
			intervalY = float.Parse(EditorGUILayout.TextField(intervalY.ToString()));

			GUILayout.Label("", EditorStyles.boldLabel);
			if (GUILayout.Button("Create")) Create();
		}
		catch (System.FormatException) { }
	}

	private void Create()
	{
		if(!prefab)
		{
			Debug.Log("Prefabが設定されていません");
			return;
		}
		if (!topPrefab)
		{
			Debug.Log("Prefabが設定されていません");
			return;
		}

		if(parent.name == "Collider")
		{
			parent = parent.transform.parent.gameObject;
		}


		Vector3 basePos = new Vector3();
		if (parent) basePos = parent.transform.position;


		Vector3 pos = basePos;

		Transform parentTrans = parent.transform.Find("Collider");
		if (!parentTrans)
		{
			Debug.Log("Colliderが存在しません");
			return;
		}
		Vector3 parentScale = parentTrans.localScale;

		int numX = (int)(parentScale.x / intervalX);
		int numY = (int)(parentScale.y / intervalY);

		//ブロックのモデルの削除
		{
			Transform tModels = parent.transform.Find("Models");
			if (tModels)
			{
				Undo.DestroyObjectImmediate(tModels.gameObject);
			}
		}
		
		GameObject models = new GameObject();
		models.name = "Models";
		models.transform.parent = parent.transform;


		GameObject usePrefab = topPrefab;

		for(int y = 0; y < numY; y++)
		{
			//X座標のリセット
			pos.x = basePos.x;

			for (int x = 0; x < numX; x++)
			{
				GameObject obj = PrefabUtility.InstantiatePrefab(usePrefab) as GameObject;
				obj.transform.position = pos;
				if (parent) obj.transform.parent = models.transform;
				Undo.RegisterCreatedObjectUndo(obj, "Create StagePrefab");

				pos.x += intervalX;
			}
			pos.y -= intervalY;
			usePrefab = prefab;
		}
	}
}