using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParabolaProjectile : MonoBehaviour
{
    public float height = 3f;
    private Vector3 startPosition;
    private Vector3 targetPosition;
    private float duration;
    private float timer;
    private bool canDamage = false;

    public void Initialize(Vector3 target, float travelTime)
    {
        startPosition = transform.position;
        targetPosition = target;
        duration = travelTime;
        StartCoroutine(EnableDamageOnArrival());
    }

    void Update()
    {
        if (timer > duration) return;

        timer += Time.deltaTime;
        float normalizedTime = timer / duration;

        Vector3 horizontalPos = Vector3.Lerp(startPosition, targetPosition, normalizedTime);
        float verticalPos = height * Mathf.Sin(normalizedTime * Mathf.PI);
        transform.position = horizontalPos + Vector3.up * verticalPos;

        if (normalizedTime >= 1f)
        {
            transform.position = targetPosition;
            Destroy(gameObject, 0.1f);
        }
    }

    IEnumerator EnableDamageOnArrival()
    {
        GetComponent<Collider2D>().enabled = false;
        yield return new WaitForSeconds(duration - 0.1f);
        canDamage = true;
        GetComponent<Collider2D>().enabled = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (canDamage && other.CompareTag("Player"))
        {
            other.GetComponent<PlayerHealth>().TakeDamage(1);
            Destroy(gameObject);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(startPosition, targetPosition);
    }
}
