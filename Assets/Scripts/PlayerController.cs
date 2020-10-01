using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    #region Variables

    public float speed = 200f;
    public float rotateTime = 0.5f;

    public float attackRange;
    public float attackWidth;
    public Transform attackPoint;
    public LayerMask enemyLayers;

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

        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (Physics2D.OverlapBox(attackPoint.position, new Vector2(attackWidth, attackRange), transform.rotation.z, enemyLayers))
            {
                EnemyController enemy = Physics2D
                    .OverlapBox(attackPoint.position, new Vector2(attackWidth, attackRange), transform.rotation.z, enemyLayers)
                    .GetComponent<EnemyController>();
                
                enemy.Kill();
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

    private void OnDrawGizmosSelected()
    {
        Color prevColor = Gizmos.color;
        Matrix4x4 prevMatrix = Gizmos.matrix;
        
        Gizmos.color = Color.red;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawWireCube(attackPoint.localPosition, new Vector3(attackWidth, attackRange, 0f));

        Gizmos.color = prevColor;
        Gizmos.matrix = prevMatrix;
    }
}