using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SceneInfo {
    /// <summary>
    /// シーン名
    /// </summary>
    [SerializeField]
    private string m_name = null;
    public string Name { get { return m_name; } }


    /// <summary>
    /// グループ名
    /// </summary>
    [SerializeField]
    private string m_group = null;
    public string Group { get { return m_group; } }


    /// <summary>
    /// 次のシーン名
    /// </summary>
    [SerializeField]
    private string m_nextSceneName = null;
    public string NextSceneName { get { return m_nextSceneName; } }
}
