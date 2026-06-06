using System;
using UnityEngine;

[DisallowMultipleComponent]
public class SecretBossHealth : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private int maxHealth = 10;
    [SerializeField] private bool destroyOnDefeat;
    [SerializeField] private bool deactivateOnDefeat = true;

    public int MaxHealth => maxHealth;
    public int CurrentHealth { get; private set; }
    public bool IsDefeated { get; private set; }

    public event Action<int, int> HealthChanged;
    public event Action Defeated;

    private void Awake()
    {
        CurrentHealth = Mathf.Max(1, maxHealth);
    }

    public void TakeDamage(int amount)
    {
        if (IsDefeated || amount <= 0)
        {
            return;
        }

        CurrentHealth = Mathf.Max(0, CurrentHealth - amount);
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
        Defeated?.Invoke();

        if (destroyOnDefeat)
        {
            Destroy(gameObject);
        }
        else if (deactivateOnDefeat)
        {
            gameObject.SetActive(false);
        }
    }

    private void OnValidate()
    {
        maxHealth = Mathf.Max(1, maxHealth);
    }
}
