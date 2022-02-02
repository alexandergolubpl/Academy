// BatchRename.cs
// Unity Editor extension that allows batch renaming for GameObjects in Hierarchy
// Via Alan Thorn (TW: @thorn_alan)
// Roman Luks - added sorting by sibling index
// AlexDove added clean duplicating indexes, added PostName
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text.RegularExpressions;

public class BatchRename : EditorWindow, IPanelTool
{
    public bool isReplaceBy = false;
    public bool isAddPostfix = false;
    public string ReplaceNameString = "MyObject";
    public string separator = "_";
    public string PostName = "";
    public int StartNumber = 0;
    public string[] radioText = new string[] { "selected", "hierarchy", "all" };
    private int transformTypeNum;

    private Vector2 scrollPosition;
    private string AssetDetailsArea = "";
    private string[] wordArray;
    private bool allowRename = false;

    public bool isUseRegexp = true;
    public bool isRegExpGlobal = true;
    public bool isRegExpMultiline = true;
    private string RegExPatternTF = "";
    private string RegExReplaceTF = "";
    private bool RegExPatternDD;
    private bool RegExReplaceDD;



    public string ToolName { get; set; }

    public int Increment = 1;
    public int PostFixNum = 0;
    //private bool _styleInitialized = false;

    public BatchRename() {
        ToolName = typeof(BatchRename).Name;
    }

    public void Execute() {
        BatchRename.exec();
    }
    public static void exec() {
        Open();
    }
    /*private void OnEnable()
    {
    }*/

    [MenuItem("AssetTools/Batch Rename %#R")]
    [MenuItem("GameObject/AssetTools/Rename/Batch Rename", false, 0)]
    private static void Open() {
        EditorWindow.GetWindow<BatchRename>("Batch Rename");
    }
    private GUIStyle style;
    private void OnDisable() {
        // _styleInitialized = false;
    }
    private void InitStyles() {
        if (EditorStyles.helpBox == null) {
            style = new GUIStyle(GUI.skin.box);
        } else {
            style = new GUIStyle(EditorStyles.helpBox);
            //style.margin = new RectOffset(0, 0, 0, 0);
            //style.padding = new RectOffset(0, 0, 0, 0);
            //style.border = new RectOffset(0, 0, 0, 0);
        }
        style.richText = true;
        style.fontSize = 10;
        //_styleInitialized = true;
    }

    //TODO to Delete **********************
    void UpdSelectionHelper() {
        string _helpString = "";
        if (Selection.objects != null)
            _helpString = "Number of objects selected: " + Selection.objects.Length;
    }
    //TODO to Delete **********************
    /// Rename
    /// 
    private void createDropDown(Rect rect, string tf, string[] _options) {
        int _selected = 0;
        //string[] _options = new string[3] { @"(\w+ )([()_])(\d+)([_()])", @"(\w+ )", @"([()_]).+([_()])" };
        GenericMenu menu = new GenericMenu();
        for (int i = 0; i < _options.Length; i++) {
            menu.AddItem(new GUIContent(_options[i]), false, handleItemClicked, _options[i]);
        }
        //menu.DropDown(rect);//position
        menu.ShowAsContext();

        void handleItemClicked(object parameter) {

            if (tf == "RegExPatternTF") {
                RegExPatternTF = parameter.ToString();
            }
            if (tf == "RegExReplaceTF") {
                RegExReplaceTF = parameter.ToString();
            }
        }

        //EditorGUI.BeginChangeCheck();
        //_selected = EditorGUILayout.Popup("My Simple Dropdown", _selected, _options);
        //if (EditorGUI.EndChangeCheck()) {
        //    Debug.Log(_options[_selected]);
        //}
    }
    private void OnGUI() {
        using (new GUILayout.VerticalScope("HelpBox")) {
            transformTypeNum = GUILayout.SelectionGrid(transformTypeNum, radioText, 1, EditorStyles.radioButton);//text.Length
        }

        using (new GUILayout.VerticalScope("HelpBox")) {
            using (new GUILayout.HorizontalScope())//"HelpBox"
            {
                isReplaceBy = GUILayout.Toggle(isReplaceBy, "Replace by:", GUILayout.ExpandWidth(false));
                GUI.enabled = isReplaceBy;
                ReplaceNameString = EditorGUILayout.TextField(ReplaceNameString, GUILayout.ExpandWidth(true));
                GUI.enabled = true;
            }
            using (new GUILayout.HorizontalScope())//"HelpBox"
            {
                isAddPostfix = GUILayout.Toggle(isAddPostfix, "Add Postfix:", GUILayout.ExpandWidth(false));
                GUI.enabled = isAddPostfix;
                separator = EditorGUILayout.TextField(separator);
                StartNumber = EditorGUILayout.IntField(StartNumber);

                PostFixNum = StartNumber;

                PostName = EditorGUILayout.TextField(PostName);
                GUILayout.FlexibleSpace();
                GUI.enabled = true;
            }
            using (new GUILayout.HorizontalScope())//"HelpBox"
            {
                Increment = EditorGUILayout.IntField("Increment:", Increment, GUILayout.ExpandWidth(false));
            }
        }
        using (new GUILayout.VerticalScope("HelpBox")) {
            using (new GUILayout.HorizontalScope())//"HelpBox"
            {
                isUseRegexp = GUILayout.Toggle(isUseRegexp, "Use RegExp");
                //isRegExpGlobal = GUILayout.Toggle(isRegExpGlobal, "Global");
                //isRegExpMultiline = GUILayout.Toggle(isRegExpMultiline, "Multiline");
            }

            //GUI.enabled = isUseRegexp;
            using (new GUILayout.HorizontalScope())//"HelpBox"
            {
                EditorGUILayout.LabelField("Find", GUILayout.ExpandWidth(false));
                RegExPatternTF = EditorGUILayout.TextField(RegExPatternTF, GUILayout.ExpandWidth(true));
                Rect RegExPatternTFRect = GUILayoutUtility.GetLastRect();
                //Rect cRect = EditorGUILayout.GetControlRect(false);
                RegExPatternDD = EditorGUILayout.DropdownButton(new GUIContent("!", "Suggest regular expressions"), FocusType.Passive, GUILayout.Width(20));
                if (RegExPatternDD) {
                    string[] _options = new string[] { @"(\w+ )([()_])(\d+)([_()])", @"(\w+ )", @"([()_]).+([_()])" };
                    createDropDown(RegExPatternTFRect, "RegExPatternTF", _options);
                }
            }
            using (new GUILayout.HorizontalScope())//"HelpBox"
            {
                EditorGUILayout.LabelField("Replace", GUILayout.ExpandWidth(false));
                RegExReplaceTF = EditorGUILayout.TextField(RegExReplaceTF, GUILayout.ExpandWidth(true));
                Rect RegExReplaceTFRect = GUILayoutUtility.GetLastRect();
                //Rect cRect = EditorGUILayout.GetControlRect(false);
                RegExReplaceDD = EditorGUILayout.DropdownButton(new GUIContent("!", "Suggest groups to replace"), FocusType.Passive, GUILayout.Width(20));
                if (RegExReplaceDD) {
                    string[] _options = new string[] { "MyObject_$3", "$1_$3" };
                    createDropDown(RegExReplaceTFRect, "RegExReplaceTF", _options);
                }
            }
            GUI.enabled = true;
        }
        scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, false);
        AssetDetailsArea = GUILayout.TextArea(AssetDetailsArea, /*style,*/ GUILayout.ExpandHeight(true));
        GUILayout.EndScrollView();


        if (GUILayout.Button("Rename")) {
            allowRename = true;
        }
        /*Selection.selectionChanged += OnSelectionChange;
    }
    void OnSelectionChange()
    {*/
        //Debug.Log("Selection changed");
        AssetDetailsArea = "";

        // If selection is empty, then exit
        if (Selection.objects == null)
            return;
        // Current Increment
        //string PostFix = ""+StartNumber.ToString();
        List<GameObject> mySelection = new List<GameObject>(Selection.gameObjects);
        mySelection.Sort((go1, go2) => go1.transform.GetSiblingIndex().CompareTo(go2.transform.GetSiblingIndex()));
        if (allowRename) {
            Undo.RecordObjects(mySelection.ToArray(), "UndoBatchRename");
        }

        //

        string tempStr = "";

        Undo.SetCurrentGroupName("Transform Scene objects");
        int group = Undo.GetCurrentGroup();

        Undo.RecordObjects(Selection.transforms, "transform selected objects");

        switch (radioText[transformTypeNum]) {
            case "all":

                foreach (GameObject go in Resources.FindObjectsOfTypeAll(typeof(GameObject)) as GameObject[]) {
                    if (go.hideFlags != HideFlags.None)
                        continue;
                    if (PrefabUtility.GetPrefabType(go) == PrefabType.Prefab || PrefabUtility.GetPrefabType(go) == PrefabType.ModelPrefab)
                        continue;

                    tempStr += getCurrentTempName(go) + "\n";
                }
                AssetDetailsArea = tempStr;

                break;

            case "selected":

                 mySelection = new List<GameObject>(Selection.gameObjects);
                mySelection.Sort((go1, go2) => go1.transform.GetSiblingIndex().CompareTo(go2.transform.GetSiblingIndex()));
                foreach (GameObject go in mySelection) {
                    tempStr += getCurrentTempName(go) + "\n";                    
                }
                AssetDetailsArea = tempStr;

                break;
            case "hierarchy":

        foreach (var go in mySelection) {
            GetAllChildren(go.transform.root, getCurrentTempName(go), "");
        }
                break;

            default:
                break;
        }
        Undo.CollapseUndoOperations(group);
        //
        allowRename = false;
    }

    public string getCurrentTempName(GameObject go) {
        var curTab = "";
        string PostFixStr = "";
        string tempName = "";
        if (StartNumber < 0) {
            PostFixStr = "";
        } else {
            PostFixStr = PostFixNum.ToString();
        }
        if (isUseRegexp) {
            tempName = RexString(go.name);
        } else {
            tempName = ReplaceString(go.name);
        }
        tempName = ((ReplaceNameString != "" && isReplaceBy) ? ReplaceNameString : tempName) + ((isAddPostfix) ? (separator + PostFixStr + PostName) : "");

        if (allowRename) {
            go.name = tempName;
        }
        //AssetDetailsArea += go.transform.GetSiblingIndex() + ".\t" + tempName + ((v == mySelection.Count - 1) ? "" : "\n");
        PostFixNum += Increment;
        return (tempName);
    }
    public string strOne = "Hello World";
    public string strTwo = "World";

    public void ReplaceStrTwoFromStrOne() {
        strOne = strOne.Replace(strTwo, "everyone"); //strOne = "Hello everyone" now
    }

    public string RexString333(string input) {
        //string pattern = "/^(\\w +)/";  (\w+ )([()_])(\d+)([_()])
        Regex regex = new Regex(RegExPatternTF);
        RegexOptions options = RegexOptions.IgnoreCase;
        MatchCollection matches = regex.Matches(input);
        if (matches.Count > 0) {
            string tempRegStr = "";
            foreach (Match match in matches) {
                CaptureCollection captures = match.Captures;
                Debug.Log(captures[0].Value);
                tempRegStr = captures[0].Value;
            }
            return tempRegStr;
        } else {
            return null;
        }
    }
    public string RexString(string input) {
        string pattern = @RegExPatternTF;
        string replacement = @RegExReplaceTF;
        string result = Regex.Replace(input, pattern, replacement);
        return result;
    }
    public string ReplaceString(string input) {
        string tempinput = input.ToLower();
        int startNum = tempinput.IndexOf(RegExPatternTF.ToLower());
        if (startNum > -1 && RegExPatternTF.Length > 0) {
            string p0 = input.Substring(0, startNum);
            string p1 = RegExReplaceTF;//input.Substring(startNum, RegExReplaceTF.Length);
            string p2 = input.Substring(startNum + RegExPatternTF.Length);

            //string result = input.Replace(RegExPatternTF, RegExReplaceTF);
            string result = p0 + p1 + p2;
            return result;
        } else {
            return input;
        }
    }

    private void GetAllChildren(Transform current, string displayName, string curTab) {
        AssetDetailsArea += curTab + displayName + "\n";//current.gameObject.name
        curTab += "    ";
        for (int i = 0; i < current.childCount; i++) {
            GetAllChildren(current.GetChild(i), getCurrentTempName(current.GetChild(i).gameObject), curTab);
        }
    }
}