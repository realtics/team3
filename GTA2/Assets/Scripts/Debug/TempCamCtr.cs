using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempCamCtr : MonoBehaviour
{
    public GameObject target;
    Vector2 targetOldPos;
    Vector3 offset = Vector3.zero;
    Vector3 shakeOffset = Vector3.zero;

    [Range(0,1)]
    public float moveLerpSpeed;
    [Range(2, 10)]
    public int zoomMin;
    [Range(2, 10)]
    public int zoomMax;
    //[Range(0, 1)]
    public int zoomSensitivity;

    void Start()
    {
        transform.position = target.transform.position + offset;
        targetOldPos = new Vector2(target.transform.position.x, target.transform.position.z);
    }

    void FixedUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, target.transform.position + offset + shakeOffset, moveLerpSpeed);

        Vector2 targetCurPos = new Vector2(target.transform.position.x, target.transform.position.z);
        Vector2 dir = targetOldPos - targetCurPos;

        //if (dir.sqrMagnitude > 0.004)
        //{
        //    offset = new Vector3(dir.x, 0, dir.y) * -25;
        //}
        //else
        //{
        //    offset = Vector3.zero;
        //}

        offset = new Vector3(dir.x, 0, dir.y) * -25;
        offset.y = dir.sqrMagnitude * zoomSensitivity;
        offset.y = Mathf.Clamp(offset.y, zoomMin, zoomMax);

        targetOldPos = targetCurPos;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            StartShake(1f);
    }

    public void ChangeTarget(GameObject newTarget)
    {
        target = newTarget;
        transform.position = Vector3.Lerp(transform.position, target.transform.position + offset + shakeOffset, moveLerpSpeed);
        targetOldPos = new Vector2(target.transform.position.x, target.transform.position.z);
    }

    public void StartShake(float power)
    {
        StartCoroutine(ShakeCor(power));
    }

    IEnumerator ShakeCor(float startPower)
    {
        float power = startPower;

        while (power > 0.1f)
        {
            shakeOffset = new Vector3(Random.Range(-power, power), Random.Range(-power, power), Random.Range(-power, power));
            power *= 0.9f;

            yield return null;
        }
    }
}
