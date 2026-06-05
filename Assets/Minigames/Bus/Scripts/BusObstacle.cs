using UnityEngine;

namespace BusMinigame
{
    public class BusObstacle : MonoBehaviour
    {
        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                BusMinigameManager.Instance.OnObstacleHit();
            }
        }
    }
}