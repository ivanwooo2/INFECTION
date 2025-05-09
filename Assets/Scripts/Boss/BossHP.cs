using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossHP : MonoBehaviour
{
    [SerializeField] private Collider2D damageCollider;
    private BossController bossController;

    void Awake()
    {
        bossController = FindObjectOfType<BossController>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        bossController.OnBossTriggerEnter(other);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        bossController.OnBossTriggerExit(other);
    }
}
