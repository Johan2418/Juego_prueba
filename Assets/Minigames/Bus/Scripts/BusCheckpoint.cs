using UnityEngine;

namespace BusMinigame
{
    public class BusCheckpoint : MonoBehaviour
    {
        public bool isReached = false;
        public SpriteRenderer spriteRenderer;
        public Color reachedColor = new Color(0, 1, 0, 0.3f);
        public Color pendingColor = new Color(1, 1, 0, 0.3f);

        void Start()
        {
            if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
            SetReached(false);
        }

        public void SetReached(bool reached)
        {
            isReached = reached;
            if (spriteRenderer != null)
            {
                spriteRenderer.color = isReached ? reachedColor : pendingColor;
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                BusMinigameManager.Instance.OnCheckpointReached(this);
            }
        }
    }
}