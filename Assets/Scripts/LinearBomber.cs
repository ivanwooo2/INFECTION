using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinearBomber : MonoBehaviour
{
    public float moveSpeed = 8f;
    public float minMoveDistance = 3f;
    public float maxMoveDistance = 8f;

    public float explosionRadius = 1.5f;
    public GameObject fragmentPrefab;
    public int fragmentCount = 5;
    public float fragmentSpeed = 6f;

    private Vector2 moveDirection;
    private float moveDistance;
    private Vector2 spawnPosition;
    private bool hasExploded = false;

    void Start()
    {
        moveDirection = GetInitialDirection();
        moveDistance = Random.Range(minMoveDistance, maxMoveDistance);
        spawnPosition = transform.position;
    }

    Vector2 GetInitialDirection()
    {
        Vector3 viewportPos = Camera.main.WorldToViewportPoint(transform.position);
        return viewportPos.x < 0.5f ? Vector2.right : Vector2.left;
    }

    void Update()
    {
        if (hasExploded) return;

        transform.Translate(moveDirection * moveSpeed * Time.deltaTime);

        if (Vector2.Distance(spawnPosition, transform.position) >= moveDistance)
        {
            Explode();
        }
    }

    void Explode()
    {
        hasExploded = true;
        GetComponent<SpriteRenderer>().enabled = false;

        SpawnFragments();

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                hit.GetComponent<PlayerHealth>().TakeDamage(1);
            }
        }

        Destroy(gameObject, 0.5f);
    }

    void SpawnFragments()
    {
        float angleStep = 360f / fragmentCount;
        Vector2 explosionPos = transform.position;

        for (int i = 0; i < fragmentCount; i++)
        {
            float angle = i * angleStep;
            Vector2 dir = new Vector2(
                Mathf.Cos(angle * Mathf.Deg2Rad),
                Mathf.Sin(angle * Mathf.Deg2Rad)
            );

            GameObject fragment = Instantiate(
                fragmentPrefab,
                explosionPos,
                Quaternion.identity
            );

            fragment.GetComponent<Rigidbody2D>().velocity = dir * fragmentSpeed;
        }
    }

    public void InitializeDirection(bool isLeftSide)
    {
        moveDirection = isLeftSide ? Vector2.right : Vector2.left;
        spawnPosition = transform.position;

        Debug.DrawLine(spawnPosition,
            spawnPosition + moveDirection * moveDistance,
            Color.red, 2f);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
