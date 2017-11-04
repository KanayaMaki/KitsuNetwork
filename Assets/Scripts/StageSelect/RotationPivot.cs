using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationPivot : MonoBehaviour
{
    public delegate void StopRotaion();
    public StopRotaion Stop;

    private enum DIRECTION
    {
        Right,
        Left,
        None,
    }
    private DIRECTION state = DIRECTION.None;

    [SerializeField]
    private Vector3 m_pivot = new Vector3(0, 0, 0);
    [SerializeField]
    private float m_speed = 1.0f;

    private float beforeAngle = 0;
    private float afterAngle = 0;
    private float timeCount = 0;

    // Update is called once per frame
    void Update()
    {
        RotationExec();
    }

    // -------------------------------- 入力待ち --------------------------------
  

    // StageSelectControllerから呼び出される
    public void SetRotationRight()
    {
        if (!(DIRECTION.None == state)) return;

        SetRotation(DIRECTION.Right);
        
    }

    // StageSelectControllerから呼び出される
    public void SetRotationLeft()
    {
        if (!(DIRECTION.None == state)) return;

        SetRotation(DIRECTION.Left);
    }


    void SetRotation(DIRECTION _rotationDirection) 
    {
        state = _rotationDirection;

        // 回転前の角度を保持
        beforeAngle = Mathf.RoundToInt(transform.rotation.eulerAngles.z);

        // 目標回転角度をセット
        afterAngle = (_rotationDirection == DIRECTION.Right) ? beforeAngle + 90.0f : beforeAngle - 90.0f;
    }
    // --------------------------------------------------------------------------




    // ---------------------------------- 回転 ----------------------------------
    void RotationExec()
    {
        // 回転してない状態では処理しない
        if (state == DIRECTION.None) return;
        
        timeCount += Time.deltaTime * m_speed;

        RotationAngle();

        CheckAngle_State();   
    }

    // 回転処理
    void RotationAngle()
    {
        var angle = Mathf.LerpAngle(beforeAngle, afterAngle, timeCount);
        transform.eulerAngles = new Vector3(0, 0, angle);
    }

    // 回転を終了させるかチェック
    void CheckAngle_State()
    {
        var difference = Mathf.DeltaAngle(transform.eulerAngles.z, beforeAngle);
        
        if (difference >= 90 || difference <= -90)
        {
            Stop();
            state = DIRECTION.None;
            timeCount = 0;
            transform.rotation.eulerAngles.Set(0, 0, Mathf.RoundToInt(afterAngle));
        }
    }

    // --------------------------------------------------------------------------
}
