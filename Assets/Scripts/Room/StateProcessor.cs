using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RoomScene_State;
using UnityEngine.Events;

namespace RoomScene
{
    //ステートの実行を管理するクラス
    public class StateProcessor
    {
        //ステート本体
        private RoomSceneState m_state;
        public RoomSceneState State
        {
            set { m_state = value; }
            get { return m_state; }
        }

        public void Init()
        {
            m_state.Init();
        }

        public void Uninit()
        {
            if (m_state != null)
            {
                m_state.Uninit();
            }
        }
   }
}
