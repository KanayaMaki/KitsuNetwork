using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ステートパターンを実装するための基底クラス
/// </summary>
public abstract class StateBase {
	//デリゲート
	public delegate void executeState();
	public executeState execDelegate;

	/// <summary>
	/// デリゲートに登録された関数を実行する
	/// </summary>
	public virtual void Execute() {
		if (execDelegate != null) {
			execDelegate();
		}
	}

	/// <summary>
	/// 状態に入る際の入り口処理を行う
	/// </summary>
	public abstract void StateEntry(GameObject obj);

	/// <summary>
	/// ステートを抜ける際の出口処理を行う
	/// </summary>
	public abstract void StateExit(ref GameObject obj);

}       // end BaseState class
