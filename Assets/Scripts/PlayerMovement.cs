using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float playermoveSpeed;
    [SerializeField] private Rigidbody2D playerRB;
    private Vector2 playerPosition;

    private float activeMoveSpeed;
    [SerializeField] private float dashSpeed;
    [SerializeField] private float dashLength;
    [SerializeField] private float dashCooldown;
    private float dashCooldownTime;
    private float dashLengthCounter;

    [SerializeField] private TrailRenderer playerTR;

    // Start is called before the first frame update
    void Start()
    {
        activeMoveSpeed = playermoveSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        playerPosition.x = Input.GetAxisRaw("Horizontal");
        playerPosition.y = Input.GetAxisRaw("Vertical");

        playerPosition.Normalize();

        playerRB.velocity = playerPosition * activeMoveSpeed;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (dashCooldownTime <= 0 && dashLengthCounter <=0 )
            {
                playerTR.emitting = true;
                activeMoveSpeed = dashSpeed;
                dashLengthCounter = dashLength;
            }
        }

        if (dashLengthCounter > 0)
        {
            dashLengthCounter -= Time.deltaTime;

            if (dashLengthCounter <= 0)
            {
                activeMoveSpeed = playermoveSpeed;
                dashCooldownTime = dashCooldown;
                playerTR.emitting = false;
            }
        }

        if (dashCooldownTime > 0)
        {
            dashCooldownTime -= Time.deltaTime;
        }
    }
}
