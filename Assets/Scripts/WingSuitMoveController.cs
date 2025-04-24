using System;
using UnityEngine;
using System.Collections;
using Unity.SharpZipLib.Zip.Compression.Streams;

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
        Debug.DrawRay(transform.position, -transform.forward * 10f, Color.red);
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
        // 调用垂直速度计算函数
        DetectDive();

        // 计算最终速度
        Vector3 glideVelocity = transform.forward * glideSpeed;
        glideVelocity.y = verticalSpeed; // 添加垂直速度

        // 应用速度
        rb.velocity = glideVelocity;
    }

    // 仅用 yaw 控制水平旋转即可
    private                  float     yaw = 0f;
    [SerializeField] private Transform leftController;
    [SerializeField] private Transform rightController;
    private                  float     currentYaw  = 0f; // 实际旋转的 Y 值（带缓动）
    private                  float     yawVelocity = 0f; // 平滑用的速度缓存变量

    private void ApplyRotation()
    {
        if (isRotatingAway) return; // 如果正在旋转，跳过输入控制

        yaw        += (leftController.position.y - rightController.position.y) * 120f * Time.deltaTime;
        currentYaw =  Mathf.SmoothDampAngle(currentYaw, yaw, ref yawVelocity, 0.2f);

        Quaternion targetRotation = Quaternion.Euler(0f, currentYaw, 0f);
        rb.MoveRotation(targetRotation);
    }

    //碰撞物体检测以及转向
    private void OnTriggerEnter(Collider other)//检测
    {
        Debug.Log("Detected object: " + other.name);
        // 计算远离物体的方向；这里可以依据需求调整策略
        Vector3 directionAway = transform.forward*1f + (transform.position - other.ClosestPoint(transform.position)).normalized;
        // 启动协程平滑转向，同时禁用控制器输入旋转
        StartCoroutine(SmoothRotateAway(directionAway, 2f));
    }

    [SerializeField] private float defaultVerticalSpeed;
    [SerializeField] private float gravityFactor = 10f;//控制俯冲比例
    private                  float verticalSpeed ;

    private void DetectDive()
    {
        float averageHeight = (leftController.position.y + rightController.position.y) / 2f;
        verticalSpeed = -defaultVerticalSpeed + averageHeight * gravityFactor;

    }

    private IEnumerator SmoothRotateAway(Vector3 direction, float duration)
    {
        isRotatingAway = true;

        Quaternion initialRotation = rb.rotation;

        // 仅计算目标方向的 yaw 旋转
        Vector3    targetDirection = new Vector3(direction.x, 0f, direction.z).normalized;
        Quaternion targetRotation  = Quaternion.LookRotation(targetDirection);

        // 保持当前的 pitch 和 roll
        targetRotation = Quaternion.Euler(0f, targetRotation.eulerAngles.y, 0f);

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float t       = elapsedTime / duration;
            float smoothT = Mathf.SmoothStep(0f, 1f, t);

            Quaternion newRotation = Quaternion.Slerp(initialRotation, targetRotation, smoothT);

            rb.MoveRotation(newRotation);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        rb.MoveRotation(targetRotation);

        // 确保最终角度更新
        yaw         = targetRotation.eulerAngles.y;
        currentYaw  = yaw;
        yawVelocity = 0f;

        isRotatingAway = false;
    }

}