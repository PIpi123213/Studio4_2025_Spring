using UnityEngine;
using UnityEngine.XR.OpenXR;

#if UNITY_ANDROID && !UNITY_EDITOR
public static class BanUserPresent
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void DisablePresenceDetection()
    {
        // ����1��ֱ�Ӹ��Ƿ���ֵ����򵥣�
        OVRPlugin.userPresent = true;

        // ����2�������޸�MetaXRFeature�����ײ㣩
        var feature = OpenXRSettings.Instance.GetFeature<Meta.XR.MetaXRFeature>();
        if (feature != null)
        {
            var field = typeof(Meta.XR.MetaXRFeature).GetField(
                "requireUserPresence", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance
            );
            field?.SetValue(feature, false);
        }

        Debug.Log("[OVR] �ѽ������״̬���");
    }
}
#endif