using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarningProjectileController : MonoBehaviour
{
    [SerializeField] private GameObject warningIndicator;
    [SerializeField] private GameObject Projectile;
    [SerializeField] private float flashInterval = 0.2f;  
    [SerializeField] private int flashCount = 3;          
    [SerializeField] private Color flashColor = Color.red;

    [SerializeField] private float projectileSpeed = 20f; 
    [SerializeField] private float aliveTime = 3f;       
    [SerializeField] private int damage = 1;             

    private SpriteRenderer warningRenderer;
    private Rigidbody2D rb;
    private bool isActivated = false;
    [SerializeField] private float downwardRotation;
    [SerializeField] private float leftwardRotation;

    private GameObject player;
    private PlayerHealth playerHealth;
    private PlayerMovement playerMovement;

    void Awake()
    {
        warningRenderer = warningIndicator.GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        InitializePosition();
        player = GameObject.FindGameObjectWithTag("Player");
        playerHealth = player.GetComponent<PlayerHealth>();
        playerMovement = player.GetComponent<PlayerMovement>();
    }

    void Start()
    {
        Projectile.SetActive(false);
        StartCoroutine(ActivationSequence());
    }

    void InitializePosition()
    {
        Vector2 spawnPos = GetEdgeSpawnPosition();
        transform.position = spawnPos;
    }

    IEnumerator ActivationSequence()
    {
        for (int i = 0; i < flashCount; i++)
        {
            warningRenderer.color = flashColor;
            yield return new WaitForSeconds(flashInterval);
            warningRenderer.color = new Color(0, 0, 0, 0);
            yield return new WaitForSeconds(flashInterval);
        }

        isActivated = true;
        warningIndicator.SetActive(false);
        Projectile.SetActive(true);
        LaunchProjectile();
        Destroy(gameObject, aliveTime);
    }

    void LaunchProjectile()
    {
        Vector2 direction = GetLaunchDirection();
        if (direction == Vector2.down)
        {
            rb.transform.rotation = Quaternion.Euler(0, 0, downwardRotation);
        }
        if (direction == Vector2.left)
        {
            rb.transform.rotation = Quaternion.Euler(0, 0, leftwardRotation);
        }
        rb.velocity = direction * projectileSpeed;
    }

    Vector2 GetLaunchDirection()
    {
        Vector2 viewportPos = Camera.main.WorldToViewportPoint(transform.position);

        if (viewportPos.x < 0.1f) return Vector2.right;    
        if (viewportPos.x > 0.9f) return Vector2.left;     
        if (viewportPos.y > 0.9f) return Vector2.down;     
        return Vector2.zero;
    }

    Vector2 GetEdgeSpawnPosition()
    {
        int side = Random.Range(0, 3); 
        Vector2 viewportPos = Vector2.zero;

        switch (side)
        {
            case 0: 
                viewportPos = new Vector2(0.05f, Random.Range(0.2f, 0.8f));
                break;
            case 1: 
                viewportPos = new Vector2(0.95f, Random.Range(0.2f, 0.8f));
                break;
            case 2: 
                viewportPos = new Vector2(Random.Range(0.2f, 0.8f), 0.95f);
                break;
        }

        return Camera.main.ViewportToWorldPoint(viewportPos);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (isActivated && other.CompareTag("Player") && !playerHealth.isInvincible && !playerMovement.isInvincible)
        {
            other.GetComponent<PlayerHealth>().TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}
