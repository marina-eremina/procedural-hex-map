using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DelVorMap))]
public class DelVorMapEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DelVorMap mapGenerator = (DelVorMap)target;

        if (DrawDefaultInspector())
        {
            mapGenerator.GenerateMap();
        }

        if (GUILayout.Button("Generate Map"))
        {
            mapGenerator.GenerateMap();
        }

        if (GUILayout.Button("Destroy Map"))
        {
            mapGenerator.DestroyMap();
        }

    }
}
