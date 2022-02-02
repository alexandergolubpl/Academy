using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

public class ParticleScaler : EditorWindow
{
    [MenuItem("AssetTools/Particle Scaler ")]//%#p
    [MenuItem("GameObject/AssetTools/Particle Scaler", false, 0)]
    public static void OpenWindow()
    {
        GetWindow<ParticleScaler>(false, "AssetTools Particle Scaler", true);
    }
    private List<ParticleSystem> particleSystemList = new List<ParticleSystem>();


    //private Vector3 initScale;


    private float oldScaleFactor = 1.0f;
    private float newScaleFactor = 1.0f;
    private int currentInstanceId = 0;
    private static Dictionary<int, float> newScaleFactors = new Dictionary<int, float>();

    private int selScaleModeInt = 0;
    private string selScaleModeString;
    private List<string> ScaleModeList = new List<string> { "Hierarchy", "Local" };
    private float ParticleStretchSliderValue = 0.04f;

    private GUIStyle style;
    private GUIStyle styleHelpBox;
    private bool _styleInitialized = false;

    private void InitStyles()
    {
        if (EditorStyles.helpBox == null)
            style = new GUIStyle(GUI.skin.box);
        else
            style = new GUIStyle(EditorStyles.helpBox);
        style.margin = new RectOffset(0, 0, style.margin.top, style.margin.bottom);

        styleHelpBox = new GUIStyle(EditorStyles.helpBox);
        _styleInitialized = true;
    }


    private void Restart()
    {
        //particleSystemList.Clear();

        if (Selection.activeGameObject == null)
            return;
        particleSystemList.Clear();

        Object[] SelectedGOs = Selection.objects;
        for (int i = 0; i < SelectedGOs.Length; i++)
        {
            GameObject go = SelectedGOs[i] as GameObject;
            GameObject[] newSelection = new GameObject[1];
            newSelection[0] = go;
            Selection.objects = newSelection;

            ParticleSystem[] temParticleSystemSelection = Selection.activeGameObject.GetComponentsInChildren<ParticleSystem>();
            particleSystemList.AddRange(temParticleSystemSelection);
        }
        Selection.objects = SelectedGOs;

        currentInstanceId = Selection.activeGameObject.GetInstanceID();
        if (!newScaleFactors.ContainsKey(currentInstanceId))
        {
            newScaleFactors[currentInstanceId] = 1.0f;
        }
        newScaleFactor = newScaleFactors[currentInstanceId];
    }

    private void CreateRadioButtonsScaleModeList()
    {
    }
    private void CreateParticleStretchSlider()
    {
        EditorGUILayout.LabelField("Stretch by Velocity: ", GUILayout.MaxWidth(120));
        ParticleStretchSliderValue = EditorGUILayout.FloatField(ParticleStretchSliderValue,GUILayout.Width(60));
        ParticleStretchSliderValue = GUILayout.HorizontalSlider(ParticleStretchSliderValue, -2, 2);
    }
    private void CreateRadioButtonsScaleMode()
    {
        EditorGUI.BeginChangeCheck();
        using (new GUILayout.HorizontalScope("Box"))
        {
            selScaleModeInt = GUILayout.SelectionGrid(selScaleModeInt, ScaleModeList.ToArray(), 1, EditorStyles.radioButton);//
            if (EditorGUI.EndChangeCheck())
            {
                selScaleModeString = ScaleModeList[selScaleModeInt];
                Rescale();
            }
        }
        //EditorGUI.EndChangeCheck();
        //GUILayout.EndVertical();
    }
    private void OnGUI()
    {
        if (!_styleInitialized)
        {
            InitStyles();
        }
        if (particleSystemList.Count == 0)
        {
            EditorGUILayout.HelpBox("Select ParticleSystem object", MessageType.Error);
            return;
        }

        EditorGUILayout.Space();

        using (new GUILayout.VerticalScope())
        {
            using (new GUILayout.HorizontalScope())
            {
                EditorGUILayout.Space();
                EditorGUI.BeginChangeCheck();
                oldScaleFactor = newScaleFactor;
                newScaleFactor = EditorGUILayout.FloatField(newScaleFactor);
                EditorGUILayout.Space();
            }
            using (new GUILayout.HorizontalScope())
            {
                //EditorGUILayout.Space();
                if (EditorGUI.EndChangeCheck())
                {
                    Rescale();
                }

                if (GUILayout.Button("x1", style))
                {
                    newScaleFactor = 1.0f;
                    Rescale();
                }

                if (GUILayout.Button("x10", style))
                {
                    newScaleFactor = 10.0f;
                    Rescale();
                }
                if (GUILayout.Button("x100", style))
                {
                    newScaleFactor = 100.0f;
                    Rescale();
                }
                if (GUILayout.Button("x500", style))
                {
                    newScaleFactor = 500.0f;
                    Rescale();
                }
                if (GUILayout.Button("x1000", style))
                {
                    newScaleFactor = 1000.0f;
                    Rescale();
                }
            }
            using (new GUILayout.HorizontalScope())
            {
                CreateRadioButtonsScaleMode();
            }
            using (new GUILayout.HorizontalScope())
            {
                EditorGUI.BeginChangeCheck();
                CreateParticleStretchSlider();
                if (EditorGUI.EndChangeCheck())
                {
                    Rescale();
                }
            }

            if (GUILayout.Button("Play"))
            {
                foreach (var ps in particleSystemList)
                {
                    ps.Play(true);
                }
            }
            if (GUILayout.Button("Pause"))
            {
                foreach (var ps in particleSystemList)
                {
                    ps.Pause(true);
                }
            }
            if (GUILayout.Button("Stop"))
            {
                foreach (var ps in particleSystemList)
                {
                    ps.Stop(true);
                }
            }
        }


        if (GUI.changed)
        {
            SceneView.RepaintAll();
        }
    }

    private void Rescale()
    {
        newScaleFactors[currentInstanceId] = newScaleFactor;
        Object[] SelectedGOs = Selection.objects;
        for (int i = 0; i < SelectedGOs.Length; i++)
        {
            GameObject go = SelectedGOs[i] as GameObject;
            GameObject[] newSelection = new GameObject[1];
            newSelection[0] = go;
            Selection.objects = newSelection;


            var temParticleSystemSelection = Selection.activeGameObject.GetComponentsInChildren<ParticleSystem>();
            particleSystemList = new List<ParticleSystem>(temParticleSystemSelection);
            foreach (var ps in particleSystemList)
            {
                ps.Stop();
                ScalePS(go, ps);
                ps.Play();
            }
        }
        Selection.objects = SelectedGOs;
    }
    private void ScalePS(GameObject go, ParticleSystem ps)
    {
        var main = ps.main;
        main.scalingMode = (ParticleSystemScalingMode) selScaleModeInt;
        ParticleSystemRenderer PSRenderObject = go.GetComponent<ParticleSystemRenderer>();
        if (ParticleStretchSliderValue == 0)
        {
            PSRenderObject.renderMode = (ParticleSystemRenderMode) 0;
        } else
        {
            PSRenderObject.renderMode = (ParticleSystemRenderMode) 1;
            PSRenderObject.velocityScale = ParticleStretchSliderValue;
        }

        var scaleFactor = newScaleFactor / oldScaleFactor;
        ps.gameObject.transform.localScale *= scaleFactor;
        ps.gravityModifier *= scaleFactor;
        //Debug.Log($"go:{go.name}\tps:{ps.scalingMode}\tgravityModifier:{ps.gravityModifier}");

    }
    void OnEnable()
    {
        //initScale = Selection.activeGameObject.transform.localScale;
        Restart();
    }

    void OnDisable()
    {
        _styleInitialized = false;
    }

    void OnSelectionChange()
    {
        Restart();
        Repaint();
    }
}