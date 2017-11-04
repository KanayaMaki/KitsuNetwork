using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeCountScript : MonoBehaviour {

    private float createTimerSeconds = 5.0f;
    private float timer;

    public Sprite[] sprites;

    DangerLineManagerScript dangerLineManagerScript;

	// Use this for initialization
	void Start () {
        // マネージャースクリプト取得
        dangerLineManagerScript = this.transform.parent.GetComponent<DangerLineManagerScript>();

        // 生成時間の取得
        timer = createTimerSeconds = dangerLineManagerScript.GetCreateTimerSecond();
	}
	
	// Update is called once per frame
	void Update () {
        // タイマーの更新
        timer -= Time.deltaTime;

        int num = (int)timer;
        GetComponent<SpriteRenderer>().sprite = sprites[num];

        // タイマー終了
        if (timer <= 0.0f)
        {
            timer = this.createTimerSeconds;

            // オブジェクト生成
            dangerLineManagerScript.Create();
        }
	}
}
