using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class FormEnemy : MonoBehaviour
{
    // フォトン追加
    PhotonView photonView;
	SoundManager soundManager;

	private void Start()
	{
		//モデルの取得
		leafModel = transform.Find("LeafModel").gameObject;
		itemModel = transform.Find("FormModel").gameObject;
		enemyModel = transform.Find("Model").gameObject;
		smokeModel = transform.Find("SmokeModel").gameObject;

		//マテリアルの取得
		enemyMaterial = enemyModel.GetComponentInChildren<SpriteRenderer>().material;
		itemMaterial = itemModel.GetComponentInChildren<SpriteRenderer>().material;

		
		//状態の初期化
		state = CState.cForm;
		stateChanged = true;

        // フォトン追加
        photonView = GetComponent<PhotonView>();

		soundManager = FindObjectOfType<SoundManager>();
	}


	private void Update()
	{
		//状態が変わっていたら初期化
		if (stateChanged)
		{
			InitState();
			stateChanged = false;
		}

		//更新
		UpdateState();
	}


	void UpdateState()
	{
		//共通の処理
		stateTime += Time.deltaTime;
		VisibleLeaf();

		//状態によって分岐する
		switch (state)
		{
			case CState.cForm:
				UpdateForm();
				break;
			case CState.cAppear:
				UpdateAppear();
				break;
			case CState.cStay:
				UpdateStay();
				break;
			case CState.cFadeout:
				UpdateFadeout();
				break;
		}
	}
	void InitState()
	{
		//共通の初期化
		stateTime = 0.0f;

		//状態によって分岐する
		switch (state)
		{
			case CState.cForm:
				InitForm();
				break;
			case CState.cAppear:
				InitAppear();
				break;
			case CState.cStay:
				InitStay();
				break;
			case CState.cFadeout:
				InitFadeout();
				break;
		}

	}

	void InitForm()
	{
		//プレイヤーとは当たっていない状態から始める
		hitPlayer = false;

		//葉っぱの表示
		validLeaf = true;

		//敵は非表示に
		enemyModel.SetActive(false);
	}
	void UpdateForm()
	{
		//もしプレイヤーと当たっていたら
		if(hitPlayer == true)
		{
			//現れる状態に移行
			ChangeState(CState.cAppear);
		}
	}


	void InitAppear()
	{
		itemAlpha = 1.0f;

		//煙の表示
		smokeModel.SetActive(true);
		smokeModel.GetComponentInChildren<ParticleSystem>().Play();

		//音の再生
		soundManager.PlaySE("Hennshin");

		//葉っぱは非表示に
		validLeaf = false;
	}
	void UpdateAppear()
	{
		//アイテムのアルファ値を減少させていく
		itemAlpha = Mathf.Clamp(itemAlpha - 1.0f / itemFadeoutTakeTime * Time.deltaTime, 0.0f, 1.0f);

		//アイテムのアルファ値を設定
		Color c = itemMaterial.color;
		c.a = itemAlpha;
		itemMaterial.color = c;


		//出現にかかる時間が経過したら
		if (stateTime >= appearTime)
		{
			//出現し終わった状態に移行
			ChangeState(CState.cStay);
		}
	}


	void InitStay()
	{
		//敵の表示
		enemyModel.SetActive(true);
	}
	void UpdateStay()
	{
		//アイテムのアルファ値を減少させていく
		itemAlpha = Mathf.Clamp(itemAlpha - 1.0f / itemFadeoutTakeTime * Time.deltaTime, 0.0f, 1.0f);

		//アイテムのアルファ値を設定
		Color c = itemMaterial.color;
		c.a = itemAlpha;
		itemMaterial.color = c;
		
		//出現後の状態でいる時間が経過したら
		if (stateTime >= fadeoutStartTime)
		{
			//フェードアウト状態に移行
			ChangeState(CState.cFadeout);
		}
	}


	void InitFadeout()
	{
	}
	void UpdateFadeout()
	{
		//フェードアウト終了までにおける、現在の時間の割合を求める
		float rate = Mathf.Clamp(stateTime / fadeoutTakeTime, 0.0f, 1.0f);


		//敵のアルファ値は、1.0から割合を引いたもの
		float enemyAlpha = 1.0f - rate;
		Color c = enemyMaterial.color;
		c.a = enemyAlpha;
		enemyMaterial.color = c;


		//フェードアウトが終了したら
		if (stateTime >= fadeoutTakeTime)
		{
			//このオブジェクトを消す
			Destroy(gameObject);
		}
	}



    //************************************************************************
    // フォトン対応
	//プレイヤーと接触したか
	bool hitPlayer;

    private void OnTriggerEnter(Collider other)
    {
        //接触したのがプレイヤーならプレイヤーなら
        if (other.tag == "Player")
        {
            if (!other.GetComponent<PhotonView>().isMine)
                return;

            // 死亡処理
            other.GetComponent<PlayerController>().SetDeathData();

            //イベントを呼び出す
            photonView.RPC("HitPlayerRPC", PhotonTargets.AllViaServer);
        }
    }

    //プレイヤーと接触したときのイベント
    public void HitPlayer()
    {
        hitPlayer = true;	//プレイヤーとヒット
        GetComponent<Collider>().enabled = false;	//当たるのは一回だけ
    }

    [PunRPC]
    private void HitPlayerRPC()
    {
        HitPlayer();
    }
    //***********************************************************************

	//葉っぱを出す状態かどうか
	bool validLeaf;

	//葉っぱの表示・非表示を行う
	void VisibleLeaf()
	{
		bool activeLeaf = false;

		//葉っぱが有効で
		if(validLeaf)
		{
			//葉っぱが見えるプレイヤーなら
			if(GetComponent<InvisibleBlock>().Visible())
			{
				activeLeaf = true;	//葉っぱを有効に
			}
		}

		//葉っぱのモデルに適用
		leafModel.SetActive(activeLeaf);
	}


	//使用するモデル
	GameObject leafModel;   //葉っぱ
	GameObject itemModel;   //アイテム
	GameObject enemyModel;  //敵のモデル
	GameObject smokeModel;  //煙のモデル

	Material itemMaterial;	//アイテムのマテリアル
	Material enemyMaterial;	//敵のマテリアル


	//かかる時間を設定する変数
	[Tooltip("敵が現れてからフェードアウトし始めるまでの時間")]
	public float fadeoutStartTime = 1.0f;

	[Tooltip("敵がフェードアウトするのにかかる時間")]
	public float fadeoutTakeTime = 1.0f;

	[Tooltip("アイテムがフェードアウトするのにかかる時間")]
	public float itemFadeoutTakeTime = 1.0f;

	[Tooltip("プレイヤーが触れてから、敵が表れるまでの時間")]
	public float appearTime = 1.0f;


	//アルファ値
	float enemyAlpha;   //敵のアルファ値
	float itemAlpha;    //アイテムのアルファ値


	//状態
	enum CState
	{
		cForm,	//アイテムに化けている
		cAppear,	//プレイヤーが触れて、現れている最中
		cStay,	//出現し終わって、居続けている
		cFadeout,	//フェードアウトしている
	}

	//現在の状態
	CState state;
	//状態が変わったか
	bool stateChanged;

	//状態が変わったときのイベント
	void ChangeState(CState aState)
	{
		state = aState;
		stateChanged = true;
	}

	//現在の状態が始まってからの時間
	float stateTime;
}