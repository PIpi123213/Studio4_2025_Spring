using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnchorPoint : MonoBehaviour
{
    public Transform otherObject; // 另一个物体
    public bool isCurrentParent = true; // 当前是否是父物体

    private Collider myCollider;
    private Rigidbody myRigidbody;

    // 用于保存原始相对变换
    private Vector3 originalLocalPosition;
    private Quaternion originalLocalRotation;
    private Vector3 originalLocalScale;

    private bool AisFather = true;
    private bool BisFather = false;

    public static AttachAnchor attachAnchor = null;

    void Start()
    {
        if (otherObject == null)
        {
            Debug.LogError("Other object not assigned!");
            return;
        }
        myCollider = GetComponent<Collider>();
        myRigidbody = GetComponent<Rigidbody>();
        // 记录初始的相对变换
        originalLocalPosition = otherObject.localPosition;
        originalLocalRotation = otherObject.localRotation;
        originalLocalScale = otherObject.localScale;
        UpdatePhysicsState();
    }

    public void ToggleParenting()
    {
        if (isCurrentParent)
        {
            // 从 A是父B是子 → 切换为 B是父A是子
            // 1. 保存当前世界变换
            Vector3 aWorldPos = transform.position;
            Quaternion aWorldRot = transform.rotation;
            Vector3 aWorldScale = transform.lossyScale;

            Vector3 bWorldPos = otherObject.position;
            Quaternion bWorldRot = otherObject.rotation;
            Vector3 bWorldScale = otherObject.lossyScale;

            // 2. 解除当前父子关系
            otherObject.SetParent(null);

            // 3. 设置新的父子关系 (A作为B的子物体)


            // 4. 恢复世界变换
            otherObject.position = bWorldPos;
            otherObject.rotation = bWorldRot;
            SetLossyScale(otherObject, bWorldScale);
            transform.position = aWorldPos;
            transform.rotation = aWorldRot;
            SetLossyScale(transform, aWorldScale);

            transform.SetParent(otherObject);

        }
        else
        {
            // 从 B是父 → 切换回 A是父

            // 1. 保存当前世界变换
            Vector3 aWorldPos = transform.position;
            Quaternion aWorldRot = transform.rotation;
            Vector3 aWorldScale = transform.lossyScale;

            Vector3 bWorldPos = otherObject.position;
            Quaternion bWorldRot = otherObject.rotation;
            Vector3 bWorldScale = otherObject.lossyScale;

            // 2. 解除当前父子关系
            transform.SetParent(null);

            // 3. 恢复原始父子关系 (B作为A的子物体)


            // 4. 恢复世界变换
            transform.position = aWorldPos;
            transform.rotation = aWorldRot;
            SetLossyScale(transform, aWorldScale);

            otherObject.position = bWorldPos;
            otherObject.rotation = bWorldRot;
            SetLossyScale(otherObject, bWorldScale);

            otherObject.SetParent(transform);
        }


        isCurrentParent = !isCurrentParent;
        UpdatePhysicsState();
    }


    // 切换父子关系
    public void ToggleParentingA()
    {
        
       
            // 从 B是父 → 切换回 A是父

            // 1. 保存当前世界变换
            Vector3 aWorldPos = transform.position;
            Quaternion aWorldRot = transform.rotation;
            Vector3 aWorldScale = transform.lossyScale;

            Vector3 bWorldPos = otherObject.position;
            Quaternion bWorldRot = otherObject.rotation;
            Vector3 bWorldScale = otherObject.lossyScale;

            // 2. 解除当前父子关系
            transform.SetParent(null);

            // 3. 恢复原始父子关系 (B作为A的子物体)
          

            // 4. 恢复世界变换
            transform.position = aWorldPos;
            transform.rotation = aWorldRot;
            SetLossyScale(transform, aWorldScale);

            otherObject.position = bWorldPos;
            otherObject.rotation = bWorldRot;
            SetLossyScale(otherObject, bWorldScale);

            otherObject.SetParent(transform);
        

        isCurrentParent = !isCurrentParent;
        UpdatePhysicsState();
    }
    public void ToggleParentingB()
    {
        
            // 从 A是父B是子 → 切换为 B是父A是子

            // 1. 保存当前世界变换
            Vector3 aWorldPos = transform.position;
            Quaternion aWorldRot = transform.rotation;
            Vector3 aWorldScale = transform.lossyScale;

            Vector3 bWorldPos = otherObject.position;
            Quaternion bWorldRot = otherObject.rotation;
            Vector3 bWorldScale = otherObject.lossyScale;

            // 2. 解除当前父子关系
            otherObject.SetParent(null);

            // 3. 设置新的父子关系 (A作为B的子物体)


            // 4. 恢复世界变换
            otherObject.position = bWorldPos;
            otherObject.rotation = bWorldRot;
            SetLossyScale(otherObject, bWorldScale);
            transform.position = aWorldPos;
            transform.rotation = aWorldRot;
            SetLossyScale(transform, aWorldScale);

            transform.SetParent(otherObject);
        
       

        isCurrentParent = !isCurrentParent;
        UpdatePhysicsState();
    }





    private void SetLossyScale(Transform target, Vector3 worldScale)
    {
        if (target.parent == null)
        {
            target.localScale = worldScale;
        }
        else
        {
            Vector3 parentScale = target.parent.lossyScale;
            target.localScale = new Vector3(
                worldScale.x / parentScale.x,
                worldScale.y / parentScale.y,
                worldScale.z / parentScale.z
            );
        }
    }
    private void UpdatePhysicsState()
    {
        if (myCollider != null)
            myCollider.isTrigger = !isCurrentParent;

        if (myRigidbody != null)
        {
            RigidbodyConstraints rotationConstraints =
                RigidbodyConstraints.FreezeRotationX |
                RigidbodyConstraints.FreezeRotationY |
                RigidbodyConstraints.FreezeRotationZ;

            RigidbodyConstraints positionConstraints;

            if (!isCurrentParent)
            {
                // 锁定位置
                positionConstraints =
                    RigidbodyConstraints.FreezePositionX |
                    RigidbodyConstraints.FreezePositionY |
                    RigidbodyConstraints.FreezePositionZ;
            }
            else
            {
                // 不锁位置（允许移动）
                positionConstraints = RigidbodyConstraints.None;
            }

            // 始终锁旋转，位置根据当前状态控制
            myRigidbody.constraints = positionConstraints | rotationConstraints;
        }
    }
    // 示例：按空格键切换
    void Update()
    {
        if (CharacterClimb.isClimbing)
        {
            if (AisFather)
            {
                ToggleParentingB();
                AisFather = false;
                BisFather = true;
            }
          

        }
        else
        {
            if (BisFather)
            {
                ToggleParentingA();
                BisFather = false;
                AisFather= true;
            }
        }


        if (Input.GetKeyDown(KeyCode.Space))
        {
            ToggleParenting();
        }
    }
}
