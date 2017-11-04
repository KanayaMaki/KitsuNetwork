using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenRangeScript : MonoBehaviour
{
    // 画面範囲
    private Rect screenRange;

    void Awake()
    {
        UpdateScreenRange();
    }

    // Use this for initialization
    void Start()
    {
        UpdateScreenRange();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateScreenRange();
    }

    private void UpdateScreenRange()
    {
        Vector3 cameraPosition = Camera.main.transform.position;
        Vector3 topLeft = Camera.main.ScreenToWorldPoint(cameraPosition);
        Vector3 botRight = Camera.main.ScreenToWorldPoint(cameraPosition + new Vector3(Screen.width, Screen.height, 0.0f));
        screenRange.xMin = topLeft.x;
        screenRange.yMin = topLeft.y;

        screenRange.xMax = botRight.x;
        screenRange.yMax = botRight.y;
    }

    // 画面内にオブジェクトが含まれている？
    public bool isContains(Vector3 position)
    {
        if (screenRange.Contains(position))
        {
            return true;
        }
        return false;
    }

    // 画面上端のワールド座標
    public float GetTopWorldPosition()
    {
        return screenRange.yMax;
    }

    // 画面下端のワールド座標
    public float GetBotWorldPosition()
    {
        return screenRange.yMin;
    }

    // 画面左端のワールド座標
    public float GetLeftWorldPosition()
    {
        return screenRange.xMin;
    }

    // 画面右端のワールド座標
    public float GetRightWorldPosition()
    {
        return screenRange.xMax;
    }

    // スクリーンの高さ（ワールド座標）
    public float GetWorldHeight()
    {
        return screenRange.height;
    }

    // スクリーンの幅（ワールド座標）
    public float GetWorldWidth()
    {
        return screenRange.width;
    }
}
