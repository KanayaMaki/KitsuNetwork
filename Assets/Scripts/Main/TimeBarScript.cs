using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*  // 参考
// http://gomafrontier.com/unity/492
// https://gametukurikata.com/ui/hpui
*/
public class TimeBarScript : MonoBehaviour
{
    private float createTimerSeconds = 5.0f;
    private float timer;

    DangerLineManagerScript dangerLineManagerScript;

    public Image timeGageBack;
    public Image timeGageFront;

    void Start()    
    {
        // マネージャースクリプト取得
        dangerLineManagerScript = this.transform.parent.GetComponent<DangerLineManagerScript>();

        // 生成時間の取得
        timer = createTimerSeconds = dangerLineManagerScript.GetCreateTimerSecond();

        timeGageBack = this.transform.Find("TimeGageBack").GetComponent<Image>();
        timeGageBack.fillAmount = 1;

        timeGageFront = this.transform.Find("TimeGageFront").GetComponent<Image>();
        timeGageFront.fillAmount = 1;
    }

    void Update()
    {
        // タイマーの更新
        timer -= Time.deltaTime;

        // タイマー終了
        if (timer <= 0.0f)
        {
            timer = this.createTimerSeconds;

            // オブジェクト生成
            dangerLineManagerScript.Create();
        }

        // 時間を 0.0f ~ 1.0fの間におさめる
        float timeNormalize = timer / this.createTimerSeconds;

        // ゲージの更新
        timeGageFront.fillAmount = ( 1.0f - timeNormalize );
    }
}
