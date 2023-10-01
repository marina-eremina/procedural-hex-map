using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PoissonExample))]
public class PoissonExampleEditor : Editor
{
    public override void OnInspectorGUI()
    {
        PoissonExample poissonExample = (PoissonExample)target;

        if (DrawDefaultInspector())
        {
            poissonExample.GenerateImage();
        }

        if (GUILayout.Button("Generate Map"))
        {
            poissonExample.GenerateImage();
        }


    }
}
