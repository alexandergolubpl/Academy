//ver 0.1.1

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;

using UnityEngine.UI;
using UnityEditorInternal;

using UnityEngine.SceneManagement;
using System.Threading.Tasks;

public class DuplicateToFolder : EditorWindow, IPanelTool
{
    public Object target;
    public bool isTargetFolder;
    public string initLabel;
    public string ToolName { get; set; }

    public DuplicateToFolder()
    {
        ToolName = typeof(DuplicateToFolder).Name;
    }

    public void Execute()
    {
        DuplicateToFolder.exec();
    }
    public static void exec()
    {
        Open();
    }

    private GUIStyle style;
    private GUIStyle style2;
    private GUIStyle TagButtonStyle;
    private GUIStyle styleHelpBox;

    private Color ColorBlue = new Color(0.5f, 0.8f, 1.0f, 1.0f);
    private Color ColorYellow = new Color(1.0f, 1.0f, 0.0f, 1.0f);
    private Color ColorViolet = new Color(1.0f, 0.6f, 1.0f, 1.0f);
    private Color ColorGreen = new Color(0.0f, 0.8f, 0.3f, 1.0f);
    private Color ColorOrange = new Color(1.0f, 0.6f, 0.0f, 1.0f);
    private Color[] colors = new Color[] { new Color(1.0f, 1.0f, 1.0f, 1.0f), new Color(0.85f, 0.85f, 0.85f, 1.0f), new Color(1.0f, 0.2f, 0.2f, 1.0f), new Color(0.0f, 0.8f, 0.3f, 1.0f), new Color(0.5f, 0.8f, 1.0f, 1.0f) };

    private Color defaultColor;
    private Color defaultContentColor;

    //private string AssetDetailsArea = "";
    private string objectPathArea = "";
    //private string objectPropsArea = "";

    private bool isSortArray = true;
    private bool isShowName;
    private bool isDuplicate = true;
    private Vector2 scrollPosition;
    private Vector2 scrollPosition3;

    //private string[] wordArray;
    //private string[] strpath;
    //private string[] strtag;
    //private string[] strProps;

    private List<string> wordArray;
    private List<string> strpath;
    private List<Object> strobj;
    private List<string> strtag;
    private List<string> strProps;
    //private string TextTree = "";

    private List<string> tempPath;
    //private int selectionGrid = 0;
    private int currElementNum = 0;
    private int currElementNumField = 1;
    private int currElementNumWidth;
    private string currElementNumTxt;

    private bool _styleInitialized = false;

    private string PathArea = "";
    //private string PropArea = "";

    private Object[] SceneImages;
    private Object[] SceneAnimations;
    private Object[] SceneModels;
    private Object[] SceneScripts;

    private bool isReplaceInScene = true;
    public bool isUpdateDependenciesList = true;
    private List<GameObject> foundObjectsWithImage;
    private List<GameObject> ListObjectsWithSelectedImage;
    private List<Object> ListObjectsWithImage;
    private List<Object> ListObjectsWithCurrentImage;
    private string fromGameObjects = "";
    private bool isSprites = true;
    private bool isScripts;
    private bool isModels;
    private bool isAnimations;
    private bool isApplyModifyInPrefab = true;
    private bool isUpdateDependenciesOnly;
    private bool isFromScene;
    private bool isCommonAsset;
    private bool isControlPressed;
    private Rect CurrBtnRect;
    private readonly float smallHeight = 16;
    private readonly float middleHeight = 24;
    private readonly float bigHeight = 40;

    [MenuItem("AssetTools/Duplicate To Folder %#f")]
    [MenuItem("Assets/AssetTools/Duplicate To Folder %#f")]
    [MenuItem("GameObject/AssetTools/Duplicate To Folder", false, 0)]
    private static void Open()
    {
        EditorWindow.GetWindow<DuplicateToFolder>(" Duplicate To Folder ");
    }

    private void OnDisable()
    {
        _styleInitialized = false;
    }

    private void InitStyles()
    {
        defaultColor = GUI.backgroundColor;
        defaultContentColor = GUI.contentColor;

        if (EditorStyles.helpBox == null)
        {
            style = new GUIStyle(GUI.skin.box);
        }
        else
        {
            style = new GUIStyle(EditorStyles.helpBox);
        }
        style.richText = true;
        style2 = new GUIStyle(style);

        TagButtonStyle = new GUIStyle(style);
        TagButtonStyle.alignment = TextAnchor.MiddleLeft;

        style2.normal.background = null;
        style2.onActive.background = null;


        styleHelpBox = new GUIStyle(EditorStyles.helpBox);
        styleHelpBox.padding.left = 0;
        styleHelpBox.padding.right = 0;
        styleHelpBox.padding.top = 0;
        styleHelpBox.padding.bottom = 0;
        _styleInitialized = true;
    }

    private void RepaintProjectView()
    {
        var projectBrowserType = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.ProjectBrowser");
        var projectBrowser = EditorWindow.GetWindow(projectBrowserType);
        projectBrowser.Repaint();
    }
    //PARSE PART
    private void SelectCurrElement(List<string> wordArray, int currElementNum)
    {
        Selection.activeObject = AssetDatabase.LoadMainAssetAtPath(strpath[currElementNum].Split("\t"[0])[0]);
        EditorGUIUtility.PingObject(Selection.activeObject);
    }
    private void AddAssetToStr(List<string> tempPath)
    {
        strpath.AddRange(tempPath);
        for (int s = 0; s < tempPath.Count; s++)
        {
            Object _object = AssetDatabase.LoadAssetAtPath<Object>(strpath[s]);
            strobj.Add(_object);
        }
    }
    private void PasteObjects()
    {
        ResetTexts();
        AppendObjects();
    }
    private void ResetTexts()
    {
        objectPathArea = "";
        PathArea = "";
        strpath = new List<string>();
        strobj = new List<Object>();
        strProps = new List<string>();
    }
    private void AppendObjects()
    {
        if (Selection.objects.Length > 0)
        {
            //if (tempstrpath != null) tempstrpath.Clear();

            List<string> tempstrpath = new List<string>(Selection.objects.Length);
            List<string> tempstrProps = new List<string>(Selection.objects.Length);

            for (int i = 0; i < Selection.objects.Length; i++)
            {
                //tempstrpath[i] = AssetDatabase.GetAssetPath(Selection.objects[i]);
                tempstrpath.Add(AssetDatabase.GetAssetPath(Selection.objects[i]));

                strpath.Add(tempstrpath[i]);
                Object _object = AssetDatabase.LoadAssetAtPath<Object>(strpath[i]);
                strobj.Add(_object);
            }
            if (isSortArray)
            {
                tempstrpath.Sort();
                strpath.Sort();
            }
            WriteToArea();
        }
    }
    private void WriteToArea()
    {
        PathArea = "";
        for (int i = 0; i < strpath.Count; i++)
        {
            //var color1 = "<color=#ffffff>";
            //var color2 = "</color>";
            //isCommonAsset = false;
            string isFirstLine = (!string.IsNullOrEmpty(PathArea) && i == 0) ? "\n" : "";
            string isEndLine = (i == strpath.Count - 1) ? "" : "\n";
            if (strpath[i].Contains("SharedContent") || strpath[i].Contains("Game/Common"))
            {
                //isCommonAsset = true;
                //color1 = "<color=#41b6ff>";
                //color2 = "</color>";
            }

            //Debug.Log("\t" + i + "\t" + isCommonAsset + "\t\t" + strpath[i]);
            //{color1}
            //{color2}
            PathArea += $"{ isFirstLine }{ ((isShowName) ? Path.GetFileName(strpath[i]) : strpath[i]) }{ isEndLine }";//\t{ strProps[i] }
        }
        //Debug.Log(PathArea);
        //objectPathArea = "";
        objectPathArea = PathArea;
    }
    private void ParseTexArea()
    {
        // .Split(">"[0])[1].Split("<"[0])[0];
        GUI.skin.textArea.richText = false;
        wordArray = objectPathArea.Split("\r"[0]).ToList();
        string word = string.Join("", wordArray);
        wordArray = word.Split("\n"[0]).ToList();

        Object[] objects = new Object[wordArray.Count];

        string[] tempTagstring;
        string linetag = "";
        if (currElementNum > wordArray.Count - 1)
            currElementNum = 0;
        string[] wordArrayElements = wordArray[currElementNum].Split("\t"[0]);
        if (wordArrayElements.Length > 0)
        {
            tempTagstring = wordArray[currElementNum].Split("\t"[0]);
            if (wordArrayElements.Length > 1)
            {
                linetag = wordArray[currElementNum].Split("\t"[0])[1];
            }
        }
    }

    //GUI PART

    private void GUI_AssetTypes()
    {
        isSprites = GUILayout.Toggle(isSprites, "Sprites");
        isAnimations = GUILayout.Toggle(isAnimations, "Animations");
        isModels = GUILayout.Toggle(isModels, "Models");
        GUI.enabled = false;
        isScripts = GUILayout.Toggle(isScripts, "Scripts");
        GUI.enabled = true;
    }
    private void GUI_SelectedLineLabel()
    {
        var defaultContentColor = GUI.contentColor;
        GUI.contentColor = new Color(1.0f, 1.0f, 0.0f, 1.0f);
        GUILayout.Label(initLabel, GUILayout.ExpandWidth(true));
        GUI.contentColor = defaultContentColor;

        GUILayout.FlexibleSpace();
    }
    private void GUI_PropsChList()
    {
        isSortArray = GUILayout.Toggle(isSortArray, " Sort A-Z (" + strpath.Count + ")");

        EditorGUI.BeginChangeCheck();
        isShowName = GUILayout.Toggle(isShowName, "Show name only");
        if (EditorGUI.EndChangeCheck())
        {
            WriteToArea();
        }
    }
    private void GUI_NumberButtonsList()
    {


        int btsSpace = -8;
#if UNITY_2020_3_OR_NEWER
        btsSpace = -6;
#endif
        //Scroller for numbers
        scrollPosition3 = GUILayout.BeginScrollView(scrollPosition, false, false,
            (strpath.Count < 99)
                ? GUILayout.Width(50 + 15)
                : GUILayout.Width(60 + 15)
                );
        if (strpath != null)
        {
            for (int i = 0; i < strpath.Count; i++)
            {
                var tooltip = "'Shift' to show in Project\n'Alt' to Find In Scene\n'Control' to replace Sprite in Image";
                using (new GUILayout.HorizontalScope())
                {
                    GUI_ChangeColor(i);
                    bool modifiersEnabled = false;
                    if (strpath[i].Contains("SharedContent"))//|| strpath[i].Contains("Game/Common")
                    {
                        GUI.backgroundColor = ColorGreen;
                        tooltip = "Assets is common.\n" + tooltip;
                        if (i == currElementNum)
                        {
                            GUI.backgroundColor = ColorViolet;
                        }
                    }
                    if (GUILayout.Button(new GUIContent("" + (i + 1), tooltip), (strpath.Count < 99) ? GUILayout.Width(26) : GUILayout.Width(32)))
                    {
                        GUI.backgroundColor = defaultContentColor;
                        GUI.backgroundColor = defaultColor;
                        GUI_ChangeColor(i);

                        //isControlPressed = (Event.current.modifiers == (EventModifiers.Control));
                        //if (isControlPressed)
                        //{
                        //EditorApplication.
                        //}

                        //Show asset in project
                        if (Event.current.modifiers == (EventModifiers.Shift))
                        {
                            var pingObject = AssetDatabase.LoadMainAssetAtPath(strpath[i]);
                            EditorGUIUtility.PingObject(pingObject);
                            modifiersEnabled = true;
                        }
                        if (Event.current.modifiers == (EventModifiers.Alt))
                        {
                            //TODO find in scene
                            string[] _sceneArr = { strpath[i] };
                            FindReferencesinScene(_sceneArr);
                            modifiersEnabled = true;
                        }
                        //Replace sprite in selected graphics
                        if (Event.current.modifiers == (EventModifiers.Control))
                        {
                            for (int s = 0; s < Selection.gameObjects.Length; s++)
                            {
                                Image currImage = Selection.gameObjects[s].GetComponent<Image>();
                                Sprite currSprite = AssetDatabase.LoadAssetAtPath<Sprite>(strpath[i]);
                                Undo.RecordObject(currImage, "Change Sprite to " + currSprite.name);
                                currImage.sprite = currSprite;

                                if (currImage != null)
                                {
                                    EditorUtility.SetDirty(currImage);
                                }
                                //SerializedObject.ApplyModifiedProperties();
                                //var hierarchy = UnityEditorWindowHelper.GetWindow(WindowType.Hierarchy);
                                //hierarchy.Repaint();
                                EditorWindow view = EditorWindow.GetWindow<SceneView>();
                                view.Repaint();
                            }
                            modifiersEnabled = true;
                        }
                        if (!modifiersEnabled)
                        {
                            currElementNum = i;
                            SelectCurrElement(wordArray, currElementNum);
                        }
                    }
                    if (i == currElementNum)
                    {
                        CurrBtnRect = GUILayoutUtility.GetLastRect();
                    }
                }
                GUILayout.Space(btsSpace);
            }
            GUI.backgroundColor = defaultColor;
            EditorGUILayout.EndScrollView();
        }
    }

    private void GUI_ChangeColor(int i)
    {
        if (i == currElementNum)
        {
            GUI.backgroundColor = new Color(1.0f, 1.0f, 0.0f, 1f);
        }
        else
        {
            GUI.backgroundColor = colors[(i % 2)];
        }
    }
    private void GUI_CreatePathsList()
    {
        //Scroller for strings


        //using (var scrollViewScope = new UnityEngine.ScrollViewScope(scrollPosition, GUILayout.Width(100), GUILayout.Height(100)))
        //{
        scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, false/*, GUILayout.Height(area.height)*/);
        //EditorStyles.textArea.wordWrap = false;
        //Rect ScrollRect = GUILayoutUtility.GetLastRect();
        //Debug.Log(ScrollRect+" 0 scrollPosition = " + scrollPosition / ScrollRect.height);

        GUI.skin.textArea.wordWrap = false;
        GUI.skin.textArea.richText = true;
        Highlighter.Highlight(typeof(DuplicateToFolder).Name, strpath[currElementNum]);
        objectPathArea = GUILayout.TextArea(objectPathArea, /*style,*/GUILayout.ExpandHeight(true));
        EditorGUILayout.EndScrollView();
        //EditorGUI.DrawRect(new Rect(CurrBtnRect.x, CurrBtnRect.y + 16, 500, 2), Color.red);
        //Debug.Log(CurrBtnRect);
        //}
    }

    private void GUI_CreateNavBtns()
    {
        int temp = currElementNum;
        GUI.backgroundColor = defaultColor;
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
            currElementNum = Mathf.Clamp(currElementNum, 0, wordArray.Count - 1);
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
            currElementNum = Mathf.Clamp(currElementNum, 0, wordArray.Count - 1);
            SelectCurrElement(wordArray, currElementNum);
        }
        currElementNumField = currElementNum + 1;
    }

    private void SelectDependencies()
    {
        Object obj = Selection.objects[0];
        Object[] roots = new Object[] { obj };
        Selection.objects = EditorUtility.CollectDependencies(roots);

        Selection.GetFiltered(typeof(Object), SelectionMode.Editable | SelectionMode.TopLevel);
    }

    private void SelectFromArea()
    {
        if (objectPathArea.Length > 0)
        {
            string objectPath = "";
            Object[] objects = new Object[wordArray.Count];
            for (int i = 0; i < wordArray.Count; i++)
            {
                objectPath = wordArray[i].Split("\t"[0])[0];
                //Selection.activeObject = AssetDatabase.LoadMainAssetAtPath(objectPath);
                objects[i] = AssetDatabase.LoadMainAssetAtPath(objectPath);
            }
            Selection.objects = objects;

        }
    }
    private void GUI_SelectAllBtn()
    {
        currElementNum = Mathf.Clamp(currElementNum, 0, wordArray.Count - 1);
        initLabel = (currElementNum < strpath.Count - 1) ? strpath[currElementNum] : "";
        GUI.backgroundColor = ColorBlue;
        if (GUILayout.Button(new GUIContent("Select items", "Select All Assets from List"), GUILayout.Height(20)))
        {
            SelectFromArea();

        }
        GUI.backgroundColor = defaultColor;
    }
    private void GUI_TargetFolder()
    {
        var FolderFromFileLabelText = "";
        var defaultContentColor = GUI.contentColor;
        var FolderPath = "";
        if (!isTargetFolder)
        {
            GUI.backgroundColor = colors[1];
        }
        else
        {
            GUI.backgroundColor = colors[3];
        }
        target = EditorGUILayout.ObjectField(target, typeof(Object), true);
        if (string.IsNullOrEmpty(AssetDatabase.GetAssetPath(target)))
        {
            isTargetFolder = false;
            GUI.backgroundColor = colors[1];
        }
        else
        {
            isTargetFolder = true;
            GUI.backgroundColor = colors[3];
            if (Path.GetExtension(AssetDatabase.GetAssetPath(target)).Length < 1)
            {
                FolderPath = AssetDatabase.GetAssetPath(target);
            }
            else
            {
                var filename = Path.GetFileName(AssetDatabase.GetAssetPath(target));
                FolderPath = AssetDatabase.GetAssetPath(target).Replace("/" + filename, "");
                FolderFromFileLabelText = " (Folder the same as file)";
            }
        }

        string targetLabel = (isTargetFolder) ? FolderPath : "Select target folder";
        EditorGUILayout.LabelField(targetLabel);

        GUI.contentColor = ColorOrange;
        GUI.backgroundColor = ColorOrange;
        EditorGUILayout.LabelField(FolderFromFileLabelText);
        //GUILayout.FlexibleSpace();

        GUI.contentColor = defaultContentColor;
        GUI.backgroundColor = colors[0];
        //isTargetFolder = (Path.GetExtension(AssetDatabase.GetAssetPath(target)) == "");
        //if (target == null || !isTargetFolder)
        //{
        //EditorGUILayout.HelpBox("Select Target Folder", MessageType.Warning);
        //return;
        //GUILayout.Space(5);
        //}
    }
    private void GUI_DuplicateOrMove()
    {
        GUI.enabled = (Selection.objects.Length > 0);
        isDuplicate = GUILayout.Toggle(isDuplicate, ((isDuplicate) ? " " + "Duplicate" + " " : " " + "Move" + " ") + strpath.Count + " " + "item(s)");
        if (!isDuplicate)
        {
            GUI.enabled = false;
        }
    }
    private void GUI_UpdateAttributes()
    {
        isReplaceInScene = GUILayout.Toggle(isReplaceInScene, new GUIContent("Replace in Scene", "Replaces by selected assets"));
        GUILayout.Space(10);
        isApplyModifyInPrefab = GUILayout.Toggle(isApplyModifyInPrefab, new GUIContent("Update in Prefab", "Apply changes in Prefab"));
        GUILayout.Space(10);
        GUI.enabled = true;
        isUpdateDependenciesList = GUILayout.Toggle(isUpdateDependenciesList, new GUIContent("Update Links", "Shows links to Moved/Duplicated assets"));
        GUILayout.FlexibleSpace();
    }
    private void GUIDuplicateBtns()
    {
        GUI.enabled = (Selection.objects.Length > 0 && target != null && isTargetFolder);
        if (GUILayout.Button((((isDuplicate) ? " Duplicate " : " Move ") + "To Folder "), GUILayout.Height(middleHeight)))
        {
            DuplicateAssets();
        }
        GUI.enabled = true;

        using (new GUILayout.HorizontalScope())
        {
            //isApplyModifyInPrefab = GUILayout.Toggle(isApplyModifyInPrefab, " Apply Modifications in Prefab");
            //EditorGUILayout.LabelField("");//, GUILayout.Width(70)
            GUI.enabled = !isFromScene;

            GUI.backgroundColor = ColorBlue;
            if (GUILayout.Button(new GUIContent("Update Sprites in scene", "Select Sprites From Project and press to replace sprites in scene"), GUILayout.Height(middleHeight)))
            {
                try
                {
                    isUpdateDependenciesOnly = true;
                    UpdateAssetsInScene();

                }
                catch (System.Exception ex)
                {
                    //Debug.LogError(ex);
                }
                finally
                {
                    EditorUtility.ClearProgressBar();
                    Canvas.ForceUpdateCanvases();
                    SceneView.RepaintAll();
                    EditorWindow.GetWindow<SceneView>().Repaint();
                    HandleUtility.Repaint();
                    isUpdateDependenciesOnly = false;
                }
            }
            GUI.backgroundColor = colors[0];
            GUI.enabled = true;
        }
    }

    private void OnGUI()
    {
        if (!_styleInitialized)
        {
            InitStyles();
        }
        //Rect lastRect;

        using (new GUILayout.HorizontalScope())
        {
            using (new GUILayout.VerticalScope())
            {
                isFromScene = false;
                if (Selection.objects.Length > 0)
                {
                    if (Selection.gameObjects.Length > 0)
                    {
                        if (Selection.activeGameObject != null)
                        {
                            isFromScene = (Selection.activeGameObject.scene.IsValid() == true);
                        }
                    }
                }
                fromGameObjects = (isFromScene) ? "From Scene" : "Assets";

                using (new GUILayout.HorizontalScope())
                {
                    GUI.enabled = (Selection.objects.Length > 0);
                    if (GUILayout.Button("Paste All Dependencies", GUILayout.ExpandWidth(true)))
                    {
                        SelectDependencies();
                        ResetTexts();
                        PasteAssetsFromSelectedGO();
                        Selection.objects = null;
                        RepaintProjectView();
                        Canvas.ForceUpdateCanvases();
                        SceneView.RepaintAll();
                        SelectFromArea();
                        PasteObjects();
                    }
                    if (GUILayout.Button("Select All Dependencies"))
                    {
                        SelectDependencies();
                    }
                }
                GUI.enabled = true;

                GUI.enabled = isFromScene;
                using (new GUILayout.HorizontalScope())
                {
                    GUI_AssetTypes();
                }

                GUILayout.Space(5);

                using (new GUILayout.HorizontalScope())
                {
                    if (Selection.objects.Length < 1)
                    {
                        GUILayout.Label("Select Any Object");
                    }
                    else
                    {
                        if (GUILayout.Button(" Paste " + fromGameObjects))
                        {
                            if (fromGameObjects == "Assets")
                            {
                                PasteObjects();
                            }
                            else
                            {
                                ResetTexts();
                                PasteAssetsFromSelectedGO();
                            }
                        }
                        if (strpath != null)
                        {
                            if (GUILayout.Button(" Append " + fromGameObjects))
                            {
                                if (fromGameObjects == "Assets")
                                {
                                    AppendObjects();
                                }
                                else
                                {
                                    PasteAssetsFromSelectedGO();
                                }
                            }
                        }
                    }
                }

                if (strpath != null)
                {
                    using (new GUILayout.HorizontalScope())
                    {
                        GUI_NumberButtonsList();
                        GUI_CreatePathsList();
                    }

                    using (new GUILayout.HorizontalScope())
                    {
                        GUI_PropsChList();
                        GUILayout.Space(10);
                        GUI_SelectedLineLabel();
                    }
                    ParseTexArea();

                    using (new GUILayout.HorizontalScope())
                    {
                        GUI_SelectAllBtn();
                        GUILayout.Space(5);
                        GUI_CreateNavBtns();
                    }

                    GUILayout.Space(5);

                    using (new GUILayout.HorizontalScope())
                    {
                        GUI_TargetFolder();
                    }
                    GUILayout.Space(5);

                    //targetPopup = EditorGUILayout.ObjectField(target, typeof(Object), true);
                    using (new GUILayout.HorizontalScope())
                    {
                        GUI_UpdateAttributes();
                    }
                    GUILayout.Space(5);

                    using (new GUILayout.HorizontalScope())
                    {
                        GUI_DuplicateOrMove();
                        GUILayout.Space(10);
                        GUIDuplicateBtns();
                    }
                }
            }
            GUI.enabled = true;
        }
    }

    private void PasteAssetsFromSelectedGO()
    {
        if (Selection.gameObjects.Length > 0)
        {
            List<string> tempPathList = new List<string>();
            int i = 0;
            for (int o = 0; o < Selection.gameObjects.Length; o++)
            {
                GameObject go = Selection.gameObjects[o];

                if (isSprites)
                {
                    if (go.GetComponent<Image>() != null)
                    {
                        Image im = go.GetComponent<Image>();
                        tempPathList.Add(AssetDatabase.GetAssetPath(im.sprite));
                    }
                }
                if (isAnimations)
                {
                    if (go.GetComponent<Animation>() != null)
                    {
                        Animation an = go.GetComponent<Animation>();

                        //List<AnimationState> states = new List<AnimationState>(an.Cast<AnimationState>());
                        foreach (AnimationState anstates in an)
                        {
                            tempPathList.Add(AssetDatabase.GetAssetPath(anstates.clip));
                        }
                    }
                }
                if (isModels)
                {
                    if (go.GetComponent<MeshFilter>() != null)
                    {
                        MeshFilter mf = go.GetComponent<MeshFilter>();
                        tempPathList.Add(AssetDatabase.GetAssetPath(mf.sharedMesh));
                        //AddAssetToStr(tempPath);
                    }
                }
            }
            if (tempPathList.Count > 0)
            {
                AddAssetToStr(tempPathList);
            }
            if (isSortArray)
            {
                //tempstrpath.Sort();
                strpath.Sort();
            }
            WriteToArea();
        }
    }

    private void UpdateDependencySceneModels(string path, string targetPath)
    {
        Mesh oldtAsset = AssetDatabase.LoadAssetAtPath<Mesh>(path);
        Mesh targetAsset = AssetDatabase.LoadAssetAtPath<Mesh>(targetPath);
        Object[] SceneModels = Object.FindObjectsOfTypeAll(typeof(MeshFilter));
        for (int i = 0; i < SceneModels.Length; i++)
        {
            MeshFilter CurrMesh = SceneModels[i] as MeshFilter;
            Undo.RecordObject(CurrMesh, "update animations");

            if (CurrMesh.sharedMesh == oldtAsset)
            {
                if (oldtAsset == targetAsset)
                {
                    continue;
                }
                CurrMesh.sharedMesh = targetAsset;
                ApplyModifyInPrefab(SceneImages[i], CurrMesh.sharedMesh.name);
            }
        }
    }

    private void UpdateDependencySceneAnimations(string path, string targetPath)
    {
        AnimationClip oldAsset = AssetDatabase.LoadAssetAtPath<AnimationClip>(path);
        AnimationClip targetAsset = AssetDatabase.LoadAssetAtPath<AnimationClip>(targetPath);

        Object[] SceneAnimations = Object.FindObjectsOfTypeAll(typeof(Animation));
        for (int i = 0; i < SceneAnimations.Length; i++)
        {
            Animation CurrAnimation = SceneAnimations[i] as Animation;
            int a = 0;
            foreach (AnimationState CurrState in CurrAnimation)
            {
                Undo.RecordObject(SceneAnimations[i], "update animations");
                string CurrClipName = CurrState.name;
                string oldClipName = oldAsset.name;

                if (CurrClipName == oldClipName)
                {
                    if (oldAsset == targetAsset)
                    {
                        continue;
                    }
                    var CurrClips = AnimationUtility.GetAnimationClips(CurrAnimation.gameObject);
                    for (int j = 0; j < CurrClips.Length; j++)
                    {
                        if (CurrClips[j] == oldAsset)
                        {
                            CurrClips[j] = targetAsset;
                        }
                    }

                    AnimationUtility.SetAnimationClips(CurrAnimation, CurrClips);
                    ApplyModifyInPrefab(SceneAnimations[i], CurrClips.ToString());
                    a++;
                }
            }
        }
    }

    private void UpdateDependencySceneImages(string path, string targetPath)
    {
        Sprite oldSprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
        Sprite targetSprite = AssetDatabase.LoadAssetAtPath<Sprite>(targetPath);

        Object[] SceneImages = Object.FindObjectsOfTypeAll(typeof(Image));
        for (int i = 0; i < SceneImages.Length; i++)
        {
            Image CurrImage = SceneImages[i] as Image;
            Undo.RecordObject(CurrImage, "update images");
            if (CurrImage.sprite == oldSprite)
            {
                if (oldSprite == targetSprite)
                {
                    continue;
                }
                CurrImage.sprite = targetSprite;
                if (isApplyModifyInPrefab)
                {
                    ApplyModifyInPrefab(SceneImages[i], CurrImage.sprite.name);
                }
            }
        }
    }

    private void ApplyModifyInPrefab(Object ob, string name)
    {
        var CurrRoot = PrefabUtility.GetNearestPrefabInstanceRoot(ob);
        if (CurrRoot != null)
        {
            PrefabUtility.RecordPrefabInstancePropertyModifications(CurrRoot);
        }
        if (PrefabUtility.IsPartOfPrefabInstance(ob))
        {
            PrefabUtility.ApplyPrefabInstance(CurrRoot, InteractionMode.AutomatedAction);

            // if (isUpdateDependenciesOnly)  Debug.Log("Updated sptrites in Prefab:\t" + CurrRoot.name + ". Asset:\t" + name);
        }
    }

    private void DuplicateAssets()
    {
        for (int i = 0; i < Selection.objects.Length; i++)
        {
            string path = AssetDatabase.GetAssetPath(Selection.objects[i]);
            string targetPath = AssetDatabase.GetAssetPath(target) + "/" + Path.GetFileName(path);


            bool newPath = false;
            if (isDuplicate)
            {
                Undo.RegisterCreatedObjectUndo(Selection.objects[i], "Copy Assets");
                if (AssetDatabase.CopyAsset(path, targetPath))
                {
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
                    newPath = true;
                }
            }
            else
            {
                if (AssetDatabase.MoveAsset(path, targetPath) == "")
                {
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
                    newPath = true;
                }
            }
            if (isReplaceInScene)
            {
                if (isSprites && Selection.objects[i] is Texture2D)
                {
                    UpdateDependencySceneImages(path, targetPath);
                }
                if (isAnimations && Selection.objects[i] is AnimationClip)
                {
                    UpdateDependencySceneAnimations(path, targetPath);
                }
                if (isScripts)
                {
                    //UpdateDependencySceneScripts(path, targetPath);
                }
                if (isModels && Selection.objects[i] is Mesh)
                {
                    UpdateDependencySceneModels(path, targetPath);
                }
                if (newPath)
                {
                    var index = strpath.IndexOf(path);
                    if (index != -1)
                    {
                        strpath[index] = targetPath;
                        Object _object = AssetDatabase.LoadAssetAtPath<Object>(strpath[index]);
                        strobj[index] = _object;
                    }
                }
                if (isUpdateDependenciesList)
                {
                    Selection.objects = strobj.ToArray();
                }
            }
        }
        PasteObjects();
    }

    private void FindReferencesinScene(string[] refs)
    {
        Selection.activeGameObject = null;
        Selection.objects = null;

        string objectPath = "";
        Object[] objects = new Object[refs.Length];
        for (int i = 0; i < refs.Length; i++)
        {
            objectPath = refs[i];
            objects[i] = AssetDatabase.LoadMainAssetAtPath(objectPath);
        }
        Selection.objects = objects;
        EditorApplication.ExecuteMenuItem("Assets/Find References In Scene");
    }

    private Task UpdateAssetsInScene()
    {
        Object[] SceneImages = Object.FindObjectsOfTypeAll(typeof(Image));
        var curr = 0;
        var replaceStr = "";
        int ImagesToCheckTotal = SceneImages.Length;//* Selection.objects.Length
                                                    //var pBarMessage = "Importing...";

        for (int s = 0; s < SceneImages.Length; s++)
        {
            bool isModifyCurrPrefab = false;
            for (int i = 0; i < Selection.objects.Length; i++)
            {
                string path = AssetDatabase.GetAssetPath(Selection.objects[i]);

                if (isSprites && Selection.objects[i] is Texture2D)
                {
                    curr = s;//(i + 1) *
                    var progress = (float) curr / ImagesToCheckTotal;
                    var pBarMessage = (curr + " / " + ImagesToCheckTotal) + "\tChecking: " + SceneImages[s].name;
                    if (EditorUtility.DisplayCancelableProgressBar("Wait please", pBarMessage, progress))
                    { break; }

                    Image SceneImage = SceneImages[s] as Image;
                    if (SceneImage.sprite == null)
                    {
                        continue;
                    }
                    Sprite SceneSprite = SceneImage.sprite;
                    Sprite SourceSprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
                    Undo.RecordObject(SceneImage, "replace images");
                    if (SceneSprite.name == SourceSprite.name)
                    {
                        if (SceneSprite != SourceSprite)
                        {
                            isModifyCurrPrefab = true;
                            Undo.RecordObject(SceneImage, "Update sprites");
                            SceneImage.sprite = SourceSprite;
                            EditorUtility.SetDirty(SceneImage);
                            replaceStr += "replaced in:\t" + SceneImages[s].name + "\t:\t" + AssetDatabase.GetAssetPath(SceneSprite) + "\t>\t" + path;
                            replaceStr += "\n";

                            //EditorUtility.GetDirtyCount(SceneImage);
                        }
                    }

                    EditorWindow view = EditorWindow.GetWindow<SceneView>();
                    view.Repaint();
                }

                if (isAnimations && Selection.objects[i] is AnimationClip)
                {
                    //UpdateDependencySceneAnimations(path, targetPath);
                }
            }
            if (isModifyCurrPrefab)
            {
                //    Debug.Log("Prefab: " + PrefabUtility.GetNearestPrefabInstanceRoot(SceneImages[s]).name+". Nothing to update");
            }
            if (isApplyModifyInPrefab && isModifyCurrPrefab)
            {
                ApplyModifyInPrefab(SceneImages[s], SceneImages[s].name);
            }
        }
        Debug.Log(replaceStr);
        return Task.CompletedTask;
    }
}