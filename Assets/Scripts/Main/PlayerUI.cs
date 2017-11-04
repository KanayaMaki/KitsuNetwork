using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour {
	
	public GameObject uiPrefab;
	GameObject ui;

	IEnumerator stopMethod;
	public float keepStandTime = 2.0f;

    private PhotonView photonView = null;
	InputManager inpManager;
	bool canInput = true;
	
	GameObject icon;

	SoundManager soundManager;

	// Use this for initialization
	void Start ()
    {
		ui = Instantiate(uiPrefab, GameObject.Find("Canvas").transform, false);
        photonView = GetComponent<PhotonView>();
		inpManager = FindObjectOfType<InputManager>();

		soundManager = FindObjectOfType<SoundManager>();

		//アイコンのオブジェクトの取得
		icon = ui.transform.Find("Icon").gameObject;

		//UIのアイコンの変更
		//もしローカルのプレイヤーのUIなら
		if (photonView.isMine)
		{
			Transform t = GameObject.Find("GameUI").transform;

			//ゲームのUIに、アイコンをセット
			t.Find("LeftIcon/Icon").GetComponent<Image>().sprite = icon.GetComponent<PlayerIconSprite>().FindSprite("Danger");
			t.Find("RightIcon/Icon").GetComponent<Image>().sprite = icon.GetComponent<PlayerIconSprite>().FindSprite("Good");

			//自分のプレイヤーの表示
			ui.transform.Find("P").gameObject.SetActive(true);
		}

	}
	
	// Update is called once per frame
	void Update () {
		
		icon.GetComponent<Animator>().SetBool("isStandChanged", false);

		if(CanInput())
		{
			if(IsAwakeDanger())
			{
				photonView.RPC("SetIconRPC", PhotonTargets.AllViaServer, "Danger");
			}

			if(IsAwakeGood())
			{
				photonView.RPC("SetIconRPC", PhotonTargets.AllViaServer, "Good");
			}
		}
	}

	//入力が可能か
	bool CanInput()
	{
		if(photonView.isMine == true)
		{
			if(canInput == true)
			{
				return true;
			}
		}
		return false;
	}

	//Dangerのエモーションが発動されたか
	bool IsAwakeDanger()
	{
		return inpManager.GetButtonTrigger(InputManager.KeyData.LB);
	}

	//Goodのエモーションが発動されたか
	bool IsAwakeGood()
	{
		return inpManager.GetButtonTrigger(InputManager.KeyData.RB);
	}

	private void LateUpdate()
	{
		if(ui)
		{
			Vector3 scrPos = Camera.main.WorldToScreenPoint(transform.position);
			scrPos.x -= Screen.width / 2;
			scrPos.y -= Screen.height / 2;
			ui.GetComponent<RectTransform>().localPosition = scrPos;
		}
	}

    [PunRPC]
    private void SetIconRPC(string name)
    {
        SetIcon(name);
	}

	private void SetIcon(string name)
	{
        if (stopMethod != null)
        {
            StopCoroutine(stopMethod);
        }
        if (startMethod != null)
        {
            StopCoroutine(startMethod);
        }
        
        icon.GetComponent<Image>().sprite = icon.GetComponent<PlayerIconSprite>().FindSprite(name);

		if(name == "Good")
		{
			soundManager.PlaySE("Good", 0.0f);
		}
		else
		{
			soundManager.PlaySE("Danger", 0.0f);
		}


        icon.GetComponent<Animator>().SetBool("isStand", false);
        icon.GetComponent<Animator>().SetBool("isStandChanged", true);
        stopMethod = NotStandIcon();
        StartCoroutine(stopMethod);
        startMethod = StandIcon();
        StartCoroutine(startMethod);  
	}

	IEnumerator NotStandIcon()
	{
		yield return new WaitForSeconds(keepStandTime);

		GameObject icon = ui.transform.Find("Icon").gameObject;
		icon.GetComponent<Animator>().SetBool("isStand", false);
		icon.GetComponent<Animator>().SetBool("isStandChanged", true);
	}

    // 追加
    IEnumerator startMethod;
    IEnumerator StandIcon()
    {
        yield return new WaitForSeconds(0.01f);

        GameObject icon = ui.transform.Find("Icon").gameObject;
        icon.GetComponent<Animator>().SetBool("isStand", true);
        icon.GetComponent<Animator>().SetBool("isStandChanged", true);
    }


	//入力不可状態にする
	public void SetCanNotInput()
	{
		canInput = false;
	}

	//入力可能状態にする
	public void SetCanInput()
	{
		canInput = true;
	}
}
