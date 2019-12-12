using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


public class QuestConditionEditor : EditorWindow
{
    public GameObject killPref;
    public GameObject arrivePref;
    public GameObject carryPref;



    [MenuItem("Tools/Quest")]
    public static void Open()
    {
        GetWindow<QuestConditionEditor>();
    }

    void OnGUI()
    {
        SerializedObject obj = new SerializedObject(this);
        EditorGUILayout.PropertyField(obj.FindProperty("killPref"));
        EditorGUILayout.PropertyField(obj.FindProperty("arrivePref"));
        EditorGUILayout.PropertyField(obj.FindProperty("carryPref"));

        EditorGUILayout.BeginVertical("box");
        DrawButtons();
        EditorGUILayout.EndVertical();

        obj.ApplyModifiedProperties();
    }

    void DrawButtons()
    {
        if (GUILayout.Button("Create Kill Mission", GUILayout.Height(40)))
        {
            CreateKillMission();
        }
        if (GUILayout.Button("Create Arrive Mission", GUILayout.Height(40)))
        {
            // CreateHumanWaypoint();
        }
        if (GUILayout.Button("Create Carry Mission", GUILayout.Height(40)))
        {
            // RemoveWaypoint();
        }

       
        GUILayout.Space(10);

    }
    void CreateKillMission()
    {
        Quest newQuest = Instantiate(killPref).GetComponent<Quest>();
        newQuest.transform.parent = QuestManager.Instance.transform;
    }
}
