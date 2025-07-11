using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Stage2BossController : MonoBehaviour
{
    [SerializeField] private GameObject projectileManager;

    [SerializeField] private GameObject playerDamageEffect;

    private TimeManager TimeManager;
    [SerializeField] public GameObject BossPreFab;
    private Animator BossAnimator;
    private GameObject Boss;
    [SerializeField] private GameObject LastBossPrefab;
    [SerializeField] public float minInterval;
    [SerializeField] public float maxInterval;
    [SerializeField] public float moveSpeed;
    [SerializeField] public float stayDuration;
    [SerializeField] private soldierManager SoldierPrefab;
    [SerializeField] private GameObject[] soldierPos;

    [SerializeField] public Vector2 initialSpawnViewport;
    [SerializeField] public Vector2 initialTargetViewport;
    [SerializeField] public Vector2 initialExitViewport;

    [SerializeField] public float horizontalMoveDistance;

    [SerializeField] private Image[] healthBars;
    [SerializeField] private Canvas healthCanvas;
    [SerializeField] private float maxHealth = 300;
    [SerializeField] private float currentHealth;
    private bool isPlayerInDamageArea;
    [SerializeField] public float damage;
    private float originalDamage;
    private float doubleDamage;
    [SerializeField] private float damageMult;
    [SerializeField] private float damageInterval;
    [SerializeField] private float SkilldamageInterval;
    [SerializeField] private float originDamageInterval;
    [SerializeField] private float activeDamageInterval;
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
    public AudioSource arc, arc2;
    public AudioClip attack1, attack2, DestroySE;

    public GameObject currentBoss;

    private GameObject LastBoss;
    private PlayerHealth playerHealth;
    private bool isOperating;

    [SerializeField] GameObject Crossfade;
    [SerializeField] Animator transition;
    [SerializeField] float transitionTime = 1f;
    [SerializeField] GameObject BossDestroyPrefab;
    [SerializeField] int randomx;
    [SerializeField] int randomy;
    [SerializeField] float EffectrandomZ;
    private bool isPhase2 = false;
    private bool phase2Triggered = false;
    private bool isSpawn;
    private bool isExit;
    public bool IsBossDead { get; private set; } = false;
    void Start()
    {
        transition = Crossfade.GetComponent<Animator>();
        transition.SetBool("Start", false);
        originDamageInterval = damageInterval;
        activeDamageInterval = originDamageInterval;
        TimeManager = GetComponent<TimeManager>();
        originalDamage = damage;
        doubleDamage = damage * damageMult;
        timeManager = TimeManager.Instance;
        InitializeHealthSystem();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        playerHealth = playerTransform.GetComponent<PlayerHealth>();
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
        Boss = currentBoss.transform.Find("Boss").gameObject;
        //BossAnimator = currentBoss.GetComponent<Animator>();
        UpdateHealthDisplay();
        Vector3 startPos = ViewportToWorld(initialSpawnViewport);
        Vector3 targetPos = ViewportToWorld(initialTargetViewport);

        currentBoss.transform.position = startPos;
        isSpawn = true;
        yield return StartCoroutine(MoveTo(targetPos));

        yield return new WaitForSeconds(stayDuration);

        Vector3 exitPos = ViewportToWorld(initialExitViewport);
        //yield return new WaitForSeconds(5f);
        isExit = true;
        yield return StartCoroutine(MoveTo(exitPos));

        if (currentWeakPoint != null)
        {
            Destroy(currentWeakPoint);
            isWeakPointActive = false;
        }

        Destroy(currentBoss);
        isOperating = false;
        StartCoroutine(SpawnLoop());
    }

    IEnumerator SpawnLoop()
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

        int side = Random.Range(0, 2);

        Vector3 spawnPos = GetSideSpawnPosition(side);
        Vector3 targetPos = GetSideTargetPosition(side);
        Vector3 exitPos = GetExitDirection(side);

        currentBoss = Instantiate(BossPreFab);
        Boss = currentBoss.transform.Find("Boss").gameObject;
        //BossAnimator = currentBoss.GetComponent<Animator>();
        currentBoss.transform.position = spawnPos;
        if (side == 1)
        {
            currentBoss.transform.rotation = Quaternion.Euler(0, 180, 0);
        }
        isSpawn = true;
        yield return StartCoroutine(MoveTo(targetPos));

        yield return new WaitForSeconds(stayDuration);
        //yield return new WaitForSeconds(5f);
        isExit = true;
        yield return StartCoroutine(MoveTo(exitPos));
        if (currentWeakPoint != null)
        {
            Destroy(currentWeakPoint);
            isWeakPointActive = false;
        }

        Destroy(currentBoss);
        isOperating = false;
    }

    Vector3 GetSideSpawnPosition(int side)
    {
        return side switch
        {
            0 => ViewportToWorld(new Vector2(-0.5f, horizontalMoveDistance)),
            1 => ViewportToWorld(new Vector2(1.3f, horizontalMoveDistance)),
            _ => Vector3.zero
        };
    }

    Vector3 GetSideTargetPosition(int side)
    {
        return side switch
        {
            0 => ViewportToWorld(new Vector2(0.01f, horizontalMoveDistance)),
            1 => ViewportToWorld(new Vector2(0.99f, horizontalMoveDistance)),
            _ => Vector3.zero
        };
    }

    Vector3 GetExitDirection(int side)
    {
        return side switch
        {
            0 => ViewportToWorld(new Vector2(1.3f, horizontalMoveDistance)),
            1 => ViewportToWorld(new Vector2(-0.5f, horizontalMoveDistance)),
            _ => Vector3.zero
        };
    }

    Vector3 ViewportToWorld(Vector2 viewportPos)
    {
        return Camera.main.ViewportToWorldPoint(new Vector3(viewportPos.x, viewportPos.y, 10));
    }
    IEnumerator MoveTo(Vector3 target)
    {
        if (timeManager.isGameOver == true)
        {
            yield return null;
        }
        Vector3 startPos = currentBoss.transform.position;
        float progress = 0;

        if (isSpawn)
        {
            while (progress < 1)
            {
                progress += Time.deltaTime * moveSpeed;
                currentBoss.transform.position = Vector3.Lerp(startPos, target, progress);
                yield return null;

            }
            isSpawn = false;
        }
        if (isExit)
        {
            bool Spawnsoldier = false;
            while (progress < 1)
            {
                progress += Time.deltaTime * moveSpeed / 5;
                currentBoss.transform.position = Vector3.Lerp(startPos, target, progress);
                if (currentBoss.transform.position.x >= -0.1f && currentBoss.transform.position.x <= 0.1f && Spawnsoldier == false)
                {
                    yield return new WaitForSeconds(0.5f);
                    for (int i = 0; i < soldierPos.Length; i++)
                    {
                        soldierManager Soldier = Instantiate<soldierManager>(SoldierPrefab, currentBoss.transform.position,Quaternion.identity);
                        StartCoroutine(Soldier.GoToTarget(soldierPos[i].transform.position, currentBoss.transform.position, 2f));
                    }
                    Spawnsoldier = true;
                }
                else
                {
                    yield return null;
                }
            }
            isExit = false;
            Spawnsoldier = true;
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

    public void OnBossTriggerEnter(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            //BossAnimator.SetBool("isHitting", true);
            isPlayerInDamageArea = true;
            StartCoroutine(ApplyDamage());
        }
    }

    public void OnBossTriggerExit(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            //BossAnimator.SetBool("isHitting", false);
            isPlayerInDamageArea = false;
        }
    }

    IEnumerator ApplyDamage()
    {
        while (isPlayerInDamageArea && currentHealth < maxHealth)
        {
            if (currentBoss != null)
            {
                arc.clip = attack1;
                arc.Play();
                GameObject EffectClone = Instantiate(playerDamageEffect, currentBoss.transform);
                EffectClone.transform.rotation = Quaternion.Euler(0, 0, EffectrandomZ);
                Boss.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.5f);
                currentHealth += damage;
                UpdateHealthDisplay();
                yield return new WaitForSeconds(activeDamageInterval / 2);
                Boss.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1f);
                if (currentHealth >= maxHealth)
                {
                    StartCoroutine(DestroyBoss());
                }

                yield return new WaitForSeconds(activeDamageInterval / 2);
            }

        }
    }
    IEnumerator DestroyBoss()
    {
        IsBossDead = true;
        timeManager.isGameOver = true;
        playerHealth.isInvincible = true;
        if (projectileManager != null)
        {
            var manager = projectileManager.GetComponent<ProjectileManagerRandom>();
            if (manager != null)
            {
                manager.CleanupProjectiles();
            }
        }
        StopCoroutine(SpawnLoop());
        if (currentBoss != null)
        {
            Destroy(currentBoss);
        }
        LastBoss = Instantiate(LastBossPrefab);
        LastBoss.transform.position = new Vector3(0, 0, 0);
        GameObject LastBossDestroyEffect = Instantiate(BossDestroyPrefab);
        LastBossDestroyEffect.transform.position = new Vector3(randomx, randomy, 0);
        LastBoss.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.9f);
        arc.clip = DestroySE;
        arc.Play();
        playerHealth.isInvincible = true;
        yield return new WaitForSeconds(0.3f);
        LastBoss.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.8f);
        arc.clip = DestroySE;
        arc.Play();
        LastBossDestroyEffect = Instantiate(BossDestroyPrefab);
        LastBossDestroyEffect.transform.position = new Vector3(randomx, randomy, 0);
        playerHealth.isInvincible = true;
        yield return new WaitForSeconds(0.3f);
        LastBoss.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.7f);
        arc.clip = DestroySE;
        arc.Play();
        LastBossDestroyEffect = Instantiate(BossDestroyPrefab);
        LastBossDestroyEffect.transform.position = new Vector3(randomx, randomy, 0);
        playerHealth.isInvincible = true;
        yield return new WaitForSeconds(0.3f);
        LastBoss.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.6f);
        arc.clip = DestroySE;
        arc.Play();
        LastBossDestroyEffect = Instantiate(BossDestroyPrefab);
        LastBossDestroyEffect.transform.position = new Vector3(randomx, randomy, 0);
        playerHealth.isInvincible = true;
        yield return new WaitForSeconds(0.3f);
        LastBoss.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.5f);
        arc.clip = DestroySE;
        arc.Play();
        LastBossDestroyEffect = Instantiate(BossDestroyPrefab);
        LastBossDestroyEffect.transform.position = new Vector3(randomx, randomy, 0);
        playerHealth.isInvincible = true;
        yield return new WaitForSeconds(0.3f);
        LastBoss.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.4f);
        arc.clip = DestroySE;
        arc.Play();
        LastBossDestroyEffect = Instantiate(BossDestroyPrefab);
        LastBossDestroyEffect.transform.position = new Vector3(randomx, randomy, 0);
        playerHealth.isInvincible = true;
        yield return new WaitForSeconds(0.3f);
        LastBoss.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.3f);
        arc.clip = DestroySE;
        arc.Play();
        LastBossDestroyEffect = Instantiate(BossDestroyPrefab);
        LastBossDestroyEffect.transform.position = new Vector3(randomx, randomy, 0);
        playerHealth.isInvincible = true;
        yield return new WaitForSeconds(0.3f);
        LastBoss.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.2f);
        arc.clip = DestroySE;
        arc.Play();
        LastBossDestroyEffect = Instantiate(BossDestroyPrefab);
        LastBossDestroyEffect.transform.position = new Vector3(randomx, randomy, 0);
        playerHealth.isInvincible = true;
        yield return new WaitForSeconds(0.3f);
        LastBoss.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.1f);
        arc.clip = DestroySE;
        arc.Play();
        LastBossDestroyEffect = Instantiate(BossDestroyPrefab);
        LastBossDestroyEffect.transform.position = new Vector3(randomx, randomy, 0);
        playerHealth.isInvincible = true;
        yield return new WaitForSeconds(0.3f);
        LastBoss.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0f);
        arc.clip = DestroySE;
        arc.Play();
        LastBossDestroyEffect = Instantiate(BossDestroyPrefab);
        LastBossDestroyEffect.transform.position = new Vector3(randomx, randomy, 0);
        playerHealth.isInvincible = true;
        transition.SetBool("Start", true);
        yield return new WaitForSeconds(transitionTime);
        timeManager.LoadResultScene();
        yield return null;
    }
    void Update()
    {
        float percentage = (currentHealth * 100 / maxHealth);
        healthText.SetText($"{Mathf.RoundToInt(percentage)} %");

        if (i > 0)
        {
            i -= Time.deltaTime;
        }

        if (i <= 0)
        {
            damage = originalDamage;
            activeDamageInterval = originDamageInterval;
        }

        randomx = Random.Range(-3, 3);
        randomy = Random.Range(-3, 3);
        EffectrandomZ = Random.Range(-360, 360);
    }
}
