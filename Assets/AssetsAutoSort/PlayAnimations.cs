using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayAnimations : MonoBehaviour
{
    public Animation referenceToAnimation;
    public AnimationClip[] clips;
    public int _currClip = 0;

    public bool isAnimStarted = false;

    private void Start()
    {
        referenceToAnimation = gameObject.GetComponent<Animation>();
        if (referenceToAnimation == null)
            return;

        foreach (AnimationState state in referenceToAnimation)
        {
            Debug.Log(state.name);
        }


        var i = 0;
        foreach (AnimationState state in referenceToAnimation)
        {
            //do something with state
            //clips[i] = referenceToAnimation[state];
            i++;
        }
    }

    public void OnAnimationComplete() //you could call this by adding an animation event at the end of all of your animations
    {
        _currClip++;
        if (_currClip == clips.Length) return; //avoid out of scope exception
        referenceToAnimation.clip = clips[_currClip];
        referenceToAnimation.Play();
    }
    private void Update()
    {
        if (isAnimStarted)  isAnimStarted = false; OnAnimationComplete();        
    }
}