using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageTaken : MonoBehaviour
{
    private float health = 100;
    public void TakeDamage(float damage) { 
        // physics to make the current component velocity fly off higher every hit
        // some random dir * ( 1 / 100 ) * 10 * velocityControl, clamp at min velocity
    
        // checking death:
        // on health = 0 -> die
    }
}
