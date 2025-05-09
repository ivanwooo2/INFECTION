using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileManagerRandom : MonoBehaviour
{
    [System.Serializable]
    public class AttackPattern
    {
        public string name;
        [Tooltip("冷卻時間(秒)")]
        public float cooldown = 2.1f;
        [Tooltip("觸發權重比例")]
        public int weight = 1;
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

        switch (pattern.name)
        {
            case "投石機":
                StartCoroutine(Pattern1Logic());
                break;
            case "苦無":
                StartCoroutine(Pattern2Logic());
                break;
            case "箭":
                StartCoroutine(Pattern3Logic());
                break;
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
}