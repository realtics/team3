using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Demo32bit : MonoBehaviour {

    private Material mat;

	void Start () {
        mat = GetComponent<Renderer>().material;
	}

    private float bodyHueValue = 0.0F;
    private float eyeHueValue = 0.0F;

    private float bodyShadeValue = 0.0F;
    private float eyeShadeValue = 0.0F;


    void OnGUI()
    {
        int X = 25;
        int Y = 25;

        GUI.Label(new Rect(X, Y, 100, 20), "Body Hue");
        bodyHueValue = GUI.HorizontalSlider(new Rect(X, Y + 25, 100, 30), bodyHueValue, 0, 1);

        GUI.Label(new Rect(X + 150, Y, 100, 20), "Eyes Hue");
        eyeHueValue = GUI.HorizontalSlider(new Rect(X + 150, Y + 25, 100, 30), eyeHueValue, 0, 1);

        GUI.Label(new Rect(X + 300, Y, 100, 20), "Body Shade");
        bodyShadeValue = GUI.HorizontalSlider(new Rect(X + 300, Y + 25, 100, 30), bodyShadeValue, -2, 2);

        GUI.Label(new Rect(X + 450, Y, 100, 20), "Eyes Shade");
        eyeShadeValue = GUI.HorizontalSlider(new Rect(X + 450, Y + 25, 100, 30), eyeShadeValue, -2, 2);

        mat.SetVector("_HueShift", new Vector2(bodyHueValue, eyeHueValue));
        mat.SetVector("_ShadeShift", new Vector2(bodyShadeValue, eyeShadeValue));
    }

}