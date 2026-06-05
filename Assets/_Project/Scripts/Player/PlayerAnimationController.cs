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

        SetAnimatorBoolIfExists("IsMoving", playerController.IsMoving);
        SetAnimatorFloatIfExists("MoveX", moveInput.x);
        SetAnimatorFloatIfExists("MoveY", moveInput.y);
        SetAnimatorFloatIfExists("LastMoveX", lookDirection.x);
        SetAnimatorFloatIfExists("LastMoveY", lookDirection.y);
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
}
