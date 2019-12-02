using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadBlock : MonoBehaviour
{
    public GameObject[] fences;
    public Transform[] policeCarPositions;
    public GameObject[] policeCar;

    void OnEnable()
    {
        for (int i = 0; i < fences.Length; i++)
        {
            // 애니메이터 초기화 필요함??
        }

        for (int i = 0; i < policeCarPositions.Length; i++)
        {
            // 경찰차를 땡겨와서 배치해야함.
            // 경찰차 2대를 땡겨오는데 실패하면 바로 끄기.

        }
    }

    void OnDisable()
    {
        // 땡겨온 경찰차를 꺼야함.
    }
}
