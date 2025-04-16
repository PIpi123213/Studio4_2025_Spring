using UnityEngine;
using Obi;
using System.Net.Mail;
using System.Collections;
public class AttachAnchor : MonoBehaviour
{
    public ObiParticleAttachment ball;
    public ObiParticleAttachment ball2;
    public ObiRope obiRope;
    public GameObject Rope;
    private ObiActor actor;      // 当前绳子/布料对应的 ObiActor
    private ObiSolver solver;    // 该 actor 使用的 Solver

    public bool isFirst = false;
    public GameObject playerAnchor;

    public bool hasAttached = false;
   
    void Start()
    {
        // 从第一个 attachment 获取 actor 和 solver
        actor = ball.GetComponentInParent<ObiActor>();
        solver = actor.solver;
        if (!isFirst)
        {
            playerAnchor = null;
        }
        // 初始绑定到自身（可省略，编辑器里也能直接设定）
        ball.target = ball.transform;
        ball2.target = ball2.transform;
        StartCoroutine(InitializeRope());
        //Rope.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        // 如果已经绑定过一次，就直接返回
        if (hasAttached || isFirst)
        {

            return;
        }
                    

        if (!other.CompareTag("Player"))
            return;

        Rope.SetActive(true);
        obiRope.enabled = true;
        if (AnchorPoint.attachAnchor != null)
        {
            AnchorPoint.attachAnchor.ResetAnchorPoint();

           

        }

        Debug.Log("第一次进入锚点，进行绑定并同步端点位置");
        SnapAndAttach(ball, other.transform);
        SnapAndAttach(ball2, other.transform);
        AnchorPoint.attachAnchor = this;
        hasAttached = true;   // 标记已完成第一次绑定



    }


    private void OnTriggerExit(Collider other)
    {
           

        if (!other.CompareTag("Player"))
            return;

        Debug.Log("离开锚点，进行绑定并同步端点位置");


        //obiRope.enabled = false; 
        //Rope.SetActive(false);
        //hasAttached = false;   // 标记已完成第一次绑定
    }

    public void ResetAnchorPoint()
    {
       
        //obiRope.enabled = false;
        //SnapAndAttach(ball, ball.transform);
        //SnapAndAttach(ball2, ball2.transform);
        ball.target = ball.transform;
        ball2.target = ball2.transform;
        ResetRope(obiRope);
        
        //Rope.SetActive(false);
        hasAttached = false;
    }
    /// <summary>
    /// 先“解开”attachment，将对应粒子瞬移到 targetPos，
    /// 再把 attachment 重新指向 target 并启用。
    /// </summary>
    public void SnapAndAttach(ObiParticleAttachment attachment, Transform newTarget)
    {
        // 1. 取出这个 attachment 影响的粒子组
        var group = attachment.particleGroup;
        if (group == null || group.particleIndices.Count == 0) return;

        // 2. 先禁用 attachment 并清空 target，避免重绑定时自动把粒子“留”在原位
        attachment.enabled = false;
        attachment.target = null;

        // 3. 找到第一个粒子在 solver 中的索引
        int blueprintIndex = group.particleIndices[0];              // 粒子在 blueprint 中的索引
        int solverIndex = actor.solverIndices[blueprintIndex];  // 对应到 solver.positions 数组的索引

        // 4. 把该粒子的位置瞬移到 newTarget 的位置（世界坐标 → solver 本地坐标）
        Vector3 worldPos = newTarget.position;
        Vector3 localPos = solver.transform.InverseTransformPoint(worldPos);
        solver.positions[solverIndex] = localPos;
        // 可选：清零速度，防止瞬移后出现抖动
        solver.velocities[solverIndex] = Vector3.zero;

        // 5. 重新绑定到新目标并启用 attachment
        attachment.target = newTarget;
        attachment.enabled = true;
    }
    public void Attach()
    {
        // 1. 取出这个 attachment 影响的粒子组
        if (playerAnchor!=null)
        {
            Rope.SetActive(true);
            obiRope.enabled = true;
            SnapAndAttach(ball, playerAnchor.transform);
            SnapAndAttach(ball2, playerAnchor.transform);
            isFirst = false;
            AnchorPoint.attachAnchor = this;
            hasAttached = true;
        }
    }




    IEnumerator InitializeRope()
    {
        // Start 中的初始化逻辑
        for (int i = 0; i < 10; i++)
        {
            yield return null; // 等待一帧
        }// 等待一帧

        obiRope.enabled = false;
        // 在下一帧执行的逻辑
    }
    void ResetRope(ObiRope rope)
    {
        if (rope.isLoaded)
        {
            Matrix4x4 l2sTransform = rope.actorLocalToSolverMatrix;

            for (int i = 0; i < rope.particleCount; ++i)
            {
                int solverIndex = rope.solverIndices[i];

                rope.solver.positions[solverIndex] = l2sTransform.MultiplyPoint3x4(rope.blueprint.positions[i]);
                rope.solver.velocities[solverIndex] = Vector3.zero;
            }
        }
    }


}
