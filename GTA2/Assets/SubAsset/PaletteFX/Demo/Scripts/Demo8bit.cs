using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Demo8bit : MonoBehaviour {

    private Material mat;

	void Start () {
        mat = GetComponent<Renderer>().material;
	}

    private float bodyHueValue = 0.0F;
    private float eyeHueValue = 0.0F;

    private float bodyShadeValue = 0.0F;
    private float eyeShadeValue = 0.0F;

    public int row = 0;

    private int X = 200;
    private int Y;

    void OnGUI()
    {
        int hueCount = mat.GetInt("_HueCount");
        int shadeCount = mat.GetInt("_ShadeCount");

        Y = 25 + row * 150;

        GUI.Label(new Rect(X, Y, 100, 20), "Body Hue");
        bodyHueValue = GUI.HorizontalSlider(new Rect(X, Y + 25, 100, 30), bodyHueValue, -hueCount, hueCount);        

        GUI.Label(new Rect(X + 150, Y, 100, 20), "Eyes Hue");
        eyeHueValue = GUI.HorizontalSlider(new Rect(X + 150, Y + 25, 100, 30), eyeHueValue, -hueCount, hueCount);

        GUI.Label(new Rect(X + 300, Y, 100, 20), "Body Shade");
        bodyShadeValue = GUI.HorizontalSlider(new Rect(X + 300, Y + 25, 100, 30), bodyShadeValue, -shadeCount, shadeCount);

        GUI.Label(new Rect(X + 450, Y, 100, 20), "Eyes Shade");
        eyeShadeValue = GUI.HorizontalSlider(new Rect(X + 450, Y + 25, 100, 30), eyeShadeValue, -shadeCount, shadeCount);

        mat.SetVector("_HueShift", new Vector2(bodyHueValue, eyeHueValue));
        mat.SetVector("_ShadeShift", new Vector2(bodyShadeValue, eyeShadeValue));
    }

}