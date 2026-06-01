using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class PlayerAttack : MonoBehaviour
{
    [Header("Attack")]
    [SerializeField] private int damage = 1;
    [SerializeField] private float attackDistance = 0.6f;
    [SerializeField] private float attackRadius = 0.45f;
    [SerializeField] private float attackCooldown = 0.25f;
    [SerializeField] private LayerMask damageableLayers = ~0;

    [Header("References")]
    [SerializeField] private PlayerController playerController;
    [SerializeField] private Animator animator;

    private readonly HashSet<Health> damagedTargets = new HashSet<Health>();
    private float nextAttackTime;

    private void Awake()
    {
        if (playerController == null)
        {
            playerController = GetComponent<PlayerController>();
        }
    }

    private void Update()
    {
        if (Input.GetButtonDown("Fire1") || Input.GetKeyDown(KeyCode.Space))
        {
            TryAttack();
        }
    }

    private void TryAttack()
    {
        if (Time.time < nextAttackTime)
        {
            return;
        }

        nextAttackTime = Time.time + attackCooldown;

        if (animator != null)
        {
            animator.SetTrigger("Attack");
        }

        Vector2 attackCenter = (Vector2)transform.position + (GetAttackDirection() * attackDistance);
        Collider2D[] hits = Physics2D.OverlapCircleAll(attackCenter, attackRadius, damageableLayers);

        damagedTargets.Clear();
        for (int i = 0; i < hits.Length; i++)
        {
            Health targetHealth = hits[i].GetComponentInParent<Health>();
            if (targetHealth == null || targetHealth.gameObject == gameObject || damagedTargets.Contains(targetHealth))
            {
                continue;
            }

            damagedTargets.Add(targetHealth);
            targetHealth.TakeDamage(damage);
        }
    }

    private Vector2 GetAttackDirection()
    {
        if (playerController == null)
        {
            return Vector2.down;
        }

        Vector2 direction = playerController.LastLookDirection;
        return direction.sqrMagnitude > 0.01f ? direction.normalized : Vector2.down;
    }

    private void OnDrawGizmosSelected()
    {
        Vector2 direction = playerController != null ? playerController.LastLookDirection : Vector2.down;
        if (direction.sqrMagnitude <= 0.01f)
        {
            direction = Vector2.down;
        }

        Vector2 attackCenter = (Vector2)transform.position + (direction.normalized * attackDistance);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(attackCenter, attackRadius);
    }

    private void OnValidate()
    {
        damage = Mathf.Max(1, damage);
        attackDistance = Mathf.Max(0f, attackDistance);
        attackRadius = Mathf.Max(0.05f, attackRadius);
        attackCooldown = Mathf.Max(0.05f, attackCooldown);
    }
}
