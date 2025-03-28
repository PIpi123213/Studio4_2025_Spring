using UnityEngine;
using UnityEngine.XR.OpenXR;

#if UNITY_ANDROID && !UNITY_EDITOR
public static class BanUserPresent
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void DisablePresenceDetection()
    {
        // 方法1：直接覆盖返回值（最简单）
        OVRPlugin.userPresent = true;

        // 方法2：反射修改MetaXRFeature（更底层）
        var feature = OpenXRSettings.Instance.GetFeature<Meta.XR.MetaXRFeature>();
        if (feature != null)
        {
            var field = typeof(Meta.XR.MetaXRFeature).GetField(
                "requireUserPresence", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance
            );
            field?.SetValue(feature, false);
        }

        Debug.Log("[OVR] 已禁用佩戴状态检测");
    }
}
#endif