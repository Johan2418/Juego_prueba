using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DemoQuestRouteManager : MonoBehaviour
{
    private const string DemoSceneName = "Map_Manta_Prototype";

    [Header("Objective UI")]
    [SerializeField] private TMP_Text currentObjectiveText;
    [TextArea(1, 3)]
    [SerializeField] private string initialObjective = "Habla con el pescador en el malecón.";
    [SerializeField] private bool showInitialObjectiveNotification = true;

    [Header("Temporary Messages")]
    [SerializeField] private NotificationUI notificationUI;
    [SerializeField] private float defaultMessageDuration = 3f;
    [SerializeField] private bool logMessagesToConsole = true;

    [Header("Secret Zone Unlock (Optional)")]
    [SerializeField] private GameObject[] activateWhenSecretUnlocked;
    [SerializeField] private GameObject[] deactivateWhenSecretUnlocked;

    [Header("Route State (Debug)")]
    [SerializeField] private bool hasFish;
    [SerializeField] private bool hasSpondylus;
    [SerializeField] private bool knowsSecret;
    [SerializeField] private bool secretDiscovered;
    [SerializeField] private bool demoFinished;
    [TextArea(1, 3)]
    [SerializeField] private string currentObjective;

    public static DemoQuestRouteManager Instance { get; private set; }

    public bool HasFish => hasFish;
    public bool HasSpondylus => hasSpondylus;
    public bool KnowsSecret => knowsSecret;
    public bool SecretDiscovered => secretDiscovered;
    public bool DemoFinished => demoFinished;
    public string CurrentObjective => currentObjective;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        SetObjective(initialObjective);

        if (showInitialObjectiveNotification)
        {
            ShowMessage(null, $"Objetivo: {currentObjective}");
        }
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void EnsureManagerExistsForDemoScene()
    {
        if (Instance != null || SceneManager.GetActiveScene().name != DemoSceneName)
        {
            return;
        }

        // Permite probar la ruta aunque la escena aun no tenga el manager agregado a mano.
        GameObject managerObject = new GameObject(nameof(DemoQuestRouteManager));
        managerObject.AddComponent<DemoQuestRouteManager>();
    }

    // Centraliza la ruta de demo por IDs para no acoplarla a un solo interactable.
    public bool TryHandleInteraction(string interactionId, PlayerInteractor interactor)
    {
        string normalizedId = NormalizeInteractionId(interactionId);
        if (string.IsNullOrWhiteSpace(normalizedId))
        {
            return false;
        }

        switch (normalizedId)
        {
            case "npc_pescador_tutorial":
                HandlePescadorTutorial(interactor);
                return true;
            case "fishingspot_muelle":
            case "fishing_spot_muelle":
                HandleMuelleFishing(interactor);
                return true;
            case "npc_vendedora_mercado":
                HandleVendedoraMercado(interactor);
                return true;
            case "foodstand_ceviche":
                HandleFoodStandCeviche(interactor);
                return true;
            case "npc_guia_pacoche":
                HandlePacocheGuide(interactor);
                return true;
            case "secretentrance_rock":
            case "secret_entrance_rock":
                HandleSecretEntrance(interactor);
                return true;
            case "trigger_secretzone":
            case "trigger_secret_zone":
                HandleSecretZoneDiscovered();
                return true;
            default:
                return false;
        }
    }

    public bool CanHandleInteraction(string interactionId)
    {
        switch (NormalizeInteractionId(interactionId))
        {
            case "npc_pescador_tutorial":
            case "fishingspot_muelle":
            case "fishing_spot_muelle":
            case "npc_vendedora_mercado":
            case "foodstand_ceviche":
            case "npc_guia_pacoche":
            case "secretentrance_rock":
            case "secret_entrance_rock":
            case "trigger_secretzone":
            case "trigger_secret_zone":
                return true;
            default:
                return false;
        }
    }

    public void HandleSecretZoneDiscovered()
    {
        if (!secretDiscovered || demoFinished)
        {
            return;
        }

        demoFinished = true;
        QuestManager.Instance?.SubmitEvent(QuestEvent.ForAreaReached("trigger_secret_zone", 1));
        ShowMessage(null, "Has descubierto una zona antigua conectada con la cultura manteña.");
        SetObjective("Demo completada.");
        ShowMessage(null, "Fin de la demo.");
    }

    private void HandlePescadorTutorial(PlayerInteractor interactor)
    {
        ShowMessage(interactor, "Bienvenido a Manta. Empieza por el muelle, allí podrás pescar.");
        SetObjective("Ve al muelle y pesca algo.");
        QuestManager.Instance?.SubmitEvent(QuestEvent.ForNpcTalked("npc_pescador_tutorial", 1));
    }

    private void HandleMuelleFishing(PlayerInteractor interactor)
    {
        ShowMessage(interactor, "Completa el minijuego de pesca para conseguir un pescado.");
    }

    public void CompleteMuelleFishingFromMinigame(PlayerInteractor interactor)
    {
        if (hasFish)
        {
            ShowMessage(interactor, "Lleva el pescado al mercado.");
            SetObjective("Lleva el pescado al mercado.");
            return;
        }

        hasFish = true;
        ShowMessage(interactor, "Lleva el pescado al mercado.");
        SetObjective("Lleva el pescado al mercado.");
        QuestManager.Instance?.SubmitEvent(QuestEvent.ForFishCaught("fish_muelle", 1));
    }

    private void HandleVendedoraMercado(PlayerInteractor interactor)
    {
        QuestManager.Instance?.SubmitEvent(QuestEvent.ForNpcTalked("npc_vendedora_mercado", 1));

        if (!hasFish)
        {
            ShowMessage(interactor, "Si consigues pescado fresco, puedo prepararte un buen ceviche.");
            return;
        }

        if (!hasSpondylus)
        {
            hasSpondylus = true;
            ShowMessage(interactor, "¡La pesca hoy en el puerto ha estado excelente! Con este pescado prepararé ceviche. Toma esta concha spondylus, quizá el guía sepa algo.");
            SetObjective("Busca al guía cerca de Pacoche.");
            QuestManager.Instance?.SubmitEvent(QuestEvent.ForItemCollected("spondylus_shell", 1));
            return;
        }

        ShowMessage(interactor, "Gracias por el pescado. Pregunta al guía sobre la concha spondylus.");
    }

    private void HandleFoodStandCeviche(PlayerInteractor interactor)
    {
        QuestManager.Instance?.SubmitEvent(QuestEvent.ForAreaReached("foodstand_ceviche", 1));

        if (!hasFish)
        {
            ShowMessage(interactor, "El puesto de ceviche necesita pescado fresco del muelle.");
            return;
        }

        if (!hasSpondylus)
        {
            hasSpondylus = true;
            ShowMessage(interactor, "El ceviche está listo. La vendedora te entrega una concha spondylus.");
            SetObjective("Busca al guía cerca de Pacoche.");
            return;
        }

        ShowMessage(interactor, "El ceviche de Manta ya está servido.");
    }

    private void HandlePacocheGuide(PlayerInteractor interactor)
    {
        QuestManager.Instance?.SubmitEvent(QuestEvent.ForNpcTalked("npc_guia_pacoche", 1));

        if (!hasSpondylus)
        {
            ShowMessage(interactor, "Primero conoce el mercado y las tradiciones del puerto.");
            return;
        }

        if (!knowsSecret)
        {
            knowsSecret = true;
            ShowMessage(interactor, "Esa concha spondylus está ligada a la cultura manteña. Busca una roca extraña cerca de la costa.");
            SetObjective("Busca la roca secreta en la costa.");
            QuestManager.Instance?.SubmitEvent(QuestEvent.ForAreaReached("secret_info_obtained", 1));
            return;
        }

        ShowMessage(interactor, "La roca antigua debería reaccionar ante la concha.");
    }

    private void HandleSecretEntrance(PlayerInteractor interactor)
    {
        QuestManager.Instance?.SubmitEvent(QuestEvent.ForAreaReached("secret_entrance_rock", 1));

        if (!knowsSecret)
        {
            ShowMessage(interactor, "La roca parece antigua, pero no sabes cómo abrirla.");
            return;
        }

        ShowMessage(interactor, "La concha spondylus revela una entrada oculta.");
        secretDiscovered = true;
        UnlockSecretZone();
        SetObjective("Explora la zona secreta.");
    }

    private static string NormalizeInteractionId(string interactionId)
    {
        return string.IsNullOrWhiteSpace(interactionId)
            ? string.Empty
            : interactionId.Trim().ToLowerInvariant();
    }

    private void UnlockSecretZone()
    {
        ToggleObjects(activateWhenSecretUnlocked, true);
        ToggleObjects(deactivateWhenSecretUnlocked, false);
    }

    private static void ToggleObjects(GameObject[] objects, bool value)
    {
        if (objects == null)
        {
            return;
        }

        for (int i = 0; i < objects.Length; i++)
        {
            if (objects[i] != null)
            {
                objects[i].SetActive(value);
            }
        }
    }

    private void SetObjective(string objectiveTextValue)
    {
        currentObjective = objectiveTextValue ?? string.Empty;

        if (currentObjectiveText != null)
        {
            currentObjectiveText.text = currentObjective;
        }

        if (logMessagesToConsole)
        {
            Debug.Log($"[DemoQuestRoute] Objetivo actualizado: {currentObjective}");
        }
    }

    private void ShowMessage(PlayerInteractor interactor, string message, float duration = -1f)
    {
        if (string.IsNullOrWhiteSpace(message))
        {
            return;
        }

        float finalDuration = duration > 0f ? duration : Mathf.Max(0.25f, defaultMessageDuration);

        if (logMessagesToConsole)
        {
            Debug.Log($"[DemoQuestRoute] {message}");
        }

        if (interactor != null)
        {
            interactor.ShowNotification(message, finalDuration);
            return;
        }

        if (notificationUI != null)
        {
            notificationUI.ShowMessage(message, finalDuration);
        }
    }

    private void OnValidate()
    {
        defaultMessageDuration = Mathf.Max(0.25f, defaultMessageDuration);
    }
}
