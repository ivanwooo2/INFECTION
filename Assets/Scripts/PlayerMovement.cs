using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float playermoveSpeed;
    [SerializeField] private float playerSkillmoveSpeed;
    [SerializeField] private float originalMoveSpeed;
    [SerializeField] private Rigidbody2D playerRB;
    private Vector2 playerPosition;

    [SerializeField] private float activeMoveSpeed;
    [SerializeField] private float dashSpeed;
    [SerializeField] private float dashLength;
    [SerializeField] private float dashCooldown;
    private float dashCooldownTime;
    private float dashLengthCounter;
    private SpriteRenderer spriteRenderer;

    [SerializeField] private float skillDuration = 5f;
    [SerializeField] private float skillCooldown = 10f;
    [SerializeField] private float scaleMultiplier = 0.5f;
    private bool isSkillReady = true;
    private bool isSkilling;
    private PlayerHealth playerHealth;
    private BossController bossController;

    [SerializeField] private TrailRenderer playerTR;

    public bool isInvincible = false;

    // Start is called before the first frame update
    void Start()
    {
        originalMoveSpeed = playermoveSpeed;
        playerHealth = GetComponent<PlayerHealth>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        activeMoveSpeed = playermoveSpeed;
        bossController = FindObjectOfType<BossController>();
    }

    // Update is called once per frame
    void Update()
    {
        playerPosition.x = Input.GetAxisRaw("Horizontal");
        playerPosition.y = Input.GetAxisRaw("Vertical");

        playerPosition.Normalize();

        playerRB.velocity = playerPosition * activeMoveSpeed;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (dashCooldownTime <= 0 && dashLengthCounter <=0 )
            {
                spriteRenderer.color = new Color(1, 1, 1, 0.5f);
                isInvincible = true; 
                playerTR.emitting = true;
                dashLengthCounter = dashLength;
                activeMoveSpeed = dashSpeed;

            }
        }

        if (dashLengthCounter > 0)
        {
            dashLengthCounter -= Time.deltaTime;

            if (dashLengthCounter <= 0)
            {
                spriteRenderer.color = new Color(1, 1, 1, 1);;
                dashCooldownTime = dashCooldown;
                playerTR.emitting = false;
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

        if (Input.GetKeyDown(KeyCode.Q) && isSkillReady)
        {
            StartCoroutine(ActivateSkill());
        }
    }

    IEnumerator ActivateSkill()
    {
        isSkillReady = false;
        isSkilling = true;
        playerHealth.ToggleSkill(true, scaleMultiplier);

        activeMoveSpeed = playerSkillmoveSpeed;

        bossController.ChrSkill1(skillDuration);

        yield return new WaitForSeconds(skillDuration);
        isSkilling = false;
        activeMoveSpeed = originalMoveSpeed;
        playerHealth.ToggleSkill(false);
        yield return new WaitForSeconds(skillCooldown);
        isSkillReady = true;
    }
}
