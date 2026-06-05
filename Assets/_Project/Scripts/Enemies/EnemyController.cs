using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform target;
    [SerializeField] private Health targetHealth;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator animator;
    [SerializeField] private Health ownHealth;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float detectionRange = 4f;
    [SerializeField] private float stopDistance = 0.7f;

    [Header("Contact Damage")]
    [SerializeField] private int contactDamage = 1;
    [SerializeField] private float contactCooldown = 0.6f;

    private Vector2 moveDirection;
    private Vector2 lastMoveDirection = Vector2.down;
    private float nextDamageTime;

    private void Awake()
    {
        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
        }

        if (ownHealth == null)
        {
            ownHealth = GetComponent<Health>();
        }

        if (target != null && targetHealth == null)
        {
            targetHealth = target.GetComponent<Health>();
        }
    }

    private void Update()
    {
        if (!CanMove())
        {
            moveDirection = Vector2.zero;
            UpdateAnimator();
            return;
        }

        Vector2 toTarget = target.position - transform.position;
        float distance = toTarget.magnitude;

        if (distance <= detectionRange && distance > stopDistance)
        {
            moveDirection = toTarget.normalized;
            lastMoveDirection = moveDirection;
        }
        else
        {
            moveDirection = Vector2.zero;
        }

        UpdateAnimator();
    }

    private void FixedUpdate()
    {
        if (rb == null || (ownHealth != null && ownHealth.IsDead))
        {
            return;
        }

        rb.MovePosition(rb.position + (moveDirection * moveSpeed * Time.fixedDeltaTime));
    }

    private bool CanMove()
    {
        if (target == null)
        {
            return false;
        }

        if (ownHealth != null && ownHealth.IsDead)
        {
            return false;
        }

        if (targetHealth != null && targetHealth.IsDead)
        {
            return false;
        }

        return true;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        TryDealContactDamage(collision.gameObject);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        TryDealContactDamage(other.gameObject);
    }

    private void TryDealContactDamage(GameObject other)
    {
        if (targetHealth == null || targetHealth.IsDead || Time.time < nextDamageTime)
        {
            return;
        }

        if (other.transform.root != targetHealth.transform.root)
        {
            return;
        }

        nextDamageTime = Time.time + contactCooldown;
        targetHealth.TakeDamage(contactDamage);
    }

    private void UpdateAnimator()
    {
        if (animator == null)
        {
            return;
        }

        bool isMoving = moveDirection.sqrMagnitude > 0.01f;
        SetAnimatorBoolIfExists("IsMoving", isMoving);
        SetAnimatorFloatIfExists("MoveX", moveDirection.x);
        SetAnimatorFloatIfExists("MoveY", moveDirection.y);
        SetAnimatorFloatIfExists("LastMoveX", lastMoveDirection.x);
        SetAnimatorFloatIfExists("LastMoveY", lastMoveDirection.y);
    }

    private void SetAnimatorBoolIfExists(string parameterName, bool value)
    {
        if (HasAnimatorParameter(parameterName, AnimatorControllerParameterType.Bool))
        {
            animator.SetBool(parameterName, value);
        }
    }

    private void SetAnimatorFloatIfExists(string parameterName, float value)
    {
        if (HasAnimatorParameter(parameterName, AnimatorControllerParameterType.Float))
        {
            animator.SetFloat(parameterName, value);
        }
    }

    private bool HasAnimatorParameter(string parameterName, AnimatorControllerParameterType parameterType)
    {
        foreach (AnimatorControllerParameter parameter in animator.parameters)
        {
            if (parameter.type == parameterType && parameter.name == parameterName)
            {
                return true;
            }
        }

        return false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, stopDistance);
    }

    private void OnValidate()
    {
        moveSpeed = Mathf.Max(0.1f, moveSpeed);
        detectionRange = Mathf.Max(0.1f, detectionRange);
        stopDistance = Mathf.Max(0f, stopDistance);
        contactDamage = Mathf.Max(1, contactDamage);
        contactCooldown = Mathf.Max(0.05f, contactCooldown);
    }
}
