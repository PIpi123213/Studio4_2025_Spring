using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class imageTrackingController : MonoBehaviour
{
    Transform _target;
    
    public void SetTarget(Transform target)
    {
        _target = target;
    }
    
    void Update()
    {
        if (_target != null)
        {
            transform.position = _target.position;
            transform.rotation = _target.rotation;
        }
    }
}
