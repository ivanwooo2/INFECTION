﻿using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Burst.Intrinsics;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float playermoveSpeed;
    [SerializeField] private float SlowMoveSpeed;
    [SerializeField] private float playerSkillmoveSpeed;
    [SerializeField] private float originalMoveSpeed;
    [SerializeField] private Rigidbody2D playerRB;
    [SerializeField] private Sprite origin;
    [SerializeField] private Sprite Skill;
    private Vector2 playerPosition;

    [SerializeField] private float activeMoveSpeed;
    [SerializeField] private float dashSpeed;
    [SerializeField] private float dashLength;
    [SerializeField] private float dashCooldown;
    private float dashCooldownTime;
    private float dashLengthCounter;
    private SpriteRenderer spriteRenderer;

    [Header("Cooldown UI")]
    [SerializeField] private Image dashCooldownImage;
    [SerializeField] private Image skillCooldownImage;
    [SerializeField] private TMP_Text dashCooldownText;
    [SerializeField] private TMP_Text skillCooldownText;
    [SerializeField] private GameObject SkillusingImage;
    [SerializeField] private GameObject SkillCooldownBG;

    private float skillCooldownRemaining;

    [SerializeField] private float skillDuration = 5f;
    [SerializeField] private float skillCooldown = 10f;
    [SerializeField] private float scaleMultiplier = 0.5f;
    private bool isSkillReady = true;
    public bool isSkilling;
    private PlayerHealth playerHealth;
    private BossController bossController;

    [SerializeField] private TrailRenderer playerTR;

    public AudioSource Player;
    public AudioClip dash,skill1,skill2;

    public bool isInvincible = false;

    void Start()
    {
        originalMoveSpeed = playermoveSpeed;
        playerHealth = GetComponent<PlayerHealth>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        activeMoveSpeed = playermoveSpeed;
        bossController = FindObjectOfType<BossController>();
    }

    void Update()
    {

        if (TimeManager.Instance != null && TimeManager.Instance.isGameOver)
        {
            playerRB.velocity = Vector2.zero;
            return;
        }

        UpdateCooldownUI();
        playerPosition.x = Input.GetAxisRaw("Horizontal");
        playerPosition.y = Input.GetAxisRaw("Vertical");

        playerPosition.Normalize();

        playerRB.velocity = playerPosition * activeMoveSpeed;

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetButtonDown("Dash"))
        {
            if (TimeManager.Instance != null && TimeManager.Instance.isGameOver)
            {
                playerRB.velocity = Vector2.zero;
                return;
            }
            if (dashCooldownTime <= 0 && dashLengthCounter <=0 )
            {
                Player.clip = dash;
                Player.Play();
                spriteRenderer.color = new Color(1, 1, 1, 0.5f);
                isInvincible = true;
                if (playerTR != null)
                {
                    playerTR.emitting = true;
                }
                dashLengthCounter = dashLength;
                activeMoveSpeed = dashSpeed;

            }
        }

        if (Input.GetButtonDown("Slow"))
        {
            if (TimeManager.Instance != null && TimeManager.Instance.isGameOver)
            {
                playerRB.velocity = Vector2.zero;
                return;
            }
            activeMoveSpeed = SlowMoveSpeed;
        }

        if (Input.GetButtonUp("Slow"))
        {
            if (TimeManager.Instance != null && TimeManager.Instance.isGameOver)
            {
                playerRB.velocity = Vector2.zero;
                return;
            }
            activeMoveSpeed = originalMoveSpeed;
        }

        if (dashLengthCounter > 0)
        {
            if (TimeManager.Instance != null && TimeManager.Instance.isGameOver)
            {
                playerRB.velocity = Vector2.zero;
                return;
            }
            dashLengthCounter -= Time.deltaTime;

            if (dashLengthCounter <= 0)
            {
                spriteRenderer.color = new Color(1, 1, 1, 1);;
                dashCooldownTime = dashCooldown;
                if (playerTR != null)
                {
                    playerTR.emitting = false;
                }
                isInvincible = false;
                if (!isSkilling)
                {
                    activeMoveSpeed = originalMoveSpeed;
                }
                else
                {
                    activeMoveSpeed = playerSkillmoveSpeed;
                }
            }
        }

        if (dashCooldownTime > 0)
        {
            dashCooldownTime -= Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.Q) && isSkillReady || Input.GetButtonDown("Skill") && isSkillReady)
        {
            if (TimeManager.Instance != null && TimeManager.Instance.isGameOver)
            {
                playerRB.velocity = Vector2.zero;
                return;
            }
            Player.clip = skill1;
            Player.Play();
            StartCoroutine(ActivateSkill());
        }
    }

    private void UpdateCooldownUI()
    {
        if (dashCooldownImage != null)
        {
            float dashFill = Mathf.Clamp01(1 - (dashCooldownTime / dashCooldown));
            dashCooldownImage.fillAmount = dashFill;

            if (dashCooldownText != null)
            {
                dashCooldownText.text = dashCooldownTime > 0 ?
                    Mathf.Ceil(dashCooldownTime).ToString() : "";
            }
        }

        if (skillCooldownImage != null)
        {
            float skillFill = Mathf.Clamp01(1 - (skillCooldownRemaining / skillCooldown));
            skillCooldownImage.fillAmount = skillFill;

            if (skillCooldownText != null)
            {
                skillCooldownText.text = skillCooldownRemaining > 0 ?
                    Mathf.Ceil(skillCooldownRemaining).ToString() : "";
            }
        }
    }

    IEnumerator ActivateSkill()
    {
        isSkillReady = false;
        isSkilling = true;
        playerHealth.ToggleSkill(true, scaleMultiplier);
        spriteRenderer.sprite = Skill;
        activeMoveSpeed = playerSkillmoveSpeed;
        SkillusingImage.SetActive(true);
        SkillCooldownBG.SetActive(false);
        if (bossController != null)
        {
            bossController.ChrSkill1(skillDuration);
        }
        yield return new WaitForSeconds(skillDuration);
        SkillusingImage.SetActive(false);
        SkillCooldownBG.SetActive(true);
        Player.clip = skill2;
        Player.Play();
        isSkilling = false;
        spriteRenderer.sprite = origin;
        activeMoveSpeed = originalMoveSpeed;
        playerHealth.ToggleSkill(false);
        skillCooldownRemaining = skillCooldown;
        while (skillCooldownRemaining > 0)
        {
            skillCooldownRemaining -= Time.deltaTime;
            yield return null;
        }
        isSkillReady = true;
        skillCooldownRemaining = 0;
    }
}
