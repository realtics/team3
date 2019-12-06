using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldUIManager : MonoSingleton<WorldUIManager>
{
    [SerializeField]
    GameObject scoreTextPref;
    [SerializeField]
    int scorePoolCount;

    // Start is called before the first frame update
    void Start()
    {
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
        PoolManager.WarmPool(scoreTextPref, scorePoolCount);
    }
    // Update is called once per frame
   
    public void SetScoreText(Vector3 targetPos, int scoreValue)
    {
        FloatingScoreText floatingScoreText = 
            PoolManager.SpawnObject(scoreTextPref).GetComponent<FloatingScoreText>();

        floatingScoreText.FloatingText(targetPos, scoreValue);
    }
}
