using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldUIManager : MonoSingleton<WorldUIManager>
{
    [SerializeField]
    GameObject scoreTextPref;
    [SerializeField]
    int scorePoolCount;
    GameObject scoreTextPool;

    List<FloatingScoreText> scoreTextList;
    int scoreTextIndex;

    // Start is called before the first frame update
    void Start()
    {
        scoreTextIndex = 0;
        InitTextMesh();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            Instance.SetScoreText(Vector3.zero, 100);
        }
    }

    void InitTextMesh()
    {
        scoreTextPool = new GameObject();
        scoreTextPool.name = "ScorePool";

        scoreTextList =
            GetPool<FloatingScoreText>.GetListComponent(
            SetPool.PoolMemory(
                scoreTextPref, scoreTextPool, scorePoolCount, "ScoreText"));

        foreach (var item in scoreTextList)
        {
            item.gameObject.transform.position = new Vector3(10000.0f, 10000.0f, 10000.0f);
        }
    }
    // Update is called once per frame
   
    public void SetScoreText(Vector3 targetPos, int scoreValue)
    {
        scoreTextList[scoreTextIndex].FloatingText(targetPos, scoreValue);
        scoreTextIndex++;
    }
}
