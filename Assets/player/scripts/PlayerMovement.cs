using static System.Math;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml.Serialization;
using Unity.VisualScripting;
using SpriteTrailRenderer;
using UnityEngine.UIElements;
using System.Runtime.CompilerServices;
using System.Linq;


// @TODO: Add animation hit ground
// @TODO: Improve ability synergy, ex: when hit enemy down: bounce up
public class PlayerMovement : MonoBehaviour
{
    #region Serialized Variables
    [Header("Movement Multipliers")]
    [SerializeField] private float moveMultiplier = 1.5f;
    [SerializeField] private float jumpMultiplier = 2f;
    [Range(0.005f, 1f)][SerializeField] private float jumpCutMultiplier = 0.5f;
    [Range(1f, 10f)][SerializeField] private float fallGravityMultiplier = 2f;

    [Header("Key Movement Variables")]
    [Range(5f, 10f)][SerializeField] private float moveSpeed = 7f;
    [Range(2f, 10f)][SerializeField] private float jumpForce = 4f;
    [Range(0.005f, 1f)][SerializeField] private float HorizontalMovementSmoothing = 0.015f;
    [Range(0.005f, 1f)][SerializeField] private float jumpForgiveness = 0.1f;
    [Range(0.005f, 5f)][SerializeField] private float apexModifier = 2f;

    [Header("Movement Limiters")]
    [Range(1f, 10f)][SerializeField] private float gravityScale = 3f;
    [Range(1f, 50f)][SerializeField] private float maxFallSpeed = 10f;

    [Header("Dash/Dodge Variables")]
    [Range(0.005f, 1f)][SerializeField] private float dashDodgeTime = 0.2f;
    [Range(0.005f, 1f)][SerializeField] private float dashDodgeAnimationDelay = 0.3f;
    [Range(1f, 30f)][SerializeField] private float dashDodgePower = 8f;
    [Range(0.005f, 5f)] [SerializeField] private float dashDodgeCooldown = 0.5f;

    [Header("Other")]
    [SerializeField] private LayerMask groundLyr;
    [SerializeField] private SpriteTrailRenderer.SpriteTrailRenderer tr;
    [Range(1, 10f)][SerializeField] private float ledgeForgiveness;
    #endregion

    private Rigidbody2D rb;
    private BoxCollider2D coll;
    private SpriteRenderer sprite;
    private Animator anim;
    private GameObject UI;

    // Movement
    private float dirX = 0f;
    private float dirY = 0f;

    private bool isGrounded => Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.down, 0.1f, groundLyr);
    private void Start()
    {
        tr.enabled = false;
        dirY = jumpForce * jumpMultiplier;
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<BoxCollider2D>();
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        UI = transform.Find("PlayerDynamicUI").gameObject;
    }

    private void Update()
    {
        dirX = anim.GetInteger("movementState") == 4 ? dirX : Input.GetAxisRaw("Horizontal") * moveSpeed * moveMultiplier;

        // Movement
        UpdateHorizonal(dirX);
        UpdateMovementState();
        UpdateAttackState();
    }

    private void UpdateHorizonal(float dirX) 
    {
        Vector2 velocity = Vector2.zero;
        if (dirX < 0f)
        {
            transform.rotation = Quaternion.Euler(transform.position.x, 180, transform.position.z);
        }
        else if (dirX > 0f) 
        {
            transform.rotation = Quaternion.Euler(transform.position.x, 0, transform.position.z);
        }

        Vector2 targetVelocity = new Vector2(dirX, rb.velocity.y);
        rb.velocity = Vector2.SmoothDamp(rb.velocity, targetVelocity, ref velocity, HorizontalMovementSmoothing); 
    }

    #region movementStates
    private enum MovementState { idle, running, jumping, falling, freeze }
    private MovementState movementState = MovementState.idle;

    private float timeLastPressedJump = 0f;
    private float timeLastGrounded = 0f;

    private bool canForgive => Time.time - timeLastPressedJump <= jumpForgiveness;
    private bool touchingLedgeLeft => Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.left, 0.1f, groundLyr);
    private bool touchingLedgeRight => Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.right, 0.1f, groundLyr);
    private bool canLedgeNudge => Time.time - timeLastPressedJump <= jumpForgiveness * ledgeForgiveness;
    private void UpdateMovementState()
    {
        // running
        if (!isGrounded && Time.time - timeLastPressedJump <= jumpForgiveness * 2.5f) movementState = MovementState.jumping;
        else if (dirX > 0f) movementState = MovementState.running;
        else if (dirX < 0f) movementState = MovementState.running;
        else movementState = MovementState.idle;

        // jumping
        if(Input.GetButtonDown("Jump")) timeLastPressedJump = Time.time;
        if (Input.GetButtonDown("Jump")){
            if (isGrounded) // If grounded and jumping
            {
                movementState = MovementState.jumping;
                rb.velocity = new Vector2(rb.velocity.x + moveMultiplier, Max(dirY, -maxFallSpeed));
            } 
            else if (isGrounded && canForgive) 
            {
                movementState = MovementState.jumping;
                rb.velocity = new Vector2(rb.velocity.x + moveMultiplier, Max(dirY, -maxFallSpeed));
            }
            else if (canForgive && Time.time - timeLastGrounded <= jumpForgiveness)
            {
                movementState = MovementState.jumping;
                rb.velocity = new Vector2(rb.velocity.x + moveMultiplier, Max(dirY, -maxFallSpeed));
            }
        }

        if(Input.GetButtonUp("Jump") && attackState == AttackState.none) rb.velocity = new Vector2(rb.velocity.x, Max(rb.velocity.y * (1 - jumpCutMultiplier), -maxFallSpeed));
         
        // falling
        if (rb.velocity.y <= 0f && !isGrounded)
        {
            movementState = MovementState.falling;
            float apexPoint = Mathf.InverseLerp(apexModifier, 0, Mathf.Abs(rb.velocity.y));
            rb.gravityScale = gravityScale * fallGravityMultiplier;
        }
        else rb.gravityScale = gravityScale;

        // Grounded
        if (isGrounded) timeLastGrounded = Time.time;
        else if ((touchingLedgeLeft || touchingLedgeRight) && canLedgeNudge && !isGrounded) {
            float direction = touchingLedgeRight ? 1 : -1;
            rb.velocity = new Vector2(rb.velocity.x, 8f);
            anim.Play("Player_Running");
        }
        
        anim.SetInteger("movementState", (int)movementState);
    }
    #endregion

    #region Attack States
    private enum AttackState { none, normal, heavy_up, heavy_down, heavy_side }
    private AttackState attackState = AttackState.none;

    // Dash
    private bool canDash = true;
    private bool canDodge = true;

    private bool isDodging;
    private bool isDashing;

    private void UpdateAttackState() 
    {
        // attack normal
        if (Input.GetButtonDown("Fire1"))
        {
            moveSpeed = 7f;
            attackState = AttackState.normal;
            anim.Play("Player_Attack_Normal");
        }

        // attack heavy
        if (Input.GetButtonDown("Fire2") && anim.GetInteger("movementState") != 0) 
        {
            moveSpeed = 7f;
            switch (anim.GetInteger("movementState"))
            {
                case 0: // idle
                    attackState = AttackState.normal;
                    anim.Play("Player_Attack_Normal");
                    break;
                case 1: // running
                    if (canDash && isGrounded) StartCoroutine(Dash());
                    break;
                case 2: // jumping
                    StartCoroutine(upHeavy());
                    break;
                case 3: // falling
                    StartCoroutine(downHeavy());
                    break;
                default:
                    Debug.Log("Cannot attack on movement state");
                    break;
            }
        }

        if (Input.GetButtonDown("Fire3") && !isGrounded && canDash)
        { 
            StartCoroutine(Dash());
        }
        else if(Input.GetButtonDown("Fire3") && isGrounded && canDodge) 
        {
            StartCoroutine(Dodge());
        }
    }
    
    private IEnumerator Dash() 
    {
        attackState = AttackState.heavy_side;
        canDash = canDodge = false;
        isDashing = true;
        anim.Play("Player_Attack_Dash");

        float originalGravity = gravityScale;  
        gravityScale = 0f;
        yield return new WaitForSeconds(dashDodgeAnimationDelay);

        int direction;
        direction = 180 - transform.localEulerAngles.y <= 0.999 ? -1 : 1;
        rb.velocity = new Vector2(direction * transform.localScale.x * dashDodgePower * 6, 1f);
        tr.enabled = true;
        yield return new WaitForSeconds(dashDodgeTime);

        tr.enabled = false;
        gravityScale = originalGravity;
        isDashing = false;
        UI.GetComponent<ShowIndicator>().Show(dashDodgeCooldown, ShowIndicator.IndicatorType.Dash);
        yield return new WaitForSeconds(dashDodgeCooldown);
        canDash = canDodge = true;
        attackState = AttackState.none;
    }
    private IEnumerator Dodge() 
    {
        attackState = AttackState.heavy_side;
        canDodge = canDash = false;
        isDodging = true;
        anim.Play("Player_Dodge");

        float originalGravity = gravityScale;
        gravityScale = 0f;

        yield return new WaitForSeconds(dashDodgeAnimationDelay);

        int direction;
        direction = 180 - transform.localEulerAngles.y <= 0.999 ? -1 : 1;
        rb.velocity = new Vector2(direction * transform.localScale.x * dashDodgePower * 5, rb.velocity.y);
        tr.enabled = true;
        yield return new WaitForSeconds(dashDodgeTime);

        tr.enabled = false;
        gravityScale = originalGravity;
        isDodging = false;
        UI.GetComponent<ShowIndicator>().Show(dashDodgeCooldown, ShowIndicator.IndicatorType.Dash);
        yield return new WaitForSeconds(dashDodgeCooldown);
        canDodge = canDash = true;
        attackState = AttackState.none;
        movementState = MovementState.running;
    }
    private IEnumerator downHeavy() 
    {
        attackState = AttackState.heavy_down;
        tr.enabled = true;
        anim.Play("Player_Attack_Down");
        rb.velocity = new Vector2(dirX, -maxFallSpeed);
        yield return new WaitUntil(() => isGrounded);
        tr.enabled = false;
        attackState = AttackState.none;
    }

    private IEnumerator upHeavy() {
        attackState = AttackState.heavy_up;
        anim.Play("Player_Attack_Up");
        rb.velocity = new Vector2(dirX, rb.velocity.y + jumpMultiplier * 2);
        yield return new WaitUntil(() => movementState != MovementState.jumping);
        attackState = AttackState.none;
    }
    #endregion

    #region Helpers
    private void incrementSpeed(float speed)
    {
        moveSpeed += speed;
    }
    #endregion
}