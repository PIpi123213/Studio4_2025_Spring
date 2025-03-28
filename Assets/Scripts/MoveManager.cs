using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class MoveManager : MonoBehaviour
{
    [SerializeField] private MoveInSpace _movementData;
    public Transform TrackingObject;
    private Vector3 localPosition = Vector3.zero;
    private void Awake()
    {
        _movementData.InitializeSystem();
    }

    void Start()
    {
       
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

   /* private bool TryGetDevicePosition(out Vector3 position)
    {
        var device = InputDevices.GetDeviceAtXRNode(XRNode.CenterEye);
        if (device.isValid && device.TryGetFeatureValue(CommonUsages.devicePosition, out position))
        {
            return true;
        }

        position = Vector3.zero; // 确保所有路径都有赋值
        return false;
    }*/
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
}
