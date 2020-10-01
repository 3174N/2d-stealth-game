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
    /// Kills the enemy
    /// </summary>
    public void Kill()
    {
        // TODO: Particals / Animations
        
        Debug.Log(gameObject.name + " was killed");
        Destroy(gameObject);
    }
}
