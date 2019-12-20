using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


public class WaypointManagerWindow : EditorWindow
{
    [MenuItem("Tools/웨이포인트 편집")]
    public static void Open()
    {
        GetWindow<WaypointManagerWindow>();
    }

    public Transform waypointCarRoot;
    public Transform waypointHumanRoot;

    GameObject wpConnectPosA = null;

    void OnGUI()
    {
        SerializedObject obj = new SerializedObject(this);
        EditorGUILayout.PropertyField(obj.FindProperty("waypointCarRoot"));
        EditorGUILayout.PropertyField(obj.FindProperty("waypointHumanRoot"));

        EditorGUILayout.BeginVertical("box");
        DrawButtons();
        EditorGUILayout.EndVertical();

        obj.ApplyModifiedProperties();
    }

    void DrawButtons()
    {
        if (GUILayout.Button("웨이포인트(차) 생성", GUILayout.Height(40)))
        {
            CreateCarWaypoint();
        }
        if (GUILayout.Button("웨이포인트(사람) 생성", GUILayout.Height(40)))
        {
            CreateHumanWaypoint();
        }
        if (GUILayout.Button("웨이포인트 제거", GUILayout.Height(40)))
        {
            RemoveWaypoint();
        }

        if(wpConnectPosA == null)
        {
            if (GUILayout.Button("웨이포인트 연결/시작점", GUILayout.Height(40)))
            {
                SelectConnectPositionA();
            }
        }
        else
        {
            if (GUILayout.Button("웨이포인트 연결/끝점", GUILayout.Height(40)))
            {
                ConnectWaypoint();
            }
        }

		if(GUILayout.Button("차선 위치 재계산", GUILayout.Height(40)))
		{
			RecalcAllLanePositions();
		}

		GUILayout.Space(20);

		if (GUILayout.Button("정렬", GUILayout.Height(40)))
		{
			//Align();
		}
	}

	void FindWaypointCarRoot()
	{
		waypointCarRoot = GameObject.Find("[WaypointforCar]").transform;
	}
	void FindWaypointHumanRoot()
	{
		waypointHumanRoot = GameObject.Find("[WaypointforHuman]").transform;
	}

    void CreateCarWaypoint()
    {
		if (waypointCarRoot == null)
			FindWaypointCarRoot();

        GameObject waypointObj = new GameObject("WPC " + waypointCarRoot.childCount, typeof(WaypointForCar));
        waypointObj.tag = "waypoint";
        waypointObj.transform.SetParent(waypointCarRoot, false);

        WaypointForCar newWaypoint = waypointObj.GetComponent<WaypointForCar>();

        GameObject go = Selection.activeGameObject;
        if (go != null)
        {
            WaypointForCar selectedWaypoint = go.GetComponent<WaypointForCar>();
            if (selectedWaypoint != null)
            {
                waypointObj.transform.position = selectedWaypoint.transform.position + new Vector3(1, 0, 1);

                selectedWaypoint.AddBranch(newWaypoint);
            }
        }

        Selection.activeGameObject = waypointObj.gameObject;
    }
    void CreateHumanWaypoint()
    {
		if (waypointHumanRoot == null)
			FindWaypointHumanRoot();

		GameObject waypointObj = new GameObject("WPH " + waypointHumanRoot.childCount, typeof(WaypointForHuman));
        waypointObj.tag = "waypointForHuman";
        waypointObj.transform.SetParent(waypointHumanRoot, false);
        WaypointForHuman newWaypoint = waypointObj.GetComponent<WaypointForHuman>();

        GameObject go = Selection.activeGameObject;
        if (go != null)
        {
            WaypointForHuman selected = go.GetComponent<WaypointForHuman>();
            if (selected != null)
            {
                waypointObj.transform.position = selected.transform.position + new Vector3(1, 0, 1);
                selected.neighbor.Add(newWaypoint);
                newWaypoint.neighbor.Add(selected);
            }
        }

        Selection.activeGameObject = waypointObj.gameObject;
    }

    void RemoveWaypoint()
    {
        Waypoint selectedWaypoint = Selection.activeGameObject.GetComponent<Waypoint>();
        if (selectedWaypoint == null)
            return;

        if (selectedWaypoint is WaypointForCar)
        {
            WaypointForCar wpc = (WaypointForCar)selectedWaypoint;

            foreach (WaypointForCar wp in wpc.next)
            {
                wp.prev.Remove(wpc);
            }
            foreach (WaypointForCar wp in wpc.prev)
            {
                wp.RemoveBranch(wpc);
            }
        }
        else if(selectedWaypoint is WaypointForHuman)
        {
            WaypointForHuman wph = (WaypointForHuman)selectedWaypoint;

            foreach (WaypointForHuman wp in wph.neighbor)
            {
                wp.neighbor.Remove(wph);
            }
        }

        DestroyImmediate(selectedWaypoint.gameObject);
    }

    void SelectConnectPositionA()
    {
        GameObject selected = Selection.activeGameObject;
        if(selected.GetComponent<Waypoint>() != null)
        {
            wpConnectPosA = selected;
            Debug.Log("웨이포인트 연결: 시작 지점 선택됨");
        }        
    }

    void ConnectWaypoint()
    {
        GameObject wpConnectPosB = Selection.activeGameObject;

        if(wpConnectPosA == wpConnectPosB)
        {
            Debug.LogWarning("시작점과 끝점이 같음");
            wpConnectPosA = null;
            return;
        }

        Waypoint wp1 = wpConnectPosA.GetComponent<Waypoint>();
        Waypoint wp2 = wpConnectPosB.GetComponent<Waypoint>();

        if (wp1 == null || wp2 == null)
        {
            Debug.LogWarning("시작점 또는 끝점이 waypoint가 아님");
            wpConnectPosA = null;
            return;
        }            
        if (wp1.GetType() != wp2.GetType())
        {
            Debug.LogWarning("두 웨이포인트의 타입이 다름");
            wpConnectPosA = null;
            return;
        }            

        if(wp1 is WaypointForCar)
        {
            WaypointForCar wpc1 = (WaypointForCar)wp1;
            WaypointForCar wpc2 = (WaypointForCar)wp2;
            wpc1.AddBranch(wpc2);

            wpConnectPosA = null;
        }
        else if(wp1 is WaypointForHuman)
        {
            WaypointForHuman wph1 = (WaypointForHuman)wp1;
            WaypointForHuman wph2 = (WaypointForHuman)wp2;

            if(!wph1.neighbor.Contains(wph2))
                wph1.neighbor.Add(wph2);

            if (!wph2.neighbor.Contains(wph1))
                wph2.neighbor.Add(wph1);

            wpConnectPosA = null;
        }
    }

	void RecalcAllLanePositions()
	{
		CarRoad[] allRoads = GameObject.FindObjectsOfType<CarRoad>();

		foreach (var r in allRoads)
		{
			r.CalcLanePos();
		}
	}

	void AlignX()
	{
		GameObject[] goArray = Selection.gameObjects;

	}
}
