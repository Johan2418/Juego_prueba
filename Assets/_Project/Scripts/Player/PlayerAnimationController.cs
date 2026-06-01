using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class PlayerAnimationController : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private Animator animator;

    private void Awake()
    {
        if (playerController == null)
        {
            playerController = GetComponent<PlayerController>();
        }
    }

    private void Update()
    {
        if (animator == null || playerController == null)
        {
            return;
        }

        Vector2 moveInput = playerController.CurrentMoveInput;
        Vector2 lookDirection = playerController.LastLookDirection;

        animator.SetBool("IsMoving", playerController.IsMoving);
        animator.SetFloat("MoveX", moveInput.x);
        animator.SetFloat("MoveY", moveInput.y);
        animator.SetFloat("LastMoveX", lookDirection.x);
        animator.SetFloat("LastMoveY", lookDirection.y);
    }
}
