using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;

public class RenameObjects : MonoBehaviour, IPanelTool
{
    public string ToolName { get; set; }

    public RenameObjects()
    {
        ToolName = typeof(RenameObjects).Name;
    }

    public void Execute()

    {
        RenameObjects.exec();
    }
    public static void exec()
    {
        ReName_auto();
    }


    [MenuItem("AssetTools/Rename Auto")]//%&t
    [MenuItem("GameObject/AssetTools/Rename Auto", false, 0)]
    public static void ReName_auto()
    {
        List<string> selectedObjects = new List<string>();
        foreach (GameObject o in Selection.objects)
        {
            selectedObjects.Add(o.name);
            Undo.RecordObject(o, "ReName Auto");
            string objectAutoName = o.name;
            if (o.GetComponent<Renderer>() != null) objectAutoName = o.GetComponent<Renderer>().sharedMaterial.name;
            if (o.GetComponent<MeshFilter>() != null) objectAutoName = o.GetComponent<MeshFilter>().sharedMesh.name;
            if (o.GetComponent<SpriteRenderer>() != null) objectAutoName = o.GetComponent<SpriteRenderer>().sprite.name;
            if (o.GetComponent<Image>() != null)    objectAutoName = o.GetComponent<Image>().sprite.name;
            if (o.GetComponent<TextMeshProUGUI>() != null) objectAutoName = o.GetComponent<TextMeshProUGUI>().text;
            if (o.GetComponent<Text>() != null) objectAutoName = o.GetComponent<Text>().text;
            o.name = objectAutoName;
        }
    }
}