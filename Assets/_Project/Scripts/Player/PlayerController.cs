using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private bool useCardinalMovement = true;
    [SerializeField] private Animator animator;

    private Vector2 moveInput;
    private Vector2 lastLookDirection = Vector2.down;
    private bool movementBlocked;

    public Vector2 LastLookDirection => lastLookDirection;
    public Vector2 CurrentMoveInput => moveInput;
    public bool IsMoving => moveInput.sqrMagnitude > 0.01f;
    public bool IsMovementBlocked => movementBlocked;

    private void Awake()
    {
        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
        }
    }

    private void Update()
    {
        if (movementBlocked)
        {
            moveInput = Vector2.zero;
            return;
        }

        ReadMovementInput();
        UpdateAnimator();
    }

    private void FixedUpdate()
    {
        if (rb == null || movementBlocked)
        {
            return;
        }

        rb.MovePosition(rb.position + (moveInput * moveSpeed * Time.fixedDeltaTime));
    }

    private void ReadMovementInput()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector2 rawInput = new Vector2(horizontal, vertical);
        moveInput = useCardinalMovement ? ClampToCardinal(rawInput) : rawInput.normalized;

        if (moveInput.sqrMagnitude > 0.01f)
        {
            lastLookDirection = moveInput;
        }
    }

    // Cozy movement starts with cardinal directions to keep interactions predictable.
    private static Vector2 ClampToCardinal(Vector2 input)
    {
        if (Mathf.Abs(input.x) > Mathf.Abs(input.y))
        {
            return new Vector2(Mathf.Sign(input.x), 0f);
        }

        if (Mathf.Abs(input.y) > 0f)
        {
            return new Vector2(0f, Mathf.Sign(input.y));
        }

        return Vector2.zero;
    }

    public void SetMovementBlocked(bool blocked)
    {
        movementBlocked = blocked;
        if (movementBlocked)
        {
            moveInput = Vector2.zero;
            UpdateAnimator();
        }
    }

    private void UpdateAnimator()
    {
        if (animator == null)
        {
            return;
        }

        animator.SetBool("IsMoving", IsMoving);
        animator.SetFloat("MoveX", moveInput.x);
        animator.SetFloat("MoveY", moveInput.y);
        animator.SetFloat("LastMoveX", lastLookDirection.x);
        animator.SetFloat("LastMoveY", lastLookDirection.y);
    }

    private void OnValidate()
    {
        moveSpeed = Mathf.Max(0.1f, moveSpeed);
    }
}
