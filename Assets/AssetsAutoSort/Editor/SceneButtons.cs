using UnityEditor;
using UnityEngine;

public class SceneButtons : EditorWindow
{
    private static float paddingX = 16;
    private static float paddingY = 16;
    private static float wid = 100;
    private static float height = 20;
    private static Color[] colors = new Color[] { new Color(1f, 1f, 1f, 1f), new Color(1f, 0.2f, 0.2f, 0.8f), new Color(0.0f, 0.8f, 0.3f, 1.0f), new Color(0.5f, 0.8f, 1.0f, 0.8f) };
    [MenuItem("AssetTools/Add Buttons")]
    public static void Enable()
    {
        //SceneView.duringSceneGui += OnSceneGUI;

        //SceneView.onSceneGUIDelegate += OnSceneGUI;


#if UNITY_2020_3_OR_NEWER
        SceneView.duringSceneGui += OnSceneGUI;
#else
        SceneView.onSceneGUIDelegate += OnSceneGUI;
#endif
    }

    [MenuItem("AssetTools/Hide Buttons")]
    public static void Disable()
    {
        //SceneView.duringSceneGui -= OnSceneGUI;

#if UNITY_2020_3_OR_NEWER
        SceneView.duringSceneGui -= OnSceneGUI;
#else
        SceneView.onSceneGUIDelegate -= OnSceneGUI;
#endif
    }

    private static void OnSceneGUI(SceneView sceneview)
    {
        Handles.BeginGUI();
        Rect Screct = sceneview.camera.pixelRect;

        //TODO
        //if changed anything https://docs.unity3d.com/ScriptReference/AssetPostprocessor.html
        GUI.backgroundColor = colors[3];
        if (GUI.Button(new Rect(Screct.width - wid - paddingX, Screct.height - height - paddingY, wid, height), new GUIContent("Save Project", "Save Project")))
            EditorApplication.ExecuteMenuItem("File/Save Project");
        GUI.backgroundColor = colors[0];
        Handles.EndGUI();

    }
}