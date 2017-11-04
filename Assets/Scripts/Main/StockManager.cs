using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StockManager : MonoBehaviour
{

    // フォトン追加
    PhotonView photonView;

    GameObject _stockUI;
    public GameObject stockUI
    {
        get
        {
            if (_stockUI == null)
            {
                _stockUI = GetComponent<NestPrefabUI>().FindData("StockUI");
            }
            return _stockUI;
        }
    }

    //残機数
    public int num = 0;

    // Use this for initialization
    void Start()
    {

        if (FindObjectOfType<StockManager>() != this)
        {
            Debug.LogError("Stockが二つ作られています");
        }

        photonView = GetComponent<PhotonView>();
    }

    // Update is called once per frame
    void Update()
    {

        if (stockUI)
        {
            stockUI.GetComponent<StockUI>().stockNum = Mathf.Clamp(num, 0, num);
        }
    }

    // 取得済みアイテムリスト
    List<int> AcquiredItem = new List<int>();
    public void Add(int addNum, int objID)
    {
        if (PhotonNetwork.isMasterClient)
        {
            bool isAdd = true;
            foreach (int p in AcquiredItem)
            {
                if (p == objID)
                {
                    isAdd = false;
                    break;
                }
            }

            if (isAdd)
            {
                // 残機減らす時は登録はしない
                if (addNum > 0)
                {
                    AcquiredItem.Add(objID);
                }
                num += addNum;
            }

            // マスターの情報を他に共有
            photonView.RPC("SetStock", PhotonTargets.Others, num);


            //ストックが０より少ないなら
            if (num < 0)
            {
                //ゲームオーバー関数を呼ぶ
                FindObjectOfType<GameManager>().OnGameover();
            }
        }
    }

    [PunRPC]
    public void SetStock(int aNum)
    {
        num = aNum;
    }

    public int Get()
    {
        return num;
    }
}
