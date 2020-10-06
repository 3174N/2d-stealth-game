using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class EnemyController : MonoBehaviour
{
    #region Variables

    private EnemyAI enemyAI;

    #endregion

    private void Start()
    {
        enemyAI = GetComponent<EnemyAI>();
    }

    /// <summary>
    /// Kills the enemy
    /// </summary>
    public void Kill(PlayerController player)
    {
        Debug.Log(gameObject.name + " was killed");
        player.lights.Remove(enemyAI.raycaster.GetComponent<Light2D>());
        Destroy(gameObject);
    }
}
