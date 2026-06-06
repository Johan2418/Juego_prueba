using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance { get; private set; }

    [Range(0f, 1f)]
    public float maxVolume = 0.5f;

    [Header("Música ambiental global (suena en todo el juego y al salir de una zona)")]
    public AudioClip defaultClip;
    public float defaultFade = 1.5f;

    private AudioSource audioSource;
    private Coroutine fadeCoroutine;

    // Zonas musicales que el jugador ocupa actualmente (la última tiene prioridad).
    private readonly List<AudioClip> activeZoneClips = new List<AudioClip>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.loop = true;
        audioSource.playOnAwake = false;
        audioSource.volume = 0; // Start at 0 for fade in
    }

    private void Start()
    {
        // Arranca la música ambiental global del juego.
        if (defaultClip != null)
            PlayMusic(defaultClip, defaultFade);
    }

    // Llamado por una zona cuando el jugador entra en ella.
    public void EnterZone(AudioClip clip, float fadeDuration)
    {
        if (clip == null) return;
        activeZoneClips.Add(clip);
        PlayMusic(clip, fadeDuration);
    }

    // Llamado por una zona cuando el jugador sale de ella.
    public void ExitZone(AudioClip clip, float fadeDuration)
    {
        activeZoneClips.Remove(clip);

        if (activeZoneClips.Count > 0)
        {
            // Aún dentro de otra zona: reproducir la última activa.
            PlayMusic(activeZoneClips[activeZoneClips.Count - 1], fadeDuration);
        }
        else if (defaultClip != null)
        {
            // Fuera de todas las zonas: volver a la música ambiental global.
            PlayMusic(defaultClip, fadeDuration);
        }
    }

    public void PlayMusic(AudioClip clip, float fadeDuration)
    {
        if (clip == null) return;
        
        audioSource.loop = true;

        if (audioSource.clip == clip && audioSource.isPlaying) return;

        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }

        fadeCoroutine = StartCoroutine(FadeMusic(clip, fadeDuration));
    }

    private IEnumerator FadeMusic(AudioClip newClip, float duration)
    {
        // Fade out
        if (audioSource.isPlaying)
        {
            float startVol = audioSource.volume;
            float elapsed = 0;
            while (elapsed < duration / 2f)
            {
                elapsed += Time.deltaTime;
                audioSource.volume = Mathf.Lerp(startVol, 0, elapsed / (duration / 2f));
                yield return null;
            }
        }

        audioSource.Stop();
        audioSource.clip = newClip;
        audioSource.volume = 0;
        audioSource.Play();

        // Fade in
        float elapsedIn = 0;
        while (elapsedIn < duration / 2f)
        {
            elapsedIn += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(0, maxVolume, elapsedIn / (duration / 2f));
            yield return null;
        }
        audioSource.volume = maxVolume;
    }
}