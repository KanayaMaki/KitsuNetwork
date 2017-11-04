using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : SingletonMonoBehaviour<AudioController> {
    void Awake()
    {
        if(this != Instance)
        {
            Destroy(this);
            Debug.Log("自分自身を破棄");
            return;
        }

        DontDestroyOnLoad(this.gameObject);
    }
}
