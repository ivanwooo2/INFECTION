using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletManager : MonoBehaviour
{
    private GameObject Player;
    private Vector3 direction;
    [SerializeField] private float BulletSpeed;
    void Start()
    {
        Player = GameObject.FindWithTag("Player");
        direction = (Player.transform.position - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle - 90);
    }

    void Update()
    {
       transform.position += direction * BulletSpeed * 1f * Time.deltaTime;
    }
}
