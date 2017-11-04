using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure;

public class InputController : SingletonMonoBehaviour<InputController>
{

	/*
    private bool decisionDown = false;
    private bool rightDown = false;
    private bool leftDown = false;
    private bool upDown = false;
    private bool downDown = false;
    
    [SerializeField]
    private float dead = 0.05f;
    */

	void Awake()
	{
		if (this != Instance)
		{
			Destroy(this);
			Debug.Log("自分自身を破棄");
			return;
		}

		DontDestroyOnLoad(this.gameObject);
	}


	public bool OnDecision()
	{
		// InputManager設定 
		/*
        if(Input.GetButton("Submit") && !decisionDown)
        {
            Debug.Log("決定");
            decisionDown = true;
            return true;
        }
        if (Input.GetButtonUp("Submit"))
        {
            decisionDown = false;
        }
        */

		return GetButtonTrigger(InputController.KeyData.B);
	}


	public bool OnDecisionPress()
	{
		return GetButtonPress(InputController.KeyData.B);
	}

	public bool OnCancel()
	{
		return GetButtonTrigger(KeyData.A);
	}

	public bool OnEnter()
	{
		return Input.GetKeyDown(KeyCode.Return);
	}

	public bool OnRiight()
	{
		/*
        if(Input.GetAxis("Horizontal")>0 && !rightDown)
        {
            Debug.Log("右");
            rightDown = true;
            return true;
        }
        if(Input.GetAxis("Horizontal") < dead)
        {
            rightDown = false;
        }
        */
		return GetButtonTrigger(InputController.KeyData.RIGHT);
	}

	public bool OnLeft()
	{

		/*
            if (Input.GetAxis("Horizontal") < 0 && !leftDown)
            {
                Debug.Log("左");
                leftDown = true;
                return true;
            }
            if (Input.GetAxis("Horizontal") > -dead)
            {
                leftDown = false;
            }
            */
		return GetButtonTrigger(InputController.KeyData.LEFT);
	}


	public bool OnUp()
	{
		/*
            if (Input.GetAxis("Vertical") > 0 && !upDown)
            {
                Debug.Log("上");
                upDown = true;
                return true;
            }
            if (Input.GetAxis("Vertical") < dead)
            {
                upDown = false;
            }
            */
		return GetButtonTrigger(InputController.KeyData.UP);
	}


	public bool OnDown()
	{
		/*
            if (Input.GetAxis("Vertical") < 0 && !downDown)
            {
                Debug.Log("下");
                downDown = true;
                return true;
            }
            if (Input.GetAxis("Vertical") > -dead)
            {
                downDown = false;
            }
            */
		return GetButtonTrigger(InputController.KeyData.DOWN);
	}


	// TODO 入力処理はスクリプトの実行優先度を上げて最優先に更新するようにする

	bool ConnectFlag = false;               //コントローラーとの接続を行ったかどうかのフラグ
	PlayerIndex playerIndex;            //現在つないでいるプレイヤーの番号
	GamePadState NowState;              //現在のフレームの入力情報

	/// <summary>
	/// 入力するキー番号。ビット演算を使用するため累乗
	/// </summary>
	public enum KeyData
	{
		A = 1,
		B = 2,
		X = 4,
		Y = 8,
		UP = 16,            //十字キー
		DOWN = 32,
		RIGHT = 64,
		LEFT = 128,
		RB = 256,       //いわゆるR1キー
		LB = 512,       //L1キー
		START = 1024,
		BACK = 2048
	}

	//下記の二つはビットフラグを使用するので注意
	short NowInp = 0;
	short OldInp = 0;

	// Update is called once per frame
	void Update()
	{
		//まずは接続処理。アップデートの中で処理しているのはゲームが始まってから
		//コントローラーをつないでも接続させるため。
		if (!ConnectFlag || !NowState.IsConnected)
		{
			PlayerIndex testPlayerIndex = PlayerIndex.One;
			GamePadState testState = GamePad.GetState(testPlayerIndex);
			//1度でも接続されればコネクトフラグを立てて、以降このブロックに入らないようにする
			if (testState.IsConnected)
			{
				playerIndex = testPlayerIndex;
				ConnectFlag = true;
			}
		}       // end if (!playerIndexSet || !prevState.IsConnected)

		//ここから更新処理
		NowState = GamePad.GetState(playerIndex);   //新しい情報を取得
		ButtonsUpdate(NowState);    //更新
	}       //void Update ()

	/// <summary>
	/// ボタンの入力情報を検査して
	/// </summary>
	/// <param name="state"></param>
	/// <returns></returns>
	void ButtonsUpdate(GamePadState Data)
	{
		OldInp = NowInp;        //前のフレームの情報を保持する
		NowInp = 0;             //一度入力情報をクリアする

		//ゲームパッドが接続されていたらそちらからデータを取得する
		if (ConnectFlag)
		{
			//新しい入力情報を反映していく
			if (Data.Buttons.A == ButtonState.Pressed) NowInp += (short)KeyData.A;
			if (Data.Buttons.B == ButtonState.Pressed) NowInp += (short)KeyData.B;
			if (Data.Buttons.X == ButtonState.Pressed) NowInp += (short)KeyData.X;
			if (Data.Buttons.Y == ButtonState.Pressed) NowInp += (short)KeyData.Y;
			if (Data.DPad.Up == ButtonState.Pressed) NowInp += (short)KeyData.UP;
			if (Data.DPad.Down == ButtonState.Pressed) NowInp += (short)KeyData.DOWN;
			if (Data.DPad.Left == ButtonState.Pressed) NowInp += (short)KeyData.LEFT;
			if (Data.DPad.Right == ButtonState.Pressed) NowInp += (short)KeyData.RIGHT;
			if (Data.Buttons.RightShoulder == ButtonState.Pressed) NowInp += (short)KeyData.RB;
			if (Data.Buttons.LeftShoulder == ButtonState.Pressed) NowInp += (short)KeyData.LB;
			if (Data.Buttons.Start == ButtonState.Pressed) NowInp += (short)KeyData.START;
			if (Data.Buttons.Back == ButtonState.Pressed) NowInp += (short)KeyData.BACK;
		}
		else
		{
			//接続されていなければキーボードからデータを取得する
			if (Input.GetKey(KeyCode.Space)) NowInp += (short)KeyData.A;
			if (Input.GetKey(KeyCode.D)) NowInp += (short)KeyData.B;
			if (Input.GetKey(KeyCode.P)) NowInp += (short)KeyData.X;
			if (Input.GetKey(KeyCode.L)) NowInp += (short)KeyData.Y;
			if (Input.GetKey(KeyCode.UpArrow)) NowInp += (short)KeyData.UP;
			if (Input.GetKey(KeyCode.DownArrow)) NowInp += (short)KeyData.DOWN;
			if (Input.GetKey(KeyCode.LeftArrow)) NowInp += (short)KeyData.LEFT;
			if (Input.GetKey(KeyCode.RightArrow)) NowInp += (short)KeyData.RIGHT;
			if (Input.GetKey(KeyCode.W)) NowInp += (short)KeyData.RB;
			if (Input.GetKey(KeyCode.A)) NowInp += (short)KeyData.LB;
			if (Input.GetKey(KeyCode.Return)) NowInp += (short)KeyData.START;
			if (Input.GetKey(KeyCode.M)) NowInp += (short)KeyData.BACK;
		}
	}

	private void FixedUpdate()
	{

	}

	/// <summary>
	/// キーのトリガー入力情報を取得する
	/// </summary>
	/// <returns></returns>
	public bool GetButtonTrigger(KeyData key)
	{
		bool Now = false;
		bool Old = false;
		//現在のフレームで指定ボタンが押されているか？
		if ((NowInp & (short)key) != 0) Now = true;
		//現在のフレームで指定ボタンが押されているか？
		if ((OldInp & (short)key) != 0) Old = true;
		//トリガー情報に変換する
		if ((Now ^ Old) & Now) return true; //今と前の入力情報をが違う状態で今がtrueのとき
		return false;
	}

	public bool GetButtonRelease(KeyData key)
	{
		bool Now = false;
		bool Old = false;
		//現在のフレームで指定ボタンが押されているか？
		if ((NowInp & (short)key) != 0) Now = true;
		//現在のフレームで指定ボタンが押されているか？
		if ((OldInp & (short)key) != 0) Old = true;
		//リリース情報に変換する
		if ((Now ^ Old) & Old) return true; //今と前の入力情報をが違う状態で前がtrueのとき
		return false;
	}

	public bool GetButtonPress(KeyData key)
	{
		if ((NowInp & (short)key) != 0) return true;
		return false;
	}


	public void SetButtonData(KeyData key, bool flag)
	{
		short No = -32768;       //intのすべてのbitを立てるため
		if (flag)
		{
			No = (short)key;          //
			NowInp |= No;
		}
		else
		{
			No -= (short)key;
			NowInp &= No;
		}

	}

	public bool GetAnyButton()
	{
		if (NowInp > 0)
		{
			return true;
		}
		return false;
	}

}
