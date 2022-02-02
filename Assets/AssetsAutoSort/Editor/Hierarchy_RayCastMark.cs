//AlexDove's tests

using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using System.Linq;

[InitializeOnLoad]
public class Hierarchy_RayCastMark
{
    private static string icon_folder = "Assets/AssetsAutoSort/HierarchyStyle/";

    private static string image_texture;
    private static string text_texture;
    private static string TMP_texture;
    private static string particle_texture;
    private static string raycast_texture;

    private static string blue_texture;
    private static string purple_texture;
    private static string yellow_texture;
    private static string green_texture;
    private static string grey_separator_texture;

    private static List<int> markedObjectsID;
    private static List<string> markedObjectsIcon;
    private static List<string> markedObjectsType;

    private static string hTab = "";
    //static string curTab = "";
    private static int curDepth = 0;

    private string hTree = "";


    private static List<GameObject> allGOs = new List<GameObject>();
    private static List<int> allGOsDepths = new List<int>();

    private static List<string> labelColors = new List<string>();
    //static int counterForColors;

    private static float timeOld;
    private static float timeInterval = 1.0f;
    private static bool timePaused;
    private static bool allowUpdate;
    private static bool isShowEnabled = true;

    private static Color bgColor = Color.yellow;
    private static Color defaultContentColor = GUI.contentColor;

    private static Texture2D icon_to_display;
    private static string tooltip;
    private static Texture2D raycast_texture2D;

    [MenuItem("GameObject/AssetTools/Hierarchy/Raycast AllowUpdate true", false, 0)]
    [MenuItem("AssetTools/Hierarchy/Raycast AllowUpdate true")]
    private static void EnableUpdateHierarchy()
    {
        allowUpdate = true;
    }
    [MenuItem("GameObject/AssetTools/Raycast AllowUpdate false", false, 0)]
    [MenuItem("AssetTools/Hierarchy/Raycast AllowUpdate false")]
    private static void DisableUpdateHierarchy()
    {
        allowUpdate = false;
    }
    [MenuItem("GameObject/AssetTools/Hierarchy/Raycast icons show", false, 0)]
    [MenuItem("AssetTools/Hierarchy/icons show")]
    private static void EnableUpdateHierarchyIcons()
    {
        isShowEnabled = true;
    }
    [MenuItem("GameObject/AssetTools/Hierarchy/Raycast icons hide", false, 0)]
    [MenuItem("AssetTools/Hierarchy/icons hide")]
    private static void DisableUpdateHierarchyIcons()
    {
        isShowEnabled = false;
    }

    [MenuItem("GameObject/AssetTools/Hierarchy/Update Hierarchy", false, 0)]
    [MenuItem("AssetTools/Hierarchy/Update Hierarchy Now %#U")]
    private static void UpdateHierarchy()
    {
        allowUpdate = true;
        timePaused = false;
        isShowEnabled = true;
    }

    static Hierarchy_RayCastMark()
    {
        raycast_texture = icon_folder + "icon_raycast.png";

        blue_texture = icon_folder + "icon_blue.png";
        purple_texture = icon_folder + "icon_purple.png";
        yellow_texture = icon_folder + "icon_yellow.png";
        green_texture = icon_folder + "icon_green.png";
        grey_separator_texture = icon_folder + "icon_grey_separator.png";

        labelColors.Add(blue_texture);
        labelColors.Add(purple_texture);
        labelColors.Add(yellow_texture);
        labelColors.Add(green_texture);

        EditorApplication.update += UpdateCB;
        EditorApplication.hierarchyWindowItemOnGUI += HierarchyItemCB;
    }

    private static void UpdateCB()
    {
        if ((Time.time > timeOld + timeInterval) && allowUpdate)
        {
            timePaused = false;
        }
        if (!timePaused)
        {
            timePaused = true;
            //Debug.Log(timeInterval + "s left. Updating tree");
            timeOld = Time.time;

            GameObject[] go = Object.FindObjectsOfTypeAll(typeof(GameObject)) as GameObject[];

            markedObjectsID = new List<int>();
            markedObjectsIcon = new List<string>();
            markedObjectsType = new List<string>();

            Texture2D raycast_texture2D = (Texture2D) AssetDatabase.LoadAssetAtPath(raycast_texture, typeof(Texture2D));

            foreach (GameObject g in go)
            {
                if (g.GetComponent<Graphic>() != null)
                {
                    if (g.GetComponent<Graphic>().raycastTarget)
                    {
                        markedObjectsID.Add(g.GetInstanceID());
                        markedObjectsIcon.Add(raycast_texture);
                        markedObjectsType.Add("Graphic");
                    }
                }
            }
        }
    }


    private static void HierarchyItemCB(int instanceID, Rect selectionRect)
    {
        int counterForColors = 0;

        if (markedObjectsID != null && markedObjectsID.Count > 1 && markedObjectsID.Contains(instanceID))
        {
            for (int i = 0; i < markedObjectsID.Count; i++)
            {
                if (markedObjectsID[i] == instanceID)
                {

                    Object obj = EditorUtility.InstanceIDToObject(instanceID);


                    GameObject go = obj as GameObject;
                    tooltip = "Hold Alt-Key to disable RayCast";
                    if (isShowEnabled)
                    {
                        if (GUI.Button(new Rect(selectionRect.position.x - 48, selectionRect.position.y, 32, 16), new GUIContent(AssetDatabase.LoadAssetAtPath(markedObjectsIcon[i], typeof(Texture2D)) as Texture2D, tooltip), EditorStyles.miniLabel))
                        {
                            if (Event.current.modifiers == (EventModifiers.Alt))
                            {
                                var cmp = go.GetComponent<Graphic>();
                                if (cmp != null)
                                {
                                    cmp.raycastTarget = false;
                                    EditorUtility.SetDirty(cmp);
                                    EditorApplication.RepaintHierarchyWindow();
                                }
                                var hierarchy = UnityEditorWindowHelper.GetWindow(WindowType.Hierarchy);
                                hierarchy.Repaint();
                                EditorWindow view = EditorWindow.GetWindow<SceneView>();
                                view.Repaint();
                            }
                        }
                    }
                }
            }
        }
    }
}