using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class XRGrabListener : MonoBehaviour
{
    public XRGrabInteractable grabInteractable; // 需要监听的 XRGrabInteractable 组件
    public bool isTwoHandGrab = false;


    private void OnEnable()
    {
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.AddListener(OnGrab);
            grabInteractable.selectExited.AddListener(OnRelease);
        }
    }

    private void OnDisable()
    {
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.RemoveListener(OnGrab);
            grabInteractable.selectExited.RemoveListener(OnRelease);
        }
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        if (grabInteractable.interactorsSelecting.Count == 2)
        {
            isTwoHandGrab = true;

            Debug.Log("监听器：物体被双手抓住！");
        }
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        if (grabInteractable.interactorsSelecting.Count < 2)
        {
            isTwoHandGrab = false;
            Debug.Log("监听器：物体被单手或松开！");
        }
    }
}