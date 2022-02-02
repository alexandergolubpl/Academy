using UnityEngine;
using UnityEditor;
using System.Collections;

[ExecuteInEditMode]
public class WireframeViewerEditor : Editor
{
    public Color wireframeBackgroundColor = Color.grey;
    public bool isInWireframeMode;
    private Camera cam;
    private CameraClearFlags origCameraClearFlags;
    private Color origBackgroundColor;
    private bool previousMode;

    private static bool initialized = false;

    private void Awake()
    {
        Debug.Log("Editor causes this Awake");
        Initialize();
    }

    private void Initialize()
    {
        Camera cam = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
        //cam = GetComponent<Camera>();
        origCameraClearFlags = cam.clearFlags;
        origBackgroundColor = cam.backgroundColor;
        previousMode = isInWireframeMode;

        if (initialized)
        { return; }

        // Your custom init code here...

        initialized = true;
    }

    [MenuItem("AssetTools/WireframeView")]
    private static void ToggleWireframe()
    {
        initialized = true;
        Debug.Log("initialized1 = " + initialized);
        //cam = GetComponent<Camera>();
        //origCameraClearFlags = cam.clearFlags;
        //origBackgroundColor = cam.backgroundColor;
        //previousMode = isInWireframeMode;
    }
    private void OnEnable()
    {
        Debug.Log("initialized OnEnable = " + initialized);

    }
    private void OnSceneGUI()
    {
        if (initialized)
        {
            Debug.Log("initialized OnSceneGUI = " + initialized);
            if (Input.GetKeyDown(KeyCode.Z))
            {
                Debug.Log("Z key was pressed");
                isInWireframeMode = !isInWireframeMode;
            }
            if (isInWireframeMode == previousMode)
            {
                return;
            }
            previousMode = isInWireframeMode;
            if (isInWireframeMode)
            {
                origCameraClearFlags = cam.clearFlags;
                origBackgroundColor = cam.backgroundColor;
                cam.clearFlags = CameraClearFlags.Color;
                cam.backgroundColor = wireframeBackgroundColor;
            }
            else
            {
                cam.clearFlags = origCameraClearFlags;
                cam.backgroundColor = origBackgroundColor;
            }

        }
    }

    void Update()
    {
        Debug.Log("Editor causes this Update");
    }
    //private void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.Z))
    //    {
    //        isInWireframeMode = !isInWireframeMode;
    //    }

    //    if (isInWireframeMode == previousMode)
    //    {
    //        return;
    //    }

    //    previousMode = isInWireframeMode;
    //    if (isInWireframeMode)
    //    {
    //        origCameraClearFlags = cam.clearFlags;
    //        origBackgroundColor = cam.backgroundColor;
    //        cam.clearFlags = CameraClearFlags.Color;
    //        cam.backgroundColor = wireframeBackgroundColor;
    //    }
    //    else
    //    {
    //        cam.clearFlags = origCameraClearFlags;
    //        cam.backgroundColor = origBackgroundColor;
    //    }
    //}

    private void OnPreRender()
    {
        if (isInWireframeMode)
        {
            GL.wireframe = true;
        }
    }

    private void OnPostRender()
    {
        if (isInWireframeMode)
        {
            GL.wireframe = false;
        }
    }
}