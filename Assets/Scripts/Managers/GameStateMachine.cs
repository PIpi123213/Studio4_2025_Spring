using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    AR1,
    WingSuit,
    AR2,
    Climb,
    AR3,
    Dive,
    AR4,
}

public class GameStateMachine : MonoBehaviour
{
    // 单例模式    
    public static GameStateMachine Instance { get; private set; }
    public GameState CurrentGameState { get; private set; }
    
    // 事件名
    public const string OnStateEnter = "OnStateEnter";
    public const string OnStateExit = "OnStateExit";

    private void Awake()
    {
        // 单例模式初始化
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this.gameObject); // 状态机在场景切换中不销毁
    }
    
    private void Start()
    {
        CurrentGameState = GameState.AR1;
    }
    
    //改变游戏状态
    public void ChangeGameState(GameState newGameState)
    {
        if (newGameState == CurrentGameState)
            return;
        
        EventManager.Instance.Trigger(OnStateExit, CurrentGameState);// 通知监听者：退出旧状态
        
        CurrentGameState = newGameState;
        Debug.Log("Current Game State: " + CurrentGameState);
        
        EventManager.Instance.Trigger(OnStateEnter, CurrentGameState); // 通知监听者：进入新状态
    }
    
}

