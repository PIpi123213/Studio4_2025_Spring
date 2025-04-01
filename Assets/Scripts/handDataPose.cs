using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.InputSystem.iOS;
using static Oculus.Interaction.GrabAPI.FingerPalmGrabAPI;

public class handDataPose : MonoBehaviour
{
    // Start is called before the first frame update
    public enum HandModelType { Left, Right }

    public HandModelType type;
    public Transform root;
    public Vector3 orginLocalPos;
    public Animator animator;
    public Transform[] fingerBones;
    public float poseTransitionDuration = 0.2f;
    [Header("Lock Settings")]
    public bool isLocked; // 新增锁定状态
    public Vector3 lockedPosition; // 新增锁定位置

    private bool ifunset = false;
    private void Awake()
    {
        orginLocalPos = transform.localPosition;
    }
    private void Update()
    {
        if (isLocked)
        {
            // 强制手部模型保持锁定位置（世界坐标）
            root.position = lockedPosition;
            ifunset = false;
        }
        else{
            if (!ifunset)
            {

                StartCoroutine(SetHandDataRoutine());

                //root.localPosition = orginLocalPos;
                ifunset = true;
            }
            
        }
    }

    public IEnumerator SetHandDataRoutine()
    {
        float timer = 0;
        Vector3 startingPosition = root.localPosition;
        while (timer < poseTransitionDuration)
        {
            Vector3 p = Vector3.Lerp(startingPosition, orginLocalPos, timer / poseTransitionDuration);
          
            root.localPosition = p;
            timer += Time.deltaTime;
            yield return null;

        }


    }
}
