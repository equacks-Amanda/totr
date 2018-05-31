using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

[Serializable]
public class ColorPulse : MonoBehaviour
{
    [SerializeField]
    private float FadeDuration = 1f;

    [SerializeField]
    private Color firstColor;

    [SerializeField]
    private Color lastColor;

    private float lastColorChangeTime;

    private Material runeMaterial;

    [SerializeField]
    private Image image;

    private Color defaultColor1;
    private Color defaultColor2;
    private Color colorPrePulse;
    private Sprite defaultSprite;
    private Material defaultMaterial;

    [SerializeField]
    private bool isPulsing;

    // Use this for initialization
    void Start () {
		image = GetComponent<Image> ();

		if (GetComponent<Renderer>() != null) {

			try {
				runeMaterial = GetComponent<Renderer> ().material;
			} catch (Exception) {

				throw;
			}

			SetDefaults ();
		}
	}

    void SetDefaults()
    {
        defaultColor1 = firstColor;
        defaultColor2 = lastColor;
 
        
        if (image != null)
        {
            defaultSprite = image.sprite;
            colorPrePulse = image.color;
        }
        if (runeMaterial != null)
            defaultMaterial = runeMaterial;
    }
	// Update is called once per frame
	void FixedUpdate ()
    {
        if( isPulsing )
        {
            float ratio = (Time.time - lastColorChangeTime) / FadeDuration;
            ratio = Mathf.Clamp01(ratio);
            if (runeMaterial != null)
            {
                runeMaterial.color = Color.Lerp(firstColor, lastColor, ratio);
            }
            else if (image != null)
            {
                image.color = Color.Lerp(firstColor, lastColor, ratio);
            }


            if (ratio == 1f)
            {
                lastColorChangeTime = Time.time;
                var temp = firstColor;
                firstColor = lastColor;
                lastColor = temp;
            }
        }
       
    }

    public void ChangeColorTime( Color c1, Color c2, float duration, bool hideImage )
    {
        if( c2 != null )
            lastColor = c2;
        if (c1 != null)
            firstColor = c1;
        if (duration > 0)
            FadeDuration = duration;
        if (hideImage)
            StartCoroutine(WaitForFade(duration));

        image.enabled = true;
    }

    IEnumerator WaitForFade(float duration)
    {
        yield return new WaitForSeconds(duration*2f);
        HideImage();
    }

    IEnumerator PlayOnePulse(float duration)
    {
        isPulsing = true;
        yield return new WaitForSeconds(duration);
        isPulsing = false;
        image.color = colorPrePulse;

    }
    public void SwapImage( Sprite imageSp, Color first, Color last, float duration )
    {
        if( image != null )
        { 
            image.sprite = imageSp;
            ChangeColorTime(first, last, duration, false);
        }
    }

    public void ResetToDefault( )
    {
        if (image != null)
            image.enabled = false;
            image.sprite = defaultSprite;
        if (runeMaterial != null)
            runeMaterial = defaultMaterial;

        firstColor = defaultColor1;
        lastColor = defaultColor2;
    }

    public void HideImage()
    {
        image.enabled = false;
    }

    public void SetPulse( bool isPulse )
    {
        isPulsing = isPulse;
    }

    public void PlayOnePulse( Color c1, Color c2, float duration )
    {
        firstColor = c1;
        lastColor = c2;
        StartCoroutine(PlayOnePulse(duration));
    }
}
