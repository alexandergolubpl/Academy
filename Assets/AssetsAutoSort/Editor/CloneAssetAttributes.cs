//ver 0.1.8
// TODO create UpdateCurrentText in tabs

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEditorInternal;


public class CloneAssetAttributes : EditorWindow, IPanelTool
{
    public Object source;
    public string ToolName { get; set; }

    public CloneAssetAttributes()
    {
        ToolName = typeof(CloneAssetAttributes).Name;
    }

    public void Execute()
    {
        CloneAssetAttributes.exec();
    }
    public static void exec()
    {
        Open();
    }

    private string initLabel = " Clone Attributes of Asset ";
    private bool AtlasNameEnabled = false;
    private bool PostfixNameEnabled = false;
    private string AtlasNametextField_prefix = "prefix_";
    private string AtlasNametextField = "";
    private string AtlasNametextField_postfix = "_postfix";
    private string AssetDetailsArea = "";
    private string objectPathArea = " put list here to select ";
    private string objectPropsArea = "";
    private string PathArea = "";
    private string PropArea = "";
    //private string objectTagArea = "";
    //private string TagArea = "";

    private bool propsAlphaTrCh = true;
    private bool propsMaxSizeCh = true;
    private bool isSortArray = true;

    private bool isShowPropsList = true;
    private bool isShowTagsList = true;
    // The variable to control where the scrollview 'looks' into its child elements.
    private Vector2 scrollPositionNum;
    private Vector2 scrollPositionPath;
    private Vector2 scrollPositionTag;
    private Vector2 scrollPosition3;

    private string[] wordArray;
    private string[] strpath;
    private string[] strtag;
    private string[] strProps;

    //private string copied_spriteImportMode = "";
    //private string copied_wrapMode = "";
    //private string copied_spritePackingTag = "";
    //private string copied_MaxSize = "";
    private bool copied_alpaIsTransparency;
    //private string copied_textureCompression = "";

    //private int selectionGrid = 0;
    private int currElementNum = 0;
    private int currElementNumField = 1;
    private int currElementNumWidth;

    private string currElementNumTxt;

    private bool _styleInitialized = false;
    private bool _tabsCreated = false;

    private Color[] colors = new Color[] { new Color(0.5f, 0.8f, 1.0f, 0.3f), new Color(1.0f, 1.0f, 1.0f, 0.3f) };

    [MenuItem("AssetTools/Clone Asset Attributes %#j")]
    private static void Open()
    {
        EditorWindow.GetWindow<CloneAssetAttributes>("Clone Asset Attributes");
    }
    private GUIStyle style;
    private GUIStyle style2;
    private GUIStyle styleHelpBox;
    private GUIStyle styleToolbarButton;
    private GUIStyle TagButtonStyle;

    private Rect pathsRect;

    private void OnDisable()
    {
        _styleInitialized = false;
        _tabsCreated = false;
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
            //style.margin = new RectOffset(0, 0, 0, 0);
            //style.padding = new RectOffset(0, 0, 0, 0);
        }
        style.richText = true;
        //style.fontSize = 12;
        style2 = new GUIStyle(style);

        styleHelpBox = new GUIStyle(EditorStyles.helpBox);
        styleToolbarButton = new GUIStyle(EditorStyles.toolbarButton);
        //styleToolbarButton.

        TagButtonStyle = new GUIStyle(style);
        //TagButtonStyle.fontSize = 10;
        TagButtonStyle.alignment = TextAnchor.MiddleLeft;
        //TagButtonStyle.normal.background = null;
        //TagButtonStyle.onActive.background = null;

        style2.normal.background = null;
        style2.onActive.background = null;
        _styleInitialized = true;
    }
    private void UpdateCurrentText(int currTab, string textSource, string textTarget)
    {

        // var dictionary = new Dictionary.< String, String> ();
        for (int i = 0; i < currTab; i++)
        {
            //dictionary["tabArea"+i] = textTarget;

        }
        //Debug.Log(dictionary["zVar"]); //prints out 200

        tabArea[currentTab] = objectPathArea;
    }
    private void SelectCurrElement(string[] wordArray, int currElementNum)
    {
        Selection.activeObject = AssetDatabase.LoadMainAssetAtPath(wordArray[currElementNum].Split("\t"[0])[0]);
        EditorGUIUtility.PingObject(Selection.activeObject);

        barTabCurrElement[currentTab] = currElementNum;
    }
    private void createTagButtonsList()
    {

        int btsSpace = -8;
#if UNITY_2020_3_OR_NEWER
        btsSpace = -6;
#endif
        Color defaultColor = GUI.backgroundColor;
        Color defaultContentColor = GUI.contentColor;

        var scrollPositionLocal = (scrollPositionPath.y > scrollPositionTag.y) ? scrollPositionTag : scrollPositionPath;
        scrollPositionTag = GUILayout.BeginScrollView(scrollPositionLocal, false, false/*, GUILayout.Height(area.height)*/);
        for (int i = 0; i < strtag.Length; i++)
        {
            ChangeColorUI(i);

            GUI.skin.button.wordWrap = false;
            GUI.skin.textArea.wordWrap = false;
            if (GUILayout.Button(i + ". " + strtag[i], TagButtonStyle/*, GUILayout.Width(200)*/))
            {
                currElementNum = i;
                SelectCurrElement(wordArray, currElementNum);
            }
            GUI.backgroundColor = defaultColor;
            GUILayout.Space(btsSpace);
        }
        GUI.backgroundColor = defaultColor;
        EditorGUILayout.EndScrollView();
    }

    private int currentTab = 0;
    private int toolbarButtonsCount = 4;
    private List<int> barTabCurrElement = new List<int>();
    private List<string> tabArea = new List<string>();
    private List<string> toolBarString = new List<string>();

    private void createMyToolBar()
    {
        if (!_tabsCreated && toolBarString.Count < toolbarButtonsCount)
        {
            _tabsCreated = true;
            for (int i = 0; i < toolbarButtonsCount; i++)
            {
                toolBarString.Add("List " + i);
                barTabCurrElement.Add(0);
                tabArea.Add("");
            }
        }
        currentTab = GUILayout.Toolbar(currentTab, toolBarString.ToArray(), styleToolbarButton, GUILayout.Width(400));
        GUILayout.FlexibleSpace();

        objectPathArea = tabArea[currentTab];
        currElementNum = barTabCurrElement[currentTab];
    }

    private void createSelectedLineLabel()
    {
        var defaultContentColor = GUI.contentColor;
        GUI.contentColor = new Color(1.0f, 1.0f, 0.0f, 1.0f);
        GUILayout.Label(initLabel, GUILayout.ExpandWidth(true));
        GUI.contentColor = defaultContentColor;
    }
    private void createPropsChList()
    {
        isSortArray = GUILayout.Toggle(isSortArray, " Sort A-Z ("+ strpath.Length +")");
        isShowPropsList = GUILayout.Toggle(isShowPropsList, " Show Props List");
        isShowTagsList = GUILayout.Toggle(isShowTagsList, " Show Tags List");
        GUILayout.FlexibleSpace();
    }

    private void createUpdatePropsList()
    {
        GUILayout.Label("Read from TextArea");
        GUILayout.Space(10);
        propsAlphaTrCh = GUILayout.Toggle(propsAlphaTrCh, " Overwrite Alpha Transparency");
        propsMaxSizeCh = GUILayout.Toggle(propsMaxSizeCh, " Overwrite MaxSize");
        GUILayout.FlexibleSpace();
        GUILayout.Space(10);

    }
    private void createNumberButtonsList()
    {

        int btsSpace = -8;
#if UNITY_2020_3_OR_NEWER
        btsSpace = -6;
#endif
        Color defaultColor = GUI.backgroundColor;
        Color defaultContentColor = GUI.contentColor;

        //Scroller for numbers
        //var scrollPositionLocal = (scrollPositionPath.y > scrollPositionNum.y) ? scrollPositionNum : scrollPositionPath;
        scrollPositionNum = GUILayout.BeginScrollView(scrollPositionPath, false, false,
            (strpath.Length < 99)
                ? GUILayout.Width(50 + 15)
                : GUILayout.Width(60 + 15)
                );
        for (int i = 0; i < strtag.Length; i++)
        {
            ChangeColorUI(i);
            if (GUILayout.Button("" + (i + 1)/*, style2*/, (strpath.Length < 99)
            ? GUILayout.Width(26)
            : GUILayout.Width(32)
            ))
            {
                currElementNum = i;
                SelectCurrElement(wordArray, currElementNum);

                GUI.backgroundColor = defaultContentColor;
            }
            GUI.backgroundColor = defaultColor;
            GUILayout.Space(btsSpace);
        }
        GUI.backgroundColor = defaultColor;
        EditorGUILayout.EndScrollView();

    }

    private void ChangeColorUI(int i)
    {
        if (i == currElementNum)
            GUI.backgroundColor = new Color(1.0f, 1.0f, 0.0f, 3f);
        else
            GUI.backgroundColor = colors[i % 2];
    }
    private void PasteObjects()
    {
        objectPathArea = "";
        PathArea = "";
        PropArea = "";
        //TagArea = "";
        AppendObjects();
    }

    private void AppendObjects()
    {
        string tempPathArea = objectPathArea;
        PathArea = "";
        PropArea = "";
        PathArea = tempPathArea;

        if (Selection.objects.Length > 0)
        {
            strpath = new string[Selection.objects.Length];
            strtag = new string[Selection.objects.Length];
            strProps = new string[Selection.objects.Length];

            for (int i = 0; i < Selection.objects.Length; i++)
            {
                strpath[i] = AssetDatabase.GetAssetPath(Selection.objects[i]);
            }
            if (isSortArray)
            {
                System.Array.Sort(strpath);
            }
            for (int i = 0; i < strpath.Length; i++)
            {
                var assetType = UnityEditor.AssetDatabase.GetMainAssetTypeAtPath(strpath[i]);
                if (assetType == typeof(Sprite) || assetType == typeof(Texture2D))
                {
                    TextureImporter importer = (TextureImporter) TextureImporter.GetAtPath(strpath[i]);

                    Texture2D tmpTexture = new Texture2D(1, 1, TextureFormat.ARGB32, false);
                    byte[] tmpBytes = System.IO.File.ReadAllBytes(strpath[i]);
                    tmpTexture.LoadImage(tmpBytes);
                    //Debug.Log("Raw size: " + tmpTexture.width + " x " + tmpTexture.height);
                    strProps[i] = $"alphaIsTr:{importer.alphaIsTransparency}\tmaxSize:{importer.maxTextureSize}\t{tmpTexture.width}x{tmpTexture.height}";
                    strtag[i] = importer.spritePackingTag;


                }
                else
                {
                    strtag[i] = "";
                }

                string isFirstLine = (!string.IsNullOrEmpty(PathArea) && i == 0) ? "\n" : "";
                string isTagEmpty = string.IsNullOrEmpty(strtag[i]) ? "" : "\t" + strtag[i];
                string isEndLine = (i == Selection.objects.Length - 1) ? "" : "\n";
                //ConstructCurrentLine();
                PathArea += $"{ isFirstLine }{ strpath[i] }{ isTagEmpty }{ isEndLine }";//\t{ strProps[i] }
                PropArea += $"{ isFirstLine }{strProps[i]}{ isEndLine }";//{ i }.\t
            }
            objectPathArea = PathArea;
            objectPropsArea = PropArea;

            tabArea[currentTab] = objectPathArea;
        }
    }

    private void createPathsList()
    {
        //Scroller for strings


        scrollPositionPath = GUILayout.BeginScrollView(scrollPositionPath, false, false/*, GUILayout.Height(area.height)*/);

        scrollPositionTag = (scrollPositionTag.y < scrollPositionPath.y) ? scrollPositionPath : scrollPositionTag;
        scrollPositionNum = (scrollPositionNum.y < scrollPositionPath.y) ? scrollPositionNum : scrollPositionTag;

        GUI.skin.textArea.wordWrap = false;
        EditorGUI.BeginChangeCheck();
        objectPathArea = GUILayout.TextArea(objectPathArea, /*style,*/GUILayout.ExpandHeight(true));
        if (EditorGUI.EndChangeCheck())
        {
            tabArea[currentTab] = objectPathArea;
        }
        EditorGUILayout.EndScrollView();
    }

    private void createPropsList()
    {
        //Scroller for strings
        var scrollPositionLocal = (scrollPositionPath.y > scrollPositionTag.y) ? scrollPositionTag : scrollPositionPath;
        scrollPositionTag = GUILayout.BeginScrollView(scrollPositionLocal, false, false/*, GUILayout.Height(area.height)*/);
        GUI.skin.textArea.wordWrap = false;
        objectPropsArea = GUILayout.TextArea(objectPropsArea, /*style,*/GUILayout.ExpandHeight(true));
        EditorGUILayout.EndScrollView();
    }

    private void OnGUI()
    {
        if (!_styleInitialized)
        {
            InitStyles();
        }
        Rect lastRect;


        using (new GUILayout.HorizontalScope())
        {
            using (new GUILayout.VerticalScope())
            {
                using (new GUILayout.HorizontalScope())
                {
                    if (GUILayout.Button("Paste objects"))
                    {
                        PasteObjects();
                    }
                    if (strpath != null)
                    {
                        if (GUILayout.Button("Append objects"))
                        {
                            AppendObjects();
                        }
                    }

                    if (Selection.objects.Length > 0 && elements.Count < 1)
                    {
                        if (GUILayout.Button("Add to Favorites -> "/*, GUILayout.Width(200)*/))
                        {
                            CreateFavButton();
                        }
                    }
                }

                if (strpath != null)
                {
                    GUILayout.Label("Atlas Name");
                    using (new GUILayout.HorizontalScope())
                    {
                        AtlasNameEnabled = GUILayout.Toggle(AtlasNameEnabled, "Use Atlas Name");
                        PostfixNameEnabled = GUILayout.Toggle(PostfixNameEnabled, "prefix and postfix (check to enable \"Paste attributes\" Button)");
                        GUILayout.FlexibleSpace();
                    }
                    using (new GUILayout.HorizontalScope())
                    {
                        AtlasNametextField_prefix = GUILayout.TextField(AtlasNametextField_prefix);
                        AtlasNametextField = GUILayout.TextField(AtlasNametextField);
                        AtlasNametextField_postfix = GUILayout.TextField(AtlasNametextField_postfix);
                    }// GUILayout.EndHorizontal();

                    using (new GUILayout.HorizontalScope())
                    {
                        GUILayout.Label("Asset Details Area");
                        AssetDetailsArea = GUILayout.TextArea(AssetDetailsArea, GUILayout.ExpandWidth(true));
                        if (AssetDetailsArea.Length > 0)
                            GUILayout.FlexibleSpace();
                    }
                    using (new GUILayout.HorizontalScope())
                    {
                        //GUILayout.Space(36);
                        createMyToolBar();
                    }

                    using (new GUILayout.HorizontalScope())
                    {
                        createNumberButtonsList();
                        createPathsList();
                        //ApplyColor(); 
                        if (isShowPropsList) createPropsList();
                        if (isShowTagsList) createTagButtonsList();
                    }

                    createSelectedLineLabel();

                    using (new GUILayout.HorizontalScope())
                    {
                        createPropsChList();
                    }

                    string objectPath = "";
                    wordArray = objectPathArea.Split("\r"[0]);
                    string word = string.Join("", wordArray);
                    wordArray = word.Split("\n"[0]);
                    Object[] objects = new Object[wordArray.Length];

                    string[] tempTagstring;
                    string linepath = "";
                    string linetag = "";
                    currElementNum = (currElementNum > wordArray.Length) ? 0 : currElementNum;
                    string[] wordArrayElements = wordArray[currElementNum].Split("\t"[0]);
                    if (wordArrayElements.Length > 0)
                    {
                        tempTagstring = wordArray[currElementNum].Split("\t"[0]);
                        if (wordArrayElements.Length > 1)
                            linetag = wordArray[currElementNum].Split("\t"[0])[1];
                    }
                    else
                        linepath = "Assets";


                    using (new GUILayout.HorizontalScope())
                    {
                        int temp = currElementNum;
                        if (GUILayout.Button("<<", GUILayout.Width(48)))
                        {
                            --temp;
                        }
                        EditorGUI.BeginChangeCheck();
                        currElementNumWidth = (currElementNum.ToString().Length > 3) ? 80 : 32;
                        currElementNumField = int.Parse(GUILayout.TextField((currElementNumField).ToString(), GUILayout.Width(currElementNumWidth)));
                        currElementNum = currElementNumField - 1;
                        if (EditorGUI.EndChangeCheck())
                        {
                            temp = currElementNum;
                            //int.TryParse(, out temp);
                            currElementNum = Mathf.Clamp(currElementNum, 0, wordArray.Length - 1);
                            Selection.activeObject = AssetDatabase.LoadMainAssetAtPath(wordArray[currElementNum].Split("\t"[0])[0]);
                            EditorGUIUtility.PingObject(Selection.activeObject);
                        }
                        if (GUILayout.Button(">>", GUILayout.Width(48)))
                        {
                            ++temp;
                        }
                        if (temp != currElementNum)
                        {
                            currElementNum = temp;
                            currElementNum = Mathf.Clamp(currElementNum, 0, wordArray.Length - 1);
                            SelectCurrElement(wordArray, currElementNum);
                        }
                        currElementNumField = currElementNum + 1;

                        GUILayout.Space(5);

                        currElementNum = Mathf.Clamp(currElementNum, 0, wordArray.Length - 1);
                        initLabel = wordArray[currElementNum];
                        if (GUILayout.Button("Select items"))
                        {
                            if (objectPathArea.Length > 0)
                            {
                                for (int i = 0; i < wordArray.Length; i++)
                                {
                                    objectPath = wordArray[i].Split("\t"[0])[0];
                                    //Selection.activeObject = AssetDatabase.LoadMainAssetAtPath(objectPath);
                                    objects[i] = AssetDatabase.LoadMainAssetAtPath(objectPath);
                                }
                                Selection.objects = objects;

                            }
                        }
                    }
                    GUILayout.Space(5);
                    using (new GUILayout.HorizontalScope())
                    {
                        createUpdatePropsList();
                        GUILayout.Space(10);
                        GUI.enabled = AtlasNameEnabled;
                        if (GUILayout.Button("Paste Attributes to selected"))
                        {
                            if (
                            EditorUtility.DisplayDialog("Update sprite Tags?",
                                "Are you sure you want to add string " + AtlasNametextField_postfix + " in sprite Tags?", "Add", "Do Not Add"))
                            {
                                if (PostfixNameEnabled)
                                {
                                    for (int i = 0; i < Selection.objects.Length; i++)
                                    {
                                        //Selection.activeObject = AssetDatabase.LoadMainAssetAtPath(Selection.objects[i]);
                                        string path = AssetDatabase.GetAssetPath(Selection.objects[i]);
                                        var assetType = UnityEditor.AssetDatabase.GetMainAssetTypeAtPath(strpath[i]);
                                        if (assetType == typeof(Sprite) || assetType == typeof(Texture2D))
                                        {
                                            TextureImporter importer = (TextureImporter) TextureImporter.GetAtPath(path);
                                            /*if(TextureImporter.GetAtPath(path) is TextureImporter impo) {
                                                impo
                                            }*/
                                            if (AtlasNametextField != "" && AtlasNameEnabled)
                                            {
                                                importer.spritePackingTag = AtlasNametextField;
                                            }
                                            if (AtlasNametextField_postfix != "_postfix")
                                            {
                                                importer.spritePackingTag += AtlasNametextField_postfix;
                                            }
                                            if (AtlasNametextField_prefix != "prefix_")
                                            {
                                                importer.spritePackingTag = AtlasNametextField_prefix + importer.spritePackingTag;
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    Debug.Log("AssignGroupTags");
                                    for (int i = 0; i < Selection.objects.Length; i++)
                                    {
                                        tempTagstring = wordArray[currElementNum].Split("\t"[0]);
                                        if (tempTagstring.Length == 2)
                                        {
                                            //parse line for path and tag
                                            linetag = wordArray[i].Split("\t"[0])[1];
                                            string path = wordArray[i].Split("\t"[0])[0];
                                            var assetType = UnityEditor.AssetDatabase.GetMainAssetTypeAtPath(strpath[i]);
                                            if (assetType == typeof(Sprite) || assetType == typeof(Texture2D))
                                            {
                                                TextureImporter importer = (TextureImporter) TextureImporter.GetAtPath(path);
                                                importer.spritePackingTag = linetag;

                                                Debug.Log($"name = {importer.name}\t:\tspritePackingTag = {importer.spritePackingTag}\n");
                                                EditorUtility.SetDirty(importer);
                                                importer.SaveAndReimport();
                                            }
                                        }
                                    }
                                    AssetDatabase.SaveAssets();
                                }
                            }
                        }
                    }
                }
                // We're done with the conditional block, so make GUI code be enabled again.
                GUI.enabled = true;
            }//GUILayout.EndVertical();


            if (elements.Count > 0)
            {

                using (new GUILayout.VerticalScope())
                {
                    using (new GUILayout.HorizontalScope())
                    {
                        GUILayout.Label("Favorites");

                        if (GUILayout.Button("Add Selected to Favorites "/*, GUILayout.Width(200)*/))
                        {
                            CreateFavButton();
                        }
                    }
                    scrollPosition3 = GUILayout.BeginScrollView(scrollPosition3, false, false/*, GUILayout.Height(area3.height)*/);
                    using (new GUILayout.VerticalScope())
                    {
                        foreach (var v in elements)
                        {
                            using (new GUILayout.HorizontalScope())//EditorStyles.helpBox
                            {
                                //GUILayout.Label(v.Name);
                                //GUILayout.FlexibleSpace();
                                GUIStyle btn_clostyle = new GUIStyle(EditorStyles.miniButtonMid);
                                btn_clostyle.alignment = TextAnchor.MiddleCenter;

                                EditorGUILayout.ObjectField(v.GO, typeof(Object));

                                if (GUILayout.Button("x", btn_clostyle, GUILayout.Width(16)))
                                {
                                    elements.Remove(v);
                                    Repaint();
                                }
                                GUILayout.Space(4);
                            }
                            GUILayout.Space(-3);
                        }
                    }
                    EditorGUILayout.EndScrollView();

                    using (new GUILayout.HorizontalScope())
                    {
                        if (elements.Count > 0)
                        {
                            GUILayout.Label("Clear All Favorites");
                            if (GUILayout.Button("Clear All Favorites"/*, GUILayout.Width(200)*/))
                            {
                                elements.Clear();
                            }
                        }
                    }
                }
            }
            //GUILayout.EndVertical();
        }//GUILayout.EndHorizontal();
    }

    private struct Element
    {
        public Object GO;
        //public string Name;
        public Element(Object g)//, string name
        {
            GO = g;
            //Name = name;
        }
    }

    private List<Element> elements = new List<Element>();

    private void CreateFavButton()
    {
        //elements.Clear();
        foreach (var v in Selection.objects)
        {
            elements.Add(new Element(v));//, v.name
        }
    }

    private void ApplyColor()
    {
        //backup color
        Color backupColor = GUI.color;
        Color backupContentColor = GUI.contentColor;
        Color backupBackgroundColor = GUI.backgroundColor;

        //add textarea with transparent text
        GUI.contentColor = new Color(1f, 1f, 1f, 0f);
        GUIStyle style = new GUIStyle(GUI.skin.textArea);
        //Rect bounds = GUILayoutUtility.GetRect(objectPathArea, style);
        Rect bounds = new Rect(100, 200, Screen.width - 10, Screen.height - 20);
        //stringToEdit = GUI.TextArea(bounds, stringToEdit);

        //get the texteditor of the textarea to control selection
        int controlID = GUIUtility.GetControlID(bounds.GetHashCode(), FocusType.Keyboard);
        TextEditor editor = (TextEditor) GUIUtility.GetStateObject(typeof(TextEditor), controlID - 1);

        //set background of all textfield transparent
        GUI.backgroundColor = new Color(1f, 1f, 1f, 0f);

        //backup selection to remake it after process
        int backupPos = editor.cursorIndex;
        int backupSelPos = editor.selectIndex;

        //get last position in text
        editor.MoveTextEnd();
        int endpos = editor.cursorIndex;

        Random.seed = 123;

        //draw textfield with color on top of text area
        editor.MoveTextStart();
        while (editor.cursorIndex != endpos)
        {
            editor.SelectToStartOfNextWord();
            string wordtext = editor.SelectedText;

            //set word color
            GUI.contentColor = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));

            //draw each word with a random color
            Vector2 pixelselpos = style.GetCursorPixelPosition(editor.position, editor.content, editor.selectIndex);
            Vector2 pixelpos = style.GetCursorPixelPosition(editor.position, editor.content, editor.cursorIndex);
            GUI.TextField(new Rect(pixelselpos.x - style.border.left, pixelselpos.y - style.border.top, pixelpos.x, pixelpos.y), wordtext);

            editor.MoveToStartOfNextWord();
        }

        //Reposition selection
        Vector2 bkpixelselpos = style.GetCursorPixelPosition(editor.position, editor.content, backupSelPos);
        editor.MoveCursorToPosition(bkpixelselpos);

        //Remake selection
        Vector2 bkpixelpos = style.GetCursorPixelPosition(editor.position, editor.content, backupPos);
        editor.SelectToPosition(bkpixelpos);

        //Reset color
        GUI.color = backupColor;
        GUI.contentColor = backupContentColor;
        GUI.backgroundColor = backupBackgroundColor;
    }

}

/* var reorderableList = new UnityEditorInternal.ReorderableList(List < SomeType > someList, typeof(SomeType), dragable, displayHeader, displayAddButton, displayRemoveButton);
 reorderableList.DoList(rect);
//or
reorderableList.DoLayoutList()*/
