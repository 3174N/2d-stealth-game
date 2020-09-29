using System;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;

public class PlayerAI : MonoBehaviour
{
    #region Variables

    public float speed = 200f;
    public float nextWayPointDistance = 0.1f;
    public float rotateTime = 0.5f;

    public LineRenderer pathRenderer;

    private int currentWaypoint;
    private int waypoint;
    private bool reachedEndOfPath = false;
    private bool reachedWaypoint;
    private Vector2 target;
    private List<Vector3> mousePositions;

    private Rigidbody2D rb;
    private Path path;
    private Seeker seeker;

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        seeker = GetComponent<Seeker>();

        target = transform.position;
        mousePositions = new List<Vector3>();

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

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            mousePositions.Clear();
            waypoint = 0;
            NextTarget();
            
            mousePositions.Add((Vector2) Camera.main.ScreenToWorldPoint(Input.mousePosition));
        }
        if (Input.GetMouseButton(0))
        {
            mousePositions.Add((Vector2) Camera.main.ScreenToWorldPoint(Input.mousePosition));
        }

        for (int i = 1; i < mousePositions.Count; i++)
        {
            if (Mathf.Approximately(mousePositions[i].x, mousePositions[i - 1].x))
            {
                if (Mathf.Approximately(mousePositions[i].y, mousePositions[i - 1].y))
                {
                    mousePositions.RemoveAt(i - 1);
                }
            }
        }

        pathRenderer.positionCount = mousePositions.Count;
        pathRenderer.SetPositions(mousePositions.ToArray());
    }

    private void FixedUpdate()
    {
        //TODO: Fix stopping
        
        // Pathfinding
        if (path == null)
            return;

        if (currentWaypoint >= path.vectorPath.Count)
        {
            waypoint += 1;
            NextTarget();
            return;
        }
        else
        {
            reachedEndOfPath = false;
        }

        Vector2 direction = ((Vector2) path.vectorPath[currentWaypoint] - rb.position).normalized;
        Vector2 force = direction * (speed * Time.fixedDeltaTime);

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;

        rb.AddForce(force);

        if (!reachedWaypoint && target != (Vector2) transform.position && target != Vector2.negativeInfinity)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, 
                Quaternion.Euler(new Vector3(0f, 0f, angle)),
                Time.deltaTime * rotateTime);
        }
        else if (reachedWaypoint)
            reachedWaypoint = false;

        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);
        if (distance < nextWayPointDistance)
        {
            currentWaypoint++;
        }
    }

    private void NextTarget()
    {
        if (waypoint < mousePositions.Count)
            target = mousePositions[waypoint];
        else
            target = Vector2.negativeInfinity;
    }
}