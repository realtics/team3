using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PoliceLevel : MonoBehaviour
{
    // Start is called before the first frame update\
    [SerializeField]
    int policeLevelIdx;


    PoliceLevelImage[] policeImages;


    void Awake()
    {
        policeImages = GetComponentsInChildren<PoliceLevelImage>();

        foreach (var item in policeImages)
        {
            item.gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < policeLevelIdx; i++)
        {
            policeImages[i].gameObject.SetActive(true);
        }
    }

    public void SetPoliceLevel(int value)
    {
        policeLevelIdx = value;
        if (policeLevelIdx >= policeImages.Length)
        {
            policeLevelIdx = policeImages.Length;
        }


        foreach (var item in policeImages)
        {
            item.gameObject.SetActive(false);
        }
    }
}
