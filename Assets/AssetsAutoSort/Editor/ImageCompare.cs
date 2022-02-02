using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class ImageCompare : MonoBehaviour, IPanelTool
{
    public string ToolName { get; set; }
    //static string FileName = "";
    //static string filesInfo = "";
    public static bool thesame = true;
    public static string currUniqFile;

    public static void exec()
    {
        ListImages();
    }

    public ImageCompare()
    {
        ToolName = typeof(ImageCompare).Name;
    }

    public void Execute()
    {
        ImageCompare.exec();
    }

    [MenuItem("AssetTools/ImageCompare %#I")]
    public static void ListImages()
    {
        List<object> selectedFiles = new List<object>();
        string[] clonedFiles;
        selectedFiles.Clear();

        foreach (Object o in Selection.objects)
        {
            string p = AssetDatabase.GetAssetPath(o.GetInstanceID());
            selectedFiles.Add(o);
        }

        clonedFiles = new string[selectedFiles.Count];
        string cloneList = "";
        for (int i = 0; i < selectedFiles.Count; i++)
        {
            Texture2D t0 = selectedFiles[i] as Texture2D;

            for (int j = 0; j < selectedFiles.Count; j++)
            {
                Texture2D t1 = selectedFiles[j] as Texture2D;
                if (CompareTexture(t0, t1))
                {
                    clonedFiles[i] = AssetDatabase.GetAssetPath(t1);
                    break;
                }
                else
                {
                    clonedFiles[i] = AssetDatabase.GetAssetPath(t0);
                }
            }
        }
        for (int i = 0; i < clonedFiles.Length; i++)
        {
            cloneList += AssetDatabase.GetAssetPath(selectedFiles[i] as Texture2D) + "\t:\t"+clonedFiles[i] + "\n";
        }
        Debug.Log(cloneList);
    }

    public static bool CompareTexture(Texture2D first, Texture2D second)
    {
        Color[] firstPix = first.GetPixels();
        Color[] secondPix = second.GetPixels();
        if (firstPix.Length != secondPix.Length)
        {
            return false;
        }
        for (int i = 0; i < firstPix.Length; i++)
        {
            if (firstPix[i] != secondPix[i])
            {
                return false;
            }
        }
        return true;
    }
}