using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private Transform attackPos;
    [SerializeField] private LayerMask EnemyLayerMask;
    [SerializeField] private float attackRange;
    [SerializeField] private float damageMultiplier;
    
    void Attack(float damage) {
        Collider2D[] enemiesToDamage = Physics2D.OverlapCircleAll(attackPos.position, attackRange, EnemyLayerMask);
        for (int i = 0; i < enemiesToDamage.Length; i++)
        {
            // @ TODO: make enemy script 
            enemiesToDamage[i].GetComponent<DamageTaken>().TakeDamage(damage * damageMultiplier);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPos.position, attackRange);
    }
}
