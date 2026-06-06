using UnityEngine;

[DisallowMultipleComponent]
public class SecretBossController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private Health playerHealth;
    [SerializeField] private Rigidbody2D bossRigidbody;
    [SerializeField] private SecretBossHealth bossHealth;

    [Header("Boss Settings")]
    [SerializeField] private float moveSpeed = 1.25f;
    [SerializeField] private float attackRange = 1f;
    [SerializeField] private int damage = 1;
    [SerializeField] private float attackCooldown = 1f;

    private float nextAttackTime;

    private void Awake()
    {
        if (bossRigidbody == null)
        {
            bossRigidbody = GetComponent<Rigidbody2D>();
        }

        if (bossHealth == null)
        {
            bossHealth = GetComponent<SecretBossHealth>();
        }

        if (player != null && playerHealth == null)
        {
            playerHealth = player.GetComponentInParent<Health>();
        }
    }

    private void Update()
    {
        if (!CanAct())
        {
            return;
        }

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        if (distanceToPlayer <= attackRange && Time.time >= nextAttackTime)
        {
            AttackPlayer();
        }
    }

    private void FixedUpdate()
    {
        if (!CanAct())
        {
            return;
        }

        Vector2 currentPosition = bossRigidbody != null
            ? bossRigidbody.position
            : (Vector2)transform.position;
        Vector2 targetPosition = player.position;

        if (Vector2.Distance(currentPosition, targetPosition) <= attackRange)
        {
            return;
        }

        Vector2 nextPosition = Vector2.MoveTowards(
            currentPosition,
            targetPosition,
            moveSpeed * Time.fixedDeltaTime);

        if (bossRigidbody != null)
        {
            bossRigidbody.MovePosition(nextPosition);
        }
        else
        {
            transform.position = nextPosition;
        }
    }

    private bool CanAct()
    {
        if (player == null || (bossHealth != null && bossHealth.IsDefeated))
        {
            return false;
        }

        return playerHealth == null || !playerHealth.IsDead;
    }

    private void AttackPlayer()
    {
        nextAttackTime = Time.time + attackCooldown;
        Debug.Log($"[SecretBossController] El jefe ataca al Player por {damage} de dano.");

        if (playerHealth != null)
        {
            playerHealth.TakeDamage(damage);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

    private void OnValidate()
    {
        moveSpeed = Mathf.Max(0f, moveSpeed);
        attackRange = Mathf.Max(0.05f, attackRange);
        damage = Mathf.Max(1, damage);
        attackCooldown = Mathf.Max(0.05f, attackCooldown);
    }
}
