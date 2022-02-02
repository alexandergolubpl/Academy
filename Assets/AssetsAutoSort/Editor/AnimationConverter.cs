using UnityEngine;
using UnityEditor;
using System.Collections;

public class AnimationConverter : EditorWindow
{
    AnimationClip target;
    SerializedProperty animationType;

    [MenuItem("Window/AnimationConverter")]
    static void OpenWindow() {
        // Get existing open window or if none, make a new one:
        EditorWindow.GetWindow(typeof(AnimationConverter));

    }

    void OnGUI() {
        target = EditorGUILayout.ObjectField("Target", target, typeof(AnimationClip), true) as AnimationClip;

        if (GUILayout.Button("Convert")) {
            SerializedObject serializedObject = new SerializedObject(target);
            animationType = serializedObject.FindProperty("m_AnimationType");
            animationType.intValue = 2;
            serializedObject.ApplyModifiedProperties();

            EditorCurveBinding[] allCurves = AnimationUtility.GetCurveBindings(target);

            foreach (var data in allCurves) {
                Debug.Log("path" + data.path);
                Debug.Log("propertyName" + data.propertyName);
                Debug.Log("type" + data.type);

                if (data.type == typeof(UnityEngine.Material)) {
                    AnimationCurve curve = AnimationUtility.GetEditorCurve(target, data);

                    EditorCurveBinding newBindings = new EditorCurveBinding();
                    newBindings.path = data.path;
                    newBindings.propertyName = "material." + data.propertyName;
                    newBindings.type = typeof(UnityEngine.MeshRenderer);

                    AnimationUtility.SetEditorCurve(target, newBindings, curve);
                }

                AnimationUtility.SetEditorCurve(target, data, null);

            }
        }
    }
}