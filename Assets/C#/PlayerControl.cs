using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    private Rigidbody2D rb;
    public float attackRange = 100.0f;

    private SpriteRenderer sprite;
    private Animator anim;
    public LayerMask enemyLayer;
    // provate BoxCollider2D coll;

    [Header("Movement Settings")]
    public float maxSpeed = 3f;
    public float maxJumpForce = 50f;

    [SerializeField] private AudioSource jumpSoundEffect;
    [SerializeField] private AudioSource attackSoundEffect;




    // 跳跃控制
    private bool canJump = true;
    private bool is_attacking = false;
    private float lastJumpTime = 0f;
    public float jumpCooldown = 0.3f;
    public float attackDuration = 0.8f;

    private bool canAttack = true;
    public float attackCooldown = 0.3f;
    public float yourPitch = 400f;
    private enum MovementState {idle,running,jumping,falling,attacking,hurt,die};

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        // coll=GetComponent<BoxCollider2D>();
    }

    void UpdateAnimationState()
    {
        MovementState state;
        // 控制跑步动画和朝向
        if (rb.velocity.x > 0.01f)
        {
            state = MovementState.running;
            sprite.flipX = false;
        }
        else if (rb.velocity.x < -0.01f)
        {
            state = MovementState.running;
            sprite.flipX = true;
        }
        else
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
            state = MovementState.idle;
        }

        // 跳跃动画
        if (rb.velocity.y > 0.1f)
        {
            state = MovementState.jumping;
        }
        else if (rb.velocity.y < -0.1f)
        {
            state = MovementState.falling;
        }
        if (is_attacking)
        {
            state = MovementState.attacking;
        }
        anim.SetInteger("state", (int)state);
    }

    // 水平移动
    public void Idle()
    {
        rb.velocity = new Vector2(0, rb.velocity.y);
        anim.SetBool("running", false);
        anim.SetBool("attack_bool", false);

    }
    public void Move(float direction, float speedMultiplier)
    {
        // rb.velocity = new Vector2(direction * maxSpeed * speedMultiplier, rb.velocity.y);
        rb.velocity = new Vector2(direction * maxSpeed * speedMultiplier, rb.velocity.y);

    }

    // 跳跃
    public void Jump(float forceMultiplier)
    {
        if (!canJump) return;
        if (Time.time - lastJumpTime < jumpCooldown) return;
        jumpSoundEffect.Play();
        rb.velocity = new Vector2(rb.velocity.x, maxJumpForce * forceMultiplier);
        canJump = false;
        lastJumpTime = Time.time;
    }

    public bool IsAttacking()
{
    return is_attacking;
}


public void Attack()
{
    if (is_attacking) return;
    is_attacking = true;
    attackSoundEffect.Play();
    canAttack = false;
    anim.SetInteger("state",(int)MovementState.attacking);

    Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, attackRange, enemyLayer);
    foreach (var hit in hits)
    {
        Debug.Log($"Hit: {hit.name}");
        if (hit.CompareTag("Enemy"))
            hit.GetComponent<Enemy>().Die();
    }

    StartCoroutine(ResetAttack());
}

IEnumerator AttackCooldown()
{
    yield return new WaitForSeconds(attackCooldown);
    canAttack = true;
}

private IEnumerator ResetAttack()
{
    yield return new WaitForSeconds(attackDuration);
    is_attacking = false;
}

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            Jump(1.0f);
        }

         if(Input.GetKey(KeyCode.D))
        {
            Move(1.0f,1.0f);
        }
        if(Input.GetKey(KeyCode.S))
        {
            Attack();
        }
        else if(Input.GetKey(KeyCode.A))
        {
            Move(-1.0f,1.0f);
        }

        // 落地检测
        if (!canJump && Mathf.Abs(rb.velocity.y) < 0.01f)
        {
            canJump = true;
        }

        UpdateAnimationState();
    }
}
