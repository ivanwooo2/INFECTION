using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class missileGroup : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    void Start()
    {
        
    }

    void Update()
    {
        transform.position += Vector3.down * moveSpeed * 1.5f * Time.deltaTime;
    }
}
