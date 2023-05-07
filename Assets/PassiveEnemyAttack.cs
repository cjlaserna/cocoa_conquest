using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassiveEnemyAttack : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log(collision.gameObject.name);
        // check if player
    }
}
