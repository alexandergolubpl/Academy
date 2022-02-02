using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using System.Linq;

[CustomEditor(typeof(AutosortTool))]
public class AutoSortToolInspector : Editor
{
    AutosortTool myScript;
    string FileName;
    string favotiteFolderButtonText="";

    // Debug.Log("Asset name: "+obj.name+"   Type: "+obj.GetType()) ;

    private void OnSceneGUI()
    {
        if (myScript == null)
        {
            myScript = (AutosortTool)target;                                                               //gets the instance of the script that it is an inspector for if its null
        }
        var path = "";
        var obj = Selection.activeObject;
        if (obj == null) myScript.selectedFolder = "Assets";
        else path = AssetDatabase.GetAssetPath(obj.GetInstanceID());
        //Debug.Log("path = "+ path);
        if (path.Length > 0)
        {
            if (Directory.Exists(path))
            {
                myScript.selectedFolder = path;
            }
            else
            {
                myScript.selectedFiles.Clear();
                foreach (Object o in Selection.objects)
                {
                    myScript.selectedFiles.Add(AssetDatabase.GetAssetPath(o.GetInstanceID()));
                }

                //myScript.selectedFile = path;
                //var arr = myScript.selectedFile.Split("/"[0]);
                //FileName = arr[arr.Length-1];
            }
        }
        else
        {
            //Debug.Log("Not in assets folder");
        }
    }

    public void OnGUI()
    {
        GUI.skin.button.padding.top = 20;
    }
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        //TODO create assets folders

        //TODO move files to folders
        //TODO add folder to favorite

        //TODO create list of favorite folders
        //TODO delete/lock/edit favorite folder
        //


        #region Red
        //GUILayout.Label("Red List");
        //favotiteFolderButtonText = (myScript.favotiteFolder == "") ? "♥     Add Folder to favorites     ♥" : "" + myScript.favotiteFolder + " (Press To Set New)";
        favotiteFolderButtonText = "♥     Add Folder to favorites     ♥";
        if (GUILayout.Button(favotiteFolderButtonText))// =  + moveAssets
        {
            myScript.favotiteFolder = myScript.selectedFolder;
        }

        GUILayout.BeginHorizontal("box");

        #endregion
        if (myScript.favotiteFolder != null)
        {
            GUILayout.Label("favorite Folder: " + myScript.favotiteFolder);
        }
        if (GUILayout.Button("Focus "))
        {
            // Load object
           UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath(myScript.favotiteFolder, typeof(UnityEngine.Object));
            // Select the object in the project folder
            Selection.activeObject = obj;
            // Also flash the folder yellow to highlight it
            EditorGUIUtility.PingObject(obj);

        }

        GUILayout.EndHorizontal();

        #region White
        GUILayout.Label("");
        GUI.color = new Color(255, 70, 255);
        GUILayout.Label("Select source folder to sort files");
        GUI.color = new Color(255, 255, 255);
        if (GUILayout.Button("Move asset inside selected folder"))// =  + moveAssets
        {
            foreach (string p in myScript.selectedFiles)
            {
                MoveFileToFolder(GetPath(p), myScript.selectedFolder);
            }
        }
        #endregion

        if (GUILayout.Button("Sort assets inside folder"))// =  + moveAssets
        {
            foreach (string p in myScript.selectedFiles)
            {
                string FileExt = Path.GetExtension(p);
                switch (FileExt)
                {
                    case ".png":
                    case ".jpg":
                    case ".tga":
                    case ".tif":
                        MoveFileToFolder(GetPath(p),createFolderByType("Textures"));
                        break;
                    case ".mat":
                        MoveFileToFolder(GetPath(p), createFolderByType("Materials"));
                        break;
                    case ".prefab":
                        MoveFileToFolder(GetPath(p), createFolderByType("Prefabs"));
                        break;
                    case ".unity":
                        MoveFileToFolder(GetPath(p), createFolderByType("Scenes"));
                        break;
                    case ".unitypackage":
                        MoveFileToFolder(GetPath(p), createFolderByType("Packages"));
                        break;
                    default:
                        Debug.LogError("Improper command given!");
                        break;
                }
            }
        }
        AssetDatabase.Refresh();
        GUILayout.Label("");
    }

    string createFolderByType(string FolderName)
    {
        string NewFolderPath = myScript.selectedFolder + "/" + FolderName;
        Debug.Log("NewFolderPath = " + NewFolderPath);
        bool folderExists = Directory.Exists(NewFolderPath);
        if (!folderExists)
        {
            AssetDatabase.CreateFolder(myScript.selectedFolder, FolderName);
        }
        return NewFolderPath;
    }

    string GetPath(string SourceFilePath)
    {
        //Debug.Log("SourceFilePath = " + SourceFilePath);
        var arr = SourceFilePath.Split("/"[0]);
        FileName = arr[arr.Length - 1];
        //arr = SourceFilePath.Split("/"[0]);
        System.Array.Resize(ref arr, arr.Length - 1);
        string PathString = System.String.Join("/", arr);
        return PathString;
    }

    void MoveFileToFolder(string SourceFolderPath, string DestFilePath)
    {
        //Debug.Log("SourceFolderPath = " + SourceFolderPath+ " : DestFilePath = " + DestFilePath);
       if (System.IO.File.Exists(SourceFolderPath + "/"+ FileName + ".meta"))
        {
            File.Move(SourceFolderPath + "/" + FileName + ".meta", DestFilePath + "/" + FileName + ".meta");
        }
        File.Move(SourceFolderPath + "/" + FileName, DestFilePath + "/" + FileName);

        Debug.Log("DestFilePath = " + DestFilePath + "/" + FileName);

        //FileUtil.MoveFileOrDirectory(o, myScript.selectedFolder + "/" + FileName);
        //FileUtil.MoveFileOrDirectory(o, myScript.selectedFolder + "/" + FileName + ".meta");
    }
}