using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CopyGameObjectPath : EditorWindow, IPanelTool
{
    public static GameObject obj;
    public string ToolName
    {
        get;
        set;
    }

    public static string paths = "";

    public CopyGameObjectPath()
    {
        ToolName = typeof(CopyGameObjectPath).Name;
    }

    public void Execute()
    {
        CopyGameObjectPath.exec();
    }
    public static void exec()
    {
        CopyPath();
    }

    [MenuItem("GameObject/AssetTools/Copy GameObject Path", false, 0)]
    [MenuItem("AssetTools/Copy GameObject Path")]
    private static void CopyPath()
    {
        Object[] obs = Selection.objects;
        //for (int i = 0; i < obs.Length; i++)        {
        foreach (Object curr_ob in obs)
        {
            //Object curr_ob as GameObject;//= obs[curr_ob];
            //Selection.activeGameObject = curr_ob as GameObject;
            //Debug.Log("curr_ob.name = " + curr_ob.name);
            var go = curr_ob as GameObject;
            Selection.activeGameObject = null;
            Selection.activeGameObject = go;
            if (go == null)
            {
                return;
            }
            var path = "/" + go.name;
            while (go.transform.parent != null)
            {
                go = go.transform.parent.gameObject;
                if (go.transform.parent == null)
                {
                    path = "" + go.name + path;
                }
                else
                {
                    path = "/" + go.name + path;
                }
                EditorGUIUtility.systemCopyBuffer = path;
            }
        }
    }

    //[MenuItem("GameObject/AssetTools/Copy GameObject Hierarchy", false, 0)]
    //[MenuItem("AssetTools/Copy GameObject Hierarchy")]
    private static void CopyHierarchy()
    {
        Object[] obs = Selection.objects;
        paths = "";
        var curTab = "";
            GetAllChildren(Selection.activeGameObject.transform, curTab);
            Debug.Log("GameObject Hierarchy copied in buffer: '\n" + paths +"\n");
            EditorGUIUtility.systemCopyBuffer = paths;
    }
    private static void GetAllChildren(Transform current, string curTab)
    {
        paths += curTab + current.gameObject.name + "\n";
        curTab += "\t";
        for (int i = 0; i < current.childCount; i++)
        {
            GetAllChildren(current.GetChild(i), curTab);
        }
    }
}