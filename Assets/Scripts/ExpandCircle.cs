using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpandCircle : MonoBehaviour
{
    public Transform outerCircle;  
    public Transform innerCircle;
    public GameObject expandCircle;

    public float expandDuration = 2f;

    public GameObject projectilePrefab;
    public float projectileSpeed = 10f;

    private Vector3 finalScale;
    private bool isExpanding;

    void Start()
    {
        finalScale = outerCircle.localScale;
        innerCircle.localScale = Vector3.one * 0.1f;
        StartCoroutine(ExpandInnerCircle());
        LaunchProjectile();
    }

    IEnumerator ExpandInnerCircle()
    {
        isExpanding = true;
        float timer = 0;
        Vector3 initialScale = innerCircle.localScale;

        while (timer < expandDuration)
        {
            float progress = timer / expandDuration;
            innerCircle.localScale = Vector3.Lerp(initialScale, finalScale, progress);
            timer += Time.deltaTime;
            yield return null;
        }

        innerCircle.localScale = finalScale;
        isExpanding = false;
        Destroy(expandCircle);
    }

    void LaunchProjectile()
    {
        Vector3 spawnPos = Camera.main.ViewportToWorldPoint(new Vector2(0.5f, 1.1f));
        GameObject projectile = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);
        projectile.GetComponent<ParabolaProjectile>().Initialize(
            target: transform.position,
            travelTime: expandDuration
        );
    }
}
