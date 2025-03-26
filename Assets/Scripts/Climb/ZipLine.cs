using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ZipLine : MonoBehaviour
{
    public Transform playerTransform; // 玩家 transform
    public Transform startPoint;      // 滑索起点
    public Transform endPoint;        // 滑索终点
    public float speed = 5f;          // 滑行速度
    public Transform zipLineHandler;  // 滑索器 (滑索器物体)
    
    public XRGrabInteractable grabInteractable;
    private IXRSelectInteractor playerInteractor;
    private bool isSliding = false;
    private XRInteractionManager interactionManager;
    private InteractionLayerMask originalLayer;

    private bool isDone=false;
    private Vector3 initialPlayerOffset; // 记录玩家和滑索器之间的初始偏移量
    public HandPoseSlider handPoseSlider;
    private void Start()
    {
       
        interactionManager = grabInteractable.interactionManager;

        // 记录原始交互层
        originalLayer = grabInteractable.interactionLayers;

        // 监听抓取事件
        grabInteractable.selectEntered.AddListener(OnGrab);
    }

    private void OnDestroy()
    {
        grabInteractable.selectEntered.RemoveListener(OnGrab);
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        if (isSliding||isDone) return;

        playerInteractor = args.interactorObject as IXRSelectInteractor;

        if (playerInteractor != null)
        {
            if (grabInteractable.interactorsSelecting.Count == 2 && !isDone)
            {
                isSliding = true;

                // **禁用玩家松手**
                //grabInteractable.interactionLayers = 0; // 禁用所有交互层
                handPoseSlider.ziplineActive = true;
                // 记录玩家和滑索器之间的初始偏移量
                if (zipLineHandler != null && playerTransform != null)
                {
                    initialPlayerOffset = playerTransform.position - zipLineHandler.position;
                }

                // 开始滑行
                StartCoroutine(SlideAlongZipline());
            }

          
        }
    }

    private System.Collections.IEnumerator SlideAlongZipline()
    {
        // 将滑索器移动到起点位置
        zipLineHandler.position = startPoint.position;
        float currentSpeed = 0f; // 当前速度从 0 开始
        float acceleration = speed / 2f; 
        // 启动滑行过程
        while (zipLineHandler != null && Vector3.Distance(zipLineHandler.position, endPoint.position) > 0.1f)
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, speed, acceleration * Time.deltaTime);
            // 滑索器按速度向终点移动
            zipLineHandler.position = Vector3.MoveTowards(
           zipLineHandler.position, endPoint.position, currentSpeed * Time.deltaTime
            );

            // 玩家根据滑索器的位置保持相对位置
            if (playerTransform != null)
            {
                playerTransform.position = zipLineHandler.position + initialPlayerOffset;
            }

            yield return null;
        }

        EndZiplineRide();
    }

    private void EndZiplineRide()
    {
        isSliding = false;
        isDone = true;
        handPoseSlider.ziplineActive = false;
        handPoseSlider.ProcessPendingExitEvents();
        // **恢复交互**
        //grabInteractable.interactionLayers = originalLayer;

        // **强制释放玩家**
        var interactors = new List<IXRSelectInteractor>(grabInteractable.interactorsSelecting);
        foreach (var interactor in interactors)
        {
            interactionManager.SelectExit(interactor, grabInteractable);
        }

    }
}
