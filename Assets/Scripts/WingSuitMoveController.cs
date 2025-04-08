using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WingSuitMoveController : MonoBehaviour
{
    private Rigidbody rb;

    [SerializeField] private float     glideFoward = 0.2f;
    private float     maxSpeed    = 8.5f;
    private float     currentSpeed = 0.0f;
    private float     turnThreshold   = 0.1f; // 转向差异阈值

    [SerializeField] private Transform leftController;
    [SerializeField] private Transform rightController;
    void Start()
    {
        rb            = GetComponent<Rigidbody>();
        rb.useGravity = false; // 关闭重力，依赖推力维持飞行
    }
    [SerializeField] private float flySpeed = 1f;
    private float yaw = 0f;
    // Update is called once per frame
    void Update()
    {
        DetectSpeed();
        transform.position += transform.forward * flySpeed * Time.deltaTime;
        yaw                += (leftController.position.y - rightController.position.y) * 120 * Time.deltaTime;
        //应用旋转
        transform.localRotation = Quaternion.Euler(Vector3.up*yaw+Vector3.right+Vector3.forward);
    }

    //检测当前速度是否超速
    private void DetectSpeed()
    {
        Vector3 currentVelocity = rb.velocity;
        if (currentVelocity.magnitude > maxSpeed)
        {
            // 计算速度在单位方向上的向量
            Vector3 normalizedVelocity = currentVelocity.normalized;

            // 将速度限制在阈值内
            rb.velocity = normalizedVelocity * maxSpeed;
        }
    }

    //检测飞行状态
    private string DetectFlyState()
    {
        float heightDifference = leftController.position.y - rightController.position.y;
        if(heightDifference > turnThreshold)
        {
            // 左手高于右手，向左转
            return  "left";
        }
        else if(heightDifference < -turnThreshold)
        {
            // 右手高于左手，向右转
            return "right";
        }
        else
        {
            return "forward";
        }
    }

    //头部转向，防止xrrig转向后，视角不转
    private void TurnLeftOrRight(float angle)
    {
        // 计算旋转的四元数
        Quaternion rotation = Quaternion.Euler(0, angle, 0);

        // 应用旋转
        rb.MoveRotation(rb.rotation * rotation);
    }

    //控制飞行状态

    private void ControlFlyState(string flyState)
    {
        Vector3 forwardForce = transform.forward * glideFoward;
        Vector3 leftForce   = -transform.right * glideFoward;
        Vector3 rightForce  = transform.right * glideFoward;
        rb.AddForce(forwardForce, ForceMode.Acceleration);

        switch (flyState)
        {
            case "left":
                TurnLeftOrRight(-turnThreshold);
                rb.AddForce(leftForce, ForceMode.Acceleration);
                break;
            case "right":
                TurnLeftOrRight(turnThreshold);
                rb.AddForce(rightForce, ForceMode.Acceleration);
                break;
            case "forward":
                break;
            default:
                break;
        }
    }
}