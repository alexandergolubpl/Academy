// RandomPosInRadius.cs

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
public class RandomPosInRadius : ScriptableWizard, IPanelTool
{
    public string ToolName { get; set; }

    public Vector3 posRadius = new Vector3(1f,1f,1f);
    public float rotValue = 0f;

    public RandomPosInRadius()
    {
        ToolName = typeof(RandomPosInRadius).Name;
    }

    public void Execute()
    {
        RandomPosInRadius.exec();
    }
    public static void exec()
    {
        CreateWizard();
    }

    [MenuItem("AssetTools/Random Pos In Radius... ")]
    static void CreateWizard()
    {
        ScriptableWizard.DisplayWizard("Random Pos In Radius", typeof(RandomPosInRadius), "Ok");
    }
    /// Called when the window first appears
    void OnEnable()
    {
        UpdateSelectionHelper();
    }
    /// Function called when selection changes in scene
    void OnSelectionChange()
    {
        UpdateSelectionHelper();
    }
    /// Update selection counter
    void UpdateSelectionHelper()
    {
        helpString = "";
        if (Selection.objects.Length > 0)
        {
            isValid = true;
            helpString = "Number of objects selected: " + Selection.objects.Length;

            errorString = "";
        }
        else
        {
            errorString = "Please select an object(s)";
            isValid = false;
        }
    }
    public void OnWizardUpdate()
    {
        if (isValid)
        {
            if (Selection.objects == null)
                return;
            List<GameObject> mySelection = new List<GameObject>(Selection.gameObjects);
            foreach (var O in mySelection)
            {
                Vector3 randomPos = new Vector3(Random.Range(0, posRadius.x) - posRadius.x / 2, Random.Range(0, posRadius.y) - posRadius.y / 2, Random.Range(0, posRadius.z) - posRadius.z / 2);
                Vector3 randomRot = new Vector3(Random.Range(0, rotValue) - rotValue / 2, Random.Range(0, rotValue) - rotValue / 2, Random.Range(0, rotValue) - rotValue / 2);
                O.transform.rotation = Quaternion.Euler(randomRot);
                O.transform.localPosition = randomPos;// + O.transform.parent.transform.position;
            }
        }
    }
}