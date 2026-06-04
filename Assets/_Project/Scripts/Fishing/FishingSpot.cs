using UnityEngine;

public class FishingSpot : MonoBehaviour, IInteractable
{
    [SerializeField] private string interactionPrompt = "Presiona E para pescar";
    [SerializeField] private float interactionDistance = 1.5f;
    [SerializeField] private float fishingDuration = 1.8f;
    [SerializeField] private float catchChance = 0.85f;
    [SerializeField] private FishData[] fishPool;

    [Header("Demo Route (Optional)")]
    [SerializeField] private bool enableDemoRouteHook;
    [SerializeField] private bool overrideDefaultFishingWithDemoRoute;
    [SerializeField] private string demoRouteInteractionId;
    [SerializeField] private MantaMinigames.Fishing.FishingMinigameLauncher fishingMinigameLauncher;
    [SerializeField] private MantaMinigames.Fishing.FishingMapMinigameSceneBuilder fishingMinigameBuilder;

    private MantaMinigames.Fishing.FishingMinigameLauncher activeMinigameLauncher;
    private PlayerInteractor activeMinigameInteractor;
    private bool completingFromMinigame;

    private const string MissingMinigameMessage = "El minijuego de pesca no est\u00e1 configurado.";

    public float FishingDuration => Mathf.Max(0.25f, fishingDuration);

    public void Interact(PlayerInteractor interactor)
    {
        MantaMinigames.Fishing.FishingMinigameLauncher minigameLauncher = ResolveMinigameLauncher();
        if (minigameLauncher != null)
        {
            if (!minigameLauncher.HasMinigameController)
            {
                fishingMinigameBuilder?.BuildIfNeeded();
            }

            if (!minigameLauncher.HasMinigameController)
            {
                interactor?.ShowNotification(MissingMinigameMessage);
                Debug.LogWarning($"{name} tiene FishingMinigameLauncher sin FishingMinigameController.");
                return;
            }

            if (activeMinigameLauncher != null)
            {
                activeMinigameLauncher.OnFishingCompleted -= HandleMinigameCompleted;
            }

            activeMinigameLauncher = minigameLauncher;
            activeMinigameInteractor = interactor;
            activeMinigameLauncher.OnFishingCompleted += HandleMinigameCompleted;
            interactor?.ShowNotification("Pescando...", 1f);
            interactor?.SetInteractionLocked(true);
            minigameLauncher.StartFishing();
            return;
        }

        if (IsMuelleDemoRoute())
        {
            interactor?.ShowNotification(MissingMinigameMessage);
            Debug.LogWarning($"{name} es la ruta del muelle y no tiene FishingMinigameLauncher configurado.");
            return;
        }

        if (overrideDefaultFishingWithDemoRoute && TryHandleDemoRoute(interactor, true))
        {
            return;
        }

        if (FishingManager.Instance == null)
        {
            Debug.LogWarning("FishingManager is not present in scene.");
            return;
        }

        FishingManager.Instance.StartFishing(interactor, this);
    }

    public string GetInteractionPrompt()
    {
        return interactionPrompt;
    }

    public bool IsPlayerInRange(Vector2 playerPosition)
    {
        return Vector2.Distance(playerPosition, transform.position) <= interactionDistance;
    }

    public FishingResult RollResult()
    {
        if (fishPool == null || fishPool.Length == 0)
        {
            return new FishingResult(false, null, "No hay peces configurados aqui.");
        }

        if (!ShouldUseDemoRouteHook() && Random.value > Mathf.Clamp01(catchChance))
        {
            return new FishingResult(false, null, "No atrapaste nada esta vez.");
        }

        FishData fish = PickWeightedFish();
        if (fish == null)
        {
            return new FishingResult(false, null, "No atrapaste nada esta vez.");
        }

        return new FishingResult(true, fish, $"Atrapaste: {fish.DisplayName}");
    }

    private FishData PickWeightedFish()
    {
        float totalWeight = 0f;
        for (int i = 0; i < fishPool.Length; i++)
        {
            if (fishPool[i] != null)
            {
                totalWeight += fishPool[i].CatchWeight;
            }
        }

        if (totalWeight <= 0f)
        {
            return null;
        }

        float roll = Random.value * totalWeight;
        float cumulative = 0f;

        for (int i = 0; i < fishPool.Length; i++)
        {
            FishData fish = fishPool[i];
            if (fish == null)
            {
                continue;
            }

            cumulative += fish.CatchWeight;
            if (roll <= cumulative)
            {
                return fish;
            }
        }

        return fishPool[fishPool.Length - 1];
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, interactionDistance);
    }

    private void OnValidate()
    {
        interactionDistance = Mathf.Max(0.25f, interactionDistance);
        fishingDuration = Mathf.Max(0.25f, fishingDuration);
        catchChance = Mathf.Clamp01(catchChance);

        if (fishingMinigameLauncher == null)
        {
            fishingMinigameLauncher = GetComponent<MantaMinigames.Fishing.FishingMinigameLauncher>();
        }
    }

    public void NotifyDemoRouteFishingSucceeded(PlayerInteractor interactor)
    {
        if (IsMuelleDemoRoute())
        {
            if (!completingFromMinigame)
            {
                Debug.Log("La ruta del muelle solo se completa al ganar el minijuego de pesca.");
                return;
            }

            DemoQuestRouteManager.Instance?.CompleteMuelleFishingFromMinigame(interactor);
            return;
        }

        TryHandleDemoRoute(interactor, false);
    }

    private void HandleMinigameCompleted(MantaMinigames.Fishing.FishingResult result)
    {
        if (activeMinigameLauncher != null)
        {
            activeMinigameLauncher.OnFishingCompleted -= HandleMinigameCompleted;
        }

        if (result == MantaMinigames.Fishing.FishingResult.Success)
        {
            completingFromMinigame = true;
            try
            {
                NotifyDemoRouteFishingSucceeded(activeMinigameInteractor);
            }
            finally
            {
                completingFromMinigame = false;
            }
        }

        activeMinigameInteractor?.SetInteractionLocked(false);
        activeMinigameLauncher = null;
        activeMinigameInteractor = null;
    }

    private MantaMinigames.Fishing.FishingMinigameLauncher ResolveMinigameLauncher()
    {
        if (fishingMinigameLauncher != null)
        {
            return fishingMinigameLauncher;
        }

        fishingMinigameLauncher = GetComponent<MantaMinigames.Fishing.FishingMinigameLauncher>();
        return fishingMinigameLauncher;
    }

    // Hook opcional para convertir un spot concreto en paso de mision de la demo.
    private bool TryHandleDemoRoute(PlayerInteractor interactor, bool consumeInteraction)
    {
        if (!ShouldUseDemoRouteHook())
        {
            return false;
        }

        bool handled = DemoQuestRouteManager.Instance.TryHandleInteraction(ResolveDemoRouteInteractionId(), interactor);
        return consumeInteraction && handled;
    }

    private bool IsMuelleDemoRoute()
    {
        string routeId = ResolveDemoRouteInteractionId().Trim().ToLowerInvariant();
        return routeId == "fishingspot_muelle" || routeId == "fishing_spot_muelle";
    }

    private bool ShouldUseDemoRouteHook()
    {
        return DemoQuestRouteManager.Instance != null &&
            (enableDemoRouteHook || DemoQuestRouteManager.Instance.CanHandleInteraction(ResolveDemoRouteInteractionId()));
    }

    private string ResolveDemoRouteInteractionId()
    {
        return string.IsNullOrWhiteSpace(demoRouteInteractionId) ? gameObject.name : demoRouteInteractionId;
    }
}
