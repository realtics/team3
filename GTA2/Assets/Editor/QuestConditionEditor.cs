using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


public class QuestConditionEditor : EditorWindow
{

    [MenuItem("Tools/Quest")]
    public static void Open()
    {
        GetWindow<WaypointManagerWindow>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
