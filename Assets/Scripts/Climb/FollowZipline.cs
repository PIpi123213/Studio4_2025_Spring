using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowZipline : MonoBehaviour
{
    // Start is called before the first frame update
    private Vector3 offset;

    void Start()
    {
        // 计算初始偏移量：子物体相对于父物体的位置
        if (transform.parent != null)
            offset = transform.localPosition;
    }

    void LateUpdate()
    {
        // 如果有父物体，则更新子物体位置
        if (transform.parent != null)
        {
            transform.position = transform.parent.position + offset;
        }
    }
}
