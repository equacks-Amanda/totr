using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorPulse : MonoBehaviour {
    public float FadeDuration = 1f;
    public Color Color1 = Color.magenta;
    public Color Color2 = Color.white;
    private Color firstColor;
    private Color lastColor;
    private float lastColorChangeTime;
    private Material runeMaterial;

    // Use this for initialization
    void Start () {
        runeMaterial = GetComponent<Renderer>().material;
        firstColor = Color1;
        lastColor = Color2;
    }
	
	// Update is called once per frame
	void Update () {
        var ratio = (Time.time - lastColorChangeTime) / FadeDuration;
        ratio = Mathf.Clamp01(ratio);
        runeMaterial.color = Color.Lerp(firstColor, lastColor, ratio);

        if (ratio == 1f)
        {
            lastColorChangeTime = Time.time;
            var temp = firstColor;
            firstColor = lastColor;
            lastColor = temp;
        }
    }
}
