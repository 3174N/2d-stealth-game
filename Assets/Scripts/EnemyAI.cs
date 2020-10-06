using System;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.SceneManagement;

public class EnemyAI : MonoBehaviour
{
    #region Variables

    [Header("Movement")] public Transform[] waypoints;

    public float initSpeed = 200f;
    public float speedMultiplier = 2f;
    float currentSpeed;
    public float nextWayPointDistance = 3f;

    [Tooltip("How long object waits after reaching to waypoint")]
    public float maxWaitTime = 0f;

    private float waitTime = 0f;

    public float rotateTime = 5f;

    [Header("Detection")] 
    [Tooltip("How long until noticing player in seconds")]
    public float awareness;

    private float alert;
    private bool isAlert;

    public ProgressBar alertBar;

    public float backupRadius = 5f;
    
    public Transform raycaster;
    public LayerMask raycastingMask;

    [Tooltip("The size of the raycast")]
    public float raySize = 60f;

    public float lightAngle = 360f;

    public float lightRadius = 1f;

    private int currentWaypoint;
    private int waypoint = 0;
    private bool reachedEndOfPath = false;
    private bool reachedWaypoint;
    private Vector2 target;

    private bool foundPlayer;
    private bool isSearching;
    private bool isDistracted;

    private Rigidbody2D rb;
    private Path path;
    private Seeker seeker;

    #endregion

    // Start is called before the first frame update
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        seeker = GetComponent<Seeker>();

        Light2D light2D = raycaster.GetComponent<Light2D>();
        if (light2D != null)
        {
            light2D.pointLightOuterAngle = lightAngle;
            light2D.pointLightInnerAngle = lightAngle;
            light2D.pointLightOuterRadius = lightRadius;
        }

        currentSpeed = initSpeed;

        alertBar.minimum = 0;
        alertBar.maximum = awareness;
        alertBar.current = alert;

        target = waypoints[0].position;

        InvokeRepeating(nameof(UpdatePath), 0f, 0.5f);
    }

    private void UpdatePath()
    {
        if (seeker.IsDone())
            seeker.StartPath(rb.position, target, OnPathComplete);
    }

    private void OnPathComplete(Path p)
    {
        if (p.error) return;

        path = p;
        currentWaypoint = 0;

        reachedWaypoint = true;
    }

    private void Update()
    {
        if (!foundPlayer && !isSearching && !isDistracted)
            target = waypoints[waypoint].position;

        if (isSearching || foundPlayer)
            currentSpeed = initSpeed * speedMultiplier;
        else
            currentSpeed = initSpeed;

        // Raycasting
        float rayAngle = raycaster.rotation.z - (raySize / 2f);

        isAlert = false;
        for (int ray = 0; ray < raySize; ray++)
        {
            RaycastHit2D hit = Physics2D.Raycast(raycaster.position,
                Quaternion.AngleAxis(rayAngle, raycaster.forward) * raycaster.up,
                Mathf.Infinity, raycastingMask);

            if (hit.collider != null)
            {
                if (hit.transform.CompareTag("Player"))
                {
                    if (hit.transform.GetComponent<PlayerController>().IsLit)
                    {
                        // Found player
                        isAlert = true;
                        if (alert >= awareness)
                        {
                            isSearching = false;
                            foundPlayer = true;
                            target = hit.point;
                            Debug.Log("Found Player!");
                            hit.transform.GetComponent<PlayerController>().CallBackup(transform.position, backupRadius);
                        }
                    }
                }
            }

            // Debug raycast
            Debug.DrawLine(raycaster.position, hit.point, Color.red);

            rayAngle++;
        }
        if (isSearching || foundPlayer)
        {
            isAlert = true;
        }

        if (isAlert)
            alert = (alert <= awareness) ? alert + Time.deltaTime : alert = awareness;
        else
            alert = (alert > 0) ? alert - Time.deltaTime : alert = 0;

        alertBar.current = alert;
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        // Pathfinding
        if (path == null || waitTime > 0f)
        {
            waitTime -= Time.fixedDeltaTime;
            return;
        }

        if (currentWaypoint >= path.vectorPath.Count)
        {
            reachedEndOfPath = true;
            waypoint = (waypoint < waypoints.Length - 1) ? waypoint + 1 : 0;
            waitTime = maxWaitTime;

            if (foundPlayer)
            {
                // Lost sight of player
                foundPlayer = false;
                isSearching = true;
                Debug.Log("Lost Player!");
                StartCoroutine(Rotate(2f));
            }

            if (isDistracted)
            {
                // Got to distraction point
                isDistracted = false;
                isSearching = true;
                StartCoroutine(Rotate(2f));
            }

            return;
        }
        else
        {
            reachedEndOfPath = false;
        }

        Vector2 direction = ((Vector2) path.vectorPath[currentWaypoint] - rb.position).normalized;
        Vector2 force = direction * (currentSpeed * Time.fixedDeltaTime);

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;

        if (!isSearching)
            rb.AddForce(force);

        if (!reachedWaypoint)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(new Vector3(0f, 0f, angle)),
                Time.deltaTime * rotateTime);
        }
        else
            reachedWaypoint = false;

        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);
        if (distance < nextWayPointDistance)
        {
            currentWaypoint++;
        }
    }

    private IEnumerator Rotate(float duration)
    {
        float startRotation = transform.eulerAngles.z;
        float endRotation = startRotation + 360.0f;
        float t = 0.0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float zRotation = Mathf.Lerp(startRotation, endRotation, t / duration) % 360.0f;
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, zRotation);

            if (foundPlayer)
                break;
            
            yield return null;
        }

        Debug.Log("Stop Search");
        isSearching = false;
    }

    public void Distract(Distraction distraction, bool fillAwarness)
    {
        if (Vector2.Distance(rb.position, distraction.position) > distraction.soundRadius)
        {
            Debug.Log("Not Distracted");
        }
        else
        {
            Debug.Log("Distracted");
            target = distraction.position;
            isDistracted = true;
            if (fillAwarness)
            {
                alert = awareness;
                isAlert = true;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        PlayerController player = other.gameObject.GetComponent<PlayerController>();
        if (player != null)
        {
            if ((isSearching || foundPlayer))
                SceneManager.LoadScene(0);
            else
            {
                Distraction distraction = ScriptableObject.CreateInstance<Distraction>();
                distraction.soundRadius = 1f;
                distraction.position = player.transform.position;
                distraction.source = player.transform.position;
                Distract(distraction, false);
            }
        }
    }
}