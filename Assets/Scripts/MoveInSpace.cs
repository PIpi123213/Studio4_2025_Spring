using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
[CreateAssetMenu(menuName = "XR/Physical Movement Data")]
public class MoveInSpace:ScriptableObject
{
    // 配置参数
    [Header("Tracking Settings")]
    [SerializeField] private bool _ignoreVerticalMovement = true;

    // 运行时数据（不序列化）
    [System.NonSerialized] private Vector3 _initialHeadPosition;
    [System.NonSerialized] private Vector3 _currentPhysicalOffset;
    [System.NonSerialized] private bool _isInitialized;

    // 当前物理位移（Editor 可见）
#if UNITY_EDITOR
    [Header("Debug View")]
    [SerializeField] private Vector3 _editorPhysicalOffset;
#endif

    // 属性访问器
    public Vector3 PhysicalOffset => _currentPhysicalOffset;
    public bool IsTracking => _isInitialized;

   

    /// <summary>
    /// 初始化头显跟踪
    /// </summary>
    public bool StartTracking()
    {
        if (TryGetHeadsetPosition(out Vector3 pos))
        {
            _initialHeadPosition = pos;
            _currentPhysicalOffset = Vector3.zero;
            _isInitialized = true;
            Debug.Log($"[HeadsetTracker] 跟踪已启动 | 初始位置: {pos}");
            return true;
        }
        Debug.LogError("[HeadsetTracker] 无法获取头显设备位置");
        return false;
    }

    /// <summary>
    /// 更新当前物理位移数据
    /// </summary>
    public bool UpdateTracking()
    {
        if (!_isInitialized) return false;

        if (TryGetHeadsetPosition(out Vector3 currentPos))
        {
            _currentPhysicalOffset = currentPos - _initialHeadPosition;

            if (_ignoreVerticalMovement)
                _currentPhysicalOffset.y = 0;

            Debug.Log($"[HeadsetTracker] 物理位移: {_currentPhysicalOffset}");
            return true;
        }
        return false;
    }

    /// <summary>
    /// 重置跟踪数据
    /// </summary>
    public void ResetTracking()
    {
        _isInitialized = false;
        _currentPhysicalOffset = Vector3.zero;
        Debug.Log("[HeadsetTracker] 跟踪已重置");
    }

    private bool TryGetHeadsetPosition(out Vector3 position)
    {
        var device = InputDevices.GetDeviceAtXRNode(XRNode.CenterEye);
        if (device.isValid && device.TryGetFeatureValue(CommonUsages.devicePosition, out position))
        {
            return true;
        }

        position = Vector3.zero; // 确保在所有路径上都赋值
        return false;
    }
}
