using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : SingletonMonoBehaviour<SceneController>
{

	private SceneInfoTable m_sceneInfoTable = null;

	// Add:弓達 前のシーン保持用
	private static int m_beforeStageNum;
	public int BeforeStage
	{
		get { return m_beforeStageNum; }
		set { m_beforeStageNum = value; }
	}

	/// <summary>
	/// 初期化
	/// </summary>
	private void Awake()
	{
		if (this != Instance)
		{
			Destroy(this);
			Debug.Log("自分自身を破棄");
			return;
		}

		DontDestroyOnLoad(this.gameObject);

		// アセットから取得
		m_sceneInfoTable = SceneInfoTable.LoadAsset();

	}

	/// <summary>
	/// シーンの読み込み
	/// </summary>
	/// <param name="_name">シーン名</param>
	public void LoadScene(string _name)
	{
		if (_name == "RoomScene")
			GameObject.Find("Cherry_Blossom_front").GetComponent<leaf_drop>().Drop();
		StartCoroutine(LoadSceneProcess(_name));
	}

	private IEnumerator LoadSceneProcess(string _name)
	{
		yield return new WaitForSeconds(1.5f);//追加
		var sceneInfo = m_sceneInfoTable.GetSceneInfo(_name);
		if (sceneInfo == null)
		{
			Debug.AssertFormat(false, "引数で指定した名前のシーン情報はありません", _name);
			yield break;
		}
		Scene nowScene = SceneManager.GetSceneByName(_name);
		if (nowScene.IsValid())
		{
			Debug.LogWarningFormat("読み込み終わったシーンです");
			yield break;
		}

		// 同じグループのシーン取得
		Scene sceneSameGroup = GetSceneSameGroupInHerarchy(sceneInfo);
		// シーンを読み込み、完了後アクティブになる

		if (sceneSameGroup.IsValid())
		{
			// シーンを破棄する
			// yield return UnLoadScene(sceneSameGroup);
			//yield return SceneManager.UnloadSceneAsync(sceneSameGroup);
			yield return SceneManager.LoadSceneAsync(sceneInfo.Name, LoadSceneMode.Single);
		}
		else
		{

			yield return SceneManager.LoadSceneAsync(sceneInfo.Name, LoadSceneMode.Additive);
		}

	}

	/// <summary>
	/// 破棄するシーンに子がいればそれを破棄し終わってから親を破棄する
	/// </summary>
	/// <param name="_scene"></param>
	/// <returns></returns>
	private IEnumerator UnLoadScene(Scene _scene)
	{
		if (!_scene.IsValid())
		{
			throw new System.ArgumentException("_scene");
		}

		yield return SceneManager.UnloadSceneAsync(_scene.name);
	}

	/// <summary>
	/// Herarchy上に存在する同じグループのシーンを取得
	/// </summary>
	/// <param name="_sceneInfo"></param>
	/// <returns></returns>
	private Scene GetSceneSameGroupInHerarchy(SceneInfo _sceneInfo)
	{
		if (_sceneInfo == null)
		{
			throw new System.ArgumentNullException("_sceneInfo");
		}

		// グループに属さないシーン
		if (string.IsNullOrEmpty(_sceneInfo.Group))
		{
			return new Scene();
		}

		// 読み込まれてるシーンの数ループ
		for (int i = 0; i < SceneManager.sceneCount; ++i)
		{
			Scene scene = SceneManager.GetSceneAt(i);
			// 同じシーンは処理しない
			if (scene.name == _sceneInfo.Name)
			{
				continue;
			}

			SceneInfo sceneInfo = m_sceneInfoTable.GetSceneInfo(scene.name);

			if (sceneInfo == null)
			{
				throw new System.ArgumentNullException("sceneInfo = " + i.ToString());
			}
			// Herarchy上に同じグループのシーン
			if (sceneInfo.Group == _sceneInfo.Group)
			{
				return scene;
			}
		}
		return new Scene();
	}

	public string GetActiveNextSceneName(string _name)
	{
		return m_sceneInfoTable.GetSceneInfo(_name).NextSceneName;
	}

	public int GetActiveScene()
	{
		return SceneManager.GetActiveScene().buildIndex;
	}

	public int GetStageScene()
	{
		switch (GetActiveSceneName())
		{
			case "tutorial":
				return 0;
			case "Stage_1":
				return 1;
			case "Stage_2":
				return 2;
			case "Stage_3":
				return 3;
		}
		return -1;
	}


	public string GetActiveSceneName()
	{
		return SceneManager.GetActiveScene().name;
	}
}
