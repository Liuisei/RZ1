using Unity.Netcode;
using UnityEngine;
using System.Collections;

public class SwordItem : ItemBase
{
    [SerializeField] private float attackRadius = 2f;
    [SerializeField] private float attackAngle = 180f;
    [SerializeField] private int attackDamage = 20;
    [SerializeField] private float cooldownTime = 1.0f;
    [SerializeField] private LayerMask targetLayer;
    [SerializeField] private Animator animator;

    private bool isCoolingDown = false;

    public override void Use()
    {
        if (isCoolingDown)
        {
            Debug.Log("クールダウン中");
            return;
        }

        StartCoroutine(Attack());
    }

    private IEnumerator Attack()
    {
        isCoolingDown = true;

        PlayAttackAnimationServerRpc();

        Collider[] hits = Physics.OverlapSphere(transform.position, attackRadius, targetLayer);
        Vector3 forward = transform.forward;

        foreach (Collider hit in hits)
        {
            Vector3 toTarget = (hit.transform.position - transform.position).normalized;
            float angle = Vector3.Angle(forward, toTarget);

            if (angle <= attackAngle / 2f)
            {
                if (hit.TryGetComponent(out HealthSystem health))
                {
                    health.TakeDamageServerRpc(attackDamage);
                }
            }
        }

        yield return new WaitForSeconds(cooldownTime);
        isCoolingDown = false;
    }

    [ServerRpc]
    private void PlayAttackAnimationServerRpc()
    {
        animator.SetTrigger("Attack");
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }
}