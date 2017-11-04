using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagerControllerLoader : MonoBehaviour {

    // ゲーム開始時に実行される（シーン読み込み前）
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void CreateManager()
    {
        CreateSceneController();

        CreateAudioController();

        CreateInputController();

        CreatePhotonController();
    }


    private static void CreateSceneController()
    {
        var prefab = Resources.Load<GameObject>("Prefab/SceneController");
        if (prefab == null)
        {
            Debug.LogError("SceneControllerがResources/Prefab/にありません");
        }
        GameObject.Instantiate(prefab);
        Debug.Log("Create SceneController");
    }

    private static void CreateAudioController()
    {
        var prefab = Resources.Load<GameObject>("Prefab/AudioController");
        if (prefab == null)
        {
            Debug.LogError("AudioControllerがResources/Prefab/にありません");
        }
        GameObject.Instantiate(prefab);
        Debug.Log("Create AudioController");
    }

    private static void CreateInputController()
    {
        var prefab = Resources.Load<GameObject>("Prefab/InputController");
        if (prefab == null)
        {
            Debug.LogError("InputControllerがResources/Prefab/にありません");
        }
        GameObject.Instantiate(prefab);
        Debug.Log("Create InputController");
    }

    public static void CreatePhotonController()
    {
        var prefab = Resources.Load<GameObject>("Prefab/PhotonController");
        if (prefab == null)
        {
            Debug.LogError("PhotonControllerがResources/Prefab/にありません");
        }
        GameObject.Instantiate(prefab);
        Debug.Log("Create PhotonController");
    }
}
