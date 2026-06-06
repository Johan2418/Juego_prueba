using UnityEngine;
using System.Collections;

/// <summary>
/// Reproduce un sonido ambiental en bucle ENCIMA de la música global
/// mientras el jugador está dentro de la zona. Usa su propio AudioSource,
/// con fade de entrada/salida. No modifica el MusicManager.
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class AmbientLayerTrigger : MonoBehaviour
{
    [Tooltip("Sonido ambiental en bucle (p. ej. olas del mar).")]
    public AudioClip ambientClip;

    [Range(0f, 1f)]
    [Tooltip("Volumen máximo de la capa ambiental.")]
    public float maxVolume = 0.3f;

    [Tooltip("Duración del fundido de entrada/salida en segundos.")]
    public float fadeDuration = 1.5f;

    private AudioSource audioSource;
    private Coroutine fadeCoroutine;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = ambientClip;
        audioSource.loop = true;          // bucle infinito
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f;    // 2D: se oye igual en toda la zona
        audioSource.volume = 0f;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player") || ambientClip == null) return;

        if (audioSource.clip != ambientClip)
            audioSource.clip = ambientClip;

        if (!audioSource.isPlaying)
            audioSource.Play();

        StartFade(maxVolume);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        StartFade(0f);
    }

    private void StartFade(float target)
    {
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(Fade(target));
    }

    private IEnumerator Fade(float target)
    {
        float start = audioSource.volume;
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(start, target, elapsed / fadeDuration);
            yield return null;
        }
        audioSource.volume = target;

        // Al terminar un fade-out, detener para liberar el canal.
        if (Mathf.Approximately(target, 0f))
            audioSource.Stop();
    }
}
