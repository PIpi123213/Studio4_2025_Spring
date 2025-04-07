using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class MoveManager : MonoBehaviour
{
    [SerializeField] private MoveInSpace _movementData;
    public Transform TrackingObject;
    private Vector3 localPosition = Vector3.zero;

    public static MoveManager Instance { get; private set; }
    private Vector3 CurrentWorldPosition = Vector3.zero;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;  // ��һ�� Awake ʱ��ֵ
        }
        else
        {
            Destroy(gameObject);  // ��ֹ�ظ�����
        }
        _movementData.InitializeSystem();
        CurrentWorldPosition = this.transform.position;
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
    private bool TryGetDevicePosition(out Vector3 position)
    {
        var device = InputDevices.GetDeviceAtXRNode(XRNode.CenterEye);
        if (device.isValid && device.TryGetFeatureValue(CommonUsages.devicePosition, out position))
        {
            return true;
        }

        position = Vector3.zero; // ȷ������·�����и�ֵ
        return false;
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
