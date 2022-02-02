using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;
using System.IO;
using UnityEngine.UI;

public class CommonUITool : EditorWindow, IPanelTool
{
    private Color[] colors = new Color[] { new Color(1f, 1f, 1f, 1f), new Color(0.5f, 0.8f, 0.0f, 1f), new Color(1f, 0.1f, 0.1f, 1f), new Color(1f, 0.8f, 0.0f, 1f) };


    [SerializeField] private GameObject prefab;
    private GameObject[] selection;
    private GameObject[] selectionNew;
    private bool isKeepTransform = true;

    private List<string> CommonButtonOkPaths;
    private List<string> CommonButtonCancelPaths;
    private Object CommonFont;
    private Object CommonButtonOk;
    private Object CommonButtonCancel;

    //private int curr_CommonFontPath = 0;
    private int curr_CommonButtonOkPath = 0;
    //private int curr_CommonButtonCancelPath = 0;

    private int ButtonWidthShort = 24;
    private int ButtonWidthMed = 64;

    public string ToolName { get; set; }
    public CommonUITool()
    {
        ToolName = typeof(CommonUITool).Name;
    }

    public void Execute()
    {
        CommonUITool.exec();
    }
    public static void exec()
    {
        Start();
    }

    [MenuItem("AssetTools/Common UI Tool")]
    [MenuItem("GameObject/AssetTools/Common UI Tool")]
    private static void Start()
    {
        EditorWindow.GetWindow<CommonUITool>("Common UI Tool");
    }
    private void OnEnable()
    {
        CommonButtonOkPaths.Clear();
        CommonButtonOkPaths.Add("Assets/Bundles/Streaming/JackpotSticker/JackpotStickerMinigame/Prefabs/BtnClaim.prefab");
        CommonButtonOkPaths.Add("Assets/Game/Features/PowerCardsQuick/Content/Prefabs/Etc/button_purchase.prefab");
        CommonButtonOkPaths.Add("Assets/Game/Features/DecoyOffer/Content/Prefabs/ButtonBuy.prefab");

        //ReplaceAssetByNew();
    }

     
    private void ReplaceAssetByNew()
    {
        CommonButtonOk = AssetDatabase.LoadAssetAtPath<GameObject>(CommonButtonOkPaths[curr_CommonButtonOkPath]);
        selection = Selection.gameObjects;
        selectionNew = Selection.gameObjects;
        Object prefab = CommonButtonOk;
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
                newObject = Instantiate((GameObject) prefab);
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
    private void OnGUI()
    {
        using (new GUILayout.HorizontalScope())
        {
            //EditorGUILayout.LabelField("Font:");

            GUI.backgroundColor = colors[2];
            CommonFont = EditorGUILayout.ObjectField("Font", CommonFont, typeof(Object), false);
            GUI.backgroundColor = colors[1];
            if (GUILayout.Button("<"/*, contentStyle,*/, GUILayout.Width(ButtonWidthShort)))
            {
            }
            if (GUILayout.Button(">"/*, contentStyle,*/, GUILayout.Width(ButtonWidthShort)))
            {
            }
            GUILayout.Space(5);
            if (GUILayout.Button("Update"/*, contentStyle,*/, GUILayout.Width(ButtonWidthMed)))
            {
            }
            GUI.backgroundColor = colors[0];
        }
        using (new GUILayout.HorizontalScope())
        {
            GUI.backgroundColor = colors[3];
            CommonButtonOk = EditorGUILayout.ObjectField("ButtonOk", CommonButtonOk, typeof(Object), false);
            GUI.backgroundColor = colors[1];

            EditorGUI.BeginChangeCheck();
            if (GUILayout.Button("<"/*, contentStyle,*/, GUILayout.Width(ButtonWidthShort)))
            {
                curr_CommonButtonOkPath--;
                curr_CommonButtonOkPath = checkUpdateCounter(curr_CommonButtonOkPath, CommonButtonOkPaths.Count);
            }
            EditorGUILayout.LabelField("" + curr_CommonButtonOkPath, GUILayout.Width(ButtonWidthShort));
            if (GUILayout.Button(">"/*, contentStyle,*/, GUILayout.Width(ButtonWidthShort)))
            {
                curr_CommonButtonOkPath++;
                curr_CommonButtonOkPath = checkUpdateCounter(curr_CommonButtonOkPath, CommonButtonOkPaths.Count);
            }
            if (EditorGUI.EndChangeCheck())
            {
                ReplaceAssetByNew();
            }
            GUILayout.Space(5);
            if (GUILayout.Button("Update"/*, contentStyle,*/, GUILayout.Width(ButtonWidthMed)))
            {
            }
            GUI.backgroundColor = colors[0];
        }
        using (new GUILayout.HorizontalScope())
        {
            CommonButtonCancel = EditorGUILayout.ObjectField("ButtonCancel", CommonButtonCancel, typeof(Object), false);
            GUI.backgroundColor = colors[1];
            if (GUILayout.Button("<"/*, contentStyle,*/, GUILayout.Width(ButtonWidthShort)))
            {
            }
            if (GUILayout.Button(">"/*, contentStyle,*/, GUILayout.Width(ButtonWidthShort)))
            {
            }
            GUILayout.Space(5);
            if (GUILayout.Button("Update"/*, contentStyle,*/, GUILayout.Width(ButtonWidthMed)))
            {
            }
            GUI.backgroundColor = colors[0];
        }
        using (new GUILayout.HorizontalScope())
        {

        }
        GUI.backgroundColor = colors[0];
    }

    private int checkUpdateCounter(int curr_counter, int counterLimit)
    {
        curr_counter = Mathf.Clamp(curr_counter, 0, counterLimit - 1);

        return curr_counter;
    }
}