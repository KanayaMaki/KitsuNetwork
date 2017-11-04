using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RankingSystem;
using UnityEngine.UI;

public class StageBoard : MonoBehaviour {

    private const int STAGE_MAX = 4;

    // インスペクター上にボードの内の画像をセットしてください
    // 差し替え用の配列です
    [SerializeField]
    private Sprite[] m_nameList = new Sprite[STAGE_MAX];
    [SerializeField]
    private Sprite[] m_screenList = new Sprite[STAGE_MAX];
    [SerializeField]
    private RankingBoard m_rankingBoard ;

    // 実際反映される画像を格納します
    private Image m_boardName = null;         
    private Image m_boardScreenImage = null;
    //private SpriteRenderer m_boardRankingBoad = null;

    private GameObject m_boardNameObj = null;
    private GameObject m_boardScreenImageObj = null;
    private GameObject m_boardRankingBoadObj = null;


    // 子オブジェクトの番号（Start時の取得に使う）
    [SerializeField]
    private const int Name = 0;
    [SerializeField]
    private const int ScreenImage = 1;
    [SerializeField]
    private const int RankingBoard = 2;
   

    // 初期化（子オブジェクト（ボード内）のスプライト取得）
    void Awake () {
        m_boardNameObj = gameObject.transform.GetChild(Name).gameObject;
        m_boardScreenImageObj = gameObject.transform.GetChild(ScreenImage).gameObject;
        m_boardRankingBoadObj = gameObject.transform.GetChild(RankingBoard).gameObject;

        m_boardName = m_boardNameObj.GetComponent<Image>();
        m_boardScreenImage = m_boardScreenImageObj.GetComponent<Image>();
        //m_boardRankingBoad = m_boardRankingBoadObj.GetComponent<SpriteRenderer>();
	}

    public void Init(StageSelectController.Stage _stage) {
        // ステージによりスプライトを貼り変える
        int num = (int)_stage;
        m_boardName.sprite = m_nameList[num];
        m_boardScreenImage.sprite = m_screenList[num];

        // ランキングデータb読み込み、表示
        m_rankingBoard.Init(_stage);

        // TODO:渡されたステージのランキングデータを取得し、表示させる
        Debug.Log("ステージボードの初期化完了");
    }
}
