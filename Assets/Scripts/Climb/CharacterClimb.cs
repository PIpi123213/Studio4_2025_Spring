using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;

public class CharacterClimb : MonoBehaviour
{
    // Start is called before the first frame update
    public CustomClimbProvider climbProvider;
    public DynamicMoveProvider dynamicMoveProvider;
    public ZipLine zipline;
    public static bool isClimbing = false;
    public static bool isStart = false;
    void Start()
    {
        if (climbProvider == null)
        {
            Debug.LogError("ClimbProvider 未找到！");
        }
        if (dynamicMoveProvider == null)
        {
            Debug.LogError("DynamicMoveProvider 未找到！");
        }
        if(zipline == null)
        {
            Debug.LogError("zipline 未找到！");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (climbProvider == null || dynamicMoveProvider == null)
            return;

        // 检测攀爬状态
        isClimbing = climbProvider.locomotionPhase == LocomotionPhase.Moving ||
                          climbProvider.locomotionPhase == LocomotionPhase.Started || ZipLine.isSliding;



        // 如果正在攀爬，关闭重力；否则开启重力
        //dynamicMoveProvider.useGravity = !isClimbing;
    }
}
