using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class ListAtlasInfo : MonoBehaviour, IPanelTool
{
    public string ToolName { get; set; }

    static string FileName = "";
    static string selectedFolder = "";
    static string filesInfo = "";
    public static void exec()
    {
        ListImageInfo();
    }

    public ListAtlasInfo()
    {
        ToolName = typeof(ListAtlasInfo).Name;
    }

    public void Execute()
    {
        ListAtlasInfo.exec();
    }

    // Add a new menu item that is accessed by right-clicking on an asset in the project view
    [MenuItem("AssetTools/ListAtlasInfo %#a")]
    public static void ListImageInfo()
    {
        List<string> selectedFiles = new List<string>();
        //if(selectedFiles.Count > 1)
        //{
            selectedFiles.Clear();
        //}

        if (Selection.activeObject == null)
        {
            selectedFolder = "Assets";
        }
        else
        {
            //print("Count = " + Selection.activeObject.name);
        }
        foreach (Object o in Selection.objects)
        {
            string p = AssetDatabase.GetAssetPath(o.GetInstanceID());
            selectedFolder = Path.GetDirectoryName(p);
            FileName = Path.GetFileName(p);
            selectedFiles.Add(p);

            //MoveFileToFolder(GetPath(p), selectedFolder);

            var isValid = false;
            var assetType = UnityEditor.AssetDatabase.GetMainAssetTypeAtPath(p);
            if (assetType == typeof(Sprite) || assetType == typeof(Texture2D))
            {
                TextureImporter importer = (TextureImporter)TextureImporter.GetAtPath(p);

                isValid = true;
            }
            if (isValid)
            {
                PrintFileInfo(o);
                /*
                 * string FileExt = Path.GetExtension(p);
                switch (FileExt)
                {
                    case ".png":
                    case ".jpg":
                    case ".tga":
                    case ".tif":
                        PrintFileInfo(o);
                        break;
                    default:
                        Debug.LogError("Sprite not selected");
                        break;
                }
                */
            }
        }
        //AssetDatabase.Refresh();
        if (filesInfo == "")
        {
            Debug.Log(" You must Select texture asset(s).");
        }
        print(filesInfo);
        filesInfo = "";
        //ShowWindow(filesInfo);
    }

    static void PrintFileInfo(Object o)
    {
        int instanceID = o.GetInstanceID();
        string path = AssetDatabase.GetAssetPath(Selection.activeObject);
        Texture2D tex = AssetDatabase.LoadAssetAtPath(path, typeof(Texture2D)) as Texture2D;
        TextureImporter importer = (TextureImporter)TextureImporter.GetAtPath(path);


        filesInfo += "\n";
        filesInfo += o.name;
        //filesInfo += "FileID = " + o.GetInstanceID();
        filesInfo += "\n" + AssetDatabase.GetAssetPath(o.GetInstanceID());
        filesInfo += "\n\tFileSize\t" + tex.width+"x"+tex.height;
        filesInfo += "\n\twrapMode\t" + importer.wrapMode;
        filesInfo += "\n\tmaxTextureSize\t" + importer.maxTextureSize;
        filesInfo += "\n\tspritePackingTag\t" + importer.spritePackingTag;
        filesInfo += "\n\tcompressionQuality\t" + importer.textureCompression;
        filesInfo += "\n";
    }
}