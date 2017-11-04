using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kitsune2DParent : MonoBehaviour {
    public float m_angleSpeed = 1.0f;
    public float m_radius = 10.0f;
    public Vector3 m_pivot = new Vector3(0, 0, 25);

    private GameObject[] m_kitsune2D = new GameObject[3];
    /// <summary>
    /// 初期化
    /// </summary>
	void Start () {
        // プレハブ読み込み
        var prefab = Resources.Load<GameObject>("Prefab/TitleScene/Kitsune2D");
        if(prefab == null)
        {
            throw new System.MissingFieldException("キツネ２Dのプレハブ読み込み失敗");
        }

        var m_kitsuneInitPos = new Vector3[3];
        var m_kitsuneInitQuaternion = new Quaternion[3];

        // 座標と角度をセット
        for (int i = 0; i < 3; ++i)
        {
            // 角度
            var angle = i * 120.0f ;
            var rad = angle * Mathf.Deg2Rad;
            m_kitsuneInitPos[i] = new Vector3(m_radius * Mathf.Sin(rad), m_radius * Mathf.Cos(rad));
            m_kitsuneInitQuaternion[i] = Quaternion.Euler(0, 0, -angle);
        }

        // インスタンス生成
        for(int i = 0; i < 3; ++i)
        {
            m_kitsune2D[i] = Instantiate(prefab, m_kitsuneInitPos[i], m_kitsuneInitQuaternion[i]);
            m_kitsune2D[i].transform.parent = transform;
        }
	}
	
	// Update is called once per frame
	void Update () {
        transform.position = m_pivot;
        transform.RotateAround(m_pivot, Vector3.back, Time.deltaTime * m_angleSpeed);
	}
}
