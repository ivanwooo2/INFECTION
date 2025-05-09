using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 500;
    [SerializeField] private int currentHealth;

    [SerializeField] private Image healthBackground; 
    [SerializeField] private Image healthFill;
    [SerializeField] private float invincibleCooldown;
    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        currentHealth = maxHealth;
        UpdateHealthUI();
    }

    public void TakeDamage(int damage)
    {
        currentHealth = Mathf.Max(currentHealth - damage, 0);
        UpdateHealthUI();
        invincibleCooldown = 2;
        if (currentHealth <= 0) Die();
    }

    private void UpdateHealthUI()
    {
        if (healthFill != null)
        {
            float fillAmount = (float)currentHealth / maxHealth;
            healthFill.fillAmount = fillAmount;
        }
    }

    private void Die()
    {
        Debug.Log("Player Died!");
        // SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void Update()
    {
        if (invincibleCooldown > 0)
        {
            spriteRenderer.color = new Color(1, 1, 1, 0.5f);
            gameObject.tag = "invincible";
            invincibleCooldown -= Time.deltaTime;
        }

        if (invincibleCooldown <= 0)
        {
            spriteRenderer.color = new Color(1, 1, 1, 1);
            gameObject.tag = "Player";
        }
    }
}
