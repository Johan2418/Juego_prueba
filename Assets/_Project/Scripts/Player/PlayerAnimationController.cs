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

        // Play the appropriate state directly (Base Layer.<StateName>) to support
        // directional animations without requiring complex transitions in the
        // Animator Controller.
        string state = playerController.IsMoving ? "WalkDown" : "IdleDown";
        if (Mathf.Abs(lookDirection.y) > 0.5f)
        {
            state = playerController.IsMoving ? "WalkUp" : "IdleUp";
        }
        else if (Mathf.Abs(lookDirection.x) > 0.5f)
        {
            state = playerController.IsMoving ? "WalkRight" : "IdleRight";
            if (lookDirection.x < 0)
            {
                state = playerController.IsMoving ? "WalkLeft" : "IdleLeft";
            }
        }

        string fullState = "Base Layer." + state;

        // Debug: log parameters when state changes or when moving
        bool shouldLog = playerController.IsMoving || Mathf.Abs(moveInput.x) > 0.001f || Mathf.Abs(moveInput.y) > 0.001f;
        if (shouldLog)
        {
            Debug.Log($"[Anim] Play request: {fullState} | IsMoving={playerController.IsMoving} Move=({moveInput.x:F1},{moveInput.y:F1}) LastLook=({lookDirection.x:F1},{lookDirection.y:F1})");
        }

        // Try to play the state. If it doesn't exist, the log above will help debug.
        animator.Play(fullState);
    }
}
