using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingletonMonoBehaviour<T> : MonoBehaviour where T :MonoBehaviour{

    private static T m_Instance;

    public static T Instance
    {
        get
        {
            if (m_Instance == null)
            {
                m_Instance = FindObjectOfType<T>();
                if(m_Instance == null)
                {
                    Debug.LogError(typeof(T) + "がシーン上に存在しません");
                }
            }

            return m_Instance;
        }
    }
}
