using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class CloneComponents : EditorWindow, IPanelTool
{

    //private bool _styleInitialized = false;


    public bool UseName = false;
    public bool UseActive = true;

    public bool UseTrOnly = false;
    public bool UseMirrX = true;
    public bool UseMirrY = false;
    public bool UseMirrZ = false;

    public bool UseTr = true;
    public bool UseRot = true;
    public bool UseScale = true;

    public Object source;
    public Object target;
    public Object[] targets;
    public Object obj = null;

    private Texture tex;


    public string ToolName { get; set; }

    public CloneComponents()
    {
        ToolName = typeof(CloneComponents).Name;
    }

    public void Execute()
    {
        CloneComponents.exec();
    }
    public static void exec()
    {
        Open();
    }

    [MenuItem("AssetTools/Clone Components")]
    private static void Open()
    {
        EditorWindow.GetWindow<CloneComponents>("Clone Components");
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
            //style.margin = new RectOffset(0, 0, 0, 0);
            //style.padding = new RectOffset(0, 0, 0, 0);
            //style.border = new RectOffset(0, 0, 0, 0);
        }
        style.richText = true;
        style.fontSize = 10;
        //_styleInitialized = true;
    }
    private void OnGUI()
    {
        Color[] colors = new Color[] { new Color(0.5f, 1.0f, 0.5f, 0.5f), new Color(1f, 0.5f, 0.5f, 1f), new Color(0.5f, 0.8f, 1.0f, 0.5f), new Color(1.0f, 1.0f, 1.0f, 0.5f) };
        using (new GUILayout.HorizontalScope())
        {
            source = EditorGUILayout.ObjectField(source, typeof(Object), true);
            tex = source as Texture;

            if (!tex)
            {
                target = EditorGUILayout.ObjectField(target, source.GetType(), true);
            }
            else
            {
                target = EditorGUILayout.ObjectField(target, typeof(Texture), true);
            }
        }
        bool assetsReady = ((source != null) && (target != null));

        //targets = EditorGUILayout.ObjectField(targets, typeof(Object), true);
        using (new GUILayout.HorizontalScope("HelpBox"))
        {
            UseName = GUILayout.Toggle(UseName, " Use source Name");
            UseActive = GUILayout.Toggle(UseActive, " Use Active");
        }


        if (!tex)
        {
            GUILayout.Space(4);

                    //GUILayout.Space(10);
                    GUILayout.Label("Transform", EditorStyles.boldLabel);
            using (new GUILayout.VerticalScope ("HelpBox"))
            {
                using (new GUILayout.HorizontalScope())
                {
                }
                UseTr = GUILayout.Toggle(UseTr, " Use Transform");
                UseTrOnly = !UseTr;
                UseRot = GUILayout.Toggle(UseRot, " Use Rotation");
                UseScale = GUILayout.Toggle(UseScale, " Use Scale");

                GUILayout.Space(10);
                using (new GUILayout.HorizontalScope())
            {
                UseTrOnly = GUILayout.Toggle(UseTrOnly, " Clone Transformations Only");
                UseTr = !UseTrOnly;
            }
                GUILayout.Space(5);
                        GUILayout.Label("Mirror");
                    using (new GUILayout.HorizontalScope())
                    {
                        UseMirrX = GUILayout.Toggle(UseMirrX, "X", GUILayout.ExpandWidth(false));
                        UseMirrY = GUILayout.Toggle(UseMirrY, "Y", GUILayout.ExpandWidth(false));
                        UseMirrZ = GUILayout.Toggle(UseMirrZ, "Z", GUILayout.ExpandWidth(false));

                    }
            }
            GUILayout.Space(10);
        }

        GUI.enabled = assetsReady;
        GUI.backgroundColor = (assetsReady) ? colors[0] : colors[1];
        if (GUILayout.Button(" >>> Clone <<< ",GUILayout.Height(32)))
        {
            string path = AssetDatabase.GetAssetPath(target);
            var assetType = AssetDatabase.GetMainAssetTypeAtPath(path);
            //if (assetType != null)

            if (tex)
            {
                CopyTextureImportSettings(source, target);
                return;
            }
            CopySpecialComponents(source as GameObject, target as GameObject);
        }
    }

    private void CopySpecialComponents(GameObject _sourceGO, GameObject _targetGO)
    {
        Undo.RecordObject(_targetGO, _targetGO.name + ": Paste Components");

        foreach (var component in _sourceGO.GetComponents<Component>())
        {
            var componentType = component.GetType();
            /*if (componentType != typeof(Transform) &&
                componentType != typeof(MeshFilter) &&
                componentType != typeof(MeshRenderer)
                )
            {*/
            Debug.Log("Found a component of type " + component.GetType());
            UnityEditorInternal.ComponentUtility.CopyComponent(component);
            if (component.GetType() == typeof(RectTransform))
            {
                //UnityEditorInternal.ComponentUtility.PasteComponentValues(_targetGO.GetComponent(component.GetType()));
                if (UseMirrX)
                {
                    _targetGO.transform.localPosition = Vector3.Scale(_sourceGO.transform.localPosition, new Vector3(-1, 1, 1));
                }
                if (UseMirrY)
                {
                    _targetGO.transform.localPosition = Vector3.Scale(_sourceGO.transform.localPosition, new Vector3(1, -1, 1));
                }
                if (UseMirrZ)
                {
                    _targetGO.transform.localPosition = Vector3.Scale(_sourceGO.transform.localPosition, new Vector3(1, 1, -1));
                }
            }
            if (!UseTrOnly && component.GetType() == typeof(ParticleSystem))
            {
                UnityEditorInternal.ComponentUtility.PasteComponentValues(_targetGO.GetComponent(component.GetType()));
            }
            if (!UseTrOnly)
                UnityEditorInternal.ComponentUtility.PasteComponentAsNew(_targetGO);

            if (UseName)
                _targetGO.name = _sourceGO.name;
            if (UseActive)
                _targetGO.active = _sourceGO.active;
            if (UseTr)
                _targetGO.transform.localPosition = _sourceGO.transform.localPosition;
            if (UseRot)
                _targetGO.transform.localRotation = _sourceGO.transform.localRotation;
            if (UseScale)
                _targetGO.transform.localScale = _sourceGO.transform.localScale;
            if (_targetGO.GetComponent<Renderer>() != null && _targetGO.GetComponent<Renderer>().material)
                _targetGO.GetComponent<Renderer>().material = _sourceGO.GetComponent<Renderer>().material;
            _targetGO.tag = _sourceGO.tag;
            _targetGO.layer = _sourceGO.layer;

            Debug.Log("Copied " + component.GetType() + " from " + _sourceGO.name + " to " + _targetGO.name);
            //}
        }
    }

    static void CopyTextureImportSettings(Object source, Object target)
    {
        string sourcestr = AssetDatabase.GetAssetPath(source);
        string targetstr = AssetDatabase.GetAssetPath(target);
        try
        {
            var sourceAst = sourcestr.Substring(sourcestr.IndexOf("/Assets/") + 1);
            var targetAst = targetstr.Substring(targetstr.IndexOf("/Assets/") + 1);

            AssetDatabase.ImportAsset(targetAst, ImportAssetOptions.ForceSynchronousImport); // If I don't do this, the next step importer will be null,  and generate GUID of this texture will be done after all my code execute, that's really too late.

            TextureImporter importer = AssetImporter.GetAtPath(sourceAst) as TextureImporter;
            TextureImporterSettings setting = new TextureImporterSettings();
            importer.ReadTextureSettings(setting);

            TextureImporter targetImporter = AssetImporter.GetAtPath(targetAst) as TextureImporter;

            // "Standalone", "Web", "iPhone", "Android", "WebGL", "Windows Store Apps", "PSP2", "PS4", "XboxOne", "Nintendo 3DS" , "tvOS"
            //string def = Application.platform;

            string[] platforms = new[] { RuntimePlatform.WindowsPlayer.ToString(), RuntimePlatform.WindowsEditor.ToString(), "iPhone", "Android" };
            foreach (var platform in platforms)
            {
                //string platform = "Android";
                TextureImporterPlatformSettings ps = importer.GetPlatformTextureSettings(platform);
                TextureImporterPlatformSettings ps2 = importer.GetDefaultPlatformTextureSettings();
                targetImporter.SetPlatformTextureSettings(ps);
                targetImporter.SetPlatformTextureSettings(ps2);
                targetImporter.SetTextureSettings(setting);
                targetImporter.spritePackingTag = importer.spritePackingTag;
            }
            targetImporter.SaveAndReimport();
        }
        catch (System.Exception ex)
        {
            Debug.LogError(ex.ToString());
        }
    }
}