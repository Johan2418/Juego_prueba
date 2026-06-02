using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class DemoSecretZoneTrigger : MonoBehaviour
{
    [SerializeField] private bool triggerOnce = true;

    private bool hasTriggered;

    private void Reset()
    {
        Collider2D triggerCollider = GetComponent<Collider2D>();
        if (triggerCollider != null)
        {
            triggerCollider.isTrigger = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (triggerOnce && hasTriggered)
        {
            return;
        }

        if (!other.CompareTag("Player"))
        {
            return;
        }

        if (DemoQuestRouteManager.Instance == null)
        {
            Debug.LogWarning("DemoQuestRouteManager is not present in scene.");
            return;
        }

        if (!DemoQuestRouteManager.Instance.SecretDiscovered)
        {
            return;
        }

        hasTriggered = true;
        DemoQuestRouteManager.Instance.HandleSecretZoneDiscovered();
    }
}
