//ver 0.1.1

using System;
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
using BoardKings.Utils.Extension;

public class DuplicateToFolder : EditorWindow, IPanelTool
{
    private static string windowName = "DuplicateToFolder";
    public UnityEngine.Object target;
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
    private GUIStyle NumberButtonStyle;
    private GUIStyle styleHelpBox;

    public Color gameObjectCommonColor = new Color(0.1f, 0.5f, 0.1f, 0.5f);
    public Color gameObjectCommonColorSelected = new Color(1.0f, 0.6f, 1.0f, 1.0f);
    private Color ColorBlue = new Color(0.5f, 0.8f, 1.0f, 1.0f);
    private Color ColorYellow = new Color(1.0f, 1.0f, 0.0f, 1.0f);
    private Color ColorViolet = new Color(1.0f, 0.6f, 1.0f, 1.0f);
    private Color ColorGreen = new Color(0.0f, 0.8f, 0.3f, 1.0f);
    private Color ColorOrange = new Color(1.0f, 0.6f, 0.0f, 1.0f);

    private Color[] colors = new Color[]
    {
        new Color(1.0f, 1.0f, 1.0f, 1.0f), new Color(0.85f, 0.85f, 0.85f, 1.0f), new Color(1.0f, 0.2f, 0.2f, 1.0f), new Color(0.0f, 0.8f, 0.3f, 1.0f),
        new Color(0.5f, 0.8f, 1.0f, 1.0f)
    };

    private Color defaultColor;
    private Color defaultContentColor;

    //private string AssetDetailsArea = "";
    private string objectPathArea = "";
    //private string objectPropsArea = "";

    private bool isSortArray = true;

    private int iconPreviewSize = 32;
    private string viewMode = "List";

    private List<Texture2D> PreviewImagesList = new List<Texture2D>();

    private bool isDisplayNum = false;
    private bool isShowName;
    private bool isShowPathsList = false;

    private bool isDuplicate = true;
    private Vector2 scrollPosition;
    private Vector2 scrollPosition3;
    private Vector2 scrollPositionTiles;

    //private string[] wordArray;
    //private string[] strpath;
    //private string[] strtag;
    //private string[] strProps;

    private List<string> wordArray;
    private List<string> strpath;
    private List<UnityEngine.Object> strobj;
    private List<string> strtag;

    private List<string> strProps;
    //private string TextTree = "";

    private List<string> tempPath;

    //private int selectionGrid = 0;
    private int currElementNum = 0;
    private int currElementNumField = 1;
    private float currElementNumWidth;
    private string currElementNumTxt;

    private bool _styleInitialized = false;

    private string PathArea = "";
    //private string PropArea = "";

    private UnityEngine.Object[] SceneImages;
    private UnityEngine.Object[] SceneAnimations;
    private UnityEngine.Object[] SceneModels;
    private UnityEngine.Object[] SceneScripts;

    private bool isReplaceInScene = true;
    public bool isUpdateDependenciesList = true;

    private List<GameObject> foundObjectsWithImage;
    private List<GameObject> ListObjectsWithSelectedImage;
    private List<UnityEngine.Object> ListObjectsWithImage;
    private List<UnityEngine.Object> ListObjectsWithCurrentImage;
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
    private bool texturesForPreviewsLoaded;
    private Rect CurrBtnRect;
    private Rect lrect;
    private Rect toprect;
    private Rect lowrect;
    private int scrollHeight;

    private readonly float smallHeight = 16;
    private readonly float middleHeight = 24;
    private readonly float bigHeight = 40;
    private readonly float middleWidth = 32;
    private readonly float bigWidth = 48;
    private readonly float shortwidth = 12;

    public static string commonPath = "Assets/Game/Common/SharedContent/Assets/Sprites/UICommon";

    private TextEditor AreaEditor;
    private bool modifiersEnabled;

    private void LoadPlayerPrefs()
    {
        PlayerPrefs.GetString("commonPath", commonPath);
        ColorUtility.TryParseHtmlString("#" + PlayerPrefs.GetString("CommonColor"), out gameObjectCommonColor); // with alpha
        ColorUtility.TryParseHtmlString("#" + PlayerPrefs.GetString("CommonColorSelected"), out gameObjectCommonColorSelected); // with alpha
    }

    private void Awake()
    {
        LoadPlayerPrefs();
    }

    private void Reset()
    {
        LoadPlayerPrefs();
    }

    [MenuItem("AssetTools/Duplicate To Folder %#f")]
    [MenuItem("Assets/AssetTools/Duplicate To Folder %#f")]
    [MenuItem("GameObject/AssetTools/Duplicate To Folder", false, 0)]
    private static void Open()
    {
        EditorWindow.GetWindow<DuplicateToFolder>(windowName);
        var window = EditorWindow.GetWindow(typeof(EditorWindow));
        //var texture = Resources.Load<Texture>("icon_copy");

        string icon_folder = "Assets/AssetsAutoSort/HierarchyStyle/";
        var texture = icon_folder + "icon_copy.png";
        Texture2D icon_texture = (Texture2D) AssetDatabase.LoadAssetAtPath(texture, typeof(Texture2D));
        Texture2D icon_texture2 = ImgResize(icon_texture, 12, 12);
        window.titleContent = new GUIContent(windowName, icon_texture2, "Useful for duplicate!"); //, GUILayout.Width(12), GUILayout.Height(12)
    }

    private static Texture2D ImgResize(Texture2D texture2D,
                                       int targetX,
                                       int targetY)
    {
        RenderTexture rt = new RenderTexture(targetX, targetY, 24);
        RenderTexture.active = rt;
        Graphics.Blit(texture2D, rt);
        Texture2D result = new Texture2D(targetX, targetY);
        result.ReadPixels(new Rect(0, 0, targetX, targetY), 0, 0);
        result.Apply();
        return result;
    }

    private void OnDisable()
    {
        _styleInitialized = false;
    }

    private void InitStyles()
    {
        defaultColor = GUI.backgroundColor;
        defaultContentColor = GUI.contentColor;

        style = EditorStyles.helpBox == null
                    ? new GUIStyle(GUI.skin.box)
                    : new GUIStyle(EditorStyles.helpBox);

        style.richText = true;
        style2 = new GUIStyle(style);

        TagButtonStyle = new GUIStyle(style);
        TagButtonStyle.alignment = TextAnchor.MiddleLeft;

        NumberButtonStyle = new GUIStyle(GUI.skin.button);
        NumberButtonStyle.alignment = TextAnchor.MiddleLeft;
        NumberButtonStyle.padding.left = 8;
        NumberButtonStyle.padding.right = 0;
        NumberButtonStyle.padding.top = 0;
        NumberButtonStyle.padding.bottom = 0;

        NumberButtonStyle.border.left = 0;
        NumberButtonStyle.border.right = 0;
        NumberButtonStyle.border.top = 0;
        NumberButtonStyle.border.bottom = 0;

        NumberButtonStyle.margin.left = 0;
        NumberButtonStyle.margin.right = 0;
        NumberButtonStyle.margin.top = 0;
        NumberButtonStyle.margin.bottom = 0;

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

    private Texture2D LoadTempImage(string path)
    {
        Texture2D tex = null;
        byte[] fileData;

        if (File.Exists(path))
        {
            fileData = File.ReadAllBytes(path);
            tex = new Texture2D(2, 2);
            tex.LoadImage(fileData);
        }

        return (tex);
    }

    private void LoadPreviews()
    {
        texturesForPreviewsLoaded = false;
        PreviewImagesList?.Clear();

        for (int i = 0; i < strpath.Count; i++)
        {
            if (!texturesForPreviewsLoaded)
            {
                var tex = (Texture) AssetDatabase.LoadAssetAtPath(strpath[i], typeof(Texture));
                //Texture2D sourceimageTexture = LoadTempImage(strpath[i]); //(Texture2D) AssetDatabase.LoadAssetAtPath(sourceToOverwrite[i], typeof(Texture2D));
                PreviewImagesList.Add(tex as Texture2D);
            }
        }

        texturesForPreviewsLoaded = true;
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
            UnityEngine.Object _object = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(strpath[s]);
            strobj.Add(_object);
        }

        LoadPreviews();
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
        strobj = new List<UnityEngine.Object>();
        strProps = new List<string>();
    }

    private void AppendObjects()
    {
        if (Selection.objects.Length > 0)
        {
            List<string> tempstrpath = new List<string>(Selection.objects.Length);
            for (int i = 0; i < Selection.objects.Length; i++)
            {
                tempstrpath.Add(AssetDatabase.GetAssetPath(Selection.objects[i]));

                strpath.Add(tempstrpath[i]);
                UnityEngine.Object _object = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(tempstrpath[i]);
                strobj.Add(_object);
            }

            if (isSortArray)
            {
                tempstrpath.Sort();
                strpath.Sort();
            }

            LoadPreviews();
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
            string countNum = (isDisplayNum)
                                  ? (i + 1) + "\t"
                                  : "";
            string isFirstLine = (!string.IsNullOrEmpty(PathArea) && i == 0)
                                     ? "\n"
                                     : "";
            string isEndLine = (i == strpath.Count - 1)
                                   ? ""
                                   : "\n";
            if (strpath[i].Contains("SharedContent") || strpath[i].Contains("Game/Common"))
            {
                //isCommonAsset = true;
                //color1 = "<color=#41b6ff>";
                //color2 = "</color>";
            }

            //Debug.Log("\t" + i + "\t" + isCommonAsset + "\t\t" + strpath[i]);
            //{color1}
            //{color2}
            PathArea += $"{countNum}{isFirstLine}{((isShowName) ? Path.GetFileName(strpath[i]) : strpath[i])}{isEndLine}"; //\t{ strProps[i] }
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

        UnityEngine.Object[] objects = new UnityEngine.Object[wordArray.Count];

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

    private static void DrawPreviewQuad(Texture2D tex)
    {
        GUI.DrawTexture(new Rect(100, 100, 200, 200), tex, ScaleMode.ScaleToFit, true, 10.0F);
        /*
        // width and height of your red rectangle
        int width = 100;
        int height = 100;

// start x and y position, in texture coordinates
        int x = 120;
        int y = 0;

// rectangles for our draw call
        Rect destinationRect = new Rect(0, 0, width, height);
        Rect sourceRect = new Rect(x, y, width, height);

        //spriteBatch.Draw(mainTexture, destinationRect, sourceRect, Color.white);

        var color = Color.white;
        Texture2D texture = new Texture2D(1, 1);
        texture.SetPixel(0, 0, color);
        texture.Apply();
        var size = 64;
        Rect icon_RectPos = new Rect(0,0, size, size);
        if (GUI.Button(icon_RectPos, new GUIContent("!!!", "image "+0)))
        {
            //EditorGUIUtility.systemCopyBuffer = ColorUtility.ToHtmlStringRGB(color);
        }

        var defaultBackgroundTexture = GUI.skin.box.normal.background;
        GUI.skin.box.normal.background = texture;
        GUI.Box(icon_RectPos, GUIContent.none);
        GUI.skin.box.normal.background = defaultBackgroundTexture;
        */
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

    private void GUI_PropsChList()
    {
        EditorGUI.BeginChangeCheck();

        isSortArray = GUILayout.Toggle(
            isSortArray, EditorGUIUtility.IconContent("AlphabeticalSorting", "AlphabeticalSorting|Enable to Sort array when updating list"));
        GUILayout.Label("Sort (" + (strpath.Count) + ")");

        GUI_CreateSizeImageButtons();
        //isShowPathsList = GUILayout.Toggle(isShowPathsList, "Show Paths List");
        if (isShowPathsList)
        {
            GUILayout.Space(10);
            isDisplayNum = GUILayout.Toggle(isDisplayNum, "Display Number");
            GUILayout.Space(5);
            isShowName = GUILayout.Toggle(isShowName, "Show name only");
        }
        else
        {
        }

        GUILayout.FlexibleSpace();
        GUI.enabled = isShowPathsList;
        GUI.enabled = true;

        if (EditorGUI.EndChangeCheck())
        {
            WriteToArea();
        }
    }

    private void GUI_CreateSizeImageButtons()
    {
        using (new GUILayout.HorizontalScope())
        {
            GUILayout.Label("Icon Size:");
            iconPreviewSize = (int) EditorGUILayout.Slider(iconPreviewSize, 8.0f, 1000.0f, GUILayout.Width(120), GUILayout.ExpandWidth(true));
        }
    }

    private void GUI_SelectedLineLabel()
    {
        if (currElementNum != null)
        {
            currElementNum = Mathf.Clamp(currElementNum, 0, strpath.Count - 1);
            initLabel = (currElementNum < strpath.Count - 1)
                            ? strpath[currElementNum]
                            : "";
            var defaultContentColor = GUI.contentColor;
            if (initLabel.Length > 0)
            {
                GUI.contentColor = ColorYellow;
            }

            GUILayout.Label(initLabel);
            GUI.contentColor = defaultContentColor;
        }
    }

    private float CalculateBtnWidth(String s)
    {
        GUIContent content = new GUIContent(s);

        // Compute how large the button needs to be.
        Vector2 w2 = NumberButtonStyle.CalcSize(content);
        return w2.x;
    }

    public static float CalculateLineHeight(Text text)
    {
        var extents = text.cachedTextGenerator.rectExtents.size * 0.5f;
        var setting = text.GetGenerationSettings(extents);
        var lineHeight = text.cachedTextGeneratorForLayout.GetPreferredHeight("A", setting);
        return lineHeight * text.lineSpacing / setting.scaleFactor;
    }

    private void AddAssetButtonAction(int i)
    {
        modifiersEnabled = false;
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
            //find in scene
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

                //!ReferenceEquals(currImage, null)
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

    private void GUI_NumberButtonsList()
    {
        var btsSpace = 0;
        if (isShowPathsList)
        {
            btsSpace = 0;
        }
        else
        {
            btsSpace = -6;
        }

        GUI.skin.scrollView = style;
        // rect and put it in a small rect on the screen.
#if UNITY_2020_3_OR_NEWER
        btsSpace = -0;
#endif
        //Scroller for numbers
        var wid = CalculateBtnWidth(strpath.Count.ToString());
        var wid2 = wid + 6;

        if (isShowPathsList)
        {
            scrollPosition3 = GUILayout.BeginScrollView(
                scrollPosition, false, false, GUILayout.Width(wid2 + 16));
            GUILayout.FlexibleSpace();
        }
        else
        {
            if (viewMode == "List")
            {
                scrollPosition3 = GUILayout.BeginScrollView(scrollPosition3, false, false);
                scrollPosition = scrollPosition3;
            }
            else
            {
                var scrollPosition4 = GUILayout.BeginScrollView(scrollPosition3, false, false);
                scrollPosition = scrollPosition4;
                GUILayout.FlexibleSpace();
                GUILayout.EndScrollView();
            }
        }

        if (strpath != null)
        {
            var hor = 0;
            var ver = 0;
            for (var i = 0; i < strpath.Count; i++)
            {
                var tooltip = "'Shift' - show in Project\n'Alt' - Find In Scene\n'Control' - replace Sprite in Image";
                using (new GUILayout.HorizontalScope(GUILayout.Height(10)))
                {
                    GUI_ChangeColor(i);
                    modifiersEnabled = false;
                    if (strpath[i].Contains(commonPath)) //|| strpath[i].Contains("Game/Common")
                    {
                        GUI.backgroundColor = gameObjectCommonColor;
                        tooltip = "Assets is common.\n" + tooltip;
                        if (i == currElementNum)
                        {
                            GUI.backgroundColor = gameObjectCommonColorSelected;
                        }
                    }

                    tooltip = Path.GetFileName(strpath[i]) + "\n\n" + strpath[i] + "\n" + tooltip;

                    if (viewMode == "List")
                    {
                        if (!isShowPathsList)
                        {
                            using (new GUILayout.HorizontalScope())
                            {
                                //Create  Number Button
                                if (GUILayout.Button(
                                        new GUIContent("" + (i + 1), tooltip), NumberButtonStyle, GUILayout.Width(wid2), GUILayout.Height(iconPreviewSize)))
                                {
                                    AddAssetButtonAction(i);
                                }

                                //Create image Preview Button
                                var button_tex = GenerateAssetPreview(i); //PreviewImagesList[i];

                                if (GUILayout.Button(
                                        new GUIContent(button_tex, tooltip), NumberButtonStyle, GUILayout.Width(iconPreviewSize),
                                        GUILayout.Height(iconPreviewSize)))

                                {
                                    AddAssetButtonAction(i);
                                    //Selection.activeObject = AssetDatabase.LoadMainAssetAtPath(strpath[i]);
                                    //EditorGUIUtility.PingObject(Selection.activeObject);
                                }

                                strobj[i] = EditorGUILayout.ObjectField("", strobj[i], typeof(System.Object), false);

                                if (GUILayout.Button(
                                        new GUIContent(EditorGUIUtility.Load("d_winbtn_win_close") as Texture2D, "delete item"), GUILayout.Width(middleWidth)))
                                {
                                    RemoveItem(i);
                                }
                            }
                        }
                    }

                    if (i == currElementNum)
                    {
                        //CurrBtnRect = GUILayoutUtility.GetLastRect();
                    }
                }

                GUILayout.Space(btsSpace);
            }

            GUI.contentColor = defaultContentColor;
            GUI.backgroundColor = defaultColor;
        }

        if (isShowPathsList)
        {
            GUILayout.FlexibleSpace();
            GUILayout.EndScrollView();
        }
        else
        {
            if (viewMode == "List")
            {
                GUILayout.FlexibleSpace();
                GUILayout.EndScrollView();
            }
            else
            {
            }
        }
    }

    private void RemoveItem(int i)
    {
        strpath.Remove(strpath[i]);
        Selection.objects = Selection.objects.RemoveAt(i);
        WriteToArea();
        PasteObjects();
    }
    
    private static void DeselectAll()
    {
        Selection.objects = null;
    }

    private void GUI_ChangeColor(int i)
    {
        if (i == currElementNum)
        {
            GUI.backgroundColor = ColorYellow;
        }
        else
        {
            GUI.backgroundColor = colors[(i % 2)];
        }
    }

    private void GUI_CreatePathsList()
    {
        //Scroller for strings
        scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, false /*, GUILayout.Height(area.height)*/);

        GUI.skin.textArea.wordWrap = false;
        GUI.skin.textArea.richText = true;
        //Highlighter.Highlight(windowName, strpath[currElementNum]);
        objectPathArea = GUILayout.TextArea(objectPathArea, /*style,*/GUILayout.ExpandHeight(true));
        EditorGUILayout.EndScrollView();
    }

    private void GUI_CreateNavBtns()
    {
        int temp = currElementNum;
        GUI.backgroundColor = defaultColor;
        if (GUILayout.Button(new GUIContent(EditorGUIUtility.Load("icons/d_Animation.PrevKey.png") as Texture2D, "prev item"), GUILayout.Width(middleWidth)))
        {
            --temp;
        }

        EditorGUI.BeginChangeCheck();
        currElementNumWidth = (currElementNum.ToString().Length > 3)
                                  ? 80
                                  : middleWidth;
        currElementNumField = int.Parse(GUILayout.TextField((currElementNumField).ToString(), GUILayout.Width(currElementNumWidth)));
        currElementNum = currElementNumField - 1;
        if (EditorGUI.EndChangeCheck())
        {
            temp = currElementNum;
            //int.TryParse(, out temp);
            currElementNum = Mathf.Clamp(currElementNum, 0, wordArray.Count - 1);
            Selection.activeObject = AssetDatabase.LoadMainAssetAtPath(
                wordArray[currElementNum].Split("\t"[0])[isDisplayNum
                                                             ? 1
                                                             : 0]);
            EditorGUIUtility.PingObject(Selection.activeObject);
        }

        if (GUILayout.Button(new GUIContent(EditorGUIUtility.Load("icons/d_Animation.NextKey.png") as Texture2D, "next item"), GUILayout.Width(middleWidth)))
        {
            ++temp;
        }

        GUI.backgroundColor = gameObjectCommonColor;
        if (GUILayout.Button(
                new GUIContent(EditorGUIUtility.IconContent("d_Favorite.png", "|Go to Common Folder")), GUILayout.Width(bigWidth), GUILayout.Height(20)))
        {
            ShowCommonFolder();
        }

        GUI.backgroundColor = defaultColor;

        if (temp != currElementNum)
        {
            currElementNum = temp;
            currElementNum = Mathf.Clamp(currElementNum, 0, wordArray.Count - 1);
            SelectCurrElement(wordArray, currElementNum);
        }

        currElementNumField = currElementNum + 1;
    }

    private void ShowCommonFolder()
    {
        var path = commonPath;
        Selection.activeObject = AssetDatabase.LoadMainAssetAtPath(path);
        EditorGUIUtility.PingObject(Selection.activeObject);
    }

    private void SelectDependencies()
    {
        UnityEngine.Object obj = Selection.objects[0];
        UnityEngine.Object[] roots = new UnityEngine.Object[] { obj };
        Selection.objects = EditorUtility.CollectDependencies(roots);

        Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Editable | SelectionMode.TopLevel);
    }

    private void SelectFromArea()
    {
        if (objectPathArea.Length > 0)
        {
            string objectPath;
            UnityEngine.Object[] objects = new UnityEngine.Object[wordArray.Count];
            for (int i = 0; i < wordArray.Count; i++)
            {
                objectPath = wordArray[i].Split("\t"[0])[(isDisplayNum)
                                                             ? 1
                                                             : 0];
                objects[i] = AssetDatabase.LoadMainAssetAtPath(objectPath);
            }

            Selection.objects = objects;
        }
    }

    private void GUI_SelectAllBtn()
    {
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

        target = EditorGUILayout.ObjectField(target, typeof(UnityEngine.Object), true);
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
                FolderFromFileLabelText = "";
            }
        }

        string targetLabel = (isTargetFolder)
                                 ? FolderPath
                                 : "Select target folder";

        EditorGUILayout.LabelField(EditorGUIUtility.IconContent("d_Project.png", "|Target Folder"), GUILayout.Width(24));
        EditorGUILayout.LabelField(targetLabel); //EditorGUILayout.LabelField(targetLabel);//

        GUI.contentColor = ColorOrange;
        GUI.backgroundColor = ColorOrange;

        GUI.contentColor = defaultContentColor;
        GUI.backgroundColor = colors[0];

        GUILayout.ExpandWidth(true);
    }

    private void GUI_DuplicateOrMove()
    {
        GUI.enabled = (Selection.objects.Length > 0);
        isDuplicate = GUILayout.Toggle(
            isDuplicate, ((isDuplicate)
                              ? " " + "Duplicate" + " "
                              : " " + "Move" + " ") + strpath.Count + " " + "item(s)");
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
        if (GUILayout.Button(
                (((isDuplicate)
                      ? " Duplicate "
                      : " Move ") + "To Folder "), GUILayout.Height(middleHeight)))
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
            if (GUILayout.Button(
                    new GUIContent("Update Sprites in scene", "Select Sprites From Project and press to replace sprites in scene"),
                    GUILayout.Height(middleHeight)))
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

        // create Top elements
        using (new GUILayout.VerticalScope("grey_border")) //GroupBox
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

            fromGameObjects = (isFromScene)
                                  ? "From Scene"
                                  : "Assets";

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

                GUILayout.Space(16);
                if (GUILayout.Button(
                        EditorGUIUtility.IconContent("d__Popup.png", "|Open Color Settings"), GUILayout.Width(30))) //ClothInspector.SettingsTool.png
                {
                    LoadPlayerPrefs();
                    if (Type.GetType("Hierarchy_Extend_Editor") != null) Hierarchy_Extend_Editor.ShowWindow();
                }
            }

            GUI.enabled = true;

            GUI.enabled = isFromScene;
            using (new GUILayout.HorizontalScope())
            {
                GUI_AssetTypes();
            }

            using (new GUILayout.HorizontalScope()) //MeTransOnRight//flow node 1
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

            GUI_VieModeButtons();
        }

        if (strpath != null)
        {
            using (new GUILayout.VerticalScope())
            {
                using (new GUILayout.HorizontalScope())
                {
                    GUI_NumberButtonsList();
                    if (isShowPathsList)
                    {
                        GUI_CreatePathsList();
                    }
                }
            }

            lowrect = GUILayoutUtility.GetLastRect();

            DrawTiles();

            using (new GUILayout.VerticalScope("grey_border"))
            {
                using (new GUILayout.HorizontalScope())
                {
                    GUI_PropsChList();
                }

                GUI_SelectedLineLabel();

                ParseTexArea();

                using (new GUILayout.HorizontalScope())
                {
                    GUI_CreateNavBtns();
                    GUILayout.Space(5);
                    GUI_SelectAllBtn();
                }

                using (new GUILayout.HorizontalScope())
                {
                    GUI_TargetFolder();
                }

                //targetPopup = EditorGUILayout.ObjectField(target, typeof(Object), true);
                using (new GUILayout.HorizontalScope())
                {
                    GUI_UpdateAttributes();
                }

                using (new GUILayout.HorizontalScope())
                {
                    GUI_DuplicateOrMove();
                    GUILayout.Space(5);
                    GUIDuplicateBtns();
                }
            }
        }

        GUI.enabled = true;
    }

    private void GUI_VieModeButtons()
    {
        using (new GUILayout.HorizontalScope())
        {
            GUILayout.FlexibleSpace();
            var icon = EditorGUIUtility.IconContent("d_align_horizontally_left", "|Switch to Path view");
            if (GUILayout.Button(icon)) //"List"
            {
                viewMode = "List";
                isShowPathsList = true;
            }

            icon = EditorGUIUtility.IconContent("d_align_vertically_top", "|Switch to List view");
            if (GUILayout.Button(icon)) //"List"
            {
                viewMode = "List";
                isShowPathsList = false;
            }

            icon = EditorGUIUtility.IconContent("d_GridLayoutGroup Icon", "|Switch to Thumb view");
            if (GUILayout.Button(icon))
            {
                viewMode = "Tiles";
                isShowPathsList = false;
            }
        }

        toprect = GUILayoutUtility.GetLastRect();
    }

    private void DrawTiles()
    {
        var fr = 4;
        var initx = lowrect.x + fr; //+ toprect.height;
        var inity = lowrect.y + fr; //+ toprect.height;
        var viewrect = new Rect(initx, inity, toprect.width - 2 * fr, lowrect.height - 2 * fr);
        var realrect = new Rect(initx, inity, toprect.width - 2 * fr - 2, scrollHeight + fr);
        scrollPositionTiles = GUI.BeginScrollView(viewrect, scrollPositionTiles, realrect);
        var hor = 0;
        var ver = 0;
        for (var i = 0; i < strpath.Count; i++)
        {
            var tooltip = "'Shift' to show in Project\n'Alt' to Find In Scene\n'Control' to replace Sprite in Image";
            using (new GUILayout.HorizontalScope(GUILayout.Height(10)))
            {
                GUI_ChangeColor(i);
                modifiersEnabled = false;
                if (strpath[i].Contains(commonPath)) //|| strpath[i].Contains("Game/Common")
                {
                    GUI.backgroundColor = gameObjectCommonColor;
                    tooltip = "Assets is common.\n" + tooltip;
                    if (i == currElementNum) GUI.backgroundColor = gameObjectCommonColorSelected;
                }

                tooltip = Path.GetFileName(strpath[i]) + "\n\n" + strpath[i] + "\n" + tooltip;

                if (viewMode == "Tiles")
                {
                    if (initx + (hor + 1) * iconPreviewSize > Screen.width - 2 * fr)
                    {
                        hor = 0;
                        ver++;
                        scrollHeight = (ver + 1) * iconPreviewSize;
                    }

                    var guiContent = new GUIContent(
                        GenerateAssetPreview(i), tooltip); //, NumberButtonStyle, GUILayout.Width(iconPreviewSize),GUILayout.Height(iconPreviewSize)
                    var guiRect = new Rect(initx + hor * iconPreviewSize, inity + ver * iconPreviewSize, iconPreviewSize, iconPreviewSize);
                    hor++;

                    if (
                        GUI.Button(guiRect, guiContent, NumberButtonStyle)
                    )
                    {
                        AddAssetButtonAction(i);
                    }
                }

                if (i == currElementNum)
                {
                    //CurrBtnRect = GUILayoutUtility.GetLastRect();
                }
            }
        }

        GUI.EndScrollView();
    }

    private Texture2D GenerateAssetPreview(int i)
    {
        var button_tex = PreviewImagesList[i];
        if (strpath[i].Contains(".prefab"))
        {
            button_tex = EditorGUIUtility.FindTexture("Prefab Icon");
        }

        if (strpath[i].Contains(".anim"))
        {
            button_tex = AssetPreview.GetMiniTypeThumbnail(typeof(AnimationClip));
        }

        if (strpath[i].Contains(".material"))
        {
            button_tex = EditorGUIUtility.FindTexture("Material Icon");
        }

        if (strpath[i].Contains(".cs"))
        {
            button_tex = EditorGUIUtility.FindTexture("d_cs Script Icon");
        }

        if (strpath[i].Contains(".shader"))
        {
            button_tex = EditorGUIUtility.FindTexture("Shader Icon");
        }

        return button_tex;
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
        UnityEngine.Object[] SceneModels = UnityEngine.Object.FindObjectsOfTypeAll(typeof(MeshFilter));
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

        UnityEngine.Object[] SceneAnimations = UnityEngine.Object.FindObjectsOfTypeAll(typeof(Animation));
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

        UnityEngine.Object[] SceneImages = UnityEngine.Object.FindObjectsOfTypeAll(typeof(Image));
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

    private void ApplyModifyInPrefab(UnityEngine.Object ob, string name)
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
                        UnityEngine.Object _object = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(strpath[index]);
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
        UnityEngine.Object[] objects = new UnityEngine.Object[refs.Length];
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
        UnityEngine.Object[] SceneImages = UnityEngine.Object.FindObjectsOfTypeAll(typeof(Image));
        var curr = 0;
        var replaceStr = "";
        int ImagesToCheckTotal = SceneImages.Length; //* Selection.objects.Length
        //var pBarMessage = "Importing...";

        for (int s = 0; s < SceneImages.Length; s++)
        {
            bool isModifyCurrPrefab = false;
            for (int i = 0; i < Selection.objects.Length; i++)
            {
                string path = AssetDatabase.GetAssetPath(Selection.objects[i]);

                if (isSprites && Selection.objects[i] is Texture2D)
                {
                    curr = s; //(i + 1) *
                    var progress = (float) curr / ImagesToCheckTotal;
                    var pBarMessage = (curr + " / " + ImagesToCheckTotal) + "\tChecking: " + SceneImages[s].name;
                    if (EditorUtility.DisplayCancelableProgressBar("Wait please", pBarMessage, progress))
                    {
                        break;
                    }

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
