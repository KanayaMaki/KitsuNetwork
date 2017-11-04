using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DangerZoneScript : MonoBehaviour
{
    private Material dangerZoneMaterial;
    private Material dangerMarkMaterial;
    private float alpha = 0;
    private bool isUpdateAlpha = false;

    [SerializeField]
    private float frashTimeSecond = 1.0f;

	// Use this for initialization
	void Start ()
    {
        dangerZoneMaterial = this.transform.Find("DangerZoneSprite").gameObject.GetComponent<SpriteRenderer>().material;
        dangerMarkMaterial = this.transform.Find("DangerMarkSprite").gameObject.GetComponent<SpriteRenderer>().material;
	}
	
	// Update is called once per frame
	void Update ()
    {
        alpha -= (Time.deltaTime / frashTimeSecond);

        if(isUpdateAlpha)
        {
            while (alpha < 0.0f)
                alpha += 1.0f;
        }
        else
        {
            if (alpha < 0.0f)
                alpha = 0.0f;
        }

        dangerZoneMaterial.color = new Color(1.0f, 1.0f, 1.0f, alpha);
        dangerMarkMaterial.color = new Color(1.0f, 1.0f, 1.0f, alpha);
	}

    public void SetUpdateAlpha(bool isUpdateAlpha)
    {
        this.isUpdateAlpha = isUpdateAlpha;
    }
}
