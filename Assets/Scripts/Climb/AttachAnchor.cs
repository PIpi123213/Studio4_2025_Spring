using UnityEngine;
using Obi;

public class AttachAnchor : MonoBehaviour
{
    public ObiParticleAttachment ball;
    public ObiParticleAttachment ball2;

    private ObiActor actor;      // ��ǰ����/���϶�Ӧ�� ObiActor
    private ObiSolver solver;    // �� actor ʹ�õ� Solver

    private bool hasAttached = false;
    void Start()
    {
        // �ӵ�һ�� attachment ��ȡ actor �� solver
        actor = ball.GetComponentInParent<ObiActor>();
        solver = actor.solver;

        // ��ʼ�󶨵�������ʡ�ԣ��༭����Ҳ��ֱ���趨��
        ball.target = ball.transform;
        ball2.target = ball2.transform;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasAttached)
            return;           // ����Ѿ��󶨹�һ�Σ���ֱ�ӷ���

        if (!other.CompareTag("Player"))
            return;

        Debug.Log("��һ�ν���ê�㣬���а󶨲�ͬ���˵�λ��");

        SnapAndAttach(ball, other.transform);
        SnapAndAttach(ball2, other.transform);

        hasAttached = true;   // �������ɵ�һ�ΰ�
    }

    /// <summary>
    /// �ȡ��⿪��attachment������Ӧ����˲�Ƶ� targetPos��
    /// �ٰ� attachment ����ָ�� target �����á�
    /// </summary>
    void SnapAndAttach(ObiParticleAttachment attachment, Transform newTarget)
    {
        // 1. ȡ����� attachment Ӱ���������
        var group = attachment.particleGroup;
        if (group == null || group.particleIndices.Count == 0) return;

        // 2. �Ƚ��� attachment ����� target�������ذ�ʱ�Զ������ӡ�������ԭλ
        attachment.enabled = false;
        attachment.target = null;

        // 3. �ҵ���һ�������� solver �е�����
        int blueprintIndex = group.particleIndices[0];              // ������ blueprint �е�����
        int solverIndex = actor.solverIndices[blueprintIndex];  // ��Ӧ�� solver.positions ���������

        // 4. �Ѹ����ӵ�λ��˲�Ƶ� newTarget ��λ�ã��������� �� solver �������꣩
        Vector3 worldPos = newTarget.position;
        Vector3 localPos = solver.transform.InverseTransformPoint(worldPos);
        solver.positions[solverIndex] = localPos;
        // ��ѡ�������ٶȣ���ֹ˲�ƺ���ֶ���
        solver.velocities[solverIndex] = Vector3.zero;

        // 5. ���°󶨵���Ŀ�겢���� attachment
        attachment.target = newTarget;
        attachment.enabled = true;
    }
}
