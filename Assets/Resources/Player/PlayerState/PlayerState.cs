using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerState
{
	/// <summary>
	/// ジャンプ状態
	/// </summary>
	public class JumpState : StateBase
	{

		public override void StateEntry(GameObject obj)
		{
		}

		public override void StateExit(ref GameObject obj)
		{

		}
	}

	/// <summary>
	/// 着地状態
	/// </summary>
	public class OnGroundState : StateBase
	{

		public override void StateEntry(GameObject obj)
		{

		}

		public override void StateExit(ref GameObject obj)
		{

		}
	}

	/// <summary>
	/// 待機状態
	/// </summary>
	public class WaitState : StateBase
	{

		public override void StateEntry(GameObject obj)
		{

		}

		public override void StateExit(ref GameObject obj)
		{

		}
	}

	/// <summary>
	/// 歩き状態
	/// </summary>
	public class WalkState : StateBase
	{

		public override void StateEntry(GameObject obj)
		{

		}

		public override void StateExit(ref GameObject obj)
		{

		}
	}

	/// <summary>
	/// 走り状態
	/// </summary>
	public class RunState : StateBase
	{

		public override void StateEntry(GameObject obj)
		{

		}

		public override void StateExit(ref GameObject obj)
		{

		}
	}

	/// <summary>
	/// ツタ捕まり状態
	/// </summary>
	public class IvyState : StateBase
	{

		public override void StateEntry(GameObject obj)
		{

		}

		public override void StateExit(ref GameObject obj)
		{

		}
	}

	/// <summary>
	/// 生存している際の全体の処理を行う
	/// </summary>
	public class SurvivalState : StateBase
	{

		public override void StateEntry(GameObject obj)
		{

		}

		public override void StateExit(ref GameObject obj)
		{

		}
	}

	// =========================================================================================================================↓
	// ADD
	public class ToBeKillState : StateBase
	{

		public override void StateEntry(GameObject obj)
		{

		}

		public override void StateExit(ref GameObject obj)
		{

		}
	}

	public class ImmobileState : StateBase
	{

		public override void StateEntry(GameObject obj)
		{

		}

		public override void StateExit(ref GameObject obj)
		{

		}
	}

	//============================================================================================================================↑
}
