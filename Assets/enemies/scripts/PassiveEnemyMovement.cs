using System.Collections.Generic;
using UnityEngine;

public class PassiveEnemyMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeedX = 1f;
    [SerializeField] private float moveSpeedY = 0f;
    [SerializeField] private List<string> tagsToAvoid = new List<string>();
    [SerializeField] private LayerMask groundLyr;

    private Rigidbody2D rb;
    private CapsuleCollider2D coll;
    private bool IsFacingRight => transform.localScale.x > Mathf.Epsilon;
    private bool isGrounded => Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.down, 0.1f, groundLyr);

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<CapsuleCollider2D>();
    }

    private void Update()
    {
        if (!isGrounded) { 
            rb.velocity = new Vector2(0f, rb.gravityScale * -3);
            return;
        }
        if (IsFacingRight) rb.velocity = new Vector2(moveSpeedX, moveSpeedY);
        if (!IsFacingRight) rb.velocity = new Vector2(-moveSpeedX, moveSpeedY);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!tagsToAvoid.Contains(collision.gameObject.tag)) transform.localScale = new Vector2(-(Mathf.Sign(rb.velocity.x)), transform.localScale.y);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (tagsToAvoid.Contains(collision.gameObject.tag)) transform.localScale = new Vector2(-(Mathf.Sign(rb.velocity.x)), transform.localScale.y);
    }
}
