using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

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

    private GameObject currentBoss;
    private bool isOperating;
    void Start()
    {
        StartCoroutine(InitialEntrance());
    }

    IEnumerator InitialEntrance()
    {
        isOperating = true;

        currentBoss = Instantiate(BossPreFab);
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
            0 => ViewportToWorld(new Vector2(verticalMoveDistance, 1.1f)),
            1 => ViewportToWorld(new Vector2(-1f, horizontalMoveDistance)),
            2 => ViewportToWorld(new Vector2(1f, horizontalMoveDistance)),
            _ => Vector3.zero
        };
    }

    Vector3 GetSideTargetPosition(int side)
    {
        return side switch
        {
            0 => ViewportToWorld(new Vector2(verticalMoveDistance, 0.8f)),
            1 => ViewportToWorld(new Vector2(0.2f, horizontalMoveDistance)),
            2 => ViewportToWorld(new Vector2(0.8f, horizontalMoveDistance)),
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

    void Update()
    {
        
    }
}
