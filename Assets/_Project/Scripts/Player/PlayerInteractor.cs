using UnityEngine;
using TMPro;
using UnityEngine.UI;

[RequireComponent(typeof(PlayerController))]
public class PlayerInteractor : MonoBehaviour
{
    private const string InspectPrompt = "Presione el boton E para inspeccionar";
    private const string MantaChairDescription =
        "La silla manteña (o silla en \"U\") es un ícono ceremonial y arqueológico de la Cultura Manteña. " +
        "Símbolo de identidad y poder de Manabí, representa la jerarquía política, religiosa y chamánica " +
        "de sus antiguos habitantes.";
    private const string TunaMonumentDescription =
        "El Monumento al Atún en Manta simboliza la enorme importancia de la pesca para el desarrollo económico, " +
        "social e histórico de la ciudad. Rinde homenaje a los pescadores, a la industria y al orgullo de Manta " +
        "por ser conocida como la \"Capital Mundial del Atún\".";

    [Header("Interaction")]
    [SerializeField] private KeyCode interactionKey = KeyCode.E;
    [SerializeField] private float interactionRadius = 0.65f;
    [SerializeField] private float forwardOffset = 0.5f;
    [SerializeField] private LayerMask interactionLayers = ~0;

    [Header("References")]
    [SerializeField] private PlayerController playerController;
    [SerializeField] private InteractionPromptUI interactionPromptUI;
    [SerializeField] private NotificationUI notificationUI;

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
        EnsureCulturalInspectable("silla manteña_0", MantaChairDescription);
        EnsureCulturalInspectable("atun_43", TunaMonumentDescription);
    }

    private void OnEnable()
    {
        ResolveOrCreateInteractionUI();
    }

    private void Update()
    {
        if (playerController == null || interactionLocked)
        {
            SetCurrentInteractable(null);
            interactionPromptUI?.HidePrompt();
            return;
        }

        SetCurrentInteractable(FindClosestInteractable());
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

    public void ShowPersistentNotification(string message)
    {
        notificationUI?.ShowPersistentMessage(message);
    }

    public void HideNotification()
    {
        notificationUI?.HideInstant();
    }

    private IInteractable FindClosestInteractable()
    {
        Vector2 center = transform.position;
        Vector2 lookDirection = playerController.LastLookDirection;
        if (lookDirection.sqrMagnitude > 0.01f)
        {
            center += lookDirection.normalized * forwardOffset;
        }

        Collider2D[] overlapResults = Physics2D.OverlapCircleAll(center, interactionRadius, interactionLayers);
        IInteractable closest = null;
        float closestDistance = float.MaxValue;

        for (int i = 0; i < overlapResults.Length; i++)
        {
            Collider2D hit = overlapResults[i];
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

    private void SetCurrentInteractable(IInteractable nextInteractable)
    {
        if (ReferenceEquals(currentInteractable, nextInteractable))
        {
            return;
        }

        if (currentInteractable is IInteractionFocusListener previousListener)
        {
            previousListener.OnInteractionFocusExited(this);
        }

        currentInteractable = nextInteractable;

        if (currentInteractable is IInteractionFocusListener nextListener)
        {
            nextListener.OnInteractionFocusEntered(this);
        }
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
        if (interactionPromptUI == null)
        {
            interactionPromptUI = FindFirstObjectByType<InteractionPromptUI>(FindObjectsInactive.Include);
        }

        if (notificationUI == null)
        {
            notificationUI = FindFirstObjectByType<NotificationUI>(FindObjectsInactive.Include);
        }

        if (interactionPromptUI != null && notificationUI != null)
        {
            return;
        }

        Canvas canvas = FindScreenSpaceCanvas();
        if (canvas == null)
        {
            GameObject canvasObject = new GameObject(
                "InteractionRuntimeCanvas",
                typeof(Canvas),
                typeof(CanvasScaler),
                typeof(GraphicRaycaster));
            canvas = canvasObject.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 1000;

            CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1280f, 720f);
            scaler.matchWidthOrHeight = 0.5f;
        }

        if (interactionPromptUI == null)
        {
            interactionPromptUI = CreateInteractionPrompt(canvas.transform);
        }

        if (notificationUI == null)
        {
            notificationUI = CreateNotification(canvas.transform);
        }
    }

    private static void EnsureCulturalInspectable(string objectName, string description)
    {
        GameObject target = GameObject.Find(objectName);
        if (target == null)
        {
            Debug.LogWarning($"[PlayerInteractor] No se encontró el objeto cultural '{objectName}'.");
            return;
        }

        CulturalInspectable inspectable = target.GetComponent<CulturalInspectable>();
        if (inspectable == null)
        {
            inspectable = target.AddComponent<CulturalInspectable>();
        }

        inspectable.Configure(InspectPrompt, description);
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

        GameObject root = CreateUiPanel(owner.transform, "NotificationRoot", new Vector2(760f, 190f), new Vector2(0f, -170f));
        TMP_Text text = CreateUiText(root.transform, "NotificationText", string.Empty, 24f);

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
