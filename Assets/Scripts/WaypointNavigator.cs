using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class WaypointNavigator : MonoBehaviour
{
    #region Variables

    public Waypoint currentWaypoint;

    private EnemyAI controller;

    private int direction;

    #endregion

    private void Awake()
    {
        controller = GetComponent<EnemyAI>();
    }

    // Start is called before the first frame update
    private void Start()
    {
        direction = Mathf.RoundToInt(Random.Range(0f, 1f));
        controller.SetTarget(currentWaypoint.GetPosition());
    }

    // Update is called once per frame
    private void Update()
    {
        if (controller.touchingEnemy)
        {
            direction = (direction == 0) ? 1 : 0;
        }
        
        if (controller.ReachedEndOfPath)
        {
            bool shouldBranch = false;

            if (currentWaypoint.branches != null && currentWaypoint.branches.Count > 0)
            {
                shouldBranch = Random.Range(0f, 1f) <= currentWaypoint.branchRatio;
            }

            if (shouldBranch)
            {
                currentWaypoint = currentWaypoint.branches[Random.Range(0, currentWaypoint.branches.Count - 1)];
            }
            else
            {
                if (direction == 0)
                {
                    if (currentWaypoint.nextWaypoint != null)
                    {
                        currentWaypoint = currentWaypoint.nextWaypoint;
                    }
                    else
                    {
                        currentWaypoint = currentWaypoint.previousWaypoint;
                        direction = 1;
                    }
                }
                else if (direction == 1)
                {
                    if (currentWaypoint.previousWaypoint != null)
                    {
                        currentWaypoint = currentWaypoint.previousWaypoint;
                    }
                    else
                    {
                        currentWaypoint = currentWaypoint.nextWaypoint;
                        direction = 0;
                    }
                }
            }
            
            controller.SetTarget(currentWaypoint.GetPosition());
        }
    }
}
