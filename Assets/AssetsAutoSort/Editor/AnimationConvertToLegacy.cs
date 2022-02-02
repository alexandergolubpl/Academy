using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Animation))]
public class AnimationConvertToLegacy : MonoBehaviour, IPanelTool
{
    public string ToolName { get; set; }

    private static GameObject go;
    private static Animator animator;
    private static List<AnimationClip> myAnimationClipList = new List<AnimationClip>();
    private static UnityEditor.Animations.AnimatorController controller;
    private static int animator_count = 0;

    public AnimationConvertToLegacy()
    {
        ToolName = typeof(AnimationConvertToLegacy).Name;
    }

    public void Execute()
    {
        AnimationConvertToLegacy.exec();
    }
    public static void exec()
    {
        GetAllAnimations();
    }

    [MenuItem("AssetTools/AnimationConvertToLegacy")]//%#d
    [MenuItem("Assets/AssetTools/AnimationConvertToLegacy")]
    [MenuItem("GameObject/Convert/AnimationConvertToLegacy", false, 0)]
    //[MenuItem("CONTEXT/Component/AnimationConvertToLegacy")]
    private static void GetAllAnimations()
    {
        List<string> selectedFiles = new List<string>();
        //if(selectedFiles.Count > 1)
        //{
        selectedFiles.Clear();
        //}
        int go_count = 1;
        foreach (GameObject go in Selection.gameObjects)
        {
            //Undo.RecordObject(go, go.name + " : Convert Animator to legacy animation");
            string p = AssetDatabase.GetAssetPath(go.GetInstanceID());
            selectedFiles.Add(p);


            animator = go.GetComponent<Animator>();
            if (animator == null)
            {
                continue;
            }
            animator_count++;

            AnimationClip[] animationClips = animator.runtimeAnimatorController.animationClips;
            myAnimationClipList = new List<AnimationClip>();

            Animation myAnim = go.GetComponent<Animation>();
            if (myAnim == null)
            {
                //myAnim = go.AddComponent<Animation>();
                Undo.AddComponent<Animation>(go);
                myAnim = go.GetComponent<Animation>();
            }

            for (int i = 0; i < animationClips.Length; i++)
            {
                myAnimationClipList.Add(animationClips[i]);

                var animclip = myAnimationClipList[i];
                var animclipPath = AssetDatabase.GetAssetPath(animclip);
                var animclipName = animclip.name;

                var animclipString =
                                        $"<color=\"yellow\">{ go_count }</color> : " +
                                        $"<color=\"green\">{go.name}</color> : " +
                                        $"<color=\"yellow\">{ i }</color> : " +
                                        $"<color=\"red\">{ animclipName }</color> : " +
                                        $"<color=\"grey\">{ animclipPath }</color>";
                Debug.Log(animclipString);

                //clip.legacy = true;
                myAnim.clip = animclip;

                animclip.legacy = true;
                myAnim.AddClip(animclip, animclipName);

            }

            foreach (AnimationState state in myAnim)
            {
                Debug.Log(state.name);
            }

            Undo.DestroyObjectImmediate(animator);
            //DestroyImmediate(animator);
            go_count++;
        }
        string result = 
            $"<color=\"red\">Animators converted</color>: <color=\"yellow\">{animator_count}</color>" +
            $"";
        Debug.Log(result);
    }
}