using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveManager : MonoBehaviour
{
    [Header("数据源")]
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
        // 仅更新头显物理位移数据
        _headsetData?.UpdateTracking();
    }

    void OnDisable()
    {
        _headsetData?.ResetTracking();
    }

    // 示例：在其他脚本中访问数据
    public Vector3 GetCurrentHeadsetOffset()
    {
        return _headsetData?.PhysicalOffset ?? Vector3.zero;
    }
}
