using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;
using System.IO;
using UnityEngine.UI;
using System.Text;
using TMPro;

//for pbar
using System.Threading;

//for atlas
using UnityEngine.U2D;
using UnityEditor.U2D;
using System.Linq;

public class ImportFromJSON : EditorWindow//, IPanelTool
{
    private Color[] colors = new Color[] { new Color(1f, 1f, 1f, 1f), new Color(0.5f, 0.8f, 0.0f, 1f), new Color(1f, 0.1f, 0.1f, 1f) };
    private string json;
    private string DocName;
    private string DocSrc;
    private string DocExportDate;
    private DocData loadedLayersData;
    private int LayersDataTotal;

    private string JSONFilePath = "";
    private GameObject PopupContainer;
    private Object SpritesFolderObject;
    private Object DefaultFontObject;
    private bool isImportToAssets = true;
    private bool isOverWriteFiles = false;
    private bool isTargetFolder;
    private bool isRectContainer;
    private bool isActiveGameObjectAsContainer;

    private string SourceFolderPath;
    private string SourceFilePath;

    private bool isSourceFileExists;
    private bool isDestinationFileExists;
    private string DestinationFilePath;
    private string TargetFolderPath;
    private bool isUseDefaultFont = true;
    private bool isUseTMPro = true;
    private bool _finished;

    private string SpriteAtlasPath;
    private SpriteAtlas CurrAtlas;
    public Sprite[] CurrAtlasSprites;
    public Object CurrAtlasO;
    public bool isSaveAtlasChanges = true;
    private string CurrAtlasPath;
    public bool isUpdateAtlas = false;
    public bool isCreatePrefab = false;

    private float shortwidth = 12;

    private List<string> sourceToOverwrite = new List<string>();
    private List<string> targetToOverwrite = new List<string>();

    private Vector2 scrollPosition = Vector2.zero;
    private bool isUpdateAssets;
    private bool isShowImageList;
    private int iconPreviewSize = 100;


    private List<Texture2D> PreviewImagesList1 = new List<Texture2D>();
    private List<Texture2D> PreviewImagesList2 = new List<Texture2D>();
    private bool texturesForPreviewsLoaded = false;

    private bool showNewAssets;


    //private GUIStyle contentStyle = new GUIStyle();
    private void OnDisable()
    {
        //_styleInitialized = false;
    }

    private void InitStyles()
    {
        //contentStyle.alignment = TextAnchor.MiddleLeft;
       // _styleInitialized = true;
    }

    public string ToolName { get; set; }
    public ImportFromJSON()
    {
        ToolName = typeof(ImportFromJSON).Name;
    }

    public void Execute()
    {
        ImportFromJSON.exec();
    }
    public static void exec()
    {
        OpenFile();
    }

    [MenuItem("AssetTools/Import From JSON")]
    [MenuItem("GameObject/AssetTools/Import From JSON")]
    private static void OpenFile()
    {
        EditorWindow.GetWindow<ImportFromJSON>(" Import from JSON ");
    }

    private void CreatePopupContainer()
    {
        GameObject MyPopupCanvas = new GameObject();
        Camera maincam = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
        Debug.Log("Cam1 = " + maincam.name);
        MyPopupCanvas.name = "PopupCanvas";

        Canvas myCanvas = MyPopupCanvas.AddComponent<Canvas>();
        if (myCanvas != null)
        {
            myCanvas.renderMode = RenderMode.WorldSpace;
            myCanvas.worldCamera = maincam;
            myCanvas.renderMode = RenderMode.ScreenSpaceCamera;
            myCanvas.planeDistance = 44.0f;
        }

        CanvasScaler myCanvasScaler = MyPopupCanvas.AddComponent<CanvasScaler>();
        myCanvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        myCanvasScaler.referenceResolution = new Vector2(1080, 1920);
        myCanvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
        //myCanvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        //myCanvasScaler.matchWidthOrHeight = 1;
        GraphicRaycaster myRaycaster = MyPopupCanvas.AddComponent<GraphicRaycaster>();
        Undo.RegisterCreatedObjectUndo(MyPopupCanvas, "Create Popup Canvas");
    }
    private void OnEnable()
    {
        if (PopupContainer == null && !isActiveGameObjectAsContainer)
        {
            PopupContainer = Selection.activeGameObject;
            isActiveGameObjectAsContainer = true;
        }
        var defaultFontPath = "Assets/Fonts/MainFonts/Kimmy Design - LunchBoxBold.otf";
        if (File.Exists(defaultFontPath))
        {
            DefaultFontObject = AssetDatabase.LoadAssetAtPath<Font>(defaultFontPath);
        }

    }

    public float secs = 5f;

    private void BuildGUISource()
    {

        using (new GUILayout.VerticalScope("HelpBox"))
        {

            GUILayout.Space(10);
            EditorGUILayout.LabelField("Source:", EditorStyles.boldLabel, GUILayout.ExpandWidth(true));
            GUILayout.Space(5);

            using (new GUILayout.HorizontalScope(GUILayout.ExpandWidth(false)))
            {
                if (JSONFilePath != "")
                {
                    GUILayout.Space(shortwidth);
                    EditorGUILayout.LabelField("JSON Path: ", JSONFilePath, GUILayout.ExpandWidth(true));//, (JSONFilePath == null) ? GUILayout.MinWidth(120) : GUILayout.Width(120)
                }
                else
                {
                    GUI.backgroundColor = colors[1];
                }
            }
            using (new GUILayout.HorizontalScope())
            {
                if (DocName != "" && JSONFilePath != "")
                {
                    GUILayout.Space(shortwidth);
                    EditorGUILayout.LabelField("PSD name: ", DocName);
                }
                string ButtonPNGText = (JSONFilePath == "") ? "Load Layout File" : "Reload Layout File";
                if (GUILayout.Button(ButtonPNGText, ((JSONFilePath == "") ? GUILayout.MinWidth(120) : GUILayout.MinWidth(60))))
                {
                    ImportJSON();
                }
                GUI.backgroundColor = colors[0];
                if (JSONFilePath == "")
                {
                    return;
                }
            }
            using (new GUILayout.HorizontalScope())
            {
                if (DocExportDate != "" && JSONFilePath != "")
                {
                    GUILayout.Space(shortwidth);
                    EditorGUILayout.LabelField("Export Date: ", DocExportDate);
                }
            }
            GUILayout.Space(10);
        }
    }
    private void BuildGUISprites()
    {
        using (new GUILayout.VerticalScope("HelpBox"))
        {
            GUILayout.Space(10);
            EditorGUILayout.LabelField("Sprites:", EditorStyles.boldLabel);
            GUILayout.Space(5);

            using (new GUILayout.HorizontalScope())
            {
                GUILayout.Space(shortwidth);
                isImportToAssets = GUILayout.Toggle(isImportToAssets, "Import Assets");
            }
            using (new GUILayout.HorizontalScope())
            {
                GUILayout.Space(shortwidth);
                if (isOverWriteFiles)
                {
                    GUI.backgroundColor = colors[0];
                }
                else
                {
                    GUI.backgroundColor = colors[1];
                }
                isOverWriteFiles = GUILayout.Toggle(isOverWriteFiles, "Overwrite Images In folder");

            }
            using (new GUILayout.HorizontalScope())
            {
                GUILayout.Space(shortwidth);
                if (SpritesFolderObject != null && isTargetFolder)
                {
                    GUI.backgroundColor = colors[0];
                }
                else
                {
                    GUI.backgroundColor = colors[2];
                }
                SpritesFolderObject = EditorGUILayout.ObjectField("Sprites Folder", SpritesFolderObject, typeof(Object), false);

                GUI.backgroundColor = colors[0];
            }

            GUI.backgroundColor = colors[0];

            using (new GUILayout.HorizontalScope())
            {
                GUILayout.Space(shortwidth);
                GUI.enabled = (SpritesFolderObject != null);
                if (GUILayout.Button(new GUIContent("Check for new Images", "Show new images to import from layout file")))
                {
                    //_itemsCreated = false;
                    isShowImageList = true;
                    checkImagesToUpdate();
                    if (sourceToOverwrite.Count > 0)
                    {
                        showNewAssets = true;
                    }
                }
            }
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.GetControlRect(true, 16f, EditorStyles.foldout);
		Rect foldRect = GUILayoutUtility.GetLastRect();
            if (Event.current.type == EventType.MouseUp && foldRect.Contains(Event.current.mousePosition))
            {
                showNewAssets = !showNewAssets;
                GUI.changed = true;
                Event.current.Use();
            }
                showNewAssets = EditorGUI.Foldout(foldRect, showNewAssets, new GUIContent((sourceToOverwrite.Count > 0) ?"New Assets ( " + sourceToOverwrite.Count+" )" : "No new assets", "Press \"Check for new Images\" to scan for new images from layout file"));
            if (EditorGUI.EndChangeCheck())
            {
            }
            if (showNewAssets && SpritesFolderObject != null)
            {
                if (sourceToOverwrite.Count > 0)
                {
                    GUILayout.Space(5);

                    using (new GUILayout.HorizontalScope())
                    {
                        GUILayout.Space(shortwidth);
                        EditorGUILayout.LabelField(new GUIContent("Icons Size", "Change Icon size for your will"));
                    }

                    using (new GUILayout.HorizontalScope())
                    {
                        GUILayout.Space(shortwidth);
                        if (GUILayout.Button("20", GUILayout.Width(64)))
                        {
                            iconPreviewSize = 20;
                            checkImagesToUpdate();
                        }
                        if (GUILayout.Button("50", GUILayout.Width(64)))
                        {
                            iconPreviewSize = 50;
                            checkImagesToUpdate();
                        }
                        if (GUILayout.Button("100", GUILayout.Width(64)))
                        {
                            iconPreviewSize = 100;
                            checkImagesToUpdate();
                        }
                        if (GUILayout.Button("200", GUILayout.Width(64)))
                        {
                            iconPreviewSize = 200;
                            checkImagesToUpdate();
                        }
                    }
                    if (isShowImageList)
                    {
                        CreateimagesToReimport();
                    }
                    if (isShowImageList)
                    {
                        GUI.enabled = isOverWriteFiles;
                        if (GUILayout.Button(new GUIContent("Replace all Images", (GUI.enabled) ? "Import all new assets or replaces old" : "Enable \"Overwrite images\" to replace")))
                        {
                            //isOverWriteFiles = true;

                            ReplaceImages();
                            checkImagesToUpdate();
                        }
                        GUI.enabled = true;

                    }
                }
                GUILayout.Space(10);
            }
        }
    }
    private void BuildGUISceneObjects()
    {
        using (new GUILayout.HorizontalScope())
        {
            //GUILayout.Space(shortwidth);
            using (new GUILayout.VerticalScope("HelpBox"))
            {


                GUILayout.Space(10);
                EditorGUILayout.LabelField("Scene Objects:", EditorStyles.boldLabel);
                GUILayout.Space(5);
                using (new GUILayout.HorizontalScope())
                {
                    GUILayout.Space(shortwidth);
                    PopupContainer = (GameObject) EditorGUILayout.ObjectField("PopupContainer", PopupContainer, typeof(GameObject), true);
                }
                if (!isRectContainer)
                {
                    GUI.backgroundColor = colors[0];
                    EditorGUILayout.HelpBox("Select UI Container", MessageType.Error);
                }
                using (new GUILayout.HorizontalScope())
                {
                    GUILayout.Space(shortwidth);
                    isUseTMPro = GUILayout.Toggle(isUseTMPro, "Use Text Mesh Pro");
                }
                if (!isUseTMPro)
                {
                    using (new GUILayout.HorizontalScope())
                    {
                        GUILayout.Space(shortwidth);
                        isUseDefaultFont = GUILayout.Toggle(isUseDefaultFont, "Use Selected Font as Default");
                        DefaultFontObject = EditorGUILayout.ObjectField("", DefaultFontObject, typeof(Font), false);
                    }
                }
                if (PopupContainer != null && PopupContainer.gameObject && PopupContainer.GetComponent<RectTransform>() != null)
                {
                    GUI.backgroundColor = colors[0];
                    isRectContainer = true;
                }
                else
                {
                    GUI.backgroundColor = colors[2];
                    isRectContainer = false;

                }
                string btnCreateText = (PopupContainer != null) ? "Create Sprites in Container: " + PopupContainer.name : "Create " + (Path.GetFileNameWithoutExtension(DocName) + " Popup");

                isTargetFolder = (Path.GetExtension(AssetDatabase.GetAssetPath(SpritesFolderObject)) == "");
                if (SpritesFolderObject != null && isTargetFolder && isRectContainer)
                {

                    using (new GUILayout.HorizontalScope())
                    {
                        GUILayout.Space(shortwidth);
                        isUpdateAtlas = GUILayout.Toggle(isUpdateAtlas, "Save atlas");
                    }
                    using (new GUILayout.HorizontalScope())
                    {
                        GUILayout.Space(shortwidth);
                        isCreatePrefab = GUILayout.Toggle(isCreatePrefab, "Create Prefab");
                    }
                    GUILayout.Space(5);
                    if (GUILayout.Button(btnCreateText, GUILayout.Height(32)))
                    {
                        _finished = false;
                        CreateSceneElements();
                        if (isUpdateAtlas)
                        {
                            UpdateAtlas();
                        }
                        if (isCreatePrefab)
                        {
                            CreatePrefab();
                        }
                        checkImagesToUpdate();
                    }
                    //isSaveAtlasChanges = GUILayout.Toggle(isSaveAtlasChanges, "Save sprites in Atlas after import");
                    if (_finished)
                    {
                        if (GUILayout.Button("Update Atlas from Sprites' folder"))
                        {
                            UpdateAtlas();
                        }
                        if (GUILayout.Button("Create Prefab"))
                        {
                            CreatePrefab();
                        }
                    }
                }
                else
                {
                    GUI.backgroundColor = colors[2];
                }
                GUILayout.Space(10);
            }

            GUI.backgroundColor = colors[0];
        }

    }
    private void OnGUI()
    {
        if (Event.current.type == EventType.Repaint)
        {
            SceneView.RepaintAll();
        }
        var oldTextAnchor = GUI.skin.button.alignment;

        //GUI.skin.button.alignment = TextAnchor.MiddleLeft;
        BuildGUISource();
        GUI.enabled = (JSONFilePath != "");
        BuildGUISprites();
        BuildGUISceneObjects();
    }


    private void CreatePrefab()
    {
        string prefabPath = TargetFolderPath + "/" + PopupContainer.name + ".prefab";
        if (Path.GetFileName(TargetFolderPath) == "Sprites")
        {
            string parent = System.IO.Directory.GetParent(TargetFolderPath).FullName;
            if (!Directory.Exists(parent + "/Prefabs"))
            {
                Directory.CreateDirectory(parent + "/Prefabs");
            }
            prefabPath = parent + "/Prefabs/" + PopupContainer.name + ".prefab";
        }
        AssetDatabase.DeleteAsset(prefabPath);
        //PrefabUtility.CreatePrefab(prefabPath, PopupContainer, ReplacePrefabOptions.ConnectToPrefab);
        PrefabUtility.SaveAsPrefabAssetAndConnect(PopupContainer, prefabPath, InteractionMode.AutomatedAction);
    }

    private void ReplaceImages()
    {
        var curr = 0;
        int ImagesToReplaceTotal = sourceToOverwrite.Count;
        var pBarMessage = "Importing...";

        for (int i = 0; i < sourceToOverwrite.Count; i++)
        {
            var spriteName = Path.GetFileName(sourceToOverwrite[i]);
            curr = i;
            var progress = (float) curr / ImagesToReplaceTotal;
            pBarMessage = (curr + " / " + ImagesToReplaceTotal) + " Importing " + Path.GetFileName(sourceToOverwrite[i]);
            if (EditorUtility.DisplayCancelableProgressBar("Wait please", pBarMessage, progress))
            { break; }

            CopyFile(Path.GetFileName(sourceToOverwrite[i]));

            EditorWindow view = EditorWindow.GetWindow<SceneView>();
            view.Repaint();
        }
        EditorUtility.ClearProgressBar();
    }
    private void checkImagesToUpdate()
    {
        int curr = 0;
        LayersDataTotal = loadedLayersData.sprites.Length;
        var pBarMessage = "Checking for new Images...";

        SourceFolderPath = Path.GetDirectoryName(JSONFilePath);
        TargetFolderPath = AssetDatabase.GetAssetPath(SpritesFolderObject);

        sourceToOverwrite.Clear();
        targetToOverwrite.Clear();

        for (int i = 0; i < loadedLayersData.sprites.Length; i++)
        {
            var spriteName = loadedLayersData.sprites[i];
            //foreach (var spriteName in loadedLayersData.sprites)
            //{
            curr = i;
            var progress = (float) curr / LayersDataTotal;
            pBarMessage = (curr + " / " + LayersDataTotal) + " Checking " + spriteName.name;
            EditorUtility.DisplayProgressBar("Wait please", pBarMessage, progress);

            string CurrFileName = Path.GetFileName(spriteName.name);
            EditorWindow view = EditorWindow.GetWindow<SceneView>();
            view.Repaint();

            SourceFilePath = SourceFolderPath + "/" + Path.GetFileNameWithoutExtension(DocName) + "-assets/" + CurrFileName;
            DestinationFilePath = TargetFolderPath + "/" + CurrFileName;
            if (IsSameContent(SourceFilePath, DestinationFilePath) == false)
            {
                sourceToOverwrite.Add(SourceFilePath);
                targetToOverwrite.Add(DestinationFilePath);
            }
        }
        EditorUtility.ClearProgressBar();

        texturesForPreviewsLoaded = false;
        CreateimagesToReimport();
    }
    private void CreateimagesToReimport()
    {
        if (sourceToOverwrite.Count > 0)
        {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, false, false);
            CreateScrollItems();
            EditorGUILayout.EndScrollView();
        }
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

    private string ShortenDisplayString(string str)
    {
        int len = 60;
        int p1 = 10;

        //if (str.Length > len)
        //{
        // string part1 = str.Substring(0, p1);
        // string part2 = str.Substring(str.Length - (len - p1));
        // str = part1 + "..." + part2;
        //}
        return str;
    }

    private void LoadPreviews()
    {
        PreviewImagesList1.Clear();
        PreviewImagesList2.Clear();

        for (int i = 0; i < sourceToOverwrite.Count; i++)
        {
            if (!texturesForPreviewsLoaded)
            {
                Texture2D sourceimageTexture = LoadTempImage(sourceToOverwrite[i]); //(Texture2D) AssetDatabase.LoadAssetAtPath(sourceToOverwrite[i], typeof(Texture2D));
                Texture2D targetimageTexture = (Texture2D) AssetDatabase.LoadAssetAtPath(targetToOverwrite[i], typeof(Texture2D));

                PreviewImagesList1.Add(sourceimageTexture);
                PreviewImagesList2.Add(targetimageTexture);
            }
        }
        texturesForPreviewsLoaded = true;
    }

    private void CreateScrollItems()
    {
        for (int i = 0; i < sourceToOverwrite.Count; i++)
        {
            if (!texturesForPreviewsLoaded)
            {
                LoadPreviews();
            }
            using (new GUILayout.VerticalScope("HelpBox"))
            {


                GUILayout.Space(shortwidth);
                using (new GUILayout.HorizontalScope())
                {

                    if (GUILayout.Button(PreviewImagesList2[i], GUILayout.Width(iconPreviewSize), GUILayout.Height(iconPreviewSize)))
                    //GUILayout.Box(PreviewImagesList1[i], GUILayout.Width(iconPreviewSize), GUILayout.Height(iconPreviewSize));
                    {
                        Selection.activeObject = AssetDatabase.LoadMainAssetAtPath(targetToOverwrite[i]);
                        EditorGUIUtility.PingObject(Selection.activeObject);
                    }
                    using (new GUILayout.HorizontalScope())
                    {
                        GUILayout.FlexibleSpace();
                        using (new GUILayout.VerticalScope())
                        {
                            GUILayout.FlexibleSpace();
                            GUILayout.Label(" ->> ");
                            GUILayout.FlexibleSpace();
                        }
                        GUILayout.FlexibleSpace();
                    }
                    //GUILayout.Box(PreviewImagesList1[i], GUILayout.Width(iconPreviewSize), GUILayout.Height(iconPreviewSize));

                    if (GUILayout.Button(PreviewImagesList1[i], GUILayout.Width(iconPreviewSize), GUILayout.Height(iconPreviewSize)))
                    //GUILayout.Box(PreviewImagesList1[i], GUILayout.Width(iconPreviewSize), GUILayout.Height(iconPreviewSize));
                    {
                        Selection.activeObject = AssetDatabase.LoadMainAssetAtPath(targetToOverwrite[i]);
                        EditorGUIUtility.PingObject(Selection.activeObject);
                    }
                }
                using (new GUILayout.HorizontalScope())
                {
                    string lbl = ShortenDisplayString(Path.GetFileName(targetToOverwrite[i]));
                    GUILayout.Label(lbl, GUILayout.ExpandWidth(true));

                    GUI.enabled = isOverWriteFiles;
                    if (GUILayout.Button(new GUIContent((PreviewImagesList2[i] == null)? "Import " + i : "Replace " + i, (GUI.enabled) ? "Replaces current asset" : "Enable \"Overwrite images\" to replace"), GUILayout.Width(128)))
                    {
                        CopyFile(Path.GetFileName(sourceToOverwrite[i]));
                        checkImagesToUpdate();
                    }
                    GUI.enabled = true;
                }
            }
            GUILayout.FlexibleSpace();
        }
        //_itemsCreated = true;
    }
    private void CopyToAssetFolder()
    {
        var curr = 0;
        LayersDataTotal = loadedLayersData.sprites.Length;
        var pBarMessage = "Importing...";

        SourceFolderPath = Path.GetDirectoryName(JSONFilePath);
        TargetFolderPath = AssetDatabase.GetAssetPath(SpritesFolderObject);

        //SourceFilePath = SourceFolderPath; //+ "/" + CurrFileName;

        string CurrFileNam_;

        for (int i = 0; i < loadedLayersData.sprites.Length; i++)
        {
            var spriteName = loadedLayersData.sprites[i];
            curr = i;
            var progress = (float) curr / LayersDataTotal;
            pBarMessage = (curr + " / " + LayersDataTotal) + " Importing " + spriteName.name;
            if (EditorUtility.DisplayCancelableProgressBar("Wait please", pBarMessage, progress))
            { break; }

            CurrFileNam_ = Path.GetFileName(spriteName.name);
            CopyFile(CurrFileNam_);
            EditorWindow view = EditorWindow.GetWindow<SceneView>();
            view.Repaint();
        }

        EditorUtility.ClearProgressBar();
    }
    private void CopyFile(string CurrFileName)
    {
        SourceFilePath = SourceFolderPath + "/" + Path.GetFileNameWithoutExtension(DocName) + "-assets/" + CurrFileName;
        isSourceFileExists = (File.Exists(SourceFilePath));
        DestinationFilePath = TargetFolderPath + "/" + CurrFileName;
        isDestinationFileExists = (File.Exists(DestinationFilePath));

        if (IsSameContent(SourceFilePath, DestinationFilePath) == false)
        {
            sourceToOverwrite.Add(SourceFilePath);
            targetToOverwrite.Add(DestinationFilePath);
        }

        if (isSourceFileExists) // && (IsSameContent(SourceFolderPath, TargetFolderPath) == false && isDestinationFileExists)
        {
            try
            {
                File.Copy(SourceFilePath, DestinationFilePath, isOverWriteFiles);
                //AssetDatabase.SaveAssets();
                AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
                if (File.Exists(DestinationFilePath))
                {
                    ConvertImageToSprite(DestinationFilePath);
                }
            }
            catch (IOException copyError)
            {
                System.Console.WriteLine(copyError.Message);
            }
        }
    }

    private void ConvertImageToSprite(string _FilePath)
    {
        var importer = AssetImporter.GetAtPath(_FilePath);
        TextureImporter textureImporter = (TextureImporter) importer;
        if (textureImporter.textureType != TextureImporterType.Sprite)
        {
            textureImporter.textureType = TextureImporterType.Sprite;
            //textureImporter.textureFormat = TextureImporterFormat.AutomaticCompressed;
            textureImporter.textureCompression = (TextureImporterCompression) 1;
            textureImporter.SaveAndReimport();
        }
    }

    private string GetRelativeFilePath(string fullPath)
    {
        if (fullPath == string.Empty)
        { return ""; }
        string relativeFilePath = "Assets" + fullPath.Substring(Application.dataPath.Length);
        return relativeFilePath;
    }

    private void ImportJSON()
    {
        JSONFilePath = EditorUtility.OpenFilePanelWithFilters("Select JSON File", "", new string[] { "JSON file", "json" });
        if (string.IsNullOrEmpty(JSONFilePath))
            return;
        //Debug.Log("JSONFilePath = " + JSONFilePath);

        string json = File.ReadAllText(JSONFilePath);
        loadedLayersData = JsonUtility.FromJson<DocData>(json);
        DocName = loadedLayersData.name;
        DocSrc = loadedLayersData.src;
        DocExportDate = loadedLayersData.info.date;
        string tempStr = "";
        tempStr += "name = " + loadedLayersData.name + "\n\n";
        foreach (var sprite in loadedLayersData.sprites)
        {
            tempStr += "\t name = " + sprite.name + "\n";
            tempStr += "\t x = " + sprite.x + "\n";
            tempStr += "\t y = " + sprite.y + "\n";
            tempStr += "\t width = " + sprite.width + "\n";
            tempStr += "\t height = " + sprite.height + "\n\n";
        }
        //Debug.Log("JSON parsed:\n"+tempStr);
    }

    private void CreateSceneElements()
    {
        //ImportJSON();
        //CreatePopupContainer();
        if (loadedLayersData == null)
        {
            ImportJSON();
        }
        TargetFolderPath = AssetDatabase.GetAssetPath(SpritesFolderObject);
        bool isDestinationFolderEmpty = (Directory.Exists(TargetFolderPath) && Directory.GetFiles(TargetFolderPath) == null);
        if (isDestinationFolderEmpty)
        {
            if (EditorUtility.DisplayDialog(
            " Import To Assets",
            "Seems you forgot to import Assets for popup...\n" +
            "Do you want to import in selected folder?",
            "Import",
            "Don't Import"))
            {
                //
                isImportToAssets = true;
            }
        }
        if (isImportToAssets)
        {
            CopyToAssetFolder();
        }

        Undo.IncrementCurrentGroup();
        Undo.SetCurrentGroupName("Create objects From JSON");
        var undoGroupIndex = Undo.GetCurrentGroup();


        foreach (var layerName in loadedLayersData.sprites)
        {
            var newObject = new GameObject();
            var rectTransform = newObject.AddComponent<RectTransform>();
            Undo.RegisterCreatedObjectUndo(newObject, "");

            newObject.name = Path.GetFileNameWithoutExtension(layerName.name);
            //newObject.transform.parent = PopupContainer.transform;
            newObject.transform.localPosition = Vector3.zero;
            newObject.transform.localScale = Vector3.one;

            PlaceLayerInHierarchy(newObject, layerName);

            if (layerName.type == "image" || layerName.type == "vectorMask")
            {
                var spriteImage = newObject.AddComponent<Image>();
                spriteImage.raycastTarget = false;

                var curr_path = AssetDatabase.GetAssetPath(SpritesFolderObject) + "/" + layerName.name;
                Sprite newSprite = AssetDatabase.LoadAssetAtPath<Sprite>(curr_path);
                spriteImage.sprite = newSprite;

                string parsedColor = layerName.fillColor;
                Color fillColor;

                if (layerName.type == "vectorMask")
                {
                    if (ColorUtility.TryParseHtmlString(parsedColor, out fillColor))
                    {
                        fillColor.a = layerName.opacity / 100;
                        spriteImage.color = fillColor;
                    }
                }
            }

            rectTransform = newObject.GetComponent<RectTransform>();
            float x = roundValue(rectTransform.rect.x);
            float y = roundValue(rectTransform.rect.y);
            float width = rectTransform.rect.width;
            float height = rectTransform.rect.height;

            rectTransform.anchorMin = new Vector2(0, 1);
            rectTransform.anchorMax = new Vector2(0, 1);
            //rectTransform.pivot = new Vector2(0.5f, 0.5f);


            width = roundValue(layerName.width);
            height = roundValue(layerName.height);
            rectTransform.sizeDelta = new Vector2(width, height);
            rectTransform.anchoredPosition = new Vector2(layerName.x + width / 2 - PopupContainer.transform.GetComponent<RectTransform>().rect.width / 2,
             -layerName.y - height / 2 + PopupContainer.transform.GetComponent<RectTransform>().rect.height / 2);

            rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            if (layerName.type == "text")
            {
                rectTransform.localRotation = Quaternion.Euler(0f, 0f, layerName.textContents.rotation);

                if (isUseTMPro)
                {
                    var textLayer = newObject.AddComponent<TextMeshProUGUI>();


                    if (layerName.textContents.italic == "true")
                    {
                        //textLayer.fontStyle = FontStyle.Italic;
                    }

                    switch (layerName.textContents.alignment)
                    {
                        case "Justification.CENTER":
                            textLayer.alignment = (TextAlignmentOptions) 514;
                            break;
                        case "Justification.LEFT":
                            textLayer.alignment = (TextAlignmentOptions) 513;
                            break;
                        case "Justification.RIGHT":
                            textLayer.alignment = (TextAlignmentOptions) 516;
                            break;

                        default:
                            break;
                    }


                    textLayer.raycastTarget = false;
                    float parsedSize1 = layerName.textContents.size;
                    int parsedSize = Mathf.RoundToInt(parsedSize1);
                    textLayer.fontSize = parsedSize;

                    string parsedColor = layerName.textContents.color;
                    Color textColor;
                    if (ColorUtility.TryParseHtmlString(parsedColor, out textColor))
                        textLayer.color = textColor;
                    string parsedContents = layerName.textContents.contents;
                    textLayer.text = parsedContents;

                    //textLayer.verticalOverflow = VerticalWrapMode.Overflow;
                }
                else
                {
                    var textLayer = newObject.AddComponent<Text>();
                    if (isUseDefaultFont && !isUseTMPro)
                    {
                        textLayer.font = (Font) DefaultFontObject;
                    }
                    else
                    {
                        //textLayer.font = layerName.textContents.font;
                    }
                    if (layerName.textContents.italic == "true")
                    {
                        textLayer.fontStyle = FontStyle.Italic;
                    }

                    switch (layerName.textContents.alignment)
                    {
                        case "Justification.CENTER":
                            textLayer.alignment = (TextAnchor) 4;
                            break;
                        case "Justification.LEFT":
                            textLayer.alignment = (TextAnchor) 3;
                            break;
                        case "Justification.RIGHT":
                            textLayer.alignment = (TextAnchor) 5;
                            break;

                        default:
                            break;
                    }

                    textLayer.raycastTarget = false;
                    float parsedSize1 = layerName.textContents.size;
                    int parsedSize = Mathf.RoundToInt(parsedSize1);
                    textLayer.fontSize = parsedSize;

                    string parsedColor = layerName.textContents.color;
                    Color textColor;
                    if (ColorUtility.TryParseHtmlString(parsedColor, out textColor))
                        textLayer.color = textColor;
                    string parsedContents = layerName.textContents.contents;
                    textLayer.text = parsedContents;

                    //EditorUtility.SetDirty(newObject);
                    //SceneView.RepaintAll();

                    Canvas.ForceUpdateCanvases();
                    textLayer.verticalOverflow = VerticalWrapMode.Overflow;

                    if (textLayer.cachedTextGenerator.lines.Count < 2)
                    {
                        textLayer.horizontalOverflow = HorizontalWrapMode.Overflow;
                    }
                }
            }

        }
        Undo.CollapseUndoOperations(undoGroupIndex);
        _finished = true;
    }
    private GameObject FindNodeByName(string NodeName)
    {
        GameObject currGroupGO = GameObject.Find(NodeName);
        if (NodeName.IndexOf(".psd") > 0 || NodeName == "Adobe Photoshop")
        {
            currGroupGO = PopupContainer;
        }
        if (!currGroupGO)
        {
            currGroupGO = new GameObject(NodeName);
            currGroupGO.transform.parent = PopupContainer.transform;
            currGroupGO.transform.localPosition = Vector3.zero;
            currGroupGO.transform.localScale = Vector3.one;

            RectTransform recTransContent = currGroupGO.AddComponent<RectTransform>();
        }
        return (currGroupGO);
    }
    private void PlaceLayerInHierarchy(GameObject layer, spriteData JSONLayer)
    {
        GameObject GroupGO = FindNodeByName(JSONLayer.layerParent);
        if (GroupGO != PopupContainer)
        {
            GroupGO.transform.parent = FindNodeByName(JSONLayer.groupParent).transform;
            GroupGO.transform.localPosition = Vector3.zero;
            var recTransContent = GroupGO.GetComponent<RectTransform>();
            recTransContent.anchorMin = new Vector2(0, 0);
            recTransContent.anchorMax = new Vector2(1, 1);
            recTransContent.offsetMin = new Vector2(0, 0);
            recTransContent.offsetMax = new Vector2(0, 0);
            recTransContent.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        }
        layer.transform.parent = GroupGO.transform;
        layer.transform.localPosition = new Vector3(layer.transform.localPosition.x, layer.transform.localPosition.y, 0);
        layer.transform.localScale = Vector3.one;
    }

    private float roundValue(float _num)
    {
        var num = System.Convert.ToDouble(_num);
        return ((float) System.Math.Round(num, 2));
    }

    [System.Serializable]
    private class DocData
    {
        public string name;
        public string src;
        public DocExport info;
        public spriteData[] sprites;
    }
    [System.Serializable]
    private class DocExport
    {
        public string date;
    }

    [System.Serializable]
    private class spriteData
    {
        public string name;
        public float x;
        public float y;
        public float width;
        public float height;
        public string type;
        public float opacity;
        public string fillColor;
        public textData textContents;
        public string layerParent;
        public string groupParent;
    }
    [System.Serializable]
    private class textData
    {
        public string font;
        public float size;
        public string color;
        public string alignment;
        public float rotation;
        public string italic;
        public string contents;
    }
    private List<string> GetPathsFromFolder(List<string> AssetPaths, string path)
    {
        foreach (string spritePath in System.IO.Directory.GetFiles(path))
        {
            AssetPaths.Add(spritePath);
        }
        return (AssetPaths);
    }

    private void CreateAtlasFromSprites()
    {
        SpriteAtlas CurrAtlas = new SpriteAtlas();
        AssetDatabase.CreateAsset(CurrAtlas, SpriteAtlasPath);
        foreach (var obj in Selection.objects)
        {
            Object o = obj as Sprite;
            if (o != null)
                SpriteAtlasExtensions.Add(CurrAtlas, new Object[] { o });
        }
        AssetDatabase.SaveAssets();
    }
    private void CreateAtlasFromFolder()
    {
        SpriteAtlas CurrAtlas = new SpriteAtlas();
        //string parent = System.IO.Directory.GetParent(TargetFolderPath).FullName;
        SpriteAtlasPath = TargetFolderPath + "/_" + PopupContainer.name + ".spriteatlas";
        if (File.Exists(SpriteAtlasPath))
        {
            CurrAtlasO = AssetDatabase.LoadAssetAtPath<Object>(SpriteAtlasPath);
        }
        else
        {
            //Object CurrAtlasO = new Object();
            AssetDatabase.CreateAsset(CurrAtlas, SpriteAtlasPath);
        }
        Object SpritesFolderObject = AssetDatabase.LoadAssetAtPath<Object>(TargetFolderPath);

        if (isSaveAtlasChanges)
        {
            SpriteAtlasExtensions.Add(CurrAtlas, new Object[] { SpritesFolderObject });
            //SpriteAtlasExtensions.Remove(CurrAtlas, CurrAtlas.GetPackables());
            //SpriteAtlasExtensions.Add(CurrAtlas, AtlasSpritesModified.ToArray());
            AssetDatabase.SaveAssets();
        }
        AssetDatabase.SaveAssets();
    }
    private void UpdateAtlas()
    {
        CreateAtlasFromFolder();
        //UpdateImagesInAtlas()
    }

    public Texture2D[] imgs;
    private int x, y;
    private bool isSameImagesBytes(string path1, string path2)
    {

        bool isSameImages = false;
        imgs = new Texture2D[2];
        imgs[0] = new Texture2D(1, 1);
        imgs[1] = new Texture2D(1, 1);
        imgs[0].LoadImage(File.ReadAllBytes(path1));
        imgs[1].LoadImage(File.ReadAllBytes(path2));
        x = imgs[0].width;
        y = imgs[0].height;

        if (x != imgs[1].width || y != imgs[1].height)
        { isSameImages = true; }
        else
        {
            while (x > 0)
            {
                x--;
                y = imgs[0].height;
                while (y > 0)
                {
                    y--;
                    if (imgs[0].GetPixel(x, y) != imgs[1].GetPixel(x, y))
                    {
                        isSameImages = true;
                    }
                }
            }
        }
        return (isSameImages);
    }


    private byte[] ComputeFileHash(string fileName)
    {
        using (var stream = File.OpenRead(fileName))
            return System.Security.Cryptography.MD5.Create().ComputeHash(stream);
    }

    private bool IsSameContent(string file1, string file2)
    {
        if (file1 == file2)
            return true;

        FileInfo file1Info = new FileInfo(file1);
        FileInfo file2Info = new FileInfo(file2);

        if (!file1Info.Exists && !file2Info.Exists)
            return true;
        if (!file1Info.Exists && file2Info.Exists)
            return false;
        if (file1Info.Exists && !file2Info.Exists)
            return false;
        if (file1Info.Length != file2Info.Length)
            return false;

        using (FileStream file1Stream = file1Info.OpenRead())
        using (FileStream file2Stream = file2Info.OpenRead())
        {
            byte[] firstHash = System.Security.Cryptography.MD5.Create().ComputeHash(file1Stream);
            byte[] secondHash = System.Security.Cryptography.MD5.Create().ComputeHash(file2Stream);
            for (int i = 0; i < firstHash.Length; i++)
            {
                if (i >= secondHash.Length || firstHash[i] != secondHash[i])
                    return false;
            }
            return true;
        }
    }
    //private void UpdateImagesInAtlas()
    //{
    // SpriteAtlasPath = TargetFolderPath + "/_" + PopupContainer.name + ".spriteatlas";
    // CurrAtlasPath = SpriteAtlasPath;
    // CurrAtlasO = AssetDatabase.LoadAssetAtPath<Object>(SpriteAtlasPath);
    // CurrAtlas = CurrAtlasO as SpriteAtlas;
    // if (CurrAtlas == null)
    // return;

    // var spriteCount = CurrAtlas.spriteCount;
    // var sprites = new Sprite[spriteCount];
    // CurrAtlas.GetSprites(sprites);

    // Object[] atlasPackables = CurrAtlas.GetPackables();

    // List<Object> AtlasSpritesInit = new List<Object>(atlasPackables);
    // List<Object> AtlasSpritesModified = new List<Object>(atlasPackables);

    // List<string> SpritePaths = new List<string>();

    // SpritePaths = GetPathsFromFolder(SpritePaths, TargetFolderPath);
    // SpriteObjects = GetPathsFromFolder(SpritePaths, TargetFolderPath);

    // var atlasPackablesString = "";
    // for (var i = 0; i < spriteCount; i++)
    // {
    // string SpritePath = AssetDatabase.GetAssetPath(atlasPackables[i]);
    // string SpriteFileName = Path.GetFileName(SpritePath);
    // var sprites2 = atlasPackables.OrderBy(sprite => sprite.name).ToArray();
    // }
    // Debug.Log(atlasPackablesString);

    // if (isSaveAtlasChanges)
    // {
    // SpriteAtlasExtensions.Remove(CurrAtlas, CurrAtlas.GetPackables());
    // AtlasSpritesModified = AtlasSpritesModified.Distinct().ToList();
    // SpriteAtlasExtensions.Add(CurrAtlas, AtlasSpritesModified.ToArray());
    // AssetDatabase.SaveAssets();
    // }
    //}
}