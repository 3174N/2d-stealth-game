using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Distraction", menuName = "Distractions/Distraction")]
public class Distraction : ScriptableObject
{
    public float soundRadius;

    public Distraction(float soundRadius)
    {
        this.soundRadius = soundRadius;
    }
}
