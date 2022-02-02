using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class GroupObjects : MonoBehaviour, IPanelTool
{
    public string ToolName { get; set; }
    public static GameObject objgroup;
    // Add a new menu item that is accessed by right-clicking on an asset in the project view

    public GroupObjects()
    {
        ToolName = typeof(GroupObjects).Name;
    }

    public void Execute()
    {
        GroupObjects.exec();
    }
    public static void exec()
    {
        //Group_Objects();
    }

    [MenuItem("AssetTools/Group Objects %&g")]
    [MenuItem("GameObject/AssetTools/Group Objects")]
    [ContextMenu("GroupObjects")]

    public static void Group_Objects(MenuCommand menuCommand)
    {
        if (Selection.objects.Length > 1)
        {
            if (menuCommand.context != Selection.objects[0])
            {
                return;
            }
        }
        //List<string> selectedObjects = new List<string>();
        //objgroup = new GameObject();
        var objgroup = new GameObject(Selection.gameObjects[0].name + "_gr", typeof(RectTransform));

        RectTransform obgrectr = objgroup.GetComponent<RectTransform>();
        //objgroup.name = Selection.gameObjects[0].name + "_gr";
        Undo.RegisterFullObjectHierarchyUndo(Selection.gameObjects[0].transform.parent, "Group_selected_object");
        objgroup.transform.parent = Selection.gameObjects[0].transform.parent;

        obgrectr.transform.localPosition = Vector2.zero;
        //obgrectr.anchorMin = new Vector2(0, 0);
        //obgrectr.anchorMax = new Vector2(1, 1);
        //obgrectr.offsetMin = new Vector2(0, 0);
        //obgrectr.offsetMax = new Vector2(0, 0);
        obgrectr.localScale = new Vector2(1.0f, 1.0f);

        var myBounds = new Bounds(Vector3.zero,Vector3.zero);
        foreach (GameObject child in Selection.gameObjects)
        {
            myBounds.Encapsulate(child.transform.localPosition);
        }
        Rect obgrrect = obgrectr.rect;
        obgrrect.center = myBounds.center;
        obgrrect.size = myBounds.size;
        foreach (GameObject o in Selection.objects)
        {
            //Undo.RecordObject(o, "Group_selected_objects");
            o.transform.SetParent(objgroup.transform);
        }
        Undo.RegisterCreatedObjectUndo(objgroup, "Create group");
    }
}
