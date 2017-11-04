using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassEnemy2 : MonoBehaviour {

    GameObject ui;
	public BufferProperty<bool> visibleWarning;

    [SerializeField]
    private float AttackSpeed = 15.0f;

    private GameObject searchTrigger;
    private GameObject attackTrigger;
    private Rigidbody rb;

    private Vector3 oldVelocity = Vector3.zero;
    PhotonView photonView;
    SpriteRenderer spriteRenderer;
    float alpha = 1.0f;

	//サウンド関係
	[Tooltip("飛び出し音の再生を遅らせる秒数")]
	[SerializeField]
	private float soundDelay = 0.0f;

    // 画像サイズ
    private Vector2 spriteSize;

	SoundManager soundManager;

    bool isDeath = false;

    GameObject particleObject;

	// Use this for initialization
	void Start ()
    {
        rb = GetComponent<Rigidbody>();

        // 探索用と攻撃用のオブジェクト取得
        searchTrigger = this.transform.Find("SearchTrigger").gameObject;
        attackTrigger = this.transform.Find("AttackTrigger").gameObject;

        attackTrigger.SetActive(false);

		ui = GetComponent<NestPrefab>().FindData("Warning");

        spriteRenderer = transform.Find("Model").Find("grassEnemy").GetComponent<SpriteRenderer>();
        spriteSize.x = spriteRenderer.size.x * spriteRenderer.transform.localScale.x * 0.5f;
        spriteSize.y = spriteRenderer.size.y * spriteRenderer.transform.localScale.y * 0.5f;

        photonView = GetComponent<PhotonView>();
		soundManager = FindObjectOfType<SoundManager>();

        particleObject = transform.Find("PTCL_EnemyLeaf").gameObject;
        particleObject.GetComponent<ParticleSystem>().Stop();
	}

	// Update is called once per frame
	void Update ()
    {
        // 死んでたらスキップ
        if (isDeath )
            return;
        
        // 発見済み
        if (searchTrigger == null || !searchTrigger.activeInHierarchy || !searchTrigger.activeSelf)
        {
            float oldDistance = oldVelocity.x * oldVelocity.x + oldVelocity.y * oldVelocity.y;
            float newDistance = rb.velocity.x * rb.velocity.x + rb.velocity.y * rb.velocity.y;

            // 前回より移動量が減っている
            if (oldDistance <= newDistance)
            {
                // 移動量が小さくなったら消滅処理
                if (newDistance < 0.01f)
                {
                    // 消滅処理
                    alpha -= Time.deltaTime * 1.0f;                     // 消滅速度
                    spriteRenderer.color = new Color(1, 1, 1, alpha);
                }
            }

            // 消滅始めたら攻撃辞める
            if (alpha < 0.95f)
            {
                attackTrigger.SetActive(false);

                if (alpha <= 0.0f)
                    Destroy(this.gameObject);
            }
        }

        oldVelocity = rb.velocity;

        // 潰されて死ぬ
        if (CollisionTop())
        {
            isDeath = true;
            ui.SetActive(false);
            GetComponent<InvisibleBlock>().enabled = false;
            searchTrigger.SetActive(false);
            attackTrigger.SetActive(false);
            GetComponent<BoxCollider>().enabled = false;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = new Vector3(3.0f, 0.0f, 0.0f);
        }
	}

	private void OnTriggerStay(Collider other)
	{
        // 死んでたらスキップ
        if (isDeath)
            return;

        // プレイヤー以外は無視
        if (other.tag != "Player")
            return;

        // 自分が生成したプレイヤー以外は無視
        if (!other.GetComponent<PhotonView>().isMine)
            return;

        // サーチトリガーがアクティブ状態でなければアタックトリガーを機能させる
        if (searchTrigger == null || !searchTrigger.activeInHierarchy || !searchTrigger.activeSelf)
        {
            if (attackTrigger.GetActive())
            {
                // 攻撃判定との衝突処理
                other.GetComponent<PlayerController>().SetDeathData();
                AttackPlayer();
            }
            return;
        }

		// サーチオブジェクトが反応したら
        photonView.RPC("CollisionRPC", PhotonTargets.All, other.transform.position);
	}

    // 上方向の判定（殺される）
    bool CollisionTop()
    {
        // Rayが当たったオブジェクト
        RaycastHit rayHit;

        bool isHit = Physics.BoxCast(transform.position + new Vector3(0.0f, spriteSize.y, 0.0f),
                                        Vector3.one * 0.5f, transform.up, out rayHit, Quaternion.identity, spriteSize.y);

        // 当たっていなければ終わり
        if (!isHit)
            return false;

        //Rayが当たったオブジェクトのtagが上下ブロック以外なら終わり
        if (rayHit.collider.tag != "UpDownBlock")
            return false;

        // ここまできたらOK
        return true;
    }

    [PunRPC]
    void AttackPlayerRPC()
    {
        //////////////////// 齊藤が追加
        //危険アイコンを非表示にする
        ui.SetActive(false);
        GetComponent<InvisibleBlock>().enabled = false;
        ////////////////////
        attackTrigger.SetActive(false);     // 攻撃トリガー消す
    }

    // 攻撃処理
    void AttackPlayer()
    {
        photonView.RPC("AttackPlayerRPC", PhotonTargets.All);
    }

    [PunRPC]
    void CollisionRPC(Vector3 playerPosition)
    {
        searchTrigger.SetActive(false);     // サーチオブジェクトをOFFにする
        attackTrigger.SetActive(true);      // 攻撃用コリジョンをONにする

        //////////////////// 齊藤が追加
        //危険アイコンを非表示にする
        ui.SetActive(false);
        GetComponent<InvisibleBlock>().enabled = false;
        ////////////////////

        // 進行方向を決める
        Vector3 direction = (playerPosition - this.transform.position);

        if (direction.x > 0.0f)
            this.transform.rotation = Quaternion.Euler(0, 180, 0);
        else
            this.transform.rotation = Quaternion.Euler(0, 0, 0);

        // 力を正規化
        Vector3 force = Vector3.Normalize(direction);

        // 加速
        force *= AttackSpeed;

        // 力を加える
        rb.AddForce(force, ForceMode.VelocityChange);

		//飛び出し音を鳴らす
		soundManager.PlaySE("Tobidashi", soundDelay);

        // パーティクル再生 + 親解除
        particleObject.GetComponent<ParticleSystem>().Play();
        particleObject.transform.parent = null;
    }
}
