using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    #region Variables

    public float speed = 200f;
    public float rotateTime = 0.5f;

    public GameObject playerLight;

    public Distraction coin;

    private EnemyAI[] enemies;

    private Vector2 movement;

    private Rigidbody2D rb;

    #endregion

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        
        enemies = FindObjectsOfType<EnemyAI>();
    }

    private void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        movement.Set(horizontal, vertical);
        movement.Normalize();
        
        if (Input.GetKeyDown(KeyCode.F))
        {
            playerLight.SetActive(!playerLight.activeSelf);
        }

        // Distraction
        if (Input.GetMouseButtonDown(1))
        {
            Vector2 distPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            for (int i = 0; i < enemies.Length; i++)
            {
                Debug.Log(enemies[i]);
                enemies[i].Distract(coin, distPos);
            }
        }
    }

    private void FixedUpdate()
    {
        Vector2 direction = (rb.position + movement) - rb.position;
        
        // Movement
        Vector2 force = movement * (speed * Time.fixedDeltaTime);
        
        rb.AddForce(force);
        
        // Rotation
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;

        transform.rotation = Quaternion.Lerp(transform.rotation,
            Quaternion.Euler(0, 0, angle),
            rotateTime * Time.deltaTime);
    }
}