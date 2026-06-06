using UnityEngine;
using TMPro;
using UnityEngine.UI;

[RequireComponent(typeof(PlayerController))]
public class PlayerInteractor : MonoBehaviour
{
    private const string RuntimeCanvasName = "GameplayRuntimeCanvas";

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

        ResolveOrCreateInteractionUI();
    }

    private void OnEnable()
    {
        ResolveOrCreateInteractionUI();
    }

    private void Update()
    {
        ResolveOrCreateInteractionUI();

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

    private void ResolveOrCreateInteractionUI()
    {
        if (interactionPromptUI == null || !interactionPromptUI.IsConfigured)
        {
            interactionPromptUI = FindFirstObjectByType<InteractionPromptUI>(FindObjectsInactive.Include);
        }

        if (notificationUI == null || !notificationUI.IsConfigured)
        {
            notificationUI = FindFirstObjectByType<NotificationUI>(FindObjectsInactive.Include);
        }

        if (interactionPromptUI != null && interactionPromptUI.IsConfigured &&
            notificationUI != null && notificationUI.IsConfigured)
        {
            return;
        }

        Canvas canvas = FindOrCreateRuntimeCanvas();

        if (interactionPromptUI == null || !interactionPromptUI.IsConfigured)
        {
            interactionPromptUI = CreateInteractionPrompt(canvas.transform);
        }

        if (notificationUI == null || !notificationUI.IsConfigured)
        {
            notificationUI = CreateNotification(canvas.transform);
        }
    }

    private static Canvas FindOrCreateRuntimeCanvas()
    {
        GameObject existingRuntimeCanvas = GameObject.Find(RuntimeCanvasName);
        Canvas runtimeCanvas = existingRuntimeCanvas != null ? existingRuntimeCanvas.GetComponent<Canvas>() : null;
        if (runtimeCanvas != null)
        {
            return runtimeCanvas;
        }

        Canvas screenSpaceCanvas = FindScreenSpaceCanvas();
        if (screenSpaceCanvas != null)
        {
            return screenSpaceCanvas;
        }

        GameObject canvasObject = new GameObject(
            RuntimeCanvasName,
            typeof(Canvas),
            typeof(CanvasScaler),
            typeof(GraphicRaycaster));
        Canvas canvas = canvasObject.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 1000;

        CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1280f, 720f);
        scaler.matchWidthOrHeight = 0.5f;
        return canvas;
    }

    private static Canvas FindScreenSpaceCanvas()
    {
        Canvas[] canvases = FindObjectsByType<Canvas>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        for (int i = 0; i < canvases.Length; i++)
        {
            Canvas canvas = canvases[i];
            if (canvas != null &&
                (canvas.renderMode == RenderMode.ScreenSpaceOverlay || canvas.renderMode == RenderMode.ScreenSpaceCamera))
            {
                return canvas;
            }
        }

        return null;
    }

    private static InteractionPromptUI CreateInteractionPrompt(Transform canvasTransform)
    {
        GameObject owner = new GameObject("InteractionPromptUI", typeof(RectTransform), typeof(InteractionPromptUI));
        owner.transform.SetParent(canvasTransform, false);

        GameObject root = CreateUiPanel(owner.transform, "PromptRoot", new Vector2(460f, 58f), new Vector2(0f, -270f));
        TMP_Text text = CreateUiText(root.transform, "PromptText", "Presiona E para interactuar", 26f);

        InteractionPromptUI prompt = owner.GetComponent<InteractionPromptUI>();
        prompt.Configure(root, text);
        return prompt;
    }

    private static NotificationUI CreateNotification(Transform canvasTransform)
    {
        GameObject owner = new GameObject("NotificationUI", typeof(RectTransform), typeof(NotificationUI));
        owner.transform.SetParent(canvasTransform, false);

        GameObject root = CreateUiPanel(owner.transform, "NotificationRoot", new Vector2(520f, 90f), new Vector2(0f, 280f));
        TMP_Text text = CreateUiText(root.transform, "NotificationText", string.Empty, 26f);

        NotificationUI notification = owner.GetComponent<NotificationUI>();
        notification.Configure(root, text);
        return notification;
    }

    private static GameObject CreateUiPanel(Transform parent, string objectName, Vector2 size, Vector2 position)
    {
        GameObject panel = new GameObject(objectName, typeof(RectTransform), typeof(Image));
        panel.transform.SetParent(parent, false);

        RectTransform rect = panel.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = size;
        rect.anchoredPosition = position;

        Image image = panel.GetComponent<Image>();
        image.color = new Color(0f, 0f, 0f, 0.72f);
        image.raycastTarget = false;
        return panel;
    }

    private static TMP_Text CreateUiText(Transform parent, string objectName, string value, float fontSize)
    {
        GameObject textObject = new GameObject(objectName, typeof(RectTransform), typeof(TextMeshProUGUI));
        textObject.transform.SetParent(parent, false);

        RectTransform rect = textObject.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = new Vector2(16f, 8f);
        rect.offsetMax = new Vector2(-16f, -8f);

        TextMeshProUGUI text = textObject.GetComponent<TextMeshProUGUI>();
        text.text = value;
        text.fontSize = fontSize;
        text.alignment = TextAlignmentOptions.Center;
        text.color = Color.white;
        text.raycastTarget = false;
        return text;
    }
}
