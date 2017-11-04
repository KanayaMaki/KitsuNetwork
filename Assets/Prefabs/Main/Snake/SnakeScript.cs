using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeScript : MonoBehaviour
{
    // true 右向き
    [SerializeField]
    private bool directionRight = false;

    [SerializeField]
    private float moveSpeed = 3.0f;

    // 死亡判定
    private bool isDeath = false;

    // 画像サイズ
    private Vector2 spriteSize;
    Rigidbody rb;
    PhotonView photonView;

    GameObject ui;

    // Use this for initialization
    void Start()
    {
        GameObject go = GetComponent<NestPrefab>().FindData("Model").transform.Find("snakeEnemy").gameObject;
        spriteSize.x = go.GetComponent<SpriteRenderer>().size.x * go.transform.localScale.x * 0.5f;
        spriteSize.y = go.GetComponent<SpriteRenderer>().size.y * go.transform.localScale.y * 0.5f;

        ui = GetComponent<NestPrefab>().FindData("Warning");

        rb = GetComponent<Rigidbody>();
        photonView = GetComponent<PhotonView>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isDeath)
            return;

        ExistUpdate();
    }

    // 壁との判定
    bool CollisionSide()
    {
        // Rayが当たったオブジェクト
        RaycastHit rayHit;

        // プレイヤーと通常オブジェだけ判定
        int layerMask = LayerMask.GetMask(new string[] { "Platform" });

        Ray ray;
        if (directionRight)
            ray = new Ray(this.transform.position, Vector3.right);
        else
            ray = new Ray(this.transform.position, Vector3.left);

        //もしRayにオブジェクトが衝突したら
        if (Physics.Raycast(ray, out rayHit, spriteSize.x, layerMask))
        {
            return true;
        }
        return false;
    }

    // 下方向の判定（赤ノコノコ）
    bool CollisionBottom(Vector3 velocity)
    {
        Ray ray;
        if (directionRight)
            ray = new Ray(transform.position + new Vector3(spriteSize.x, 1.0f, 0.0f), Vector3.down);
        else
            ray = new Ray(transform.position + new Vector3(-spriteSize.x, 1.0f, 0.0f), Vector3.down);

        //もしRayにオブジェクトが衝突しなかったら
        if (!Physics.Raycast(ray))
            return true;

        return false;
    }

    bool CollisionTop()
    {
        // Rayが当たったオブジェクト
        RaycastHit rayHit;

        // プレイヤーと通常オブジェだけ判定
        int layerMask = LayerMask.GetMask(new string[] { "Platform" });

        Ray ray = new Ray(this.transform.position, Vector3.up);

        //もしRayにオブジェクトが衝突したら
        if (Physics.Raycast(ray, out rayHit, spriteSize.y, layerMask))
        {
            if (rayHit.transform.tag == "UpDownBlock")
            {
                return true;
            }
        }
        return false;
    }

    // 接触時処理
    private void OnTriggerEnter(Collider other)
    {
        // 死んでたらスキップ
        if (isDeath)
            return;

        // プレイヤーとの判定
        if (other.tag == "Player")
        {
            // 自分のプレイヤーでなければスキップ
            if (!other.GetComponent<PhotonView>().isMine)
                return;

            // 敵（自分）のほうが上
            if (other.transform.position.y < this.transform.position.y)
            {
                // プレイヤーが死ぬ
                other.GetComponent<PlayerController>().SetDeathData();
            }
            // プレイヤーのほうが上
            else
            {
                // 敵（自分）が死ぬ
                SetDeath();
            }
        }
    }
    
    //　生存時の更新
    void ExistUpdate()
    {
        Vector3 velocity = Vector3.left * moveSpeed * Time.deltaTime;

        // 移動の反映
        this.transform.Translate(velocity);

        // 横と穴の判定
        if (CollisionSide() || CollisionBottom(velocity))
        {
            directionRight = !directionRight;
        }

        if (directionRight)
            this.transform.rotation = Quaternion.Euler(0.0f, 180.0f, 0.0f);
        else
            this.transform.rotation = Quaternion.Euler(0.0f,   0.0f, 0.0f);

        // ダウンブロックに潰される処理
        if (CollisionTop())
        {
            SetDeath();
        }
    }

    public void SetDeath()
    {
        if (isDeath)
            return;

        photonView.RPC("SetDeathRPC", PhotonTargets.AllViaServer);
    }

    [PunRPC]
    void SetDeathRPC()
    {
        isDeath = true;
        ui.SetActive(false);
        GetComponent<BoxCollider>().enabled = false;
        rb.useGravity = true;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = new Vector3(3.0f, 0.0f, 0.0f);
    }
}
