using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class AlignTransforms : EditorWindow, IPanelTool
{
    public string ToolName { get; set; }
    public AlignTransforms()
    {
        ToolName = typeof(AlignTransforms).Name;
    }

    public static void exec()
    {
        ShowAlignWindow();
    }
    public void Execute()
    {
        AlignTransforms.exec();
    }
    [MenuItem("AssetTools/AlignTransforms")]
    [MenuItem("GameObject/AssetTools/AlignTransforms", false, 0)]
    public static void ShowAlignWindow()
    {
        GetWindow<AlignTransforms>("AlignTransforms");
    }

    private string[] ButtonsTexts = new string[] { "  ", "/\\", "  ",
                                                   "<", "Х", ">",
                                                   "  ", "\\/", "  "};
    private string[] ButtonsHints = new string[] { "", "Horizontal Top", "",
                                                   "Left", "Horizontal Center", "Right",
                                                   "", "Horizontal Bottom", "" };
    private void OnGUI()
    {
        var GUIButtonOldalignment = GUI.skin.button.alignment;
        var GUIButtonOldfixedHeight = GUI.skin.button.fixedHeight;
        var GUIButtonOldfixedWidth = GUI.skin.button.fixedWidth;

        using (new GUILayout.HorizontalScope())
        {
            if (GUILayout.Button(new GUIContent("Hor", "Align Horizantally"), GUILayout.Width(64)))
            {
                int curri = 10;
                o_AlignTransforms("Hor");
            }
            if (GUILayout.Button(new GUIContent("Ver", "Align Vertically"), GUILayout.Width(64)))
            {
                int curri = 11;
                o_AlignTransforms("Ver");
            }
        }
        GUI.skin.button.alignment = TextAnchor.MiddleCenter;
        GUI.skin.button.fixedHeight = 20;
        GUI.skin.button.fixedWidth = 20;
        int s = 0;
        for (int i = 0; i < 3; i++)
        {
            using (new GUILayout.HorizontalScope())
            {
                for (int j = 0; j < 3; j++)
                {
                    //if (GUI.Button(new Rect(selectionRect.position.x - 30, selectionRect.position.y, 16, 16), new GUIContent(AssetDatabase.LoadAssetAtPath(markedObjectsIcon[i], typeof(Texture2D)) as Texture2D, tooltip), EditorStyles.miniLabel))

                    if (GUILayout.Button(new GUIContent(ButtonsTexts[s], ButtonsHints[s]), GUILayout.Width(30)))
                    {
                        int curri = s;
                    }
                    s++;
                }
            }
        }

        GUI.skin.button.alignment = GUIButtonOldalignment;
        GUI.skin.button.fixedHeight = GUIButtonOldfixedHeight;
        GUI.skin.button.fixedWidth = GUIButtonOldfixedWidth;
    }
    public void o_AlignTransforms(string AlignMode)
    {
        var alignDir = Vector4.zero;
        Bounds coord;
        switch (AlignMode)
        {
            case "Hor":
                alignDir = new Vector4(1, 0, 0, 0);
                coord = CalcBounds(Selection.gameObjects);
                break;

            case "Horizontal Top":
                alignDir = new Vector4(0, 1, 0, 0);
                coord = CalcBounds(Selection.gameObjects);
                break;

            default:
                break;
        }
        //var selection = Selection.gameObjects;
        //for (var i = selection.Length - 1; i >= 0; --i)
        //{
        //    var selected = selection[i];
        //    Undo.RegisterUndo(selected, "Reset transforms");
        //    Vector3 oldScale = selection[i].transform.localScale;
        //    RectTransform rectTr = selection[i].transform.GetComponent<RectTransform>();
        //    selection[i].transform.localScale = Vector3.one;
        //    rectTr.sizeDelta *= oldScale;
        //}
    }

    private Bounds CalcBounds(GameObject[] gos)
    {
        Bounds newBound = new Bounds(Vector3.zero, Vector3.zero);
        for (int i = 0; i < gos.Length; i++)
        {
            var tempBound = RectTransformUtility.CalculateRelativeRectTransformBounds(gos[i].transform);
            newBound.Encapsulate(tempBound);
        }
        return (newBound);
    }


}