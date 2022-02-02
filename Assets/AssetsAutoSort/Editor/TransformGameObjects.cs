// TransformGameObjects.cs
// Unity Editor extension that allows transform all GameObjects

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

public class TransformGameObjects : EditorWindow, IPanelTool
{
    public bool isReplaceX = false;
    public bool isReplaceY = false;
    public bool isReplaceZ = true;

    public bool isRoundValues = true;
    public int RoundToDigits = 0;

    public string[] radioText = new string[] { "selected", "hierarchy", "all" };
    private int transformTypeNum;

    public Vector3 NewCoords;
    public Vector3 NewScale;
    public Vector3 NewRotation;
    public int StartNumber = 0;

    public float NewX;
    public float NewY;
    public float NewZ;

    private Vector2 scrollPosition;
    private static string AssetDetailsArea = "";
    private bool IsAllowUpdateOnce = false;

    public string ToolName { get; set; }
    //private bool _styleInitialized = false;

    private List<GameObject> selGo = new List<GameObject>();
    private GameObject[] objectsToCopy;

    //private static Dictionary<int, Transform> obTr = new Dictionary<int, Transform>();

    public TransformGameObjects()
    {
        ToolName = typeof(TransformGameObjects).Name;
    }

    public void Execute()
    {
        TransformGameObjects.exec();
    }
    public static void exec()
    {
        Open();
    }
    /*private void OnEnable()
    {
    }*/

    [MenuItem("AssetTools/Transform GameObjects")]
    [MenuItem("GameObject/Transform/TransformGameObjects", false, 0)]
    private static void Open()
    {
        EditorWindow.GetWindow<TransformGameObjects>("Transform GameObjects");
    }
    private GUIStyle style;
    private void OnDisable()
    {
        //_styleInitialized = false;
    }
    private void InitStyles()
    {
        if (EditorStyles.helpBox == null)
        {
            style = new GUIStyle(GUI.skin.box);
        }
        else
        {
            style = new GUIStyle(EditorStyles.helpBox);
        }
        style.richText = true;
        style.fontSize = 10;
        //_styleInitialized = true;
    }

    private void CopyObjectsInCoords()
    {
        selGo = Selection.gameObjects.ToList<GameObject>();

        objectsToCopy = selGo.OrderBy(go => go.transform.GetSiblingIndex()).ToArray();
        var objectsStrToCopy = selGo.OrderBy(go => go.transform.GetSiblingIndex()).Select(s => s.name).ToArray();
        //obTr.Add(selGo[i].GetInstanceID(), gotr); //Debug.Log(obTr[id]); //Debug.Log(i.Key); //Debug.Log(i.Value);
        string word = objectsStrToCopy.Select(i => i.ToString()).Aggregate((i, j) => i + "\n" + j);
        AssetDetailsArea = "";
        AssetDetailsArea = word;
    }
    private void PasteObjectsInCoords()
    {
        //Undo.SetCurrentGroupName("Copy Scene objects");
        //int group2 = Undo.GetCurrentGroup();
        //Undo.RecordObjects(Selection.transforms, "Copy selected objects");
        for (int i = 0; i < objectsToCopy.Length; i++)
        {
            GameObject go = GameObject.Instantiate(objectsToCopy[i]);
            go.name = objectsToCopy[i].name;
            go.transform.parent = Selection.gameObjects[0].transform;
            go.transform.localPosition = objectsToCopy[i].transform.localPosition;
            go.transform.localRotation = objectsToCopy[i].transform.localRotation;
            go.transform.localScale = objectsToCopy[i].transform.localScale;

            if (objectsToCopy[i].GetComponent<RectTransform>() != null)
            {
                go.GetComponent<RectTransform>().localPosition = objectsToCopy[i].GetComponent<RectTransform>().localPosition;
                go.GetComponent<RectTransform>().localRotation = objectsToCopy[i].GetComponent<RectTransform>().localRotation;
                go.GetComponent<RectTransform>().localScale = objectsToCopy[i].GetComponent<RectTransform>().localScale;
            }

            //TransformGo(go);
            Undo.RegisterCreatedObjectUndo(go, "Copy selected objects");
        }

        //Undo.CollapseUndoOperations(group2);
    }
    private void OnGUI()
    {

        using (new GUILayout.HorizontalScope())//helpBox
        {
            isRoundValues = GUILayout.Toggle(isRoundValues, (isRoundValues) ? "Round Values" : "Round Values");
            RoundToDigits = EditorGUILayout.IntField(RoundToDigits);
        }

        EditorGUILayout.LabelField("New Pos");

        using (new GUILayout.HorizontalScope())//helpBox
        {
            using (new GUILayout.VerticalScope())
            {
                NewX = EditorGUILayout.FloatField(NewX);
                isReplaceX = GUILayout.Toggle(isReplaceX, "Replace X");
            }

            using (new GUILayout.VerticalScope())
            {
                NewY = EditorGUILayout.FloatField(NewY);
                isReplaceY = GUILayout.Toggle(isReplaceY, "Replace Y");
            }

            using (new GUILayout.VerticalScope())
            {
                NewZ = EditorGUILayout.FloatField(NewZ);
                isReplaceZ = GUILayout.Toggle(isReplaceZ, "Replace Z");
            }
        }

        transformTypeNum = GUILayout.SelectionGrid(transformTypeNum, radioText, 1, EditorStyles.radioButton);//text.Length

        if (GUILayout.Button("Copy"))
        {
            CopyObjectsInCoords();
        }
        if (GUILayout.Button("Paste In Place"))
        {
            PasteObjectsInCoords();
        }
        scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, true/*, GUILayout.Height(area.height)*/);
        AssetDetailsArea = GUILayout.TextArea(AssetDetailsArea, /*style,*/ GUILayout.ExpandHeight(true));
        EditorGUILayout.EndScrollView();

        if (GUILayout.Button(" Transform "))
        {
            IsAllowUpdateOnce = true;
        }
        if (Selection.objects == null)
            return;

        if (IsAllowUpdateOnce)
        {
            //Undo.RecordObjects(mySelection.ToArray(), "UndoTransformGameObjects");

            int v = 0;
            string tempStr = "";


            Undo.SetCurrentGroupName("Transform Scene objects");
            int group = Undo.GetCurrentGroup();

            Undo.RecordObjects(Selection.transforms, "transform selected objects");

            switch (radioText[transformTypeNum])
            {
                case "all":

                    foreach (GameObject go in Resources.FindObjectsOfTypeAll(typeof(GameObject)) as GameObject[])
                    {
                        if (go.hideFlags != HideFlags.None)
                            continue;
                        if (PrefabUtility.GetPrefabType(go) == PrefabType.Prefab || PrefabUtility.GetPrefabType(go) == PrefabType.ModelPrefab)
                            continue;

                        tempStr += go.name + "\n";
                        TransformGo(go);
                    }
                    AssetDetailsArea = tempStr;

                    break;

                case "selected":

                    List<GameObject> mySelection = new List<GameObject>(Selection.gameObjects);
                    mySelection.Sort((go1, go2) => go1.transform.GetSiblingIndex().CompareTo(go2.transform.GetSiblingIndex()));
                    foreach (GameObject go in mySelection)
                    {
                        tempStr += go.name + "\n";
                        TransformGo(go);
                    }
                    AssetDetailsArea = tempStr;

                    break;
                case "hierarchy":

                    //Object[] obs = Selection.objects;
                    paths = "";
                    var curTab = "";
                    GetAllChildren(Selection.activeGameObject.transform, curTab);
                    AssetDetailsArea = paths;

                    break;

                default:
                    break;
            }
            Undo.CollapseUndoOperations(group);
            IsAllowUpdateOnce = false;
            //Debug.Log("AssetDetailsArea = " + AssetDetailsArea);


        }
    }
    private void GetAllChildren(Transform current, string curTab)
    {
        paths += curTab + current.gameObject.name + "\n";
        GameObject go = current.gameObject;
        if (radioText[transformTypeNum] == "hierarchy")
        {
            TransformGo(go);
        }
        curTab += "    ";
        for (int i = 0; i < current.childCount; i++)
        {
            GetAllChildren(current.GetChild(i), curTab);
        }
    }

    public void TransformGo(GameObject go)
    {
        if (go.GetComponent<RectTransform>() != null)//go.activeInHierarchy &&
        {


            Undo.RecordObject(go, "Transform " + go.name);
            var gort = go.GetComponent<RectTransform>();
            var gortpos = gort.localPosition;
            var golp = go.transform.localPosition;
            //golp = new Vector3(golp.x, golp.y, NewZ);
            var RoundToDigits2 = Mathf.Pow(10, RoundToDigits);
            if (isRoundValues)
            {
                gortpos.x = Mathf.Round(gortpos.x * RoundToDigits2) / RoundToDigits2;
                gortpos.y = Mathf.Round(gortpos.y * RoundToDigits2) / RoundToDigits2;
                gortpos.z = Mathf.Round(gortpos.z * RoundToDigits2) / RoundToDigits2;


                //go.GetComponent<RectTransform>().anchoredPosition = 1;
                go.GetComponent<RectTransform>().anchoredPosition = new Vector2(Mathf.Round(gort.anchoredPosition.x), Mathf.Round(gort.anchoredPosition.y));
                go.GetComponent<RectTransform>().sizeDelta = new Vector2(Mathf.Round(gort.sizeDelta.x), Mathf.Round(gort.sizeDelta.y));
            }
            var toPosX = 0f;
            var toPosY = 0f;
            var toPosZ = 0f;

            toPosX = (isReplaceX) ? NewX : 0f;
            toPosY = (isReplaceY) ? NewY : 0f;
            toPosZ = (isReplaceZ) ? NewZ : 0f;

            toPosX = (isReplaceX) ? gortpos.x : gortpos.x + toPosX;
            toPosY = (isReplaceY) ? gortpos.y : gortpos.y + toPosY;
            toPosZ = (isReplaceZ) ? gortpos.z : gortpos.z + toPosZ;
            go.GetComponent<RectTransform>().localPosition = new Vector3(toPosX + NewX, toPosY + NewY, toPosZ + NewZ);
            go.GetComponent<RectTransform>().anchoredPosition = new Vector2(toPosX + NewX, toPosY + NewY);
        }
    }

    public static string paths = "";

    //[MenuItem("GameObject/AssetTools/Copy GameObject Hierarchy", false, 0)]
    //[MenuItem("AssetTools/Copy GameObject Hierarchy")]
    private static void CopyHierarchy()
    {
        Object[] obs = Selection.objects;
        paths = "";
        var curTab = "";
        GetAllChildrenStatic(Selection.activeGameObject.transform, curTab);
        Debug.Log("GameObject Hierarchy copied in buffer: '\n" + paths + "\n");
        EditorGUIUtility.systemCopyBuffer = paths;
    }
    private static void GetAllChildrenStatic(Transform current, string curTab)
    {
        paths += curTab + current.gameObject.name + "\n";
        curTab += "\t";
        for (int i = 0; i < current.childCount; i++)
        {
            GetAllChildrenStatic(current.GetChild(i), curTab);
        }
    }
}