using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Vida y estamina del jugador.
/// </summary>
public class PlayerStats : MonoBehaviour
{
    [Header("Vida")]
    public float maxHealth = 100f;
    [SerializeField] float currentHealth;

    [Header("Estamina (para correr)")]
    public float maxStamina = 100f;
    public float staminaRegenPerSecond = 15f;
    [Tooltip("Segundos de espera antes de empezar a recuperar estamina.")]
    public float staminaRegenDelay = 1f;
    [SerializeField] float currentStamina;

    [Header("Eventos")]
    public UnityEvent<float, float> onHealthChanged;   // (actual, máxima)
    public UnityEvent<float, float> onStaminaChanged;  // (actual, máxima)
    public UnityEvent onDeath;

    float lastStaminaUseTime = -999f;

    public float Health => currentHealth;
    public float Stamina => currentStamina;
    public bool IsDead => currentHealth <= 0f;

    void Awake()
    {
        currentHealth = maxHealth;
        currentStamina = maxStamina;
    }

    void Update()
    {
        if (currentStamina < maxStamina && Time.time - lastStaminaUseTime >= staminaRegenDelay)
        {
            currentStamina = Mathf.Min(maxStamina, currentStamina + staminaRegenPerSecond * Time.deltaTime);
            onStaminaChanged?.Invoke(currentStamina, maxStamina);
        }
    }

    public void TakeDamage(float amount)
    {
        if (IsDead || amount <= 0f) return;

        currentHealth = Mathf.Max(0f, currentHealth - amount);
        onHealthChanged?.Invoke(currentHealth, maxHealth);

        if (IsDead)
            onDeath?.Invoke();
    }

    public void Heal(float amount)
    {
        if (IsDead || amount <= 0f) return;

        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
        onHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    /// <summary>Gasta estamina. Devuelve false si no hay suficiente.</summary>
    public bool UseStamina(float amount)
    {
        if (currentStamina <= 0f) return false;

        currentStamina = Mathf.Max(0f, currentStamina - amount);
        lastStaminaUseTime = Time.time;
        onStaminaChanged?.Invoke(currentStamina, maxStamina);
        return true;
    }
}
