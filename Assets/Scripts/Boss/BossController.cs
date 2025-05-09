using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class BossController : MonoBehaviour
{
    [SerializeField] public GameObject BossPreFab;
    [SerializeField] public float minInterval;
    [SerializeField] public float maxInterval;
    [SerializeField] public float moveSpeed;
    [SerializeField] public float stayDuration;

    [SerializeField] public Vector2 initialSpawnViewport;
    [SerializeField] public Vector2 initialTargetViewport;

    [SerializeField] public float verticalMoveDistance;
    [SerializeField] public float horizontalMoveDistance;

    [SerializeField] private Image[] healthBars;
    [SerializeField] private Canvas healthCanvas;
    [SerializeField] private int maxHealth = 300;
    [SerializeField] private int currentHealth;
    private bool isPlayerInDamageArea;
    [SerializeField] private int damage;

    [SerializeField] private TMP_Text healthText;

    private GameObject currentBoss;
    private bool isOperating;
    void Start()
    {
        InitializeHealthSystem();
        StartCoroutine(InitialEntrance());
    }

    void InitializeHealthSystem()
    {
        currentHealth = 0;
        healthCanvas.gameObject.SetActive(true);
        UpdateHealthDisplay();
    }

    IEnumerator InitialEntrance()
    {
        isOperating = true;

        currentBoss = Instantiate(BossPreFab);
        UpdateHealthDisplay();
        Vector3 startPos = ViewportToWorld(initialSpawnViewport);
        Vector3 targetPos = ViewportToWorld(initialTargetViewport);

        currentBoss.transform.position = startPos;
        yield return StartCoroutine(MoveTo(targetPos));

        yield return new WaitForSeconds(stayDuration);

        Vector3 exitPos = startPos + Vector3.up * 2f;
        yield return StartCoroutine(MoveTo(exitPos));

        Destroy(currentBoss);
        isOperating = false;
        StartCoroutine(RandomSpawnLoop());
    }

    IEnumerator RandomSpawnLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minInterval, maxInterval));
            if (!isOperating) StartCoroutine(SpawnFromRandomSide());
        }
    }

    IEnumerator SpawnFromRandomSide()
    {
        isOperating = true;

        int side = Random.Range(0, 3);

        Vector3 spawnPos = GetSideSpawnPosition(side);
        Vector3 targetPos = GetSideTargetPosition(side);
        Vector3 exitPos = spawnPos + GetExitDirection(side) * 2f;

        currentBoss = Instantiate(BossPreFab);
        currentBoss.transform.position = spawnPos;

        yield return StartCoroutine(MoveTo(targetPos));

        yield return new WaitForSeconds(stayDuration);

        yield return StartCoroutine(MoveTo(exitPos));

        Destroy(currentBoss);
        isOperating = false;
    }

    IEnumerator MoveTo(Vector3 target)
    {
        Vector3 startPos = currentBoss.transform.position;
        float progress = 0;

        while (progress < 1)
        {
            progress += Time.deltaTime * moveSpeed;
            currentBoss.transform.position = Vector3.Lerp(startPos, target, progress);
            yield return null;
        }
    }

    Vector3 ViewportToWorld(Vector2 viewportPos)
    {
        return Camera.main.ViewportToWorldPoint(new Vector3(viewportPos.x, viewportPos.y, 10));
    }

    Vector3 GetSideSpawnPosition(int side)
    {
        return side switch
        {
            0 => ViewportToWorld(new Vector2(verticalMoveDistance, 1.5f)),
            1 => ViewportToWorld(new Vector2(-0.5f, horizontalMoveDistance)),
            2 => ViewportToWorld(new Vector2(1.5f, horizontalMoveDistance)),
            _ => Vector3.zero
        };
    }

    Vector3 GetSideTargetPosition(int side)
    {
        return side switch
        {
            0 => ViewportToWorld(new Vector2(verticalMoveDistance, 1f)),
            1 => ViewportToWorld(new Vector2(0f, horizontalMoveDistance)),
            2 => ViewportToWorld(new Vector2(1f, horizontalMoveDistance)),
            _ => Vector3.zero
        };
    }

    Vector3 GetExitDirection(int side)
    {
        return side switch
        {
            0 => Vector3.up,
            1 => Vector3.left,
            2 => Vector3.right,
            _ => Vector3.zero
        };
    }

    public void OnBossTriggerEnter(Collider2D other)
    {
        if (other.CompareTag("Player") || other.CompareTag("invincible"))
        {
            isPlayerInDamageArea = true;
            StartCoroutine(ApplyDamage());
        }
    }

    public void OnBossTriggerExit(Collider2D other)
    {
        if (other.CompareTag("Player") || other.CompareTag("invincible"))
        {
            isPlayerInDamageArea = false;
        }
    }

    void UpdateHealthDisplay()
    {
        int filledBars = Mathf.FloorToInt((float)currentHealth / maxHealth * healthBars.Length);
        filledBars = Mathf.Clamp(filledBars, 0, healthBars.Length);

        for (int i = 0; i < healthBars.Length; i++)
        {
            healthBars[i].gameObject.SetActive(i < filledBars);
        }

    }

    IEnumerator DestroyBoss()
    {
        Destroy(currentBoss);
        healthCanvas.gameObject.SetActive(false);
        isOperating = false;
        yield return null;
    }

    IEnumerator ApplyDamage()
    {
        while (isPlayerInDamageArea && currentHealth < maxHealth)
        {
            currentHealth += damage;
            UpdateHealthDisplay();
            if (currentHealth >= maxHealth)
            {
                StartCoroutine(DestroyBoss());
            }

            yield return new WaitForSeconds(0.5f);
        }
    }

    void Update()
    {
        float percentage = (currentHealth * 100 / maxHealth);
        healthText.SetText($"{Mathf.RoundToInt(percentage)} %");
    }
}
