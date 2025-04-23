using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstBoss : MonoBehaviour
{
    [SerializeField] public float moveSpeed;
    [SerializeField] public float targetY;

    private Vector3 targetPosition;
    private bool isMoving = true;
    // Start is called before the first frame update
    void Start()
    {
        Vector3 screenTop = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, targetY, 10));
        transform.position = screenTop;
    }

    // Update is called once per frame
    void Update()
    {
        if (isMoving)
        {
            transform.position = Vector3.Lerp(
                transform.position,
                targetPosition,
                moveSpeed * Time.deltaTime
            );

            if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
            {
                isMoving = false;
            }
        }
    }
}
