using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileManagerRandom : MonoBehaviour
{
    [System.Serializable]
    public class AttackPattern
    {
        public string name;
        public float cooldown = 2.1f;
        public int weight = 1;

        [Header("鎖定彈幕專用參數")]
        public bool isLockOnAttack = false;
        public GameObject lockOnLaserPrefab;
        public Transform[] laserSpawnPoints;

        [Header("新彈幕專用參數")]
        public bool isAdvancedType = false;
        public GameObject advancedProjectilePrefab;
        public float phase1Duration = 5f;
        public float phase2Duration = 1f;
        public float moveSpeed = 3f;

        [HideInInspector]
        public bool isReady = true;
    }

    public GameObject linearBombPrefab;
    public GameObject warningProjectilePrefab;
    [SerializeField] private GameObject expandCirclePrefab;

    public AttackPattern[] patterns = new AttackPattern[3];

    private bool isGlobalCooldown = false;
    private List<AttackPattern> availablePatterns = new List<AttackPattern>();

    void Start()
    {
        StartCoroutine(AttackScheduler());
    }

    IEnumerator AttackScheduler()
    {
        yield return new WaitForSeconds(2);
        while (true)
        {
            yield return new WaitUntil(() => !isGlobalCooldown);
            UpdateAvailablePatterns();

            if (availablePatterns.Count > 0)
            {
                AttackPattern selected = SelectByWeight();
                isGlobalCooldown = true;

                StartCoroutine(ExecuteAttack(selected));

                yield return new WaitForSeconds(selected.cooldown);
                isGlobalCooldown = false;
            }
            yield return null;
        }
    }

    void UpdateAvailablePatterns()
    {
        availablePatterns.Clear();
        foreach (AttackPattern pattern in patterns)
        {
            if (pattern.isReady) availablePatterns.Add(pattern);
        }
    }

    AttackPattern SelectByWeight()
    {
        int totalWeight = 0;
        foreach (AttackPattern p in availablePatterns)
        {
            totalWeight += p.weight;
        }

        int randomPoint = Random.Range(0, totalWeight);
        int currentWeight = 0;

        foreach (AttackPattern p in availablePatterns)
        {
            currentWeight += p.weight;
            if (randomPoint < currentWeight)
            {
                return p;
            }
        }
        return availablePatterns[0];
    }

    IEnumerator ExecuteAttack(AttackPattern pattern)
    {
        pattern.isReady = false;
        Debug.Log($"觸發攻擊: {pattern.name}");

        if (pattern.isAdvancedType)
        {
            Vector2 spawnPos = GetScreenEdgeSpawnPosition();
            GameObject projectile = Instantiate(
                pattern.advancedProjectilePrefab,
                spawnPos,
                Quaternion.identity
            );

            AdvancedPathProjectile ap = projectile.GetComponent<AdvancedPathProjectile>();
            ap.phase1Duration = pattern.phase1Duration;
            ap.phase2Duration = pattern.phase2Duration;
            ap.baseSpeed = pattern.moveSpeed;

            yield return new WaitUntil(() => projectile == null);
        }
        else if (pattern.isLockOnAttack)
        {
            GameObject laserObj = Instantiate(
                pattern.lockOnLaserPrefab,
                Vector3.zero,
                Quaternion.identity
            );
            LockOnLaser laserComp = laserObj.GetComponent<LockOnLaser>();
            laserComp.Initialize(pattern.laserSpawnPoints);

            yield return new WaitUntil(() => laserComp.IsComplete);
            Destroy(laserObj);
        }
        else
        {
            switch (pattern.name)
            {
                case "投石機":
                    yield return StartCoroutine(Pattern1Logic());
                    break;
                case "苦無":
                    yield return StartCoroutine(Pattern2Logic());
                    break;
                case "箭":
                    yield return StartCoroutine(Pattern3Logic());
                    break;
            }
        }

        StartCoroutine(CooldownTimer(pattern));
        yield return null;
    }

    IEnumerator CooldownTimer(AttackPattern pattern)
    {
        yield return new WaitForSeconds(pattern.cooldown);
        pattern.isReady = true;
    }

    IEnumerator Pattern1Logic()
    {
        Vector2 spawnPos = new Vector2(
    Random.Range(Camera.main.ViewportToWorldPoint(new Vector2(0.2f, 0)).x,
                 Camera.main.ViewportToWorldPoint(new Vector2(0.8f, 0)).x),
    Random.Range(Camera.main.ViewportToWorldPoint(new Vector2(0, 0.2f)).y,
                 Camera.main.ViewportToWorldPoint(new Vector2(0, 0.8f)).y)
);

        GameObject circle = Instantiate(expandCirclePrefab, spawnPos, Quaternion.identity);
        yield return new WaitForSeconds(1f);
        yield return new WaitUntil(() => circle == null);
    }

    IEnumerator Pattern2Logic()
    {
        bool isLeft = Random.value > 0.5f;
        Vector2 spawnPos = GetSpawnPosition(isLeft);

        GameObject bomb = Instantiate(linearBombPrefab, spawnPos, Quaternion.identity);
        bomb.GetComponent<LinearBomber>().InitializeDirection(isLeft);

        yield return new WaitUntil(() => bomb == null);
        yield return new WaitForSeconds(1f);
    }

    Vector2 GetSpawnPosition(bool isLeft)
    {
        float yPos = Random.Range(0.2f, 0.8f);
        Vector2 viewportPos = new Vector2(
            isLeft ? -0.1f : 1.1f,
            yPos
        );
        return Camera.main.ViewportToWorldPoint(viewportPos);
    }

    IEnumerator Pattern3Logic()
    {
        GameObject projectile = Instantiate(
           warningProjectilePrefab,
           Vector3.zero,
           Quaternion.identity
       );
        yield return new WaitUntil(() => projectile == null);
        yield return new WaitForSeconds(1f);
    }

    Vector2 GetScreenEdgeSpawnPosition()
    {
        Camera cam = Camera.main;
        int edge = Random.Range(0, 4); // 0:上 1:下 2:左 3:右
        Vector2 viewportPos = Vector2.zero;

        switch (edge)
        {
            case 0: viewportPos = new Vector2(Random.Range(0.1f, 0.9f), 1.1f); break;
            case 1: viewportPos = new Vector2(Random.Range(0.1f, 0.9f), -0.1f); break;
            case 2: viewportPos = new Vector2(-0.1f, Random.Range(0.1f, 0.9f)); break;
            case 3: viewportPos = new Vector2(1.1f, Random.Range(0.1f, 0.9f)); break;
        }

        return cam.ViewportToWorldPoint(viewportPos);
    }
}