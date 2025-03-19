using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class EventManager
{
    public static event Action GotoNextPhase;
    public static void OnGotoNextPhase()
    {
        GotoNextPhase?.Invoke();
    }
}