using UnityEngine;

namespace BusMinigame
{
    public class BusDestination : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                BusMinigameManager.Instance.OnDestinationReached();
            }
        }
    }
}