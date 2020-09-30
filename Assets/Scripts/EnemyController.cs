using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    #region Variables

    public int maxHealth;
    private int currentHealth;

    #endregion
    
    // Start is called before the first frame update
    private void Start()
    {
        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Changes enemy's health
    /// </summary>
    /// <param name="amount"></param>
    public void ChangeHealth(int amount)
    {
        currentHealth += amount;
    }
}
