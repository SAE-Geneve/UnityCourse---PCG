using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[CustomEditor(typeof(BSPGenerator))]
public class BSPGenerator_Editor : Editor
{

    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();

        BSPGenerator bspGenerator = (BSPGenerator)target;

        DrawDefaultInspector();

        EditorGUILayout.Space(25);
        EditorGUILayout.LabelField("Generate tool", EditorStyles.boldLabel);

        if (GUILayout.Button("Generate"))
        {
            Debug.Log("generate baby !!!!!");
            bspGenerator.Generate();
        }
    }

}

