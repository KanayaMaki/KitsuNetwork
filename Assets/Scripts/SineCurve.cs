using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SineCurve : MonoBehaviour
{
    [SerializeField]
    private Vector3 m_pos;
    [SerializeField]
    private Vector3 m_scale;

	/// <summary>
	/// 変更パラメータ
	/// </summary>
	[Tooltip("何秒で往復するか")]
	public float m_second = 1.0f;
    public float m_width = 5.0f;
    public float m_scaler =1.0f;

	//経過時間
	float m_takeTime = 0.0f;

    // Use this for initialization
    void Start()
    {
        m_pos = gameObject.GetComponent<Transform>().position;
        m_scale = gameObject.GetComponent<Transform>().localScale;
    }

    // Update is called once per frame
    void Update()
    {
		//時間を経過させる
		m_takeTime += Time.deltaTime;

        m_scale = new Vector3(m_width + (Mathf.Sin(m_takeTime * (2 * Mathf.PI) / m_second)/ m_scaler), m_scale.y, m_scale.z);

        gameObject.GetComponent<Transform>().localScale = m_scale;
    }
}
