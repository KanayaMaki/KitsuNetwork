/*=================================================
//								FILE : InputManager.cs
//		ファイル説明 :
//		ファイルの入力を管理するクラス
//		可能な限りでスクリプト順を最優先にしてね。以下のサイトの設定を行う必要がある
//		http://mizusoba.blog.fc2.com/blog-entry-3.html
//							HAL大阪 : 松本 雄之介
=================================================*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure;

public class InputManager : MonoBehaviour {

	// TODO 入力処理はスクリプトの実行優先度を上げて最優先に更新するようにする

	bool ConnectFlag = false;               //コントローラーとの接続を行ったかどうかのフラグ
	PlayerIndex playerIndex;            //現在つないでいるプレイヤーの番号
	GamePadState NowState;              //現在のフレームの入力情報

	//振動量を管理
	float LeftVibration = 0.0f;
	float RightVibration = 0.0f;

	/// <summary>
	/// 入力するキー番号。ビット演算を使用するため累乗
	/// </summary>
	public enum KeyData {
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
	void Update() {
		//まずは接続処理。アップデートの中で処理しているのはゲームが始まってから
		//コントローラーをつないでも接続させるため。
		if (!ConnectFlag || !NowState.IsConnected) {
			PlayerIndex testPlayerIndex = PlayerIndex.One;
			GamePadState testState = GamePad.GetState(testPlayerIndex);
			//1度でも接続されればコネクトフラグを立てて、以降このブロックに入らないようにする
			if (testState.IsConnected) {
				playerIndex = testPlayerIndex;
				ConnectFlag = true;
			}
		}       // end if (!playerIndexSet || !prevState.IsConnected)

		//ここから更新処理
		NowState = GamePad.GetState(playerIndex);   //新しい情報を取得
		ButtonsUpdate(NowState);    //更新

		//最後に振動を減速させる
		LeftVibration *= 0.6f;
		RightVibration *= 0.4f;
	}       //void Update ()

	/// <summary>
	/// ボタンの入力情報を検査して
	/// </summary>
	/// <param name="state"></param>
	/// <returns></returns>
	void ButtonsUpdate(GamePadState Data) {
		OldInp = NowInp;        //前のフレームの情報を保持する
		NowInp = 0;             //一度入力情報をクリアする

		//ゲームパッドが接続されていたらそちらからデータを取得する
		if (ConnectFlag) {
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
		else {
			//接続されていなければキーボードからデータを取得する
			if (Input.GetKey(KeyCode.Space)) NowInp += (short)KeyData.A;
			if (Input.GetKey(KeyCode.D)) NowInp += (short)KeyData.B;
			if (Input.GetKey(KeyCode.D)) NowInp += (short)KeyData.X;
			if (Input.GetKey(KeyCode.S)) NowInp += (short)KeyData.Y;
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

	private void FixedUpdate() {
		//振動させる
		GamePad.SetVibration(playerIndex, LeftVibration, RightVibration);
	}

	/// <summary>
	/// キーのトリガー入力情報を取得する
	/// </summary>
	/// <returns></returns>
	public bool GetButtonTrigger(KeyData key) {
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

	public bool GetButtonRelease(KeyData key) {
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

	public bool GetButtonPress(KeyData key) {
		if ((NowInp & (short)key) != 0) return true;
		return false;
	}


	public void SetButtonData(KeyData key, bool flag) {
		short No = -32768;       //intのすべてのbitを立てるため
		if (flag) {
			No = (short)key;          //
			NowInp |= No;
		}
		else {
			No -= (short)key;
			NowInp &= No;
		}
	}

	//左側の大きい振動をセットする
	public void SetLeftVibration(float rate) {
		LeftVibration = rate;       //振動をセットする
	}

	//右側の大きい振動をセットする
	public void SetRightVibration(float rate) {
		RightVibration = rate;      //振動をセットする
	}

	////======================== 変数 ===============================

	///// <summary>
	///// 入力情報を一元管理する構造体
	///// </summary>
	//struct InputData {
	//	public bool A, B, Y, X;        //各ボタンの入力フラグ
	//	public bool Up, Down, Left, Right;		//十字キーのフラグ
	//}

	//public enum InputCode {
	//	A = 1,
	//	B = 2,
	//	X = 4,
	//	Y = 8,
	//	UP = 16,
	//	DOWN = 32,
	//	LEFT = 64,
	//	RIGHT = 128
	//}

	//InputData[] inputData;		//ここに全員分の入力情報を集約させる

	////======================== 関数 ===============================

	//// Use this for initialization
	//void Start () {
	//	inputData = new InputData[3];

	//	//初期化処理
	//	for(int i = 0; i < 3; i++) {
	//		inputData[i].A = inputData[i].B = inputData[i].Y = inputData[i].X = false;
	//		inputData[i].Up = inputData[i].Down = inputData[i].Left = inputData[i].Right = false;
	//	}
	//}

	//// Update is called once per frame
	//void Update () {
	//	//初期化処理
	//	for (int i = 0; i < 3; i++) {
	//		inputData[i].A = inputData[i].B = inputData[i].Y = inputData[i].X = false;
	//		inputData[i].Up = inputData[i].Down = inputData[i].Left = inputData[i].Right = false;
	//	}

	//	//ここで3つのコントローラの状態に更新をかける(入力番号の添え字は1からスタート)
	//	for (int i = 0; i < 2; i++) {
	//		//コントローラの入力はコントロール番号が１変わるごとに20進んでいる
	//		inputData[i].A = Input.GetKey(KeyCode.Joystick1Button0 + i * 20);
	//		inputData[i].B = Input.GetKey(KeyCode.Joystick1Button1 + i * 20);
	//		inputData[i].X = Input.GetKey(KeyCode.Joystick1Button3 + i * 20);
	//		inputData[i].Y = Input.GetKey(KeyCode.Joystick1Button4 + i * 20);

	//		if (i < 2) {
	//			var hor = "PadX" + (i + 1);
	//			var vert = "PadY" + (i + 1);
	//			float InpX = Input.GetAxis(hor);
	//			float InpY = Input.GetAxis(vert);

	//			if (InpY > 0.0f) inputData[i].Up = true;
	//			if (InpY < 0.0f) inputData[i].Down = true;
	//			if (InpX > 0.0f) inputData[i].Right = true;
	//			if (InpX < 0.0f) inputData[i].Left = true;
	//		}
	//	}

	//	//最後にキーボードからの入力を取る
	//	// TODO a版専用の機能なのであとで削除して大丈夫
	//	inputData[2].A = Input.GetKey(KeyCode.Space);
	//	inputData[2].B = Input.GetKey(KeyCode.Z);
	//	inputData[2].X = Input.GetKey(KeyCode.X);
	//	inputData[2].Y = Input.GetKey(KeyCode.C);

	//	inputData[2].Up = Input.GetKey(KeyCode.UpArrow);
	//	inputData[2].Down = Input.GetKey(KeyCode.DownArrow);
	//	inputData[2].Right = Input.GetKey(KeyCode.RightArrow);
	//	inputData[2].Left = Input.GetKey(KeyCode.LeftArrow);
	//}

	///// <summary>
	///// コントローラ番号とキーコードを指定して、ボタンが押されているか確認する
	///// </summary>
	///// <param name="No">コントローラ番号</param>
	///// <param name="code">列挙対で定義しているキーコード</param>
	///// <returns>押しているか否か？</returns>
	//public bool GetKey(int No, InputCode code) {
	//	switch (code) {
	//		case InputCode.A:
	//			return inputData[No].A;
	//		case InputCode.B:
	//			return inputData[No].B;
	//		case InputCode.X:
	//			return inputData[No].X;
	//		case InputCode.Y:
	//			return inputData[No].Y;
	//		case InputCode.UP:
	//			return inputData[No].Up;
	//		case InputCode.DOWN:
	//			return inputData[No].Down;
	//		case InputCode.RIGHT:
	//			return inputData[No].Right;
	//		case InputCode.LEFT:
	//			return inputData[No].Left;
	//	}

	//	return false;
	//}
}
