using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEditor;
using System.Linq;

// Asset/Createから作成できる
[CreateAssetMenu(menuName = "ScriptableObject/Create SceneInfoList", fileName = "SceneInfoList", order = 0)]

// ScriptableObjectの説明
// アセットとして保持するデータ群
// このアセットを参照してデータを取得（シーン上に置かなくてよい）
public class SceneInfoTable : ScriptableObject
{
    [SerializeField]
    private SceneInfo[] m_sceneInfoList = null;

    private static readonly string ASSETS_PATH = "ScriptableObject/SceneInfoList";

    /// <summary>
    /// シーン情報テーブルのアセット読み込み、取得
    /// </summary>
    /// <returns></returns>
    public static SceneInfoTable LoadAsset()
    {
        // using UnityEditor; を宣言すると
        // "Error building Player because scripts have compile errors in the editor"が出る
        // Editorフォルダー内に置かないといけない？
        //var asset =AssetDatabase.LoadAssetAtPath<SceneInfoTable>(ASSETS_PATH);

        var asset = Resources.Load(ASSETS_PATH) as SceneInfoTable;

        if (asset == null)
        {
            // シーン情報テーブルのアセットがありません
            throw new System.IO.FileNotFoundException(ASSETS_PATH);
        }
        return asset;
    }
    /// <summary>
    /// シーン情報取得
    /// </summary>
    /// <param name="_name">シーン名</param>
    /// <returns>シーン情報</returns>
    public SceneInfo GetSceneInfo(string _name)
    {
        if (string.IsNullOrEmpty(_name))
        {
            // 引数が空です
            throw new System.ArgumentNullException("_name");
        }

        if (m_sceneInfoList == null || m_sceneInfoList.Length == 0)
        {
            // シーン情報が空です
            Debug.AssertFormat(false, "m_sceneInfoにデータがありません");
            return null;
        }

        // 引数と同じシーンネームを渡す
        return m_sceneInfoList.FirstOrDefault(info => info.Name == _name);
    }

}
