using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[DisallowMultipleComponent]
public class SecretBossPlayerAttack : MonoBehaviour
{
    [Header("Attack")]
    [SerializeField] private KeyCode attackKey = KeyCode.Space;
    [FormerlySerializedAs("damage")]
    [SerializeField] private int attackDamage = 1;
    [SerializeField] private float attackRange = 1f;
    [SerializeField] private float attackCooldown = 0.25f;
    [FormerlySerializedAs("enemyLayers")]
    [SerializeField] private LayerMask enemyLayer = ~0;

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
        Collider2D[] hits = Physics2D.OverlapCircleAll(center, attackRange, enemyLayer);
        Debug.Log("[SecretBossPlayerAttack] El Player ataca con Space.");

        damagedBosses.Clear();
        for (int i = 0; i < hits.Length; i++)
        {
            SecretBossHealth bossHealth = hits[i].GetComponentInParent<SecretBossHealth>();
            if (bossHealth == null || damagedBosses.Contains(bossHealth))
            {
                continue;
            }

            damagedBosses.Add(bossHealth);
            bossHealth.TakeDamage(attackDamage);
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
        attackDamage = Mathf.Max(1, attackDamage);
        attackRange = Mathf.Max(0.05f, attackRange);
        attackCooldown = Mathf.Max(0.05f, attackCooldown);
    }
}
