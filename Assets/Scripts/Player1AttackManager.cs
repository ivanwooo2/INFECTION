using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player1AttackManager : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    private Vector3 direction;
    private BossController BossController;
    private SpriteRenderer SpriteRenderer;
    private PlayerMovement PlayerMovement;
    [SerializeField] private Sprite NormalAttack;
    [SerializeField] private Sprite SkillAttack;
    void Start()
    {
        SpriteRenderer = GetComponent<SpriteRenderer>();
        BossController = FindAnyObjectByType<BossController>();
        PlayerMovement = FindAnyObjectByType<PlayerMovement>();
        if (BossController != null)
        {
            direction = BossController.currentBoss.transform.position - transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle - 90);
        }

        if (PlayerMovement.isSkilling == true)
        {
            SpriteRenderer.sprite = SkillAttack;
        }
        else
        {
            SpriteRenderer.sprite = NormalAttack;
        }
    }

    void Update()
    {
        transform.position += direction * moveSpeed * Time.deltaTime;
    }
}
