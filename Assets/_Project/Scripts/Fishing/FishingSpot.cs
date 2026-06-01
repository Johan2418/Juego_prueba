using UnityEngine;

public class FishingSpot : MonoBehaviour, IInteractable
{
    [SerializeField] private string interactionPrompt = "Presiona E para pescar";
    [SerializeField] private float interactionDistance = 1.5f;
    [SerializeField] private float fishingDuration = 1.8f;
    [SerializeField] private float catchChance = 0.85f;
    [SerializeField] private FishData[] fishPool;

    public float FishingDuration => Mathf.Max(0.25f, fishingDuration);

    public void Interact(PlayerInteractor interactor)
    {
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

        if (Random.value > Mathf.Clamp01(catchChance))
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
    }
}
