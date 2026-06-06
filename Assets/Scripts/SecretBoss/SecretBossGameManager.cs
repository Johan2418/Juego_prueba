using UnityEngine;
using UnityEngine.SceneManagement;

[DisallowMultipleComponent]
public class SecretBossGameManager : MonoBehaviour
{
    [Header("Fight References")]
    [SerializeField] private SecretBossHealth bossHealth;
    [SerializeField] private Health playerHealth;

    [Header("Fight Settings")]
    [SerializeField] private bool startFightOnStart = true;
    [SerializeField] private KeyCode restartKey = KeyCode.R;

    private bool fightIsActive;

    private void OnEnable()
    {
        if (bossHealth != null)
        {
            bossHealth.Defeated += HandlePlayerVictory;
        }

        if (playerHealth != null)
        {
            playerHealth.Died += HandlePlayerDefeat;
        }
    }

    private void Start()
    {
        if (startFightOnStart)
        {
            StartFight();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(restartKey))
        {
            RestartScene();
        }
    }

    private void OnDisable()
    {
        if (bossHealth != null)
        {
            bossHealth.Defeated -= HandlePlayerVictory;
        }

        if (playerHealth != null)
        {
            playerHealth.Died -= HandlePlayerDefeat;
        }
    }

    public void StartFight()
    {
        if (fightIsActive)
        {
            return;
        }

        fightIsActive = true;
        Debug.Log("[SecretBossGameManager] La pelea contra el jefe secreto ha comenzado.");
    }

    public void PlayerLost()
    {
        HandlePlayerDefeat();
    }

    private void HandlePlayerVictory()
    {
        if (!fightIsActive)
        {
            return;
        }

        fightIsActive = false;
        Debug.Log("[SecretBossGameManager] Victoria del jugador.");
    }

    private void HandlePlayerDefeat()
    {
        if (!fightIsActive)
        {
            return;
        }

        fightIsActive = false;
        Debug.Log("[SecretBossGameManager] El jugador ha perdido.");
    }

    private void RestartScene()
    {
        Scene activeScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(activeScene.name);
    }
}
