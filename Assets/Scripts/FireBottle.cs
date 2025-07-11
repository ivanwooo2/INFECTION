using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBottle : MonoBehaviour
{
    public float height = 3f;
    private Vector3 startPosition;
    public Vector3 targetPosition;
    private float duration;
    private float timer;
    private GameObject SoundEffect;
    public AudioSource stone;
    public AudioClip sfx1, sfx2;
    private Transform _transform;
    private ExpandCircle parentCircle;
    [SerializeField] private Vector3 rotation;
    [SerializeField] private GameObject Fire;

    void Start()
    {
        GetComponent<Collider2D>().enabled = false;
        _transform = transform;
        SoundEffect = GameObject.FindGameObjectWithTag("SoundEffectPlayer");
        stone = SoundEffect.GetComponent<AudioSource>();
        stone.clip = sfx1;
        stone.Play();
    }
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
        _transform.Rotate(rotation * Time.deltaTime);
        timer += Time.deltaTime;
        float normalizedTime = timer / duration;

        Vector3 horizontalPos = Vector3.Lerp(startPosition, targetPosition, normalizedTime);
        float verticalPos = height * Mathf.Sin(normalizedTime * Mathf.PI);
        transform.position = horizontalPos + Vector3.up * verticalPos;

        if (normalizedTime >= 1f)
        {
            stone.clip = sfx2;
            stone.Play();
            transform.position = targetPosition;

            Destroy(gameObject, 0.1f);
        }
    }

    IEnumerator EnableDamageOnArrival()
    {
        yield return new WaitForSeconds(duration);
        GameObject FireEffect = Instantiate(Fire,transform.position,Quaternion.identity);
        yield return new WaitForSeconds(0.1f);
        Destroy(gameObject);
    }
}
