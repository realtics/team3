using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoneyText : MonoBehaviour
{
    Text myText;

    // Start is called before the first frame update
    void Start()
    {
        myText = GetComponent<Text>();
    }

    public void SetMoney(int Money)
    {
        myText.text = "$" + Money;
    }
}
