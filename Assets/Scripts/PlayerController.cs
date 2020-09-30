using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //TODO: Add player keyboard controls

    #region Variables

    public GameObject playerLight;

    public Distraction coin;

    private EnemyAI[] enemies;

    #endregion

    private void Start()
    {
        enemies = FindObjectsOfType<EnemyAI>();
        //Debug.Log(enemies[0]);
    }

    private void Update()
    {
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
}