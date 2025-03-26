using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ZipLine : MonoBehaviour
{
    public Transform playerTransform; // ��� transform
    public Transform startPoint;      // �������
    public Transform endPoint;        // �����յ�
    public float speed = 5f;          // �����ٶ�
    public Transform zipLineHandler;  // ������ (����������)
    
    public XRGrabInteractable grabInteractable;
    private IXRSelectInteractor playerInteractor;
    private bool isSliding = false;
    private XRInteractionManager interactionManager;
    private InteractionLayerMask originalLayer;

    private bool isDone=false;
    private Vector3 initialPlayerOffset; // ��¼��Һͻ�����֮��ĳ�ʼƫ����
    public HandPoseSlider handPoseSlider;
    private void Start()
    {
       
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

    private System.Collections.IEnumerator SlideAlongZipline()
    {
        // ���������ƶ������λ��
        zipLineHandler.position = startPoint.position;
        float currentSpeed = 0f; // ��ǰ�ٶȴ� 0 ��ʼ
        float acceleration = speed / 2f; 
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
