using UnityEngine;

public class MusicZoneTrigger : MonoBehaviour
{
    public AudioClip musicClip;
    public float fadeDuration = 1f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (MusicManager.Instance != null)
            {
                MusicManager.Instance.EnterZone(musicClip, fadeDuration);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (MusicManager.Instance != null)
            {
                MusicManager.Instance.ExitZone(musicClip, fadeDuration);
            }
        }
    }
}