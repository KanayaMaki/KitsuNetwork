using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RoomScene_State
{
    //ルームシーンステートクラス
    public abstract class RoomSceneState :MonoBehaviour
    {
        protected SoundManager soundMng; 

        //デリゲート
        public delegate void OnCallBack();

        public abstract string GetStateName();

        public abstract void Init();
        public abstract void Uninit();

        protected abstract void SetParentOtherObj();
        protected abstract void SetButtonExec();
        protected abstract void StartFocus();

        protected void InitPostion()
        {
            gameObject.GetComponent<RectTransform>().localPosition = Vector3.zero;
            gameObject.GetComponent<RectTransform>().localRotation = Quaternion.identity;
        }
    }
}