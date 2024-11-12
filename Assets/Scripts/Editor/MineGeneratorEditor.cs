using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MineGenerator))]
public class MineGeneratorEditor : Editor {
    public override void OnInspectorGUI() {
        DrawDefaultInspector();

        MineGenerator generator = (MineGenerator)target;
        if (GUILayout.Button("Generate Mine")) {
            generator.GenerateMine();
        }
    }
}