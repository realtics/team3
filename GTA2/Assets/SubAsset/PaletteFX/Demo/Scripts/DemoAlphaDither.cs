using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoAlphaDither : MonoBehaviour {

    public bool useDither;

    private Material mat;

    void Start()
    {
        mat = GetComponent<Renderer>().material;
    }

    void Update () {
        float alpha = Mathf.Abs(Mathf.Cos(Time.time));
        mat.SetColor("_Color", new Color(1, 1, 1, alpha));
	}
}
