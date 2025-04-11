using AmazingAssets.DynamicRadialMasks;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections;
using UnityEngine.SceneManagement;

public class SceneTransitionTrigger : MonoBehaviour
{
    // 挂载在 XRGrabInteractable 物体上
    private                  XRGrabInteractable grabInteractable;
    // [SerializeField] private GameObject         DissolveEffectTool;
    [SerializeField] private DRMGameObject drmGameObject;
    [SerializeField] private OVRPassthroughLayer ptLayer;
    bool hasTriggered = false;
    private bool radiusFinished =false;
    private bool opacityFinished = false;


    void Start()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();

        // 订阅 Select Enter 事件
        grabInteractable.selectEntered.AddListener(OnSelectEnter);
        drmGameObject.radius = 0;
    }
    private void Update()
    {

    }
    private void OnSelectEnter(SelectEnterEventArgs args)
    {
        Debug.Log("catch it！");
        // 例如：改变材质颜色
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
        // 同时启动两个动画
        hasTriggered = true;
        Coroutine radiusRoutine = StartCoroutine(AnimateRadius());
        Coroutine opacityRoutine = StartCoroutine(AnimateOpacity());

        // 等待两者完成（总时间取最大值）
        yield return radiusRoutine;
        yield return opacityRoutine;

        MoveManager.Instance.OnSceneIn();//记录位置
        // 完成后执行场景切换或其他逻辑
        Debug.Log("All animations completed!");

    }


    [SerializeField] float RadiusDuration    = 5f;
    [SerializeField] float startRadius = 0f;
    [SerializeField] float endRadius   = 100f;

    private IEnumerator AnimateRadius()
    {

        float elapsedTime = 0f;


        while (elapsedTime < RadiusDuration)
        {
            drmGameObject.radius =  Mathf.Lerp(startRadius, endRadius, elapsedTime / RadiusDuration);
            elapsedTime          += Time.deltaTime;
            yield return null;
        }
        // 示例代码：禁用Passthrough后恢复设置
        if (ptLayer != null)
        {
            ptLayer.enabled = false;
            Destroy(ptLayer);
        }
        Camera.main.clearFlags = CameraClearFlags.Skybox;

        hasTriggered = true;
        radiusFinished = true;
        drmGameObject.radius = endRadius;
    }
    [SerializeField] float opacityDuration    = 5f;
    [SerializeField] float startOpacity = 1f;
    [SerializeField] float endOpacity   = 0f;
    private IEnumerator AnimateOpacity()
    {

        float elapsedTime = 0f;
        Debug.Log("AnimateOpacity");


        while (elapsedTime < opacityDuration)
        {
            ptLayer.textureOpacity =  Mathf.Lerp(startOpacity, endOpacity, elapsedTime / opacityDuration);
            elapsedTime            += Time.deltaTime;
            yield return null;
        }
        opacityFinished = true;
        hasTriggered           = true;
        ptLayer.textureOpacity = endOpacity;
    }
    void OnDestroy()
    {
        // 取消订阅事件
        grabInteractable.selectEntered.RemoveListener(OnSelectEnter);
    }
}