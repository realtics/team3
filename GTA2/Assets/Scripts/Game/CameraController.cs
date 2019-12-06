using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoSingleton<CameraController>
{
    public GameObject target;
    Vector2 targetCurPos;
    Vector2 targetOldPos;
    Vector2 targetMoveDir;

    Vector3 cameraBasePosition;
    Vector3 offset = Vector3.zero;
    Vector3 shakeOffset = Vector3.zero;

    [Range(0, 1)]
    public float moveLerpSpeed;
    [Range(0, 1)]
    public float offsetLerpSpeed;
    [Space(10)]
    [Range(2, 10)]
    public int zoomMin;
    [Range(2, 10)]
    public int zoomMax;
    public int zoomSensitivity;
    float zoomOverride = 0;

    public enum TrackingMode
    {
        car, human
    }
    TrackingMode trackingMode = TrackingMode.human;

    void Start()
    {
        cameraBasePosition = target.transform.position;
        cameraBasePosition.y = 0;
        offset = CalcCameraOffset();
        transform.position = cameraBasePosition + offset;

        targetOldPos = new Vector2(target.transform.position.x, target.transform.position.z);
    }

    void FixedUpdate()
    {
        targetCurPos = new Vector2(target.transform.position.x, target.transform.position.z);
        targetMoveDir = targetOldPos - targetCurPos;

        cameraBasePosition = Vector3.Lerp(cameraBasePosition, target.transform.position, moveLerpSpeed);
        cameraBasePosition.y = 0;

        offset = Vector3.Lerp(offset, CalcCameraOffset(), offsetLerpSpeed);

        transform.position = cameraBasePosition + offset + shakeOffset;

        targetOldPos = targetCurPos;
    }

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Space))
        //    StartShake(0.25f, target.transform.position);

        //if (Input.GetKeyDown(KeyCode.K))
        //    ZoomIn();
        //if (Input.GetKeyDown(KeyCode.L))
        //    ZoomOut();
    }

    Vector3 CalcCameraOffset()
    {
        Vector3 newOffset = Vector3.zero;

        if (trackingMode == TrackingMode.car)
        {
            newOffset = new Vector3(targetMoveDir.x, 0, targetMoveDir.y) * -25;
        }

        if (zoomOverride != 0)
        {
            newOffset.y = zoomOverride;
        }
        else
        {
            newOffset.y = targetMoveDir.sqrMagnitude * zoomSensitivity;
            newOffset.y = Mathf.Clamp(newOffset.y, zoomMin, zoomMax);
        }

        return newOffset;
    }

    public void ChangeTarget(GameObject newTarget)
    {
        target = newTarget;

        cameraBasePosition = target.transform.position;
        cameraBasePosition.y = 0;
        offset = CalcCameraOffset();
        transform.position = cameraBasePosition + offset;

        targetOldPos = new Vector2(target.transform.position.x, target.transform.position.z);
    }

    public void StartShake(float power, Vector3 origin)
    {
        StopAllCoroutines();
        StartCoroutine(ShakeCor(power, origin));
    }

    IEnumerator ShakeCor(float startPower, Vector3 origin)
    {
        float power = startPower;
        Vector3 dir = origin - transform.position;
        dir.y = 0;
        float dist = dir.magnitude;

        if(dist > 0)
            power /= dist;

        while (power > 0.1f)
        {
            shakeOffset = new Vector3(Random.Range(-power, power), Random.Range(-power, power), Random.Range(-power, power));
            power *= 0.9f;

            yield return null;
        }
    }

    public void SetTrackingMode(TrackingMode mode)
    {
        trackingMode = mode;
    }

    public void ZoomIn()
    {
        zoomOverride = 1;
    }

    public void ZoomOut()
    {
		StartCoroutine(ZoomOutCor());
	}

	IEnumerator ZoomOutCor()
	{
		yield return new WaitForSeconds(0.5f);
		zoomOverride = zoomMin;
		yield return new WaitForSeconds(0.5f);
		zoomOverride = 0;
	}
}
