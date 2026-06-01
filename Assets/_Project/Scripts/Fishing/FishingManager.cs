using System.Collections;
using UnityEngine;

public class FishingManager : MonoBehaviour
{
    [SerializeField] private InventoryManager inventoryManager;
    [SerializeField] private NotificationUI fallbackNotificationUI;

    public static FishingManager Instance { get; private set; }
    public bool IsFishing { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (inventoryManager == null)
        {
            inventoryManager = InventoryManager.Instance;
        }
    }

    public void StartFishing(PlayerInteractor interactor, FishingSpot fishingSpot)
    {
        if (IsFishing || interactor == null || fishingSpot == null)
        {
            return;
        }

        if (!fishingSpot.IsPlayerInRange(interactor.transform.position))
        {
            Notify(interactor, "Acercate mas al agua para pescar.");
            return;
        }

        StartCoroutine(FishingRoutine(interactor, fishingSpot));
    }

    private IEnumerator FishingRoutine(PlayerInteractor interactor, FishingSpot fishingSpot)
    {
        IsFishing = true;
        interactor.SetInteractionLocked(true);
        Notify(interactor, "Pescando...", fishingSpot.FishingDuration);

        yield return new WaitForSeconds(fishingSpot.FishingDuration);

        FishingResult result = fishingSpot.RollResult();
        if (result.Success && result.Fish != null)
        {
            if (inventoryManager == null)
            {
                inventoryManager = InventoryManager.Instance;
            }

            if (inventoryManager != null && inventoryManager.AddItem(result.Fish, 1))
            {
                QuestManager.Instance?.SubmitEvent(QuestEvent.ForFishCaught(result.Fish.ItemId, 1));
            }

            Notify(interactor, result.Message);
        }
        else
        {
            Notify(interactor, result.Message);
        }

        interactor.SetInteractionLocked(false);
        IsFishing = false;
    }

    private void Notify(PlayerInteractor interactor, string message, float duration = -1f)
    {
        if (interactor != null)
        {
            interactor.ShowNotification(message, duration);
            return;
        }

        if (fallbackNotificationUI != null)
        {
            fallbackNotificationUI.ShowMessage(message, duration);
            return;
        }

        Debug.Log(message);
    }
}
