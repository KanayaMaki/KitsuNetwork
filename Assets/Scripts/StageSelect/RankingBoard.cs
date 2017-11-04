using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RankingSystem;
using UnityEngine.UI;

public class RankingBoard : MonoBehaviour {
    private const int ORDER_MAX = 3;

    // 
    private List<RankingData> m_rankingDataList ;
    private FileOperation m_file ;

    [SerializeField]
    private Text[] m_nameList = new Text[ORDER_MAX];
    [SerializeField]
    private ScoreRender[] m_score = new ScoreRender[ORDER_MAX];

    // 初期化ステージ
    public void Init(StageSelectController.Stage _stage)
    {
        m_rankingDataList = new List<RankingData>();
        m_file = new FileOperation();

        m_rankingDataList.Clear();
        m_file.ReadData(m_rankingDataList,(int)_stage);

        SetValue(m_rankingDataList);
    }

    // 値をセット
    void SetValue(List<RankingData> _dataList)
    {
        for (int i = 0; i < ORDER_MAX; ++i)
        {
            m_nameList[i].text = _dataList[i].name[0] + '\n' + _dataList[i].name[1] + '\n' + _dataList[i].name[2];
            m_score[i].SetScore( _dataList[i].score);
        }
    }
}
