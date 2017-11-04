using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class ScoreRender : MonoBehaviour {
    private const int DIGIT_MAX = 3;

    [SerializeField]
    private ScoreSprite[] scoreImage = new ScoreSprite[DIGIT_MAX];


	// Use this for initialization
	void Start () {
        // ０ で初期化する
        foreach (ScoreSprite value in scoreImage) {
            value.SetNumber(0);
        }
		
	}
	
    // スコアをセット
	public void SetScore(int _score)
    {
        var scoreStr = _score.ToString();

        scoreStr = String.Format("{0:D3}",scoreStr);

        for (int i = 0; i < DIGIT_MAX; ++i)
        {
            scoreImage[0].SetNumber((int)char.GetNumericValue(scoreStr[0]));
        }
    }
}
