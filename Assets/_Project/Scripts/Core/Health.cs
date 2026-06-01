using System;
using UnityEngine;

[DisallowMultipleComponent]
public class Health : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private int maxHealth = 4;
    [SerializeField] private bool destroyOnDeath;

    public int MaxHealth => maxHealth;
    public int CurrentHealth { get; private set; }
    public bool IsDead => CurrentHealth <= 0;

    public event Action<int, int> HealthChanged;
    public event Action Died;

    private void Awake()
    {
        CurrentHealth = Mathf.Max(1, maxHealth);
    }

    // Useful for respawn/checkpoint logic in future phases.
    public void ResetHealth()
    {
        CurrentHealth = Mathf.Max(1, maxHealth);
        HealthChanged?.Invoke(CurrentHealth, MaxHealth);
    }

    public void TakeDamage(int amount)
    {
        if (IsDead || amount <= 0)
        {
            return;
        }

        CurrentHealth = Mathf.Max(0, CurrentHealth - amount);
        HealthChanged?.Invoke(CurrentHealth, MaxHealth);

        if (CurrentHealth == 0)
        {
            Died?.Invoke();

            if (destroyOnDeath)
            {
                Destroy(gameObject);
            }
        }
    }

    public void Heal(int amount)
    {
        if (IsDead || amount <= 0)
        {
            return;
        }

        int nextHealth = Mathf.Min(MaxHealth, CurrentHealth + amount);
        if (nextHealth == CurrentHealth)
        {
            return;
        }

        CurrentHealth = nextHealth;
        HealthChanged?.Invoke(CurrentHealth, MaxHealth);
    }

    private void OnValidate()
    {
        maxHealth = Mathf.Max(1, maxHealth);
    }
}
