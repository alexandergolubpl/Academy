using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class ReplaceWithPrefab : EditorWindow, IPanelTool
{
    [SerializeField] private GameObject prefab;
    private GameObject[] selection;
    private GameObject[] selectionNew;
    private bool isKeepTransform = true;

    public string ToolName { get; set; }
    public ReplaceWithPrefab()
    {
        ToolName = typeof(ReplaceWithPrefab).Name;
    }

    public static void exec()
    {
        CreateReplaceWithPrefab();
    }
    public void Execute()
    {
        ReplaceWithPrefab.exec();
    }
    [MenuItem("AssetTools/Replace With Prefab")]//&#s
    static void CreateReplaceWithPrefab()
    {
        EditorWindow.GetWindow<ReplaceWithPrefab>();
    }

    private void OnGUI()
    {
        prefab = (GameObject) EditorGUILayout.ObjectField("Prefab", prefab, typeof(GameObject), false);

        using (new GUILayout.HorizontalScope())
        {
            isKeepTransform = EditorGUILayout.Toggle(isKeepTransform);
            EditorGUILayout.LabelField("Keep Transform in Layout");
        }
        if (!prefab)
        {
            GUI.enabled = false;
        }
        else
        {
            GUI.enabled = true;
            if (GUILayout.Button("Replace"))
            {
                selection = Selection.gameObjects;
                selectionNew = Selection.gameObjects;

                for (var i = selection.Length - 1; i >= 0; --i)
                {
                    var selected = selection[i];

                    var selectedName = selection[i].name;
                    Undo.RecordObject(selected, "Undo Replace with Prefab object" + selected.name);
                    var prefabType = PrefabUtility.GetPrefabType(prefab);
                    GameObject newObject;

                    if (prefabType == PrefabType.Prefab)
                    {
                        newObject = (GameObject) PrefabUtility.InstantiatePrefab(prefab);
                    }
                    else
                    {
                        newObject = Instantiate(prefab);
                        newObject.name = prefab.name;
                    }
                    selectionNew[i] = newObject;
                    selectionNew[i].name = selectedName;

                    if (newObject == null)
                    {
                        Debug.LogError("Error instantiating prefab");
                        break;
                    }

                    Undo.RegisterCreatedObjectUndo(newObject, "Replace With Prefabs");
                    newObject.transform.parent = selected.transform.parent;
                    newObject.transform.localPosition = selected.transform.localPosition;
                    newObject.transform.localRotation = selected.transform.localRotation;
                    newObject.transform.localScale = selected.transform.localScale;
                    newObject.transform.SetSiblingIndex(selected.transform.GetSiblingIndex());

                    if (isKeepTransform)
                    {
                        var rectTransform = selected.GetComponent<RectTransform>();
                        float width = rectTransform.rect.width;
                        float height = rectTransform.rect.height;

                        RectTransform rectTransform2 = newObject.GetComponent(typeof(RectTransform)) as RectTransform;
                        rectTransform2.sizeDelta = new Vector2(width, height);
                    }

                    Undo.DestroyObjectImmediate(selected);
                }
                Selection.objects = selectionNew;
            }
        }
        GUI.enabled = false;
        EditorGUILayout.LabelField("Selection count: " + Selection.objects.Length);
        //gameObjects = selection;
    }
}