using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;

public class ZipLine : MonoBehaviour
{
    public Transform playerTransform; // 玩家 transform
    //public Transform startPoint;      // 滑索起点
    //public Transform endPoint;        // 滑索终点
    public float speed = 5f;          // 滑行速度
    public Transform zipLineHandler;
    public float rotationSpeed = 30f;      // 旋转速度（度/秒）
   // 滑索器 (滑索器物体)
    public Transform[] waypoints;
    public CustomClimbInteractable grabInteractable;
    private IXRSelectInteractor playerInteractor;
    public static bool isSliding = false;
   
    private XRInteractionManager interactionManager;
    private InteractionLayerMask originalLayer;
    private Quaternion[] waypointRotations;
    private bool isDone=false;
    private Vector3 initialPlayerOffset; // 记录玩家和滑索器之间的初始偏移量
    public HandPoseSlider handPoseSlider;
    public CustomClimbProvider climbProvider;
    public AttachAnchor attachAnchor;
    private void Start()
    {
        PrecalculateWaypointRotations();
        interactionManager = grabInteractable.interactionManager;

        // 记录原始交互层
        originalLayer = grabInteractable.interactionLayers;

        // 监听抓取事件
        grabInteractable.selectEntered.AddListener(OnGrab);
        //grabInteractable.selectExited.AddListener(OnRelase);
        grabInteractable.selectExited.AddListener(UnSetPose);
    }

    private void OnDestroy()
    {
        grabInteractable.selectEntered.RemoveListener(OnGrab);
        grabInteractable.selectExited.RemoveListener(UnSetPose);
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
                attachAnchor.ResetAnchorPoint();
                // **禁用玩家松手**
                //grabInteractable.interactionLayers = 0; // 禁用所有交互层
                handPoseSlider.ziplineActive = true;
                // 记录玩家和滑索器之间的初始偏移量
              

                // 开始滑行
                StartCoroutine(SlideAlongZipline());
            }

          
        }
    }
    private void UnSetPose(SelectExitEventArgs args)
    {
        
        if (isSliding || isDone) return;

        playerInteractor = args.interactorObject as IXRSelectInteractor;
        if (playerInteractor != null)
        {
            if (grabInteractable.interactorsSelecting.Count == 0 )
            {
              
            }


        }



    }
    /*  private System.Collections.IEnumerator SlideAlongZipline()
      {
          // 将滑索器移动到起点位置
          zipLineHandler.position = startPoint.position;
          float currentSpeed = 0f; // 当前速度从 0 开始
          float acceleration = speed / 2.5f; 
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
      }*/
    void PrecalculateWaypointRotations()
    {
        waypointRotations = new Quaternion[waypoints.Length];
        for (int i = 0; i < waypoints.Length - 1; i++)
        {
            Vector3 direction = (waypoints[i + 1].position - waypoints[i].position).normalized;
            // 让X轴指向方向，其余轴根据需求调整
            waypointRotations[i] = Quaternion.LookRotation(direction) * Quaternion.Euler(0, -90, 0);
        }
        waypointRotations[waypoints.Length - 1] = waypointRotations[waypoints.Length - 2];
    }
    private System.Collections.IEnumerator SlideAlongZipline()
    {
        // 确保至少有两个点
        if (waypoints == null || waypoints.Length < 2)
        {
            Debug.LogError("需要至少设置2个路径点！");
            yield break;
        }
        if (zipLineHandler != null && playerTransform != null)
        {
            initialPlayerOffset = playerTransform.position - zipLineHandler.position;
        }
        //设置为zipline子物体
        Transform originalParent = playerTransform.parent;
        playerTransform.SetParent(zipLineHandler);
        // 初始化：移动到第一个点
        zipLineHandler.position = waypoints[0].position;
        int currentWaypointIndex = 1;
        float currentSpeed = 0f;
        float acceleration = speed / 3f;

        // 遍历所有路径段
        while (currentWaypointIndex < waypoints.Length)
        {
            Transform start = waypoints[currentWaypointIndex - 1];
            Transform end = waypoints[currentWaypointIndex];
            Quaternion targetRotation = waypointRotations[currentWaypointIndex - 1];
            // 单段滑动逻辑
            while (Vector3.Distance(zipLineHandler.position, end.position) > 0.1f)
            {
                // 加速逻辑
                currentSpeed = Mathf.MoveTowards(currentSpeed, speed, acceleration * Time.deltaTime);

                // 移动滑索器
                zipLineHandler.position = Vector3.MoveTowards(
                    zipLineHandler.position,
                    end.position,
                    currentSpeed * Time.deltaTime
                );
            
                zipLineHandler.rotation = Quaternion.RotateTowards(
                  zipLineHandler.rotation,
                  targetRotation,
                  rotationSpeed * Time.deltaTime
              );
                // 同步玩家位置
              /*  if (playerTransform != null)
                {
                    playerTransform.position = zipLineHandler.position + initialPlayerOffset;
                    playerTransform.rotation = zipLineHandler.rotation * Quaternion.Euler(0, -180, 0);
                }*/

                yield return null;
            }

            // 到达当前终点后，切换到下一段
            currentWaypointIndex++;
            //initialPlayerOffset = playerTransform.position - zipLineHandler.position;
            //currentSpeed = 0f; // 重置速度（可选）
        }
        playerTransform.SetParent(originalParent);
        EndZiplineRide();
    }

    private void EndZiplineRide()
    {
        //isSliding = false;
        isDone = true;
        handPoseSlider.ziplineActive = false;
        handPoseSlider.ProcessPendingExitEvents();
        // **恢复交互**
        //grabInteractable.interactionLayers = originalLayer;
        //dynamicMoveProvider.useGravity = true;
        // **强制释放玩家**
        var interactors = new List<IXRSelectInteractor>(grabInteractable.interactorsSelecting);
        foreach (var interactor in interactors)
        {
            interactionManager.SelectExit(interactor, grabInteractable);
        }

    }
}
