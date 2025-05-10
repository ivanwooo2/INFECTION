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
    [SerializeField] private float maxHealth = 300;
    [SerializeField] private float currentHealth;
    private bool isPlayerInDamageArea;
    [SerializeField] public float damage;
    private float originalDamage;
    private float doubleDamage;
    private float i;

    [SerializeField] private TMP_Text healthText;

    [SerializeField] private GameObject weakPointPrefab;
    [SerializeField] private float weakPointDuration = 5f;
    [SerializeField] private int weakPointDamage = 50;
    [SerializeField] private float dashCheckDistance = 3f;
    [SerializeField] private Vector2[] weakPointOffsets;

    private TimeManager timeManager;

    private GameObject currentWeakPoint;
    private bool isWeakPointActive;
    private Transform playerTransform;
    private List<Vector3> currentWeakPointPositions = new List<Vector3>();

    private GameObject currentBoss;
    private bool isOperating;
    void Start()
    {
        originalDamage = damage;
        doubleDamage = damage * 2;
        timeManager = TimeManager.Instance;
        InitializeHealthSystem();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        StartCoroutine(InitialEntrance());
        StartCoroutine(WeakPointSpawnLoop());
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
            UpdateWeakPointPositions();
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
        //Destroy(currentBoss);
        //healthCanvas.gameObject.SetActive(false);
        //isOperating = false;
        timeManager.LoadResultScene();
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

    void DealWeakPointDamage()
    {
        currentHealth = Mathf.Clamp(currentHealth + weakPointDamage, 0, maxHealth);

        UpdateHealthDisplay();

        if (currentHealth >= maxHealth)
        {
            StartCoroutine(DestroyBoss());
        }

        // AudioManager.Instance.Play("WeakPointHit");
    }

    void UpdateWeakPointPositions()
    {
        currentWeakPointPositions.Clear();
        if (currentBoss == null) return;

        Vector3 bossPos = currentBoss.transform.position;
        foreach (Vector2 offset in weakPointOffsets)
        {
            Vector3 worldOffset = new Vector3(offset.x, offset.y, 0);
            currentWeakPointPositions.Add(bossPos + worldOffset);
        }
    }

    IEnumerator WeakPointSpawnLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(5f, 15f));
            if (!isWeakPointActive && currentBoss != null)
            {
                UpdateWeakPointPositions();
                SpawnClosestWeakPoint();
            }
        }
    }

    void SpawnClosestWeakPoint()
    {

        Vector3 closestPos = Vector3.zero;
        float minDistance = Mathf.Infinity;
        foreach (Vector3 pos in currentWeakPointPositions)
        {
            float distance = Vector3.Distance(playerTransform.position, pos);
            if (distance < minDistance)
            {
                minDistance = distance;
                closestPos = pos;
            }
        }

        if (closestPos != Vector3.zero)
        {
            currentWeakPoint = Instantiate(weakPointPrefab, closestPos, Quaternion.identity);
            isWeakPointActive = true;
            StartCoroutine(RemoveWeakPointAfterDelay());
        }
    }

    IEnumerator RemoveWeakPointAfterDelay()
    {
        yield return new WaitForSeconds(weakPointDuration);
        if (currentWeakPoint != null)
        {
            Destroy(currentWeakPoint);
            isWeakPointActive = false;
        }
    }

    public void ChrSkill1(float duration)
    {
        if (duration > 0 )
        {
            i = duration;
            damage = doubleDamage;
        }
    }

    void TryAttackWeakPoint()
    {
        if (isWeakPointActive && currentWeakPoint != null)
        {
            float distance = Vector3.Distance(
                playerTransform.position,
                currentWeakPoint.transform.position
            );

            if (distance <= dashCheckDistance)
            {
                DealWeakPointDamage();
                Destroy(currentWeakPoint);
                isWeakPointActive = false;
            }
        }
    }


    void Update()
    {
        float percentage = (currentHealth * 100 / maxHealth);
        healthText.SetText($"{Mathf.RoundToInt(percentage)} %");

        if (Input.GetKeyDown(KeyCode.Space))
        {
            TryAttackWeakPoint();
        }

        if ( i > 0)
        {
            i -= Time.deltaTime;
        }

        if ( i <= 0 )
        {
            damage = originalDamage;
        }
    }


}
