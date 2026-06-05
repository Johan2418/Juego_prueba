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
        float horizontal = GetHorizontalInput();
        float vertical = GetVerticalInput();

        Vector2 rawInput = new Vector2(horizontal, vertical);
        moveInput = useCardinalMovement ? ClampToCardinal(rawInput) : rawInput.normalized;

        if (moveInput.sqrMagnitude > 0.01f)
        {
            lastLookDirection = moveInput;
        }
    }

    private static float GetHorizontalInput()
    {
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            return -1f;
        }

        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            return 1f;
        }

        return 0f;
    }

    private static float GetVerticalInput()
    {
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            return -1f;
        }

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            return 1f;
        }

        return 0f;
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

        SetAnimatorBoolIfExists("IsMoving", IsMoving);
        SetAnimatorFloatIfExists("MoveX", moveInput.x);
        SetAnimatorFloatIfExists("MoveY", moveInput.y);
        SetAnimatorFloatIfExists("LastMoveX", lastLookDirection.x);
        SetAnimatorFloatIfExists("LastMoveY", lastLookDirection.y);
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

    private void OnValidate()
    {
        moveSpeed = Mathf.Max(0.1f, moveSpeed);
    }
}
