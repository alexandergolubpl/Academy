using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class SceneButtons : EditorWindow
{
    private static float paddingX = 16;
    private static float paddingY = 16;
    private static float wid = 100;
    private static float height = 20;
    private static float btn_space = 4;
    private static int currNumButton = 0;
    private static Color[] colors = new Color[] { new Color(1f, 1f, 1f, 1f), new Color(1f, 0.2f, 0.2f, 0.8f), new Color(0.0f, 0.8f, 0.3f, 1.0f), new Color(0.5f, 0.8f, 1.0f, 0.8f) };
    private static bool isEnabled;
    private static bool isStarted;
    private static Rect Screct;

    private static GameObject StickerCard;

    private void onAwake()
    {
        if (GameObject.Find("CardBaseTool(Clone)") != null)
        {
            StickerCard = GameObject.Find("CardBaseTool(Clone)");
        }
        ShowButtons();
    }

    [MenuItem("AssetTools/Show Buttons")]
    public static void ShowButtons()
    {
        EditorApplication.projectWindowChanged += OnProjectChanged;

        //SceneView.duringSceneGui += OnSceneGUI;

        //SceneView.onSceneGUIDelegate += OnSceneGUI;
        isEnabled = true;
        if (!isStarted)
        {
            isStarted = true;
#if UNITY_2020_3_OR_NEWER
            SceneView.duringSceneGui += OnSceneGUI;
#else
            SceneView.onSceneGUIDelegate += OnSceneGUI;
#endif
        }
    }

    [MenuItem("AssetTools/Hide Buttons")]
    public static void HideButtons()
    {
        //SceneView.duringSceneGui -= OnSceneGUI;

        isEnabled = false;
    }
    
    private static void OnSceneGUI(SceneView sceneview)
    {
        if (isEnabled)
        {

            Handles.BeginGUI();
            Screct = sceneview.camera.pixelRect;

            //TODO
            //if changed anything https://docs.unity3d.com/ScriptReference/AssetPostprocessor.html
            GUI.backgroundColor = colors[3];
            if (GUI.Button(createSceneButtonRect(0), new GUIContent("Save Project", "Save Project")))
                EditorApplication.ExecuteMenuItem("File/Save Project");
            GUI.backgroundColor = colors[0];

            if (StickerCard != null && GUI.Button(createSceneButtonRect(1), new GUIContent("Ground Sticker", "Rotate Ground")))
                StickersGroundPos.Open();
            GUI.backgroundColor = colors[0];

            GUI.backgroundColor = colors[0];

            Handles.EndGUI();

        }
    }
    private static Rect createSceneButtonRect(int num)
    {
        var rect = new Rect(Screct.width - wid - paddingX, Screct.height - (height + btn_space) * num - 2 * paddingY, wid, height);
        return rect;
    }

    public static void OnProjectChanged()
    {
        Debug.Log("OnProjectChanged");
        ShowButtons();
    }
}