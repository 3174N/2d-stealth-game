using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Distraction", menuName = "Distractions/Distraction")]
public class Distraction : ScriptableObject
{
    public float soundRadius;
    
    public Vector2 source;
    public Vector2 position;

    public GameObject particals;
}
