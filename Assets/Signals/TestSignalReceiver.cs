using UnityEngine;
/// <summary>
/// 用来接收并处理信号的脚本
/// </summary>
public class TestSignalReceiver : MonoBehaviour
{
    //处理信号的回调函数
    public void OnSignalReceive(string signalName)
    {
        Debug.LogFormat("Signal Receive! {0}", signalName);
    }
} 