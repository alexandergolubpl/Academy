using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ConvertToSprite : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [MenuItem("AssetTools/ConvertToSprite")]//%#d
    [MenuItem("Assets/ConvertToSprite")]
    static void MakeSprite()
    {
        string path = AssetDatabase.GetAssetPath(Selection.activeObject);
        TextureImporter tImporter = AssetImporter.GetAtPath(path) as TextureImporter;
        tImporter.textureType = TextureImporterType.Sprite;
        tImporter.textureFormat = TextureImporterFormat.AutomaticCompressed;
        tImporter.SaveAndReimport();
    }
}
