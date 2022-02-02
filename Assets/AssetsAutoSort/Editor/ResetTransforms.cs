using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class ResetTransforms : EditorWindow, IPanelTool
{
    [SerializeField] private GameObject prefab;

    public string ToolName { get; set; }
    public ResetTransforms()
    {
        ToolName = typeof(ResetTransforms).Name;
    }

    public static void exec()
    {
        o_ResetTransforms();
    }
    public void Execute()
    {
        ResetTransforms.exec();
    }
    [MenuItem("AssetTools/Reset transforms %#z")]
    [MenuItem("GameObject/AssetTools/Reset transforms", false, 0)]
    public static void o_ResetTransforms()
    {
        //EditorWindow.GetWindow<ResetTransforms>();

        var selection = Selection.gameObjects;
        for (var i = selection.Length - 1; i >= 0; --i)
        {
            var selected = selection[i];
            Undo.RegisterUndo(selected, "Reset transforms");

            //selected.transform.localPosition = Vector3.zero;
            //selection[i].transform.localRotation = Quaternion.Euler(Vector3.zero);
            Vector3 oldScale = selection[i].transform.localScale;
            RectTransform rectTr = selection[i].transform.GetComponent<RectTransform>();
            selection[i].transform.localScale = Vector3.one;
            rectTr.sizeDelta *= oldScale;
        }
    }
}