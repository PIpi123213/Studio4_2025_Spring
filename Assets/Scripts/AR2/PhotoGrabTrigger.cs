using AmazingAssets.DynamicRadialMasks;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections;

public class PhotoGrabTrigger : MonoBehaviour
{
    private XRGrabInteractable grabInteractable;
    private bool hasTriggered = false;
    
    public const string OnPhotoGrabbed = "OnPhotoGrabbed";
    
    void Start()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();

        // 订阅 Select Enter 事件
        grabInteractable.selectEntered.AddListener(OnSelectEnter);
    }

    private void OnSelectEnter(SelectEnterEventArgs args)
    {
        Debug.Log("Photo grabbed!");
        // 例如：改变材质颜色
        GetComponent<Renderer>().material.color = Color.red;
        if (!hasTriggered)
        {
            hasTriggered = true;
            // 触发事件
            EventManager.Instance.Trigger(OnPhotoGrabbed,"PhotoGrabbed");
        }
    }

    void OnDestroy()
    {
        // 取消订阅事件
        grabInteractable.selectEntered.RemoveListener(OnSelectEnter);
    }
    
}
