using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestoryPlayer : MonoBehaviour
{
    // Start is called before the first frame update

    public static DontDestoryPlayer Instance { get; private set; }

    private void Awake()
    {
        // 双重判空保险
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);  // 销毁新实例
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);  // 修正参数为 gameObject
    }

}
