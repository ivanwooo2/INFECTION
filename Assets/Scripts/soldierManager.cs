using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class soldierManager : MonoBehaviour
{
    private GameObject Player;
    [SerializeField] private float LockSpeed;
    void Start()
    {
        Player = GameObject.FindWithTag("Player");
    }

    void Update()
    {
        Vector3 direction = Player.transform.position - transform.position;
        float angle = Mathf.Atan2 (direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle - 90);
    }
}
