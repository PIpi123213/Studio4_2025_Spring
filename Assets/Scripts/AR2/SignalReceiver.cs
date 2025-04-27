using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class SignalReceiver : MonoBehaviour
{
    /*private PlayableDirector director;

    private void Awake()
    {
        // 尝试找到同物体上的PlayableDirector组件
        director = GetComponent<PlayableDirector>();
        if (director == null)
        {
            Debug.LogWarning("TimelineSignalReceiver: 没有找到 PlayableDirector 组件！");
        }
    }*/

    public void OnInterationComplete()
    {
        EventManager.Instance.Trigger(ARInteractionManager.InteractionComplete,null);
    }

}
