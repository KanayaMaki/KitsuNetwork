using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage_Logo : MonoBehaviour {

    [SerializeField]
    private SpriteRenderer m_stageSelectLogo;

    [SerializeField]
    private Sprite[] m_stageName = new Sprite[4];
    [SerializeField]
    private GameObject m_stageNameObj;

    // ロゴセット
    public void SetLogo(StageSelectController.Stage _stage)
    {
        m_stageNameObj.GetComponent<SpriteRenderer>().sprite = m_stageName[(int)_stage];
    }
}
