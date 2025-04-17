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
    public GameObject lake;
    void Start()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();

        // ���� Select Enter �¼�
        grabInteractable.selectEntered.AddListener(OnSelectEnter);
        drmGameObject.radius = 0;
        RenderSettings.skybox.SetFloat("_Exposure", 0);
        lake.SetActive(false);
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
    [SerializeField]float skyboxFadeDuration = 10f;
    private IEnumerator RunBothAnimations()
    {
        // ͬʱ������������
        hasTriggered = true;

        Coroutine radiusRoutine  = StartCoroutine(AnimateRadius());
        Coroutine opacityRoutine = StartCoroutine(AnimateOpacity());
        attachAnchor.Attach();
        // �ȴ�������ɣ���ʱ��ȡ���ֵ��
        yield return radiusRoutine;
        yield return opacityRoutine;
        //��������

        lake.SetActive(true);
        MoveManager.Instance.OnSceneIn();//��¼λ��
        // ��ɺ�ִ�г����л��������߼�
        Debug.Log("All animations completed!");

        gameObject.SetActive(false);


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
            if (drmGameObject.radius > 200)
            {
                playercamera.clearFlags = CameraClearFlags.Skybox;
                float extraSpeedFactor = 5f; // �ɸ�����Ҫ�������ٱ���
                float extraT = Mathf.Pow(elapsedTime / RadiusDuration, 2) * extraSpeedFactor;

                // ���Ӷ���İ뾶����
                drmGameObject.radius += Mathf.Lerp(0, endRadius - startRadius, extraT) * Time.deltaTime;
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        // ʾ�����룺����Passthrough��ָ�����


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
        if (ptLayer != null)
        {
            ptLayer.enabled = false;
            Destroy(ptLayer);
        }
        Coroutine skyboxRoutine = StartCoroutine(AnimateSkyboxExposure(0f, 1f, skyboxFadeDuration));
        yield return skyboxRoutine;

    }

    private IEnumerator AnimateSkyboxExposure(float startExposure, float endExposure, float duration)
    {
        if (RenderSettings.skybox.HasProperty("_Exposure"))
        {
            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                float exposure = Mathf.Lerp(startExposure, endExposure, elapsedTime / duration);
                RenderSettings.skybox.SetFloat("_Exposure", exposure);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // 确保最终曝光度为目标值
            RenderSettings.skybox.SetFloat("_Exposure", endExposure);
        }
    }

    void OnDestroy()
    {
        // ȡ�������¼�
        grabInteractable.selectEntered.RemoveListener(OnSelectEnter);
    }
}