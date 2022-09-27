using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPlatformerController : MonoBehaviour
{
    public bool finishedLevel = false;
    private float jumpPower = 7.0f; //JUMP POWER!
    private Rigidbody2D _playerRigidbody;
    private Vector3 originalPos;
    private Vector3 finishedPos;
    private Vector2 smoothV = Vector2.zero;
    
    private void Start()
    {
        _playerRigidbody = GetComponent<Rigidbody2D>();
        originalPos = transform.position;
        finishedPos = transform.position + 10 * transform.right;
    }
    private void Update()
    {
        if (IsGrounded() && !finishedLevel)
        {
            if (Input.GetButtonDown("Jump"))
                Jump();
            originalPos.y = transform.position.y;
            if (transform.position != originalPos)
            {
                transform.position = Vector2.SmoothDamp(transform.position, originalPos, ref smoothV, 1f, 1f);  
            }
        }
        if (finishedLevel)
        {
            transform.position = Vector2.SmoothDamp(transform.position, finishedPos, ref smoothV, 1f, 2f);
        }
    }
    
    private void Jump() => _playerRigidbody.velocity = new Vector2( 0, jumpPower);

    private bool IsGrounded()
    {
        var groundCheck = Physics2D.Raycast(transform.position, Vector2.down, 0.7f, LayerMask.GetMask("Ground"));
        return groundCheck.collider != null;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Obstacles") && !finishedLevel)
        {
            _playerRigidbody.velocity += new Vector2(-3, 5);
        }
    }
}