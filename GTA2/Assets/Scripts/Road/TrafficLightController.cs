using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrafficLightController : MonoBehaviour
{
    public List<TrafficLight> trafficLightList = new List<TrafficLight>();

    void OnEnable()
    {
        trafficLightList.Clear();

        foreach (Transform t in transform)
        {
            TrafficLight tl = t.GetComponent<TrafficLight>();
            if (tl == null)
                continue;
            trafficLightList.Add(tl);
        }

        StartCoroutine(SignalCor());
    }

    IEnumerator SignalCor()
    {
        while (true)
        {
            foreach (var light in trafficLightList)
            {
                light.ToggleSignal();
            }

            yield return new WaitForSeconds(7f);
        }
    }
}
