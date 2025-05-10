using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockOnLaser : MonoBehaviour
{
    [Header("í«ÁFê›íË")]
    public float trackingDuration = 3f;
    public float attackDelay = 1f;
    public GameObject projectilePrefab;

    private Transform[] spawnPoints;
    private Transform player;
    private PlayerHealth playerHealth;
    private LineRenderer laserLine;
    public bool IsComplete { get; private set; }

    public void Initialize(Transform[] points)
    {
        spawnPoints = points;
        player = GameObject.FindGameObjectWithTag("Player").transform;
        laserLine = GetComponent<LineRenderer>();
        StartCoroutine(LockOnRoutine());
    }

    IEnumerator LockOnRoutine()
    {
        laserLine.positionCount = spawnPoints.Length * 2;
        IsComplete = false;

        float timer = 0;
        while (timer < trackingDuration)
        {
            UpdateLaserPositions();
            timer += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(attackDelay);
        FireProjectiles();
        IsComplete = true;
    }

    void UpdateLaserPositions()
    {
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            Vector3 startPos = spawnPoints[i].position;
            Vector3 endPos = player.position;
            laserLine.SetPosition(i * 2, startPos);
            laserLine.SetPosition(i * 2 + 1, endPos);
        }
    }

    void FireProjectiles()
    {
        foreach (Transform point in spawnPoints)
        {
            Vector3 direction = (player.position - point.position).normalized;
            GameObject projectile = Instantiate(
                projectilePrefab,
                point.position,
                Quaternion.LookRotation(direction)
            );
            projectile.GetComponent<LockProjectile>().Initialize(direction, player.gameObject,player.gameObject);
        }
    }
}
