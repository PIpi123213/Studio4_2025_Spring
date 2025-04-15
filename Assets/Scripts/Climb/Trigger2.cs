using AmazingAssets.DynamicRadialMasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Trigger2 : MonoBehaviour
{
    // Start is called before the first frame update
    private XRGrabInteractable grabInteractable;
    // [SerializeField] private GameObject         DissolveEffectTool;
    [SerializeField] private DRMGameObject drmGameObject;
    [SerializeField] private OVRPassthroughLayer ptLayer;
    bool hasTriggered = false;
    private bool radiusFinished = false;
    private bool opacityFinished = false;
    public Camera playercamera;
    public AttachAnchor attachAnchor;
    void Start()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();

        // ���� Select Enter �¼�
        grabInteractable.selectEntered.AddListener(OnSelectEnter);
        drmGameObject.radius = 0;
    }

    private void OnSelectEnter(SelectEnterEventArgs args)
    {
        Debug.Log("catch it��");
        // ���磺�ı������ɫ
        GetComponent<Renderer>().material.color = Color.red;
        if (!hasTriggered)
        {
            //StartCoroutine(AnimateRadius());
            //StartCoroutine(AnimateOpacity());
            StartCoroutine(RunBothAnimations());
        }
    }
    private IEnumerator RunBothAnimations()
    {
        // ͬʱ������������
        hasTriggered = true;
        Coroutine radiusRoutine = StartCoroutine(AnimateRadius());
        Coroutine opacityRoutine = StartCoroutine(AnimateOpacity());

        // �ȴ�������ɣ���ʱ��ȡ���ֵ��
        yield return radiusRoutine;
        yield return opacityRoutine;
        //��������
        attachAnchor.Attach();

        MoveManager.Instance.OnSceneIn();//��¼λ��
        // ��ɺ�ִ�г����л��������߼�
        Debug.Log("All animations completed!");

    }

    [SerializeField] float RadiusDuration = 5f;
    [SerializeField] float startRadius = 0f;
    [SerializeField] float endRadius = 100f;

    private IEnumerator AnimateRadius()
    {

        float elapsedTime = 0f;

        while (elapsedTime < RadiusDuration)
        {
            // ʹ�÷����Բ�ֵ����
            float t = Mathf.Pow(elapsedTime / RadiusDuration, 2); // ��������
            drmGameObject.radius = Mathf.Lerp(startRadius, endRadius, t);
            if (drmGameObject.radius > 220)
            {
                playercamera.clearFlags = CameraClearFlags.Skybox;
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        // ʾ�����룺����Passthrough��ָ�����
        if (ptLayer != null)
        {
            ptLayer.enabled = false;
            Destroy(ptLayer);
        }

        hasTriggered = true;
        radiusFinished = true;
        drmGameObject.radius = endRadius;
    }
    [SerializeField] float opacityDuration = 5f;
    [SerializeField] float startOpacity = 1f;
    [SerializeField] float endOpacity = 0f;
    private IEnumerator AnimateOpacity()
    {

        float elapsedTime = 0f;
        Debug.Log("AnimateOpacity");


        while (elapsedTime < opacityDuration)
        {
            ptLayer.textureOpacity = Mathf.Lerp(startOpacity, endOpacity, elapsedTime / opacityDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        opacityFinished = true;
        hasTriggered = true;
        ptLayer.textureOpacity = endOpacity;
    }
    void OnDestroy()
    {
        // ȡ�������¼�
        grabInteractable.selectEntered.RemoveListener(OnSelectEnter);
    }
}
