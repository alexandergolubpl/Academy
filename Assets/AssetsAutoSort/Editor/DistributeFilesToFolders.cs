using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class DistributeFilesToFolders : MonoBehaviour, IPanelTool
{
    public string ToolName { get; set; }
    private static string FileName = "";
    private static string selectedFolder = "";

    public DistributeFilesToFolders()
    {
        ToolName = typeof(DistributeFilesToFolders).Name;
    }

    public void Execute()
    {
        DistributeFilesToFolders.exec();
    }
    public static void exec()
    {
        DistributeFiles();
    }

    [MenuItem("AssetTools/Distribute Files To Folders %#d")]
    [MenuItem("Assets/AssetTools/Distribute Files To Folders %#d")]
    private static void DistributeFiles()
    {
        List<string> selectedFiles = new List<string>();


        if (Selection.activeObject == null)
        {
            selectedFolder = "Assets";
        }
        else
        {
        }
        foreach (Object o in Selection.objects)
        {
            Undo.RecordObject(o, "Undo Move files");
            string p = AssetDatabase.GetAssetPath(o.GetInstanceID());
            selectedFolder = Path.GetDirectoryName(p);
            FileName = Path.GetFileName(p);
            selectedFiles.Add(p);            

            string FileExt = Path.GetExtension(p);
            switch (FileExt)
            {
                case ".png":
                case ".jpg":
                case ".jpeg":
                case ".tga":
                case ".tif":

                    bool isSprite = false;
                    var importer = AssetImporter.GetAtPath(p);
                    var typeimporter = importer.GetType();
                    TextureImporter textureImporter = (TextureImporter) importer;
                    isSprite = (textureImporter.textureType == TextureImporterType.Sprite);

                    if (isSprite)
                    {
                        MoveFileToFolder(GetPath(p), createFolderByType("Sprites"));
                    }
                    else
                    {
                        MoveFileToFolder(GetPath(p), createFolderByType("Textures"));
                    }
                    break;
                case ".mat":
                    MoveFileToFolder(GetPath(p), createFolderByType("Materials"));
                    break;
                case ".prefab":
                    MoveFileToFolder(GetPath(p), createFolderByType("Prefabs"));
                    break;
                case ".fbx":
                    MoveFileToFolder(GetPath(p), createFolderByType("Models"));
                    break;
                case ".unity":
                    MoveFileToFolder(GetPath(p), createFolderByType("Scenes"));
                    break;
                case ".unitypackage":
                    MoveFileToFolder(GetPath(p), createFolderByType("Packages"));
                    break;
                case ".cs":
                    MoveFileToFolder(GetPath(p), createFolderByType("Scripts"));
                    break;
                case ".shader":
                case ".shadergraph":
                    MoveFileToFolder(GetPath(p), createFolderByType("Shaders"));
                    break;
                case ".anim":
                case ".controller":
                    MoveFileToFolder(GetPath(p), createFolderByType("Animation"));
                    break;
                case ".ttf":
                case ".otf":
                    MoveFileToFolder(GetPath(p), createFolderByType("Fonts"));
                    break;
                default:
                    Debug.LogError("Improper command given!");
                    break;
            }
        }
        AssetDatabase.Refresh();
    }

    private static string createFolderByType(string FolderName)
    {
        string NewFolderPath = selectedFolder + "/" + FolderName;
        bool folderExists = Directory.Exists(NewFolderPath);
        if (!folderExists)
        {
            AssetDatabase.CreateFolder(selectedFolder, FolderName);
        }
        return NewFolderPath;
    }
    private static string GetPath(string SourceFilePath)
    {
        var arr = SourceFilePath.Split("/"[0]);
        FileName = arr[arr.Length - 1];
        System.Array.Resize(ref arr, arr.Length - 1);
        string PathString = System.String.Join("/", arr);
        return PathString;
    }

    private static void MoveFileToFolder(string SourceFolderPath, string DestFilePath)
    {
        if (System.IO.File.Exists(SourceFolderPath + "/" + FileName + ".meta"))
        {
            File.Move(SourceFolderPath + "/" + FileName + ".meta", DestFilePath + "/" + FileName + ".meta");
        }
        File.Move(SourceFolderPath + "/" + FileName, DestFilePath + "/" + FileName);
    }
}