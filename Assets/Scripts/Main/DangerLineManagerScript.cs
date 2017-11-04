using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DangerLineManagerScript : MonoBehaviour
{
    // 各種調整用パラメータ
    [Tooltip("チェックをつけると縦方向")]
    [SerializeField]
    private bool isVertical = true;       // 垂直方向の移動

    [Tooltip("敵オブジェクトの移動速度(小数点以下も可能)")]
    [SerializeField]
    private float moveSpeed = 7.5f;         // 進行速度

    [Tooltip("生成時間(秒数)小数点以下も可能")]
    [SerializeField]
    private float createTimerSecond = 5.0f; // 生成時間（秒）

    [Tooltip("抽選確立（小数点以下も可能")]
    [SerializeField]
    private float lotteryProbability = 20.0f;

    public GameObject Danger;
    public GameObject TimeBar;

    // Find系オブジェクト待避
    DangerModelScript dangerModelScript;
    ScreenRangeScript screenRangeScript;
    GameObject cameraObject;
    bool isMyActive = true;

    PhotonView photonView;

    // Use this for initialization
    void Start()
    {
        screenRangeScript = GameObject.Find("ScreenManager").GetComponent<ScreenRangeScript>();
        cameraObject = GameObject.Find("Camera2").gameObject;

        // 配置の更新
        UpdatePosition();

        // サイズの更新
        Vector3 dangerZoneScale;
        // 横
        if (!isVertical)
        {
            dangerZoneScale = new Vector3(250.0f, 1.0f, 1.0f);
        }
        else
        {
            dangerZoneScale = new Vector3(1.0f, 100.0f, 1.0f);

            Vector3 position = this.transform.position;
            position.x += screenRangeScript.GetWorldWidth();
            transform.Find("Stop").transform.position = position;
        }

        Danger.transform.Find("DangerZoneSprite").transform.localScale = dangerZoneScale;
        dangerModelScript = transform.Find("Model").transform.Find("DangerModel").GetComponent<DangerModelScript>();
        dangerModelScript.SetVelocity(isVertical, moveSpeed);

        // 見えなくする
        GetComponent<MeshRenderer>().enabled = false;
        this.transform.Find("Stop").GetComponent<MeshRenderer>().enabled = false;

        photonView = GetComponent<PhotonView>();

        Danger.GetComponent<DangerZoneScript>().SetUpdateAlpha(false);
        TimeBar.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        UpdatePosition();
    }

    private void UpdatePosition()
    {
        Vector3 dangerPosition = this.transform.position;
        Vector3 timeBarPosition = this.transform.position;

        // 横
        if (!isVertical)
        {
            float screenWorldWidth = screenRangeScript.GetWorldWidth();

            dangerPosition.x = cameraObject.transform.position.x + screenWorldWidth * 0.5f * 0.8f;
            timeBarPosition.x = cameraObject.transform.position.x + screenWorldWidth * 0.5f * 0.65f;
        }
        else
        {
            float screenWorldHeight = screenRangeScript.GetWorldHeight();

            dangerPosition.y = screenWorldHeight * 0.70f;
            timeBarPosition.y = screenWorldHeight * 0.55f;
        }

        Danger.transform.Find("DangerMarkSprite").transform.position = dangerPosition;
        TimeBar.transform.position = timeBarPosition;
    }

    public void Create()
    {
        // 横
        if (!isVertical)
        {
            Vector3 createPosition;
            createPosition.x = screenRangeScript.GetRightWorldPosition();
            createPosition.y = this.transform.position.y;
            createPosition.z = 0.0f;
            dangerModelScript.Create(createPosition);
        }
        else
        {
            Vector3 createPosition;
            createPosition.x = this.transform.position.x;
            createPosition.y = screenRangeScript.GetTopWorldPosition();
            createPosition.z = 0.0f;
            dangerModelScript.Create(createPosition);
        }

        Danger.GetComponent<DangerZoneScript>().SetUpdateAlpha(false);
        TimeBar.SetActive(false);
    }

    // 抽選時間
    private float lotteryTimer = 0.0f;
    public void Reconduct()
    {
        if (this.isMyActive)
        {
            // 抽選時間更新
            lotteryTimer += Time.deltaTime;

            // 一定時間経過で抽選開始
            if (lotteryTimer > 1.0f)
            {
                // 時間リセット
                lotteryTimer = 0.0f;

                // マスターが生成判断
                if (PhotonNetwork.isMasterClient)
                {
                    float random = Random.Range(0.0f, 100.0f);
                    if (random <= lotteryProbability)
                    {
                        photonView.RPC("CreateModel", PhotonTargets.AllViaServer);
                    }
                }
            }
        }
    }

    [PunRPC]
    private void CreateModel()
    {
        Danger.GetComponent<DangerZoneScript>().SetUpdateAlpha(true);
        TimeBar.SetActive(true);
    }

    public float GetCreateTimerSecond()
    {
        return createTimerSecond;
    }

    public void SetMyActive(bool active)
    {
        this.isMyActive = active;
    }

    public void SetVertical(bool isVertical)
    {
        this.isVertical = isVertical;
    }

    public void SetMoveSpeed(float moveSpeed)
    {
        this.moveSpeed = moveSpeed;
    }

    public void SetCreateTimerSpeed(float createTimerSecond)
    {
        this.createTimerSecond = createTimerSecond;
    }

    public void PlayOnce()
    {
        Danger.GetComponent<DangerZoneScript>().SetUpdateAlpha(true);
        TimeBar.SetActive(true);

        this.lotteryProbability = 0.0f;
    }
}
