using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
[CreateAssetMenu(menuName = "XR/Physical Movement Data")]
public class MoveInSpace:ScriptableObject
{
    // ���ò���
    [Header("Tracking Settings")]
    [SerializeField] private bool _ignoreVerticalMovement = true;

    // ����ʱ���ݣ������л���
    [System.NonSerialized] private Vector3 _initialHeadPosition;
    [System.NonSerialized] private Vector3 _currentPhysicalOffset;
    [System.NonSerialized] private bool _isInitialized;

    // ��ǰ����λ�ƣ�Editor �ɼ���
#if UNITY_EDITOR
    [Header("Debug View")]
    [SerializeField] private Vector3 _editorPhysicalOffset;
#endif

    // ���Է�����
    public Vector3 PhysicalOffset => _currentPhysicalOffset;
    public bool IsTracking => _isInitialized;

   

    /// <summary>
    /// ��ʼ��ͷ�Ը���
    /// </summary>
    public bool StartTracking()
    {
        if (TryGetHeadsetPosition(out Vector3 pos))
        {
            _initialHeadPosition = pos;
            _currentPhysicalOffset = Vector3.zero;
            _isInitialized = true;
            Debug.Log($"[HeadsetTracker] ���������� | ��ʼλ��: {pos}");
            return true;
        }
        Debug.LogError("[HeadsetTracker] �޷���ȡͷ���豸λ��");
        return false;
    }

    /// <summary>
    /// ���µ�ǰ����λ������
    /// </summary>
    public bool UpdateTracking()
    {
        if (!_isInitialized) return false;

        if (TryGetHeadsetPosition(out Vector3 currentPos))
        {
            _currentPhysicalOffset = currentPos - _initialHeadPosition;

            if (_ignoreVerticalMovement)
                _currentPhysicalOffset.y = 0;

            Debug.Log($"[HeadsetTracker] ����λ��: {_currentPhysicalOffset}");
            return true;
        }
        return false;
    }

    /// <summary>
    /// ���ø�������
    /// </summary>
    public void ResetTracking()
    {
        _isInitialized = false;
        _currentPhysicalOffset = Vector3.zero;
        Debug.Log("[HeadsetTracker] ����������");
    }

    private bool TryGetHeadsetPosition(out Vector3 position)
    {
        var device = InputDevices.GetDeviceAtXRNode(XRNode.CenterEye);
        if (device.isValid && device.TryGetFeatureValue(CommonUsages.devicePosition, out position))
        {
            return true;
        }

        position = Vector3.zero; // ȷ��������·���϶���ֵ
        return false;
    }
}
