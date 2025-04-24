using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Vfxtrigger : MonoBehaviour
{
    // Start is called before the first frame update
    private XRGrabInteractable grabInteractable;
    public ParticleSystem particleSystemToControl;
    public bool trigger = false; // 你可以通过其它脚本设置这个值

    public enum Trigger
    {
        Grab,
        Touch

    }

    public Trigger triggerWay;


    private void Start()
    {
        particleSystemToControl = GetComponentInChildren<ParticleSystem>();
        grabInteractable = GetComponent<XRGrabInteractable>();
        grabInteractable.selectEntered.AddListener(OnSelectEnter);
        grabInteractable.selectExited.AddListener(OnSelectExit);
        grabInteractable.hoverEntered.AddListener(OnHandTouchEnter);
        grabInteractable.hoverExited.AddListener(OnHandTouchExit);

    }
    void Update()
    {
        if (trigger)
        {
            if (!particleSystemToControl.isPlaying)
            {
                particleSystemToControl.Play();
            }
        }
        else
        {
            if (particleSystemToControl.isPlaying)
            {
                //particleSystemToControl.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                particleSystemToControl.Stop(false, ParticleSystemStopBehavior.StopEmitting);
            }
        }
    }

    private void OnSelectEnter(SelectEnterEventArgs args)
    {
        if (triggerWay == Trigger.Touch) return;



        if (!trigger)
        {
         trigger = true;
            
        }
    }
    private void OnSelectExit(SelectExitEventArgs args)
    {
        if (triggerWay == Trigger.Touch) return;
        if (trigger)
        {
            trigger = false;

        }
    }
    private void OnHandTouchEnter(HoverEnterEventArgs args)
    {
        if (triggerWay == Trigger.Grab) return;
        if (!trigger)
        {
            trigger = true;

        }
    }

    private void OnHandTouchExit(HoverExitEventArgs args)
    {
        if (triggerWay == Trigger.Grab) return;
        if (trigger)
        {
            trigger = false;

        }
    }


}
