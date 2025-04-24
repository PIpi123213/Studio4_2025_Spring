using System;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public static EventManager Instance { get; private set; }

    // 事件表：事件名 -> 委托
    private Dictionary<string, Action<object>> eventTable = new Dictionary<string, Action<object>>();

    private void Awake()
    {
        // 单例初始化
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    /// 订阅事件监听
    public void Subscribe(string eventName, Action<object> listener)
    {
        if (!eventTable.ContainsKey(eventName))
            eventTable[eventName] = delegate { }; // 初始化

        eventTable[eventName] += listener;
    }

    /// 移除事件监听
    public void Unsubscribe(string eventName, Action<object> listener)
    {
        if (eventTable.ContainsKey(eventName))
            eventTable[eventName] -= listener;
    }

    /// 触发事件（可传参）
    public void Trigger(string eventName, object param = null)
    {
        if (eventTable.ContainsKey(eventName))
        {
            eventTable[eventName]?.Invoke(param);
        }
        else
        {
            PrintAllEventNames();
            Debug.LogWarning($"事件 {eventName} 未被订阅，但仍被触发。");
        }
    }
    public void PrintAllEventNames()
    {
        foreach (var eventName in eventTable.Keys)
        {
            Debug.Log($"Event Name: {eventName}");
        }
    }
}