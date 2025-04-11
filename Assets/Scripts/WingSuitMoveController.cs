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

        //生成尾翼
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
    private                  float     yaw = 0f;
    [SerializeField] private Transform leftController;
    [SerializeField] private Transform rightController;
    private                  float     currentYaw  = 0f; // 实际旋转的 Y 值（带缓动）
    private                  float     yawVelocity = 0f; // 平滑用的速度缓存变量

    private void ApplyRotation()
    {
        // 1. 计算目标 yaw
        yaw += (leftController.position.y - rightController.position.y) * 120f * Time.deltaTime;

        // 2. 平滑过渡 currentYaw → yaw（使用 SmoothDamp）
        currentYaw = Mathf.SmoothDampAngle(currentYaw, yaw, ref yawVelocity, 0.2f);

        // 3. 应用平滑后的角度
        Quaternion targetRotation = Quaternion.Euler(0f, currentYaw, 0f);
        rb.MoveRotation(targetRotation);
    }

    //碰撞物体检测以及转向
    private void OnTriggerEnter(Collider other)//检测
    {
        Debug.Log("Detected object: " + other.name);
        // 计算远离物体的方向；这里可以依据需求调整策略
        Vector3 directionAway = transform.forward*1.5f + (transform.position - other.ClosestPoint(transform.position)).normalized;
        // 启动协程平滑转向，同时禁用控制器输入旋转
        StartCoroutine(SmoothRotateAway(directionAway, 2f));
    }
    private IEnumerator SmoothRotateAway(Vector3 direction, float duration)//转向
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

        currentYaw     = yaw;
        yawVelocity    = 0f;

        isRotatingAway = false;
    }

}