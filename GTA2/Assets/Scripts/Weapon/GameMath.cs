using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMath : MonoBehaviour
{
    static public List<Vector3> DivideAngleFromCount(
        float startAngle,
        float endAngle, 
        float originRotate,
        int divideCnt)
    {
        List<Vector3> returnList = new List<Vector3>(divideCnt);

        if (startAngle > endAngle)
        {
            float tmp = startAngle;
            startAngle = endAngle;
            endAngle = tmp;
        }

        float spacing = (endAngle - startAngle) / (divideCnt - 1);


        for (int i = 0; i < divideCnt; i++)
        {
            float Value = (startAngle + spacing * i);
            float GunRad = .0f;
            GunRad = (originRotate + Value + 90.0f) * Mathf.Deg2Rad;

            Vector3 tempDir = new Vector3(Mathf.Cos(GunRad), .0f, Mathf.Sin(GunRad));
            tempDir.x *= -1.0f;
            tempDir.y = originRotate + Value;

            returnList.Add(tempDir);
        }

        return returnList;
    }
}
