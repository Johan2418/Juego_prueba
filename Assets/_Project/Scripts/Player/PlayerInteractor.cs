using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class PlayerInteractor : MonoBehaviour
{
    [Header("Interaction")]
    [SerializeField] private KeyCode interactionKey = KeyCode.E;
    [SerializeField] private float interactionRadius = 0.65f;
    [SerializeField] private float forwardOffset = 0.5f;
    [SerializeField] private LayerMask interactionLayers = ~0;

    [Header("References")]
    [SerializeField] private PlayerController playerController;
    [SerializeField] private InteractionPromptUI interactionPromptUI;
    [SerializeField] private NotificationUI notificationUI;

    private readonly Collider2D[] overlapResults = new Collider2D[12];
    private IInteractable currentInteractable;
    private bool interactionLocked;

    public PlayerController PlayerController => playerController;
    public NotificationUI NotificationUI => notificationUI;
    public bool IsInteractionLocked => interactionLocked;

    private void Awake()
    {
        if (playerController == null)
        {
            playerController = GetComponent<PlayerController>();
        }
    }

    private void Update()
    {
        if (playerController == null || interactionLocked)
        {
            currentInteractable = null;
            interactionPromptUI?.HidePrompt();
            return;
        }

        currentInteractable = FindClosestInteractable();
        UpdatePrompt();

        if (currentInteractable != null && Input.GetKeyDown(interactionKey))
        {
            Debug.Log($"[PlayerInteractor] Interacting with {currentInteractable.GetType().Name} using key {interactionKey}");
            currentInteractable.Interact(this);
        }
    }

    public void SetInteractionLocked(bool locked)
    {
        interactionLocked = locked;

        if (playerController != null)
        {
            playerController.SetMovementBlocked(locked);
        }

        if (locked)
        {
            interactionPromptUI?.HidePrompt();
        }
    }

    public void ShowNotification(string message, float duration = -1f)
    {
        notificationUI?.ShowMessage(message, duration);
    }

    private IInteractable FindClosestInteractable()
    {
        Vector2 center = transform.position;
        Vector2 lookDirection = playerController.LastLookDirection;
        if (lookDirection.sqrMagnitude > 0.01f)
        {
            center += lookDirection.normalized * forwardOffset;
        }

        int hitCount = Physics2D.OverlapCircleNonAlloc(center, interactionRadius, overlapResults, interactionLayers);
        if (hitCount > 0)
        {
            Debug.Log($"[PlayerInteractor] Detected {hitCount} potential interactables at {center}");
        }
        IInteractable closest = null;
        float closestDistance = float.MaxValue;

        for (int i = 0; i < hitCount; i++)
        {
            Collider2D hit = overlapResults[i];
            overlapResults[i] = null;
            if (hit == null)
            {
                continue;
            }

            IInteractable interactable = hit.GetComponent(typeof(IInteractable)) as IInteractable;
            if (interactable == null)
            {
                interactable = hit.GetComponentInParent(typeof(IInteractable)) as IInteractable;
            }

            if (interactable == null)
            {
                continue;
            }

            float sqrDistance = ((Vector2)hit.transform.position - (Vector2)transform.position).sqrMagnitude;
            if (sqrDistance < closestDistance)
            {
                closestDistance = sqrDistance;
                closest = interactable;
            }
        }

        return closest;
    }

    private void UpdatePrompt()
    {
        if (interactionPromptUI == null)
        {
            return;
        }

        if (currentInteractable == null)
        {
            interactionPromptUI.HidePrompt();
            return;
        }

        string prompt = currentInteractable.GetInteractionPrompt();
        if (string.IsNullOrWhiteSpace(prompt))
        {
            interactionPromptUI.HidePrompt();
            return;
        }

        interactionPromptUI.ShowPrompt(prompt);
    }

    private void OnDrawGizmosSelected()
    {
        if (playerController == null)
        {
            playerController = GetComponent<PlayerController>();
        }

        Vector2 center = transform.position;
        if (playerController != null)
        {
            Vector2 lookDirection = playerController.LastLookDirection;
            if (lookDirection.sqrMagnitude > 0.01f)
            {
                center += lookDirection.normalized * forwardOffset;
            }
        }

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(center, interactionRadius);
    }

    private void OnValidate()
    {
        interactionRadius = Mathf.Max(0.05f, interactionRadius);
        forwardOffset = Mathf.Max(0f, forwardOffset);
    }
}
