using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagadamaManager : MonoBehaviour
{

    // フォトン追加
    PhotonView photonView;

    GameObject _UI;
    public GameObject UI
    {
        get
        {
            if (_UI == null)
            {
                _UI = GetComponent<NestPrefabUI>().FindData("MagadamaUI");
            }
            return _UI;
        }
    }



    // Use this for initialization
    void Start()
    {

        if (FindObjectOfType<MagadamaManager>() != this)
        {
            Debug.LogError("MagadamaManagerが二つ作られています");
        }

        // フォトン追加
        photonView = GetComponent<PhotonView>();
    }

    // Update is called once per frame
    void Update()
    {
        if (UI)
        {
            UI.GetComponent<StockUI>().stockNum = num;
        }
    }

    //残機数
    public int num = 0;

    public int Get()
    {
        return num;
    }

    // 取得済みアイテムリスト
    List<int> AcquiredMagadama = new List<int>();
    public void Add(int addNum, int objID)
    {
        if (PhotonNetwork.isMasterClient)
        {
            bool isAdd = true;
            foreach (int p in AcquiredMagadama)
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
                    AcquiredMagadama.Add(objID);
                }
                num += addNum;
            }

            // マスターの情報を他に共有
            photonView.RPC("SetMagadama", PhotonTargets.Others, num);
        }
    }

    [PunRPC]
    public void SetMagadama(int aNum)
    {
        num = aNum;
    }
}
