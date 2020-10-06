using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointNavigator : MonoBehaviour
{
    #region Variables

    public Waypoint currentWaypoint;

    private EnemyAI controller;

    #endregion

    private void Awake()
    {
        controller = GetComponent<EnemyAI>();
    }

    // Start is called before the first frame update
    private void Start()
    {
        controller.SetTarget(currentWaypoint.GetPosition());
    }

    // Update is called once per frame
    private void Update()
    {
        if (controller.ReachedEndOfPath)
        {
            currentWaypoint = currentWaypoint.nextWaypoint;
            controller.SetTarget(currentWaypoint.GetPosition());
        }
    }
}
