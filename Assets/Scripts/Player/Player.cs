using UnityEngine;

public class Player : MonoBehaviour, IDamageable
{
    [Header("Stats")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private bool destroyOnDeath;
    [SerializeField] private GameObject deathEffect;

    private float currentHealth;
    private bool isDead;

    public float CurrentHealth => currentHealth;
    public float MaxHealth => maxHealth;
    public bool IsDead => isDead;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float amount)
    {
        if (isDead || amount <= 0f)
        {
            return;
        }

        currentHealth -= amount;
        if (currentHealth <= 0f)
        {
            currentHealth = 0f;
            Die();
        }
    }

    private void Die()
    {
        isDead = true;
        deathEffect?.SetActive(true);
        if (destroyOnDeath)
        {
            Destroy(gameObject);
        }
    }
}
