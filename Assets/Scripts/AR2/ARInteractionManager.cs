using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class ARInteractionManager : MonoBehaviour
{
    public static ARInteractionManager Instance { get; private set; }

    [Header("交互物体列表（按顺序）")]
    public List<GameObject> interactableObjects;

    private int currentIndex = 0;
    
    // 事件名
    public const string InteractionComplete = "InteractionComplete";

    private void Awake()
    {
        // 单例模式
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); 
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        // 先全部隐藏
        for (int i = 0; i < interactableObjects.Count; i++)
        {
            interactableObjects[i].SetActive(false);
        }

        // 订阅交互完成事件
        EventManager.Instance.Subscribe(InteractionComplete, OnInteractionComplete);

        // 激活第一个物体
        ActivateCurrentObject();
    }

    private void OnDestroy()
    {
        // 如果是自己才取消订阅
        if (Instance == this)
        {
            EventManager.Instance.Unsubscribe(InteractionComplete, OnInteractionComplete);
        }
    }

    private void ActivateCurrentObject()
    {
        if (currentIndex >= 0 && currentIndex < interactableObjects.Count)
        {
            interactableObjects[currentIndex].SetActive(true);
        }
    }

    private void OnInteractionComplete(object param)
    {
        // 当前物体交互完，隐藏
        if (currentIndex >= 0 && currentIndex < interactableObjects.Count)
        {
            interactableObjects[currentIndex].SetActive(false);
        }

        currentIndex++;

        if (currentIndex < interactableObjects.Count)
        {
            // 激活下一个物体
            ActivateCurrentObject();
        }
        else
        {
            // 所有交互完成
            Debug.Log("All interactions completed!");
        }
    }

    /*// 可选：手动重置流程
    public void ResetInteractions()
    {
        // 先隐藏所有
        foreach (var obj in interactableObjects)
        {
            obj.SetActive(false);
        }

        currentIndex = 0;
        ActivateCurrentObject();
    }*/
}
