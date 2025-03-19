using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
public class DisableGrabHand : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject leftHandModel;
    public GameObject rightHandModel;

    void Start()
    {
        XRGrabInteractable xRGrabInteractable = GetComponent<XRGrabInteractable>();
        xRGrabInteractable.selectEntered.AddListener(HidergrabbingHand);
        xRGrabInteractable.selectExited.AddListener(ShowGrabbingHand);
    }
    public void HidergrabbingHand(BaseInteractionEventArgs args)
    {
        
        if(args.interactorObject.transform.tag=="Left Hand")
        {
            leftHandModel.SetActive(false);
        }
        else if (args.interactorObject.transform.tag == "Right Hand")
        {
           
            rightHandModel.SetActive(false);
        }

    }
    public void ShowGrabbingHand(BaseInteractionEventArgs args)
    {
        if (args.interactorObject.transform.tag == "Left Hand")
        {
            leftHandModel.SetActive(true);
        }
        else if (args.interactorObject.transform.tag == "Right Hand")
        {
            Debug.LogWarning("right");
            rightHandModel.SetActive(true);
        }




    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
