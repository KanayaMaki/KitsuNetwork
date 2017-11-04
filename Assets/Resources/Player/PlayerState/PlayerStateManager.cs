using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateManager {

	private StateBase m_State;      //ステートの実態
	private StateBase m_NextState;

	/// <summary>
	/// 基底クラスのデリゲートに登録されてる関数をまとめて呼び出す
	/// </summary>
	public void Execute() {
		if (m_State != null)
			m_State.Execute();
	}

	public void SetState(StateBase state) {
		m_State = m_NextState = state;
	}

	public void ChangeState(StateBase state, ref GameObject obj) {
		//現在のステートと違ってる？？
		if (m_NextState != state) {
			state.StateEntry(obj);      //新しいステートの入り口処理
			m_NextState.StateExit(ref obj);     //古いステートの出口処理

			m_NextState = state;                        //状態を更新する
		}
	}

	public void StateUpdate() {
		if (m_NextState != m_State) {
			m_State = m_NextState;                        //状態を更新する
		}
	}
}
