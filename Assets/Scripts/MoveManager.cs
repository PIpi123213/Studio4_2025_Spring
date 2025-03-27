using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveManager : MonoBehaviour
{
    [Header("����Դ")]
    [SerializeField] private MoveInSpace _headsetData;

    void OnEnable()
    {
        if (_headsetData != null)
        {
            _headsetData.StartTracking();
        }
    }

    void Update()
    {
        // ������ͷ������λ������
        _headsetData?.UpdateTracking();
    }

    void OnDisable()
    {
        _headsetData?.ResetTracking();
    }

    // ʾ�����������ű��з�������
    public Vector3 GetCurrentHeadsetOffset()
    {
        return _headsetData?.PhysicalOffset ?? Vector3.zero;
    }
}
