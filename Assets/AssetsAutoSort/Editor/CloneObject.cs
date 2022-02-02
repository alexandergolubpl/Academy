using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

public class CloneGameObject : ScriptableWizard, IPanelTool
{
    public string ToolName { get; set; }

    public GameObject objectToCopy;
    public bool isSelectedGOs;
    public bool isReverseList;
    private GameObject[] sel;
    public GameObject[] objectsToTransform;
    public int numberOfCopies = 2;
    public bool Sin = false;
    public bool Radial = true;
    public bool zTop = true;
    public bool alignToCurve = false;

    private bool isZinitFound = false;
    public Vector3 step = new Vector3(1.5f, 0f, 0f);
    private Vector3 zinit = new Vector3(1.5f, 0f, 0f);
    public Vector3 rotation = new Vector3(0f, 0f, 0f);
    public Vector3 scale = new Vector3(1f, 1f, 1f);
    private GameObject firstHObj;

    private Vector3 additional_transform = new Vector3(0f, 0f, 0f);
    public float amplitude = 1;
    public float frequency = 1;
    public bool alignRotation = false;

    public List<GameObject> clonedObjects = new List<GameObject>();

    // Add a new menu item that is accessed by right-clicking on an asset in the project view
    // [MenuItem("Assets/Clone Object With Transform %#c")]
    //private static void CloneObject()
    // {}

    public CloneGameObject() {
        ToolName = typeof(CloneGameObject).Name;
    }

    public void Execute() {
        CloneGameObject.exec();
    }
    public static void exec() {
        CreateWindow();
    }

    [MenuItem("GameObject/AssetTools/Clone Object", false, 0)]
    [MenuItem("AssetTools/Clone Object")]
    public static void CreateWindow() {
        // Creates the wizard for display
        ScriptableWizard.DisplayWizard("Clone Game Object", typeof(CloneGameObject), "Clone");
    }
    public void OnSelectionChange() {
        sel = Selection.gameObjects;
        isZinitFound = false;
        firstHObj = null;
        var tempSiblingIndex = sel.Length - 1;

        objectsToTransform = sel.OrderBy(go => go.transform.GetSiblingIndex()).ToArray();
        if (isReverseList) {
            System.Array.Reverse(objectsToTransform);
        }

        //for (int z = 0; z < sel.Length; z++) {
        //    if (sel[z].transform.GetSiblingIndex() < tempSiblingIndex) {
        //        tempSiblingIndex = sel[z].transform.GetSiblingIndex();
        //        firstHObj = sel[z];
        //    }
        //}
        firstHObj = objectsToTransform[0];
        //Debug.Log("(" + selOrdered.Length + ") base go = " + firstHObj);
        isZinitFound = true;
    }
    public void transformZSelected() {

        zinit = firstHObj.transform.localPosition;
        var lr = firstHObj.transform.localRotation;
        var ls = firstHObj.transform.localScale;
        Undo.SetCurrentGroupName("Zero out selected gameObjects");
        int group = Undo.GetCurrentGroup();
        Undo.RecordObjects(Selection.transforms, "transform selected objects");

        for (int i = 1; i < objectsToTransform.Length; i++) {
            //if (firstHObj.GetInstanceID() != selOrdered[i].GetInstanceID()) {
            Vector3 gortlp = objectsToTransform[i].GetComponent<RectTransform>().localPosition;
            Vector3 newPos = new Vector3(step.x * i + zinit.x, step.y * i + zinit.y, step.z * i + zinit.z);
            Vector3 newRotation = new Vector3(rotation.x * i + lr.x, rotation.y * i + lr.y, rotation.z * i + lr.z);
            Vector3 newScale = new Vector3(scale.x * i + ls.x, scale.y * i + ls.y, scale.z * i + ls.z);
            objectsToTransform[i].GetComponent<RectTransform>().localPosition = newPos;
            objectsToTransform[i].GetComponent<RectTransform>().localRotation = Quaternion.Euler(newRotation);
            objectsToTransform[i].GetComponent<RectTransform>().localScale = newScale;
            //sel[i].name = "s_" + sel[i].transform.GetSiblingIndex();
            //}
        }
        Undo.CollapseUndoOperations(group);
    }
    public void OnWizardUpdate() {
        if (Sin) {
            Radial = false;
            alignToCurve = false;
        } else if (Radial) {
            Sin = false;
            alignToCurve = false;
        } else if (alignToCurve) {
            Sin = false;
            Radial = false;
        }

        helpString = "Clones GameObject";
        errorString = "";
        if (!objectToCopy && !isSelectedGOs) {
            errorString = "Please assign an object";
            isValid = false;
        } else {
            if (numberOfCopies < 0)
                numberOfCopies = 0;

            errorString = "";
            isValid = true;
        }
        if (isSelectedGOs) {
            if (isZinitFound) {
                transformZSelected();
            }
        } else {
            if (isValid) {
                for (int i = 0; i < clonedObjects.Count; i++) {
                    DestroyImmediate(clonedObjects[i]);
                }
                clonedObjects.Clear();

                for (int i = 0; i < numberOfCopies; i++) {
                    additional_transform = new Vector3(0, 0, 0);
                    if (Sin) {
                        additional_transform = new Vector3(0, amplitude * Mathf.Sin(i / frequency) / Mathf.Deg2Rad, 0);
                    }
                    if (Radial) {
                        additional_transform = new Vector3(amplitude * Mathf.Cos(i / frequency) / Mathf.Deg2Rad, 0, amplitude * Mathf.Sin(i / frequency) / Mathf.Deg2Rad);
                    }
                    GameObject go = Instantiate(objectToCopy, objectToCopy.transform.parent);
                    go.transform.localPosition = (i * step) + additional_transform;//, Quaternion.identity) as GameObject;
                    go.name = objectToCopy.name + "_" + i;

                    //go.transform.parent = objectToCopy.transform.parent;
                    go.transform.localScale = scale;

                    if (!alignRotation) {
                        go.transform.localRotation = Quaternion.Euler(rotation);
                    } else {
                        float alignRotationAngle = 0;
                        if (zTop) {
                            alignRotationAngle = -Mathf.Rad2Deg * Mathf.Atan2(go.transform.localPosition.y, go.transform.localPosition.x);
                            go.transform.localRotation = Quaternion.Euler(new Vector3(rotation.x, rotation.y + alignRotationAngle, rotation.z));
                        } else {
                            alignRotationAngle = -Mathf.Rad2Deg * Mathf.Atan2(go.transform.localPosition.z, go.transform.localPosition.x);
                            go.transform.localRotation = Quaternion.Euler(new Vector3(rotation.x, rotation.y + alignRotationAngle, rotation.z));
                        }
                    }
                    clonedObjects.Add(go);
                }
            }
        }
    }
}