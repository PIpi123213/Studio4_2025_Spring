using UnityEngine;
using Obi;

[RequireComponent(typeof(ObiRopeExtrudedRenderer))]
public class RopeUVFixer : MonoBehaviour
{
    // U 轴每圈平铺几次
    public float uTiles = 4;
    // V 轴每单位长度平铺几次（关闭 NormalizeV 时生效）
    public float vTilesPerUnit = 1;

    private ObiRopeExtrudedRenderer rend;

    void Awake()
    {
        rend = GetComponent<ObiRopeExtrudedRenderer>();

        // 关闭 NormalizeV，这样 V 轴就会根据实际长度平铺
        rend.normalizeV = false;

        // 设置 UVScale：
        //   x = uTiles（U 轴重复次数）
        //   y = vTilesPerUnit（V 轴每单位长度重复次数）
        rend.uvScale = new Vector2(uTiles, vTilesPerUnit);
    }

    // 如果你的绳子长度会动态变化，且你想要保持 V 轴固定次数，可以在绑定后或每帧调用：
    void Update()
    {
        // float length = rend.ropeActor.Length;
        // rend.UVScale = new Vector2(uTiles, vTilesPerUnit * length);
    }
}
