using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[CustomEditor(typeof(GeneratorPCG))]
public class GeneratorPCG_Editor : Editor
{

    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();

        GeneratorPCG generatorPcg = (GeneratorPCG)target;

        DrawDefaultInspector();

        EditorGUILayout.Space(25);
        EditorGUILayout.LabelField("Generate tool", EditorStyles.boldLabel);

        if (GUILayout.Button("Generate"))
        {
            Debug.Log("generate baby !!!!!");
            generatorPcg.StartGenerateCoroutines();
        }
    }

}
