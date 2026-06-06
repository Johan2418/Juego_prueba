using System;
using System.Collections;
using UnityEngine;

[DisallowMultipleComponent]
public class SecretBossHealth : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private int maxHealth = 10;
    [SerializeField] private bool destroyOnDefeat;
    [SerializeField] private bool deactivateOnDefeat = true;
    [SerializeField] private float defeatVisualDuration = 1f;
    [SerializeField] private Animator animator;

    public int MaxHealth => maxHealth;
    public int CurrentHealth { get; private set; }
    public bool IsDefeated { get; private set; }

    public event Action<int, int> HealthChanged;
    public event Action Defeated;

    private void Awake()
    {
        CurrentHealth = Mathf.Max(1, maxHealth);
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
    }

    public void TakeDamage(int amount)
    {
        if (IsDefeated || amount <= 0)
        {
            return;
        }

        CurrentHealth = Mathf.Max(0, CurrentHealth - amount);
        Debug.Log(
            $"[SecretBossHealth] El boss recibe {amount} de dano. Vida restante: {CurrentHealth}/{MaxHealth}.");

        if (animator != null)
        {
            animator.SetTrigger("Hurt");
        }

        HealthChanged?.Invoke(CurrentHealth, MaxHealth);

        if (CurrentHealth == 0)
        {
            Defeat();
        }
    }

    private void Defeat()
    {
        IsDefeated = true;
        Debug.Log("[SecretBossHealth] Jefe secreto derrotado.");

        if (animator != null)
        {
            animator.SetTrigger("Death");
        }

        Defeated?.Invoke();
        StopBossMovement();

        if (destroyOnDefeat)
        {
            Destroy(gameObject, defeatVisualDuration);
        }
        else if (deactivateOnDefeat)
        {
            StartCoroutine(DeactivateAfterDefeat());
        }
    }

    private void StopBossMovement()
    {
        SecretBossController controller = GetComponent<SecretBossController>();
        if (controller != null)
        {
            controller.enabled = false;
        }

        Rigidbody2D bossRigidbody = GetComponent<Rigidbody2D>();
        if (bossRigidbody != null)
        {
            bossRigidbody.linearVelocity = Vector2.zero;
            bossRigidbody.angularVelocity = 0f;
            bossRigidbody.simulated = false;
        }

        Collider2D bossCollider = GetComponent<Collider2D>();
        if (bossCollider != null)
        {
            bossCollider.enabled = false;
        }
    }

    private IEnumerator DeactivateAfterDefeat()
    {
        yield return new WaitForSeconds(defeatVisualDuration);
        gameObject.SetActive(false);
    }

    private void OnValidate()
    {
        maxHealth = Mathf.Max(1, maxHealth);
        defeatVisualDuration = Mathf.Max(0f, defeatVisualDuration);
    }
}
