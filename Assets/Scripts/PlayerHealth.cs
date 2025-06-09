using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 500;
    [SerializeField] public int currentHealth;

    [SerializeField] private Image healthBackground; 
    [SerializeField] private Image healthFill;
    [SerializeField] private float invincibleDuration;
    [SerializeField] private GameObject PlayerDamageBackGround;
    private SpriteRenderer spriteRenderer;

    public bool isInvincible = false;

    public bool isSkillActive = false;
    private float originalScale;

    private TimeManager timeManager;

    public AudioSource Player;
    public AudioClip Damaged;

    void Start()
    {
        timeManager = TimeManager.Instance;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    void Awake()
    {
        originalScale = transform.localScale.x;
        currentHealth = maxHealth;
        UpdateHealthUI();
    }

    public void ToggleSkill(bool state, float scaleMultiplier = 1f)
    {
        isSkillActive = state;
        Vector3 newScale = state ?
            new Vector3(originalScale * scaleMultiplier, originalScale * scaleMultiplier, 1) :
            new Vector3(originalScale, originalScale, 1);
        transform.localScale = newScale;
    }

    public void TakeDamage(int damage)
    {
        if (isInvincible) return;
        Player.clip = Damaged;
        Player.Play();
        StartCoroutine(InvincibleRoutine());

        int finalDamage = isSkillActive ? damage * 2 : damage;
        currentHealth = Mathf.Max(currentHealth - finalDamage, 0);
        UpdateHealthUI();
        if (currentHealth <= 0) Die();
    }

    IEnumerator InvincibleRoutine()
    {
        isInvincible = true;
        GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.5f);
        PlayerDamageBackGround.SetActive(true);
        yield return new WaitForSeconds(invincibleDuration);
        PlayerDamageBackGround.SetActive(false);
        GetComponent<SpriteRenderer>().color = Color.white;
        isInvincible = false;
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
        timeManager.LoadResultScene();
    }
    void Update()
    {

    }
}
