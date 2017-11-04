using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// １、アニメーション追加の可能性の高さ = 低い　
/// ２、アニメ―ション処理の複雑さ = 簡易
/// ３、他メンバーが関与する可能性 = かなり低い
/// ４、残り開発日数 = かなり短い
/// 
/// 上記の理由から全てのステートアニメーションを行うクラスとして扱う 
/// </summary>

// 指定したコンポーネントを必ずアタッチする規則
[RequireComponent(typeof(SpriteRenderer))]
public class Fox2DAnimation : MonoBehaviour {
    public enum State
    {
        IDLE,
        RIHGHTWORK,
        LEFTWORK,
        NUM
    }

    private State m_state = State.IDLE;

    
    // それぞれのアニメーションを配列で持つ
    // （多次元配列だとInspector上に表示するのに工夫がいるっぽい）
    [SerializeField]
    private Sprite[] m_animationIdle;
    [SerializeField]
    private Sprite[] m_animetionRightWork;
    [SerializeField]
    private Sprite[] m_animationLeftWork;

    [SerializeField]
    private float m_waitSecond = 2.0f;

    private int m_animCount = 0;


	void Start () {
        StartCoroutine(AnimationExec());
	}
	

    // 右に歩かせるアニメーションを開始
    public void RightWorkAnimationStart()
    {
        m_state = State.RIHGHTWORK;
        m_animCount = 0;
        Debug.Log("キツネ右歩きスタート");
    }

    // 左に歩かせるアニメーションを開始
    public void LeftWorkAnimationStart()
    {
        m_state = State.LEFTWORK;
        m_animCount = 0;
        Debug.Log("キツネ左歩きスタート");

        // 右向きアニメ―ションを回転させて左に見せる
        gameObject.transform.rotation = Quaternion.Euler(0, 180, 0);
    }

    // アニメーションを止め、キツネを正面に向ける
    public void StopAnimation()
    {
        m_state = State.IDLE;
        m_animCount = 0;
    }



    IEnumerator AnimationExec()
    {
        while (true)
        {
            // 一定時間待機
            yield return new WaitForSeconds(m_waitSecond);

            AnimationCount();

            // スプライト切り替え
            GetComponent<SpriteRenderer>().sprite = GetAnimationSprite();
            Debug.Log("アニメーション中");
        }
    }

    // ステートによってカウント処理が別
    void AnimationCount()
    {
        m_animCount++;
        
        // 注意：アニメーションが増えると肥大化してしまう
        int lengh = 0;
        switch (m_state)
        {
            case State.IDLE:
                gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
                lengh = m_animationIdle.Length;
                break;
            case State.RIHGHTWORK:
                lengh = m_animetionRightWork.Length;
                break;
            case State.LEFTWORK:
                lengh = m_animationLeftWork.Length;
                break;
        }
        m_animCount = m_animCount % lengh;
    }

    // ステートによってスプライトを返す
    // 注意：アニメーションが増えると肥大化してしまう
    Sprite GetAnimationSprite()
    {
        switch (m_state)
        {
            case State.IDLE:
                return m_animationIdle[m_animCount];
            case State.RIHGHTWORK:
                return m_animetionRightWork[m_animCount];
            case State.LEFTWORK:
                return m_animationLeftWork[m_animCount];
        }
        return null;
    }
}
