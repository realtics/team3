using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class LifeCount : MonoBehaviour
{
    // Start is called before the first frame update
    Text lifeText;

    void Start()
    {
        lifeText = GetComponentInChildren<Text>();
    }

    // Update is called once per frame
    public void UpdateLifeCount(int value)
    {
        lifeText.text = value.ToString();
    }
}
