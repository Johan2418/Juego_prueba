using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class SecretBossPlayerAttack : MonoBehaviour
{
    [Header("Attack")]
    [SerializeField] private KeyCode attackKey = KeyCode.Space;
    [SerializeField] private int damage = 1;
    [SerializeField] private float attackRange = 1f;
    [SerializeField] private float attackCooldown = 0.25f;
    [SerializeField] private LayerMask enemyLayers = ~0;

    [Header("Optional Reference")]
    [SerializeField] private Transform attackOrigin;

    private readonly HashSet<SecretBossHealth> damagedBosses = new HashSet<SecretBossHealth>();
    private float nextAttackTime;

    private void Update()
    {
        if (Input.GetKeyDown(attackKey) && Time.time >= nextAttackTime)
        {
            Attack();
        }
    }

    private void Attack()
    {
        nextAttackTime = Time.time + attackCooldown;
        Vector2 center = attackOrigin != null ? attackOrigin.position : transform.position;
        Collider2D[] hits = Physics2D.OverlapCircleAll(center, attackRange, enemyLayers);

        damagedBosses.Clear();
        for (int i = 0; i < hits.Length; i++)
        {
            SecretBossHealth bossHealth = hits[i].GetComponentInParent<SecretBossHealth>();
            if (bossHealth == null || damagedBosses.Contains(bossHealth))
            {
                continue;
            }

            damagedBosses.Add(bossHealth);
            bossHealth.TakeDamage(damage);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 center = attackOrigin != null ? attackOrigin.position : transform.position;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(center, attackRange);
    }

    private void OnValidate()
    {
        damage = Mathf.Max(1, damage);
        attackRange = Mathf.Max(0.05f, attackRange);
        attackCooldown = Mathf.Max(0.05f, attackCooldown);
    }
}
