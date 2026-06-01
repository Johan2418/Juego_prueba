using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ReachAreaTrigger : MonoBehaviour
{
    [SerializeField] private string areaId = "cozy_area";

    private void Reset()
    {
        Collider2D trigger = GetComponent<Collider2D>();
        if (trigger != null)
        {
            trigger.isTrigger = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        QuestManager.Instance?.SubmitEvent(QuestEvent.ForAreaReached(areaId, 1));
    }
}
