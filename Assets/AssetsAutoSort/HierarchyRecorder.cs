using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine.UI;

public class HierarchyRecorder : MonoBehaviour
{
    public AnimationClip clip;
    private bool isRecording;

    private GameObjectRecorder m_Recorder;

    private void OnGUI()
    {
        if (GUI.Button(new Rect(10, 10, 100, 100), "Record Animations"))
        {
            Debug.Log("Thanks for clicking this Button");
        }
    }
    void Start()
    {
        DefaultControls.Resources uiResources = new DefaultControls.Resources();
        //Set the Button Background Image someBgSprite;
        //uiResources.standard = someBgSprite;
        GameObject uiButton = DefaultControls.CreateButton(uiResources);
        //uiButton.transform.SetParent(canvas.transform, false);


        // Create recorder and record the script GameObject.
        m_Recorder = new GameObjectRecorder(gameObject);

        // Bind all the Transforms on the GameObject and all its children.
        m_Recorder.BindComponentsOfType<Transform>(gameObject, true);
    }

    void LateUpdate()
    {
        if (clip == null)
            return;

        // Take a snapshot and record all the bindings values for this frame.
        m_Recorder.TakeSnapshot(Time.deltaTime);
    }

    void OnDisable()
    {
        if (clip == null)
            return;

        if (m_Recorder.isRecording)
        {
            // Save the recorded session to the clip.
            m_Recorder.SaveToClip(clip);
        }
    }
}