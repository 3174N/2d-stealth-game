using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    // Start is called before the first frame update
    private void Start()
    {
        
    }

    // Update is called once per frame
    private void Update()
    {
        
    }

    /// <summary>
    /// Kills the enemy
    /// </summary>
    public void Kill()
    {
        Debug.Log(gameObject.name + " was killed");
        Destroy(gameObject);
    }
}
