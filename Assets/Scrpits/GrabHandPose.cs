using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
public class GrabHandPose : MonoBehaviour
{
    public HandData rightHandPose;

    private Vector3 startingHandPosition;
    private Quaternion startingHandRotation;

    private Vector3 finalHandPosition;
    private Quaternion finalHandRotation;

    private Quaternion[] startingFingerRotation;
    private Quaternion[] finalFingerRotation;
    // Start is called before the first frame update
    void Start()
    {
        XRGrabInteractable grabInteractable = GetComponent<XRGrabInteractable>();
        grabInteractable.selectEntered.AddListener(SetupPose);
        rightHandPose.gameObject.SetActive(false);
    }
    public void SetupPose(BaseInteractionEventArgs arg)
    {
       /*if(arg.interactorObject is XRDirectInteractor)
        {
            HandData handData = arg.interactorObject.transform.GetComponentInChildren<HandData>();
            handData.animator.enabled = false;
            //Debug.Log(handData.animator);
            SetHandDataValues(handData, rightHandPose);
            SetHandData(handData, finalHandPosition, finalHandRotation, finalFingerRotation);
        }*/
        if (arg.interactorObject is XRBaseControllerInteractor controllerInteractor && controllerInteractor != null)
        {
            HandData handData = controllerInteractor.xrController.transform.GetComponentInChildren<HandData>();
            handData.animator.enabled = false;
            SetHandDataValues(handData, rightHandPose);
            SetHandData(handData, finalHandPosition, finalHandRotation, finalFingerRotation);


        }


    }
    public void SetHandDataValues(HandData h1,HandData h2)
    {
        startingHandPosition = h1.root.localPosition;
        startingHandRotation = h1.root.localRotation;
        startingFingerRotation = new Quaternion[h1.fingerBones.Length];
        finalHandPosition = h2.root.localPosition;
        finalHandRotation = h2.root.localRotation;
        finalHandPosition = h2.root.localPosition;
        finalHandRotation = h2.root.localRotation;
        finalFingerRotation = new Quaternion[h2.fingerBones.Length];

      


        Debug.Log("1Position: " + h1.root.position +
          ", 1LocalPosition: " + h1.root.localPosition +
          ", 2Position: " + h2.root.position +
          ", 2LocalPosition: " + h2.root.localPosition);


        for (int i = 0; i < h1.fingerBones.Length; i++)
        {
            startingFingerRotation[i] = h1.fingerBones[i].rotation;
            finalFingerRotation[i] = h2.fingerBones[i].rotation;


        } 


    }
    public void SetHandData(HandData h,Vector3 newPosition,Quaternion newRotation, Quaternion[] newBonesRotation)
    {
        h.root.localPosition = newPosition;
        h.root.localRotation = newRotation;
        for(int i = 0; i < newBonesRotation.Length; i++)
        {
            h.fingerBones[i].localRotation = newBonesRotation[i];


        }


    }
    // Update is called once per frame

}
