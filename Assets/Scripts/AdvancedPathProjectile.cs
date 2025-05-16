using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdvancedPathProjectile : MonoBehaviour
{
    [Header("¶¥¬q±±¨î")]
    public float phase1Duration = 5f;
    public float phase2Duration = 1f;
    public float baseSpeed = 3f;

    private Transform player;
    private Vector2 currentDirection;
    private Camera mainCam;
    private bool isPhase2;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        mainCam = Camera.main;
        StartCoroutine(BehaviorRoutine());
    }

    IEnumerator BehaviorRoutine()
    {
        yield return new WaitForSeconds(phase1Duration);

        isPhase2 = true;
        currentDirection = (player.position - transform.position).normalized;
        UpdateRotation();

        float timer = 0;
        while (timer < phase2Duration)
        {
            transform.Translate(currentDirection * baseSpeed * 2 * Time.deltaTime);
            timer += Time.deltaTime;
            yield return null;
        }
        Destroy(gameObject);
    }

    void Update()
    {
        if (!isPhase2)
        {
            Vector2 viewportPos = mainCam.WorldToViewportPoint(transform.position);
            Vector2 centerBias = (Vector2.one * 0.5f - (Vector2)viewportPos).normalized;
            currentDirection = Vector2.Lerp(
                Random.insideUnitCircle.normalized,
                centerBias,
                0.7f
            ).normalized;

            UpdateRotation();
            transform.Translate(currentDirection * baseSpeed * Time.deltaTime);
            ClampPosition();
        }
    }

    void ClampPosition()
    {
        Vector3 viewportPos = mainCam.WorldToViewportPoint(transform.position);
        viewportPos.x = Mathf.Clamp(viewportPos.x, 0.05f, 0.95f);
        viewportPos.y = Mathf.Clamp(viewportPos.y, 0.05f, 0.95f);
        transform.position = mainCam.ViewportToWorldPoint(viewportPos);
    }

    void UpdateRotation()
    {
        float angle = Mathf.Atan2(currentDirection.y, currentDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
}
