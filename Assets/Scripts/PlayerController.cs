using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class PlayerController : MonoBehaviour
{
    #region Variables

    public float speed = 200f;
    public float rotateTime = 0.5f;

    public float attackRange;
    public float attackWidth;
    public Transform attackPoint;
    public LayerMask enemyLayers;
    public LayerMask wallLayer;

    public GameObject playerLight;

    public Distraction coin;

    private EnemyAI[] enemies;
    private List<Light2D> lights;
    private bool isLit;
    public bool IsLit => isLit;

    private Vector2 movement;

    private Rigidbody2D rb;

    #endregion

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        enemies = FindObjectsOfType<EnemyAI>();
        lights = FindObjectsOfType<Light2D>().ToList();
    }

    private void Update()
    {
        // Movement
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        movement.Set(horizontal, vertical);
        movement.Normalize();

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

        // Killing enemies
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

        isLit = false;
        foreach (Light2D light2D in lights)
        {
            if (light2D.lightType == Light2D.LightType.Global)
            {
                lights.Remove(light2D);
                return;
            }

            Vector2 lightDir = light2D.transform.position - transform.position;
            RaycastHit2D hit = Physics2D.Raycast(transform.position, 
                Quaternion.AngleAxis(Mathf.Atan2(lightDir.y, lightDir.x) * Mathf.Rad2Deg, transform.forward) * transform.up,
                Vector2.Distance(light2D.transform.position, transform.position),
                wallLayer);
            Debug.DrawLine(transform.position, hit.point, Color.yellow);

            if (Vector2.Distance(transform.position, light2D.transform.position) < light2D.pointLightOuterRadius)
            {
                if (Math.Abs(light2D.transform.rotation.z - Mathf.Atan2(lightDir.y, lightDir.x) * Mathf.Rad2Deg) < light2D.pointLightOuterAngle)
                {
                    if (hit.collider != null) return;
                    
                    isLit = true;
                    Debug.Log("Player lit by " + light2D.name);
                }
            }
        }
        
        if (Input.GetKeyDown(KeyCode.F))
        {
            playerLight.SetActive(!playerLight.activeSelf);
        }
        isLit = playerLight.activeSelf || isLit;
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