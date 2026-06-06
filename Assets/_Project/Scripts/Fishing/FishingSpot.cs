using UnityEngine;

public class FishingSpot : MonoBehaviour, IInteractable
{
    [SerializeField] private string interactionPrompt = "Presiona E para pescar";
    [SerializeField] private float interactionDistance = 1.5f;
    [SerializeField] private float fishingDuration = 1.8f;
    [SerializeField] private float catchChance = 0.85f;
    [SerializeField] private FishData[] fishPool;
    [SerializeField] private bool ensureInteractionCollider = true;
    [SerializeField] private string fishingZoneObjectName = "Trigger_FishingZone";
    [SerializeField] private string dockWoodObjectName = "Dock_Wood";
    [SerializeField, Min(0.1f)] private float dockEdgeZoneHeight = 0.55f;
    [SerializeField, Range(0.25f, 1f)] private float dockEdgeZoneWidthRatio = 0.9f;

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

    private void Awake()
    {
        EnsureRuntimeFishingSetup();
        EnsureInteractionCollider();
        EnsureDockEdgeInteractionZones();
    }

    private void OnEnable()
    {
        EnsureRuntimeFishingSetup();
        EnsureInteractionCollider();
        EnsureDockEdgeInteractionZones();
    }

    public void Interact(PlayerInteractor interactor)
    {
        MantaMinigames.Fishing.FishingMinigameLauncher minigameLauncher = ResolveMinigameLauncher();

        if (minigameLauncher != null)
        {
            if (!minigameLauncher.EnsureConfigured())
            {
                ResolveMinigameBuilder()?.BuildIfNeeded();
            }

            if (!minigameLauncher.EnsureConfigured())
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
            FishData selectedFish = PickWeightedFish();
            if (selectedFish == null)
            {
                activeMinigameLauncher.OnFishingCompleted -= HandleMinigameCompleted;
                activeMinigameLauncher = null;
                activeMinigameInteractor = null;
                interactor?.ShowNotification("No hay peces configurados aqui.");
                Debug.LogWarning($"{name} no tiene peces validos en fishPool.");
                return;
            }

            interactor?.ShowNotification("Pescando...", 1f);
            interactor?.SetInteractionLocked(true);
            minigameLauncher.StartFishing(selectedFish);
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

        for (int i = fishPool.Length - 1; i >= 0; i--)
        {
            if (fishPool[i] != null)
            {
                return fishPool[i];
            }
        }

        return null;
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
        dockEdgeZoneHeight = Mathf.Max(0.1f, dockEdgeZoneHeight);

        if (fishingMinigameLauncher == null)
        {
            fishingMinigameLauncher = GetComponent<MantaMinigames.Fishing.FishingMinigameLauncher>();
        }

        if (fishingMinigameBuilder == null)
        {
            fishingMinigameBuilder = FindFirstObjectByType<MantaMinigames.Fishing.FishingMapMinigameSceneBuilder>(FindObjectsInactive.Include);
        }
    }

    private void EnsureInteractionCollider()
    {
        if (!ensureInteractionCollider)
        {
            return;
        }

        CircleCollider2D interactionCollider = GetComponent<CircleCollider2D>();
        if (interactionCollider == null)
        {
            interactionCollider = gameObject.AddComponent<CircleCollider2D>();
        }

        if (IsMuelleDemoRoute())
        {
            interactionCollider.enabled = false;
            return;
        }

        interactionCollider.enabled = true;
        interactionCollider.isTrigger = true;
        interactionCollider.radius = Mathf.Max(0.7f, interactionDistance);
    }

    private void EnsureDockEdgeInteractionZones()
    {
        if (!IsMuelleDemoRoute())
        {
            return;
        }

        RemoveLegacyFishingZoneProxy();

        GameObject dockWood = FindSceneObjectByName(dockWoodObjectName);
        SpriteRenderer dockRenderer = dockWood != null ? dockWood.GetComponent<SpriteRenderer>() : null;
        if (dockRenderer == null)
        {
            Debug.LogWarning($"{name} no encontro el SpriteRenderer {dockWoodObjectName} para crear los bordes de pesca.");
            return;
        }

        Bounds dockBounds = dockRenderer.bounds;
        float zoneWidth = Mathf.Max(0.5f, dockBounds.size.x * dockEdgeZoneWidthRatio);
        float halfHeight = dockEdgeZoneHeight * 0.5f;

        GameObject zonesRoot = FindSceneObjectByName("FishingDockInteractionZones");
        if (zonesRoot == null)
        {
            zonesRoot = new GameObject("FishingDockInteractionZones");
        }

        ConfigureDockEdgeZone(
            zonesRoot.transform,
            "FishingDockEdge_Top",
            new Vector2(dockBounds.center.x, dockBounds.max.y - halfHeight),
            new Vector2(zoneWidth, dockEdgeZoneHeight));
        ConfigureDockEdgeZone(
            zonesRoot.transform,
            "FishingDockEdge_Bottom",
            new Vector2(dockBounds.center.x, dockBounds.min.y + halfHeight),
            new Vector2(zoneWidth, dockEdgeZoneHeight));
    }

    private void ConfigureDockEdgeZone(Transform parent, string zoneName, Vector2 worldPosition, Vector2 size)
    {
        Transform existing = parent.Find(zoneName);
        GameObject zone = existing != null ? existing.gameObject : new GameObject(zoneName);
        zone.transform.SetParent(parent, true);
        zone.transform.position = worldPosition;
        zone.transform.localScale = Vector3.one;

        BoxCollider2D collider = zone.GetComponent<BoxCollider2D>();
        if (collider == null)
        {
            collider = zone.AddComponent<BoxCollider2D>();
        }

        collider.enabled = true;
        collider.isTrigger = true;
        collider.offset = Vector2.zero;
        collider.size = size;

        MantaMinigames.Fishing.FishingZoneInteractionProxy proxy =
            zone.GetComponent<MantaMinigames.Fishing.FishingZoneInteractionProxy>();
        if (proxy == null)
        {
            proxy = zone.AddComponent<MantaMinigames.Fishing.FishingZoneInteractionProxy>();
        }

        proxy.Configure(this);
    }

    private void RemoveLegacyFishingZoneProxy()
    {
        if (string.IsNullOrWhiteSpace(fishingZoneObjectName))
        {
            return;
        }

        GameObject legacyZone = FindSceneObjectByName(fishingZoneObjectName);
        MantaMinigames.Fishing.FishingZoneInteractionProxy proxy =
            legacyZone != null ? legacyZone.GetComponent<MantaMinigames.Fishing.FishingZoneInteractionProxy>() : null;
        if (proxy != null)
        {
            Destroy(proxy);
        }
    }

    private void EnsureRuntimeFishingSetup()
    {
        if (!IsMuelleDemoRoute())
        {
            return;
        }

        if (fishingMinigameLauncher == null)
        {
            fishingMinigameLauncher = GetComponent<MantaMinigames.Fishing.FishingMinigameLauncher>();
        }

        if (fishingMinigameLauncher == null)
        {
            fishingMinigameLauncher = gameObject.AddComponent<MantaMinigames.Fishing.FishingMinigameLauncher>();
        }

        if (GetComponent<MantaMinigames.Fishing.FishingPlayerFishingState>() == null)
        {
            gameObject.AddComponent<MantaMinigames.Fishing.FishingPlayerFishingState>();
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
        if (fishingMinigameLauncher == null && IsMuelleDemoRoute())
        {
            fishingMinigameLauncher = gameObject.AddComponent<MantaMinigames.Fishing.FishingMinigameLauncher>();
        }

        return fishingMinigameLauncher;
    }

    private MantaMinigames.Fishing.FishingMapMinigameSceneBuilder ResolveMinigameBuilder()
    {
        if (fishingMinigameBuilder != null)
        {
            return fishingMinigameBuilder;
        }

        fishingMinigameBuilder = FindFirstObjectByType<MantaMinigames.Fishing.FishingMapMinigameSceneBuilder>(FindObjectsInactive.Include);
        return fishingMinigameBuilder;
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
        string objectName = gameObject.name.Trim().ToLowerInvariant();
        return routeId == "fishingspot_muelle" ||
            routeId == "fishing_spot_muelle" ||
            objectName == "fishingspot_muelle" ||
            objectName == "fishing_spot_muelle";
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

    private static GameObject FindSceneObjectByName(string objectName)
    {
        GameObject directMatch = GameObject.Find(objectName);
        if (directMatch != null)
        {
            return directMatch;
        }

        Transform[] transforms = FindObjectsByType<Transform>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        for (int i = 0; i < transforms.Length; i++)
        {
            if (transforms[i] != null && transforms[i].name == objectName)
            {
                return transforms[i].gameObject;
            }
        }

        return null;
    }
}
