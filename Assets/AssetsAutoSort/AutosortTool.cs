using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class AutosortTool : MonoBehaviour
{
    [HideInInspector] // Hides var below
    public string favotiteFolder;
    [HideInInspector] // Hides var below
    public string Path = "Assets";
    [Tooltip("Select folder first")]
    public string selectedFolder;
    public bool createFolders = true;
        //[System.Serializable]
    public bool moveAssets = true;
    [Space(10)]
    public List<string> selectedFiles;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
