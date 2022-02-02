using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
public class GetCountOfAssets : EditorWindow
{
    private string PathArea = "";
    private string objectPathArea = "";
    private Vector2 scrollPosition;
    private Color[] colors = new Color[] { new Color(0.5f, 0.8f, 1.0f, 0.8f), new Color(1.0f, 1.0f, 1.0f, 0.8f) };
    private static GameObject[] selections;
    private int minAmount;

    private Dictionary<string, uint> PrefabCount = new Dictionary<string, uint>();
    private Dictionary<string, uint> AssetCount = new Dictionary<string, uint>();

    private bool isSprites = true;
    private bool isAnimations;
    private bool isModels;
    private bool isScripts;
    private bool isStart;
    private bool isIgnoreExcludedFolders;
    private bool isIncludeFolders;
    private bool isAllowRun = true;

    private string includedFoldersStr = "Included Folders (separate with ;)";
    private string ignoreExcludedFoldersStr = "Exclude Folders with ;";

    private Object[] IncludeFoldersArr;
    private Object[] ExcludeFoldersArr;

    private void GetAllAssetsByExtentions(string[] exts)
    {
        foreach (string s in AssetDatabase.GetAllAssetPaths()
             .Where(s => s.EndsWith(exts[0], System.StringComparison.OrdinalIgnoreCase) || s.EndsWith(exts[1], System.StringComparison.OrdinalIgnoreCase)))
        {
            Texture2D texture = (Texture2D) AssetDatabase.LoadAssetAtPath(s, typeof(Texture2D));
        }
    }

    [UnityEditor.MenuItem("AssetTools/Count Assets Use")]
    [UnityEditor.MenuItem("GameObject/AssetTools/Count Assets Use")]
    public static void InitHierarchy()
    {
        selections = UnityEditor.Selection.gameObjects;
        EditorWindow.GetWindow<GetCountOfAssets>("Count Assets Use");
    }

    private void createPathsList()
    {
        //Scroller for strings
        scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, false);
        //EditorStyles.textArea.wordWrap = false;
        GUI.skin.textArea.wordWrap = false;
        objectPathArea = GUILayout.TextArea(objectPathArea, /*style,*/GUILayout.ExpandHeight(true));
        EditorGUILayout.EndScrollView();
    }

    private void checkIsAllowStart()
    {
        isStart = (isSprites || isAnimations || isModels || isScripts);
    }
    public void OnGUI()
    {
        if (Input.GetMouseButtonDown(1))
        { isAllowRun = !isAllowRun; }
        if (!isAllowRun)
        {
            return;
        }
        isIncludeFolders = GUILayout.Toggle(isIncludeFolders, "Included Folders Only");
        includedFoldersStr = GUILayout.TextField(includedFoldersStr);
        GUILayout.Space(5);
        isIgnoreExcludedFolders = GUILayout.Toggle(isIgnoreExcludedFolders, "Ignore Excluded Folders");
        ignoreExcludedFoldersStr = GUILayout.TextField(ignoreExcludedFoldersStr);

        GUILayout.Space(5);
        using (new GUILayout.HorizontalScope())
        {
            isSprites = GUILayout.Toggle(isSprites, "Sprites");
            GUI.enabled = false;
            isAnimations = GUILayout.Toggle(isAnimations, "Animations");
            isModels = GUILayout.Toggle(isModels, "Models");
            isScripts = GUILayout.Toggle(isScripts, "Scripts");
            GUI.enabled = true;
            checkIsAllowStart();
        }
        GUILayout.Space(5);
        GUI.enabled = isStart;
        if (GUILayout.Button("Start Count"))
        {
            selections = UnityEditor.Selection.gameObjects;
            CollectPrefabs(selections);
        }
        GUI.enabled = true;

        createPathsList();

        using (new GUILayout.HorizontalScope())
        {
            GUILayout.Label(" Minimal Amount for atlas");
            minAmount = EditorGUILayout.IntField(minAmount);
        }

        //todo check allow  to update atlas
        GUI.enabled = false;
        if (GUILayout.Button("Create Atlas"))
        {
            //todo
        }
        GUI.enabled = true;

    }
    private void CollectPrefabs(GameObject[] selections)
    {
        Debug.Log("CollectPrefabs");
        //if (selections == null || selections.Length < 1)
        //return;
        AddPrefabsToDictionary();
        AddAllAssetsToDictionary("Sprite");
        GetDependenciesInPrefabs(PrefabCount);
    }
    private void GetDependenciesInPrefabs(Dictionary<string, uint> PrefabDict)
    {

        var curr = 0;
        int ImagesToReplaceTotal = PrefabDict.Count;
        var pBarMessage = "Counting...";

        foreach (var element in PrefabDict)
        {
            curr++;
            var progress = (float) curr / ImagesToReplaceTotal;
            pBarMessage = (curr + " / " + ImagesToReplaceTotal) + " Counting " + element;
            if (EditorUtility.DisplayCancelableProgressBar("Wait please", pBarMessage, progress))
            { break; }


            Object obj = AssetDatabase.LoadAssetAtPath<Object>(element.Key);
            Object[] roots = new Object[] { obj };
            Object[] ObjectsInPrefab = EditorUtility.CollectDependencies(roots);

            CheckAssetsInDict(ObjectsInPrefab);

            EditorWindow view = EditorWindow.GetWindow<SceneView>();
            view.Repaint();
        }
        EditorUtility.ClearProgressBar();
    }
    private void CheckAssetsInDict(Object[] obs)
    {

        //foreach (var o in obs)
        //{
        Object o = obs[0];
        var path = AssetDatabase.GetAssetPath(o);
        if (path == null)
        {
            //continue;
        }
        var pathString = path;
        if (AssetCount.ContainsKey(pathString))
            AssetCount[pathString]++;
        //}
        WriteToArea(AssetCount);
    }

    private void AddAllAssetsToDictionary(string typ)
    {
        Debug.Log("AddAllAssetsToDictionary" + ": " + typ);
        //Put all the types that were found in the typeCount dictionary and increment their count
        foreach (var guid in AssetDatabase.FindAssets("t:" + typ, new[] { "Assets" }))
        {
            if (guid == null)
            {
                continue;
            }
            var path = AssetDatabase.GUIDToAssetPath(guid);
            if (path == null)
            {
                continue;
            }
            if (AssetDatabase.GetMainAssetTypeAtPath(path) == null)
            {
                continue;
            }
            var pathString = path;
            if (AssetCount.ContainsKey(pathString))
                AssetCount[pathString]++;
            else
                AssetCount.Add(pathString, 0);
        }
    }
    private void AddPrefabsToDictionary()
    {
        foreach (var guid in AssetDatabase.FindAssets("t:Prefab", new[] { "Assets" }))
        {
            if (guid == null)
            {
                continue;
            }
            var path = AssetDatabase.GUIDToAssetPath(guid);
            if (path == null)
            {
                continue;
            }
            if (AssetDatabase.GetMainAssetTypeAtPath(path) == null)
            {
                continue;
            }
            var pathString = path;
            if (PrefabCount.ContainsKey(pathString))
                PrefabCount[pathString]++;
            else
                PrefabCount.Add(pathString, 0);
        }
        Debug.Log($"AddPrefabsInDict ({ PrefabCount.Count })");
    }


    private void PrintPrefabsDict(Dictionary<string, uint> PrefabCount)
    {
        string tempStr = "";
        int cnt = 0;
        foreach (var element in PrefabCount)
        {
            cnt++;
            var oneStr = "\t\t" + element + "\n";
            tempStr += oneStr;
            Debug.Log(cnt + "\t\t" + oneStr);
        }
        //Debug.Log(tempStr);
    }

    private void WriteToArea(Dictionary<string, uint> Dict)
    {
        int cnt = 0;
        foreach (var element in Dict)
        {
            cnt++;
            string isFirstLine = (!string.IsNullOrEmpty(PathArea) && cnt == 0) ? "\n" : "";
            string isEndLine = (cnt == 1) ? "" : "\n";
            PathArea += $"{ isFirstLine }{ element}{ isEndLine }";//\t{ strProps[i] }
        }
        //Debug.Log(PathArea);
        //objectPathArea = "";
        objectPathArea = PathArea;
    }
    private void ResetTexts()
    {
        PathArea = "";
    }
}