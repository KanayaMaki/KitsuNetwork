using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreSprite : MonoBehaviour
{
    Texture[] textures;
    Sprite[] sprites;

    void Awake()
    {
        sprites = new Sprite[10];
        sprites = (Sprite[])Resources.LoadAll<Sprite>("number");
        SetNumber(0);
    }

    public void SetNumber(int number)
    {
        switch (number)
        {
            case 0: GetComponent<Image>().sprite = sprites[9]; break;
            case 1: GetComponent<Image>().sprite = sprites[0]; break;
            case 2: GetComponent<Image>().sprite = sprites[1]; break;
            case 3: GetComponent<Image>().sprite = sprites[2]; break;
            case 4: GetComponent<Image>().sprite = sprites[3]; break;
            case 5: GetComponent<Image>().sprite = sprites[4]; break;
            case 6: GetComponent<Image>().sprite = sprites[5]; break;
            case 7: GetComponent<Image>().sprite = sprites[6]; break;
            case 8: GetComponent<Image>().sprite = sprites[7]; break;
            case 9: GetComponent<Image>().sprite = sprites[8]; break;
        }
    }
}