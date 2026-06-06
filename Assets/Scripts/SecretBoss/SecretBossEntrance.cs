using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Collider2D))]
public class SecretBossEntrance : MonoBehaviour
{
    [Header("Entrance")]
    [SerializeField] private string arenaSceneName = "SecretBossArena_Test";
    [SerializeField] private KeyCode interactionKey = KeyCode.E;
    [SerializeField] private string playerTag = "Player";

    [Header("Optional Prompt")]
    [SerializeField] private GameObject interactionPrompt;

    private bool playerIsNearby;

    private void Awake()
    {
        Collider2D entranceCollider = GetComponent<Collider2D>();
        entranceCollider.isTrigger = true;
        SetPromptVisible(false);
    }

    private void Update()
    {
        if (playerIsNearby && Input.GetKeyDown(interactionKey))
        {
            EnterArena();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!IsPlayer(other))
        {
            return;
        }

        playerIsNearby = true;
        SetPromptVisible(true);
        Debug.Log($"[SecretBossEntrance] Presiona {interactionKey} para entrar a la arena secreta.");
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!IsPlayer(other))
        {
            return;
        }

        playerIsNearby = false;
        SetPromptVisible(false);
    }

    private void EnterArena()
    {
        if (string.IsNullOrWhiteSpace(arenaSceneName))
        {
            Debug.LogWarning("[SecretBossEntrance] No se configuro el nombre de la escena de arena.");
            return;
        }

        if (!Application.CanStreamedLevelBeLoaded(arenaSceneName))
        {
            Debug.LogWarning(
                $"[SecretBossEntrance] La escena '{arenaSceneName}' no esta disponible en Build Settings.");
            return;
        }

        SceneManager.LoadScene(arenaSceneName);
    }

    private bool IsPlayer(Collider2D other)
    {
        if (other == null)
        {
            return false;
        }

        if (other.GetComponentInParent<SecretBossPlayerAttack>() != null)
        {
            return true;
        }

        if (string.IsNullOrWhiteSpace(playerTag))
        {
            return false;
        }

        try
        {
            return other.CompareTag(playerTag) || other.transform.root.CompareTag(playerTag);
        }
        catch (UnityException)
        {
            Debug.LogWarning(
                $"[SecretBossEntrance] El tag configurado '{playerTag}' no existe. " +
                "Agrega SecretBossPlayerAttack al Player o configura un tag valido.",
                this);
            return false;
        }
    }

    private void SetPromptVisible(bool visible)
    {
        if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(visible);
        }
    }
}
