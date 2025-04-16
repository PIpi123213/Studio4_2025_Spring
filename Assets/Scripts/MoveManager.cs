using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR;

public class MoveManager : MonoBehaviour
{
    [SerializeField] private MoveInSpace _movementData;
    public Transform TrackingObject;
    private Vector3 localPosition = Vector3.zero;
    XROrigin xrOrigin;
    Vector3 lastEyeLocalPos;
    public static MoveManager Instance { get; private set; }
    private Vector3 CurrentWorldPosition = Vector3.zero;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;  // 第一次 Awake 时赋值
        }
        else
        {
            Destroy(gameObject);  // 防止重复创建
        }
        _movementData.InitializeSystem();
        CurrentWorldPosition = this.transform.position;
        xrOrigin = GetComponent<XROrigin>();
    }

    void Start()
    {
        lastEyeLocalPos = TrackingObject.localPosition;
    }
    private void OnApplicationQuit()
    {
        _movementData.InitializeSystem();
    }


    void Update()
    {
        if (TryGetDevicePosition(out Vector3 currentPos))
        {
            localPosition = TrackingObject.localPosition;
            _movementData.UpdateOffset(localPosition);
        }
    }
    private bool TryGetDevicePosition(out Vector3 position)
    {
        var device = InputDevices.GetDeviceAtXRNode(XRNode.CenterEye);
        if (device.isValid && device.TryGetFeatureValue(CommonUsages.devicePosition, out position))
        {
            return true;
        }

        position = Vector3.zero; // 确保所有路径都有赋值
        return false;
    }

    void LateUpdate()
    {
        // 当前 CenterEye 在本地空间的位置
        // 当前帧头显在本地空间的位置
        Vector3 currentEyeLocalPos = TrackingObject.localPosition;

        // 计算相对于上一帧的增量（只在 XZ 平面）
        Vector3 delta = currentEyeLocalPos- lastEyeLocalPos   ;
        Vector3 deltaXZ = new Vector3(delta.x, 0f, delta.z);

        Vector3 deltaWorld = TrackingObject.parent.TransformVector(deltaXZ);
        // 把这个增量应用到 XROrigin 上
        //xrOrigin.transform.position += deltaWorld;

        // 更新 lastEyeLocalPos，为下一帧做准备
        lastEyeLocalPos = currentEyeLocalPos;

        // 最后，把 centerEye.localPosition 重置为 (0,0,0) 或者你想要的初始偏移
        // 如果你的 XR Rig 在设计时 centerEye 本地坐标并非 (0,0,0)，
        // 请把下面这一行改成你的初始本地坐标。
        //TrackingObject.localPosition = Vector3.zero;

      

    }
    public void OnSceneIn()
    {
        CurrentWorldPosition = this.transform.position;
        Debug.Log(this.transform.position);


    }

    public void OnSceneOut()
    {
        CurrentWorldPosition = CurrentWorldPosition + localPosition;
        this.transform.position = CurrentWorldPosition;
        Debug.Log(this.transform.position);


    }
}
