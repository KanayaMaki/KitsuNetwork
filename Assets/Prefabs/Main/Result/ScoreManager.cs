using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RankingSystem;

public class ScoreManager : MonoBehaviour
{
    [SerializeField]
    GameObject OutSide;
    [SerializeField]
    GameObject InSide;
    [SerializeField]
    GameObject Stock;
    GameObject StockIcon;
    GameObject StockNumber;
    [SerializeField]
    GameObject Magadama;
    GameObject MagadamaIcon;
    GameObject MagadamaNumber;
    [SerializeField]
    GameObject Equal;
    [SerializeField]
    GameObject SumScore;

    [Tooltip("ストック一個あたりのスコア")]
    [SerializeField]
    int StockScore = 100;
    [Tooltip("マガダマ一個あたりのスコア")]
    [SerializeField]
    int MagadamaScore = 10;

    private StockManager stockManagerScript;
    private MagadamaManager magadamaManagerScript;

    GameObject stockNum10Image;
    GameObject stockNum1Image;

    GameObject magadamaNum10Image;
    GameObject magadamaNum1Image;

    // 描画用値
    float currentStock = 0.0f;
    float currentMagadama = 0.0f;
    float currentScore = 0.0f;

    // 集計
    int totalStock = 0;
    int totalMagadama = 0;
    int totalScore = 0;

    void Start()
    {
        stockManagerScript = GameObject.Find("Item").GetComponent<StockManager>();
        magadamaManagerScript = GameObject.Find("Item").GetComponent<MagadamaManager>();

        // オブジェクト取得
        StockIcon = Stock.transform.Find("Icon").gameObject;
        StockNumber = Stock.transform.Find("Number").gameObject;
        MagadamaIcon = Magadama.transform.Find("Icon").gameObject;
        MagadamaNumber = Magadama.transform.Find("Number").gameObject;

        // アクティブ状態変化
        OutSide.SetActive(false);
        InSide.SetActive(false);
        Stock.SetActive(false);
        StockIcon.SetActive(false);
        StockNumber.SetActive(false);
        Magadama.SetActive(false);
        MagadamaIcon.SetActive(false);
        MagadamaNumber.SetActive(false);
        Equal.SetActive(false);
        SumScore.SetActive(false);
    }

    void Update()
    {
        totalStock = stockManagerScript.Get();
        totalMagadama = magadamaManagerScript.Get();
        totalScore = totalStock * StockScore + totalMagadama * MagadamaScore;

        // スコアの上限
        if (totalScore >= 999)
            totalScore = 999;
    }

    bool UpdateStock()
    {
        float up = totalStock * Time.deltaTime / 1.0f;

        currentStock += up;
        if (currentStock > totalStock)
            currentStock = totalStock;
        int tempStock = (int)currentStock;

        int stockNum1 = tempStock % 10;
        tempStock /= 10;
        int stockNum10 = tempStock % 10;

        StockNumber.transform.Find("Num10Image").GetComponent<ScoreSprite>().SetNumber(stockNum10);
        StockNumber.transform.Find("Num1Image").GetComponent<ScoreSprite>().SetNumber(stockNum1);

        if (currentStock == totalStock)
            return true;

        return false;
    }

    bool UpdateMagadama()
    {
        float up = totalMagadama * Time.deltaTime / 2.0f;

        currentMagadama += up;
        if (currentMagadama > totalMagadama)
            currentMagadama = totalMagadama;
        int tempMagadama = (int)currentMagadama;

        int magadamaNum1 = tempMagadama % 10;
        tempMagadama /= 10;
        int magadamaNum10 = tempMagadama % 10;

        MagadamaNumber.transform.Find("Num10Image").GetComponent<ScoreSprite>().SetNumber(magadamaNum10);
        MagadamaNumber.transform.Find("Num1Image").GetComponent<ScoreSprite>().SetNumber(magadamaNum1);

        if (currentMagadama == totalMagadama)
            return true;

        return false;
    }

    bool UpdateScore()
    {
        float up = totalScore * Time.deltaTime / 2.0f;
        
        currentScore += up;
        if (currentScore > totalScore)
            currentScore = totalScore;
        int tempScore = (int)currentScore;

        int sumNum1 = tempScore % 10;
        tempScore /= 10;
        int sumNum10 = tempScore % 10;
        tempScore /= 10;
        int sumNum100 = tempScore % 10;

        SumScore.transform.Find("Num100Image").GetComponent<ScoreSprite>().SetNumber(sumNum100);
        SumScore.transform.Find("Num10Image").GetComponent<ScoreSprite>().SetNumber(sumNum10);
        SumScore.transform.Find("Num1Image").GetComponent<ScoreSprite>().SetNumber(sumNum1);

        if (currentScore >= totalScore)
        {
            currentScore = totalScore;
            return true;
        }

        return false;
    }

    /// <summary>
    /// 描画開始
    /// </summary>
    public void DrawStart()
    {
        OutSide.SetActive(true);
        InSide.SetActive(true);

        currentStock = 0;
        currentMagadama = 0;
        currentScore = 0;

        // Add:弓達　ランキングデータ保存
        SaveData();
        // コルーチン生成
        StartCoroutine("UpdateResult");
    }

    /// <summary>
    /// 合計スコア取得
    /// </summary>
    public int GetScore()
    {
        return this.totalScore;
    }

    /// <summary>
    /// 関数内の一番最後の行に終了処理がある
    /// </summary>
    private IEnumerator UpdateResult()
    {
        yield return new WaitWhile(StockIcon.GetActive);
        
        // ストックアイコン描画
        yield return new WaitForSeconds(1.0f);
        Stock.SetActive(true);
        StockIcon.SetActive(true);

        // ストックスコア描画
        yield return new WaitForSeconds(1.0f);
        StockNumber.SetActive(true);

        // スコア加算
        yield return new WaitForSeconds(1.0f);
        yield return new WaitUntil(UpdateStock);

        // マガダマアイコン描画
        yield return new WaitForSeconds(1.0f);
        Magadama.SetActive(true);
        MagadamaIcon.SetActive(true);

        // マガダマスコア描画
        yield return new WaitForSeconds(1.0f);
        MagadamaNumber.SetActive(true);

        // マガダマ加算
        yield return new WaitForSeconds(1.0f);
        yield return new WaitUntil(UpdateMagadama);

        // イコール描画
        yield return new WaitForSeconds(1.0f);
        Equal.SetActive(true);

        // 合計スコア描画
        yield return new WaitForSeconds(1.0f);
        SumScore.SetActive(true);

        // 合計スコア加算
        yield return new WaitForSeconds(1.0f);
        yield return new WaitUntil(UpdateScore);

        // 終わり
        Debug.Log("終わり");

		isResultEnd = true;
    }

	bool isResultEnd = false;
	public bool IsResultEnd()
	{
		return isResultEnd;
	}

    // Add:弓達　ランキングデータ保存
    void SaveData()
    {
        var stageName = SceneController.Instance.GetActiveSceneName();
    }
}