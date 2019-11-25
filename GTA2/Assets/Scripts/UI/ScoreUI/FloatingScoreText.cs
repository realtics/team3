using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingScoreText : MonoBehaviour
{
    [SerializeField]
    float floatSpeed;

    [SerializeField]
    float activeTime;



    // Start is called before the first frame update
    TextMesh myTextMesh;
    float activeDelta;


    public void FloatingText(Vector3 targetPos, int scoreValue)
    {
        targetPos.y += 1.0f;
        activeDelta = .0f;
        transform.position = targetPos;
        gameObject.SetActive(true);

        SetTextMesh(scoreValue);
    }

    void SetTextMesh(int scoreValue)
    {
        if (myTextMesh == null)
        {
            myTextMesh = GetComponent<TextMesh>();
        }

        myTextMesh.color = RandomColor();
        myTextMesh.text = scoreValue.ToString();
    }

    void Update()
    {
        // 90 도 누운 상태이기에 back으로 보내야 방향이 맞다.
        transform.Translate(Vector3.back * Time.deltaTime * floatSpeed);

        activeDelta += Time.deltaTime;
        if (activeDelta > activeTime)
        {
            gameObject.SetActive(false);
        }
    }


    Color RandomColor()
    {
        int idx = Random.Range(0, 9);

        switch (idx)
        {
            case 0:
                return Color.black;
            case 1:
                return Color.white;
            case 2:
                return Color.yellow;
            case 3:
                return Color.red;
            case 4:
                return Color.blue;
            case 5:
                return Color.green;
            case 6:
                return Color.cyan;
            case 7:
                return Color.magenta;
            case 8:
                return Color.grey;
        }

        return Color.clear;
    }
}
