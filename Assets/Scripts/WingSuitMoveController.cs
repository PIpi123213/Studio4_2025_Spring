using System;
using UnityEngine;
using System.Collections;

public class WingSuitMoveController : MonoBehaviour
{
    private Rigidbody rb;
    private TrailRenderer trailRenderer;
    private bool isRotatingAway = false; // 新增标志

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;

        trailRenderer = gameObject.AddComponent<TrailRenderer>();
        trailRenderer.time       = 100.0f;
        trailRenderer.startWidth = 0.5f;
        trailRenderer.endWidth   = 0.1f;
        trailRenderer.material   = new Material(Shader.Find("Sprites/Default"));
        trailRenderer.startColor = Color.white;
        trailRenderer.endColor   = Color.clear;
    }

    void Update()
    {
        Debug.DrawRay(transform.position, -transform.forward * 10000f, Color.red);
    }

    private void FixedUpdate()
    {
        ApplyMovement();
        if (!isRotatingAway) // 只有不处于纠正旋转时才响应输入
        {
            ApplyRotation();
        }
    }

    [SerializeField] private float glideSpeed = 1000f;
    private void ApplyMovement()
    {
        rb.velocity = transform.forward * glideSpeed;
    }

    // 仅用 yaw 控制水平旋转即可
    private float yaw = 0f;
    [SerializeField] private Transform leftController;
    [SerializeField] private Transform rightController;
    private void ApplyRotation()
    {
        // 根据控制器的高度差更新 yaw
        yaw += (leftController.position.y - rightController.position.y) * 120f * Time.deltaTime;
        Debug.Log("yaw: " + yaw);
        // 这里建议只用 Y 轴旋转，如果需要其他轴，可以调整
        rb.MoveRotation(Quaternion.Euler(0, yaw, 0));
    }

    [SerializeField] private float correctiveForce = 1000f;
    private Coroutine correctiveForceCoroutine;
    private IEnumerator ApplyCorrectiveForce(Vector3 direction, float duration)
    {
        float elapsedTime = 0f;
        Quaternion initialRotation = rb.rotation;
        Quaternion targetRotation  = Quaternion.LookRotation(direction);

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            rb.velocity = rb.velocity + direction * correctiveForce * t;
            rb.MoveRotation(Quaternion.Slerp(initialRotation, targetRotation, t));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        rb.MoveRotation(targetRotation);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Detected object: " + other.name);
        // 计算远离物体的方向；这里可以依据需求调整策略
        Vector3 directionAway = transform.forward + (transform.position - other.ClosestPoint(transform.position)).normalized;
        // 启动协程平滑转向，同时禁用控制器输入旋转
        StartCoroutine(SmoothRotateAway(directionAway, 1f));
    }

    private IEnumerator SmoothRotateAway(Vector3 direction, float duration)
    {
        isRotatingAway = true;

        Quaternion initialRotation = rb.rotation;
        Quaternion targetRotation  = Quaternion.LookRotation(direction);
        float      elapsedTime     = 0f;

        while (elapsedTime < duration)
        {
            // 计算 0~1 的 t 值
            float t = elapsedTime / duration;

            // 使用 SmoothStep 创建渐进式插值因子（缓入缓出）
            float smoothT = Mathf.SmoothStep(0f, 1f, t);

            // 使用平滑后的插值因子进行球形插值
            Quaternion newRotation = Quaternion.Slerp(initialRotation, targetRotation, smoothT);

            rb.MoveRotation(newRotation);
            yaw = newRotation.eulerAngles.y;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        rb.MoveRotation(targetRotation);
        yaw            = targetRotation.eulerAngles.y;
        isRotatingAway = false;
    }

}