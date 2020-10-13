using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.SceneManagement;

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

    public LineRenderer throwRenderer;
    public int distractionBounces;
    public float distractionDistance;
    public GameObject distractionPreview;
    private GameObject preview = null;

    public GameObject playerLight;

    public Distraction coin;
    
    [HideInInspector]
    public List<Light2D> lights;

    private EnemyAI[] enemies;
    private bool isLit;
    public bool IsLit => isLit;

    private Vector2 movement;
    private bool isMoving;

    private List<Vector3> contacts;

    private Rigidbody2D rb;

    #endregion

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        enemies = FindObjectsOfType<EnemyAI>();
        lights = FindObjectsOfType<Light2D>().ToList();
        
        contacts = new List<Vector3>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(0);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
        
        // Movement
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        movement.Set(horizontal, vertical);
        movement.Normalize();
        isMoving = movement.x != 0 || movement.y != 0;

        // Distraction

        if (Input.GetMouseButtonDown(1))
        {
            preview = Instantiate(distractionPreview);
        }
        
        if (Input.GetMouseButton(1))
        {
            contacts.Clear();
            contacts.Add(transform.position);

            Vector2 direction = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
            RaycastHit2D hit = Physics2D.Raycast(transform.position, 
                Quaternion.AngleAxis(Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f, Vector3.forward) * Vector3.up,
                Mathf.Infinity, wallLayer);
            
            contacts.Add(hit.point);
            Debug.DrawLine(transform.position, hit.point, new Color(1f, 0f, 0.87f));

            for (int i = 0; i < distractionBounces; i++)
            {
                Vector2 reflection = Vector2.Reflect(direction, hit.normal);
                hit = Physics2D.Raycast(hit.point - direction * 0.0000003f, 
                    Quaternion.AngleAxis(Mathf.Atan2(reflection.y, reflection.x) * Mathf.Rad2Deg - 90f, Vector3.forward) * Vector3.up,
                    Mathf.Infinity, wallLayer);
                
                contacts.Add(hit.point);
                Debug.DrawLine(transform.position, hit.point, new Color(1f, 0f, 0.87f));
                
                direction = reflection;
            }

            preview.transform.position = hit.point;

            throwRenderer.positionCount = contacts.Count;
            throwRenderer.SetPositions(contacts.ToArray());
        }

        if (Input.GetMouseButtonUp(1))
        {
            if (preview != null) Destroy(preview.gameObject);
            
            /*foreach (var enemy in enemies)
            {
                coin.position = distPos;
                coin.source = transform.position;
                enemy.Distract(coin, false);
                GameObject particals = Instantiate(coin.particals, coin.position, Quaternion.identity);
                Destroy(particals.gameObject, 5f);
            }*/
            
            contacts.Clear();
            throwRenderer.positionCount = 0;
        }

        // Killing enemies
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (Physics2D.OverlapBox(attackPoint.position, new Vector2(attackWidth, attackRange), transform.rotation.z, enemyLayers))
            {
                EnemyController enemy = Physics2D
                    .OverlapBox(attackPoint.position, new Vector2(attackWidth, attackRange), transform.rotation.z, enemyLayers)
                    .GetComponent<EnemyController>();
                
                enemy.Kill(this);
            }
        }

        isLit = false;
        foreach (Light2D light2D in lights)
        {
            if (light2D.lightType == Light2D.LightType.Global)
            {
                lights.Remove(light2D);
                break;
            }

            Vector2 lightDir = light2D.transform.position - transform.position;
            RaycastHit2D hit = Physics2D.Raycast(transform.position, 
                Quaternion.AngleAxis(Mathf.Atan2(lightDir.y, lightDir.x) * Mathf.Rad2Deg - 90f, Vector3.forward) * Vector3.up,
                Vector2.Distance(light2D.transform.position, transform.position),
                wallLayer);
            Debug.DrawLine(transform.position, hit.point, Color.yellow);

            if (Vector2.Distance(transform.position, light2D.transform.position) < light2D.pointLightOuterRadius)
            {
                if (Math.Abs(light2D.transform.rotation.z - Mathf.Atan2(lightDir.y, lightDir.x) * Mathf.Rad2Deg) < light2D.pointLightOuterAngle)
                {
                    if (hit.collider != null) break;
                    
                    isLit = true;
                }
            }
        }
        
        if (Input.GetKeyDown(KeyCode.F))
        {
            playerLight.SetActive(!playerLight.activeSelf);
        }
        isLit = playerLight.activeSelf || isLit;
    }

    public void CallBackup(Distraction distraction)
    {
        distraction.position = transform.position;
        foreach (var enemy in enemies)
        {
            enemy.Distract(distraction, true);
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

        if (isMoving)
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