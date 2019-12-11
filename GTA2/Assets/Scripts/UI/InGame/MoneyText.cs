using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;

public class MoneyText : MonoBehaviour
{
    Text text;

    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<Text>();
    }

    public void SetMoney(int Money)
    {
        // StringBuilder 쓰쇼
        StringBuilder sb = new StringBuilder();

        sb.Append("$");
        sb.Append(Money);

        text.text = sb.ToString();
    }
}
