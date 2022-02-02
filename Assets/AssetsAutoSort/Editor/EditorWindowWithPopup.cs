using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System;

public class EditorWindowWithPopup : EditorWindow
{
    static private List<Tuple<IPanelTool, string>> _tools;
    //static private List<string> tip = new List<string>();
    
    [MenuItem("AssetTools/AssetTools %#t")]//%&t
    [MenuItem("Assets/AssetTools %#t")]
    static void Init()
    {
        if (_tools == null)
        {
            InitButtons();
        }

        EditorWindow window = EditorWindow.CreateInstance<EditorWindowWithPopup>();
        window.Show();
    }
    void OnGUI()
    {
        var style = new GUIStyle(GUI.skin.button);
        if (_tools != null)
        {
            int i = 0;
            foreach (var t in _tools)
            {
                //GUILayout.Space(0);
                //GUI.backgroundColor = Color.gray;//new Color(23, 167, 177);
                if (GUILayout.Button(new GUIContent(t.Item1.ToolName, t.Item2), style/*, GUILayout.Height(24)*/))//, new GUIContent("I have a tooltip", "The button overrides the box")
                {
                    t.Item1.Execute();
                }
                i++;
            }
        }
        else
        {
            InitButtons();
        }
    }
    static void InitButtons()
    {
        _tools = new List<Tuple<IPanelTool, string>>()
        {
            new Tuple<IPanelTool, string>(new CloneGameObject(), "Clones game objects with transforms"),
            new Tuple<IPanelTool, string>(new ListAtlasInfo(), "Prints in console list of texture(s) attributes"),
            new Tuple<IPanelTool, string>(new MoveAssetsToFolders(), "Moves assets to folders by its types"),
            new Tuple<IPanelTool, string>(new ReplaceWithPrefab(), "Replaces gameObjects in scene to selected prefab in project window"),
            new Tuple<IPanelTool, string>(new BatchRename(), "Renames selected files to new name [index, prefix, postfix]"),
            new Tuple<IPanelTool, string>(new RenameObjects(), "Renames GameObjects to asset names"),
            new Tuple<IPanelTool, string>(new CloneAssetAttributes(), "Work with selected assets"),
            new Tuple<IPanelTool, string>(new RandomPosInRadius(), "Sets Random Position to selected GameObjects"),
            new Tuple<IPanelTool, string>(new CloneComponents(), "Clones Components from GameObject to other"),
            new Tuple<IPanelTool, string>(new CopyGameObjectPath(), "Copies Path of GameObject"),
            new Tuple<IPanelTool, string>(new GroupObjects(), "Groupping gameObjects In New GameObject"),
            new Tuple<IPanelTool, string>(new DuplicateToFolder(), "Duplicates assets to folder"),
            //new Tuple<IPanelTool, string>(new ImportFromJSON(), "Poses sprites in scene from JSON coordinates"),


        };
        /*
        _tools.Add(new CloneGameObject());
        _tools.Add(new ListAtlasInfo());
        _tools.Add(new DistributeFilesToFolders());
        _tools.Add(new ReplaceWithPrefab());
        _tools.Add(new BatchRename());
        _tools.Add(new RenameObjectsAsMaterial());
        _tools.Add(new CloneAssetAttributes());
        _tools.Add(new RandomPosInRadius());
        _tools.Add(new CloneComponents());
        _tools.Add(new CopyGameObjectPath());
        _tools.Add(new RenameObjectsAsSprite());
        _tools.Add(new RenameObjectsAsText());
        _tools.Add(new GroupObjects());

        tip.Add("Clones game objects with transforms");
        tip.Add("Prints in console list of texture(s) attributes");
        tip.Add("Moves assets to folders by its types");
        tip.Add("Replaces gameObjects in scene to selected prefab in project window");
        tip.Add("Renames selected files to new name [index, prefix, postfix]");
        tip.Add("Renames gameObjects to names of material component");
        tip.Add("Work with selected assets");
        tip.Add("Sets Random Position to selected GameObjects");
        tip.Add("Clones Components from GameObject to other");
        tip.Add("Copies Path of GameObject");
        tip.Add("Renames gameObjects to names of sprite component");
        tip.Add("Groupping gameObjects In New GameObject");
        */
    }
}