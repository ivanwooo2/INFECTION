using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FragmentProjectile : MonoBehaviour
{
    public int damage = 1;
    public float lifetime = 2f;

    private GameObject player;
    private PlayerHealth playerHealth;
    private PlayerMovement playerMovement;

    private Transform _transform;
    [SerializeField] private Vector3 rotation;

    void Start()
    {
        _transform = transform;
        player = GameObject.FindGameObjectWithTag("Player");
        playerHealth = player.GetComponent<PlayerHealth>();
        playerMovement = player.GetComponent<PlayerMovement>();
        Destroy(gameObject, lifetime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !playerHealth.isInvincible  && !playerMovement.isInvincible)
        {
            other.GetComponent<PlayerHealth>().TakeDamage(damage);
            Destroy(gameObject);
        }
    }

    void Update()
    {
        _transform.Rotate(rotation * Time.deltaTime);
    }
}
