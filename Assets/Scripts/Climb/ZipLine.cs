using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ZipLine : MonoBehaviour
{
    public Transform playerTransform; // ��� transform
    //public Transform startPoint;      // �������
    //public Transform endPoint;        // �����յ�
    public float speed = 5f;          // �����ٶ�
    public Transform zipLineHandler;
    public float rotationSpeed = 30f;      // ��ת�ٶȣ���/�룩
   // ������ (����������)
    public Transform[] waypoints;
    public XRGrabInteractable grabInteractable;
    private IXRSelectInteractor playerInteractor;
    private bool isSliding = false;
    private XRInteractionManager interactionManager;
    private InteractionLayerMask originalLayer;
    private Quaternion[] waypointRotations;
    private bool isDone=false;
    private Vector3 initialPlayerOffset; // ��¼��Һͻ�����֮��ĳ�ʼƫ����
    public HandPoseSlider handPoseSlider;
    private void Start()
    {
        PrecalculateWaypointRotations();
        interactionManager = grabInteractable.interactionManager;

        // ��¼ԭʼ������
        originalLayer = grabInteractable.interactionLayers;

        // ����ץȡ�¼�
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

                // **�����������**
                //grabInteractable.interactionLayers = 0; // �������н�����
                handPoseSlider.ziplineActive = true;
                // ��¼��Һͻ�����֮��ĳ�ʼƫ����
                if (zipLineHandler != null && playerTransform != null)
                {
                    initialPlayerOffset = playerTransform.position - zipLineHandler.position;
                }

                // ��ʼ����
                StartCoroutine(SlideAlongZipline());
            }

          
        }
    }

    /*  private System.Collections.IEnumerator SlideAlongZipline()
      {
          // ���������ƶ������λ��
          zipLineHandler.position = startPoint.position;
          float currentSpeed = 0f; // ��ǰ�ٶȴ� 0 ��ʼ
          float acceleration = speed / 2.5f; 
          // �������й���
          while (zipLineHandler != null && Vector3.Distance(zipLineHandler.position, endPoint.position) > 0.1f)
          {
              currentSpeed = Mathf.MoveTowards(currentSpeed, speed, acceleration * Time.deltaTime);
              // ���������ٶ����յ��ƶ�
              zipLineHandler.position = Vector3.MoveTowards(
             zipLineHandler.position, endPoint.position, currentSpeed * Time.deltaTime
              );

              // ��Ҹ��ݻ�������λ�ñ������λ��
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
            waypointRotations[i] = Quaternion.LookRotation(direction);
        }
        // ���һ���㱣��ǰһ�������ת
        waypointRotations[waypoints.Length - 1] = waypointRotations[waypoints.Length - 2];
    }

    private System.Collections.IEnumerator SlideAlongZipline()
    {
        // ȷ��������������
        if (waypoints == null || waypoints.Length < 2)
        {
            Debug.LogError("��Ҫ��������2��·���㣡");
            yield break;
        }

        // ��ʼ�����ƶ�����һ����
        zipLineHandler.position = waypoints[0].position;
        int currentWaypointIndex = 1;
        float currentSpeed = 0f;
        float acceleration = speed / 2.5f;

        // ��������·����
        while (currentWaypointIndex < waypoints.Length)
        {
            Transform start = waypoints[currentWaypointIndex - 1];
            Transform end = waypoints[currentWaypointIndex];
            Quaternion targetRotation = waypointRotations[currentWaypointIndex - 1];
            // ���λ����߼�
            while (Vector3.Distance(zipLineHandler.position, end.position) > 0.1f)
            {
                // �����߼�
                currentSpeed = Mathf.MoveTowards(currentSpeed, speed, acceleration * Time.deltaTime);

                // �ƶ�������
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
                // ͬ�����λ��
                if (playerTransform != null)
                {
                    playerTransform.position = zipLineHandler.position + initialPlayerOffset;
                    playerTransform.rotation = Quaternion.RotateTowards(
                      playerTransform.rotation,
                      targetRotation,
                      rotationSpeed * Time.deltaTime
                  );
                }

                yield return null;
            }

            // ���ﵱǰ�յ���л�����һ��
            currentWaypointIndex++;
            //currentSpeed = 0f; // �����ٶȣ���ѡ��
        }

        EndZiplineRide();
    }

    private void EndZiplineRide()
    {
        isSliding = false;
        isDone = true;
        handPoseSlider.ziplineActive = false;
        handPoseSlider.ProcessPendingExitEvents();
        // **�ָ�����**
        //grabInteractable.interactionLayers = originalLayer;

        // **ǿ���ͷ����**
        var interactors = new List<IXRSelectInteractor>(grabInteractable.interactorsSelecting);
        foreach (var interactor in interactors)
        {
            interactionManager.SelectExit(interactor, grabInteractable);
        }

    }
}
