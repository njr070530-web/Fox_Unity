using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    private Rigidbody2D rb;
    private SpriteRenderer sprite;
    private Animator anim;
    // provate BoxCollider2D coll;

    [Header("Movement Settings")]
    public float maxSpeed = 3f;
    public float maxJumpForce = 50f;

    // 跳跃控制
    private bool canJump = true;
    private bool is_attacking = false;
    private float lastJumpTime = 0f;
    public float jumpCooldown = 0.3f;
    public float attackDuration = 0.8f;
    public float yourPitch = 600f;
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
    if (is_attacking) return; // 防重入
    is_attacking = true;
    // 这里你可以播放音效或设置anim参数
    anim.SetInteger("state", (int)MovementState.attacking); // 立即同步动画（可选）
    StartCoroutine(ResetAttack());
}

private IEnumerator ResetAttack()
{
    yield return new WaitForSeconds(attackDuration);
    is_attacking = false;
}

    void Update()
    {
        // 落地检测
        if (!canJump && Mathf.Abs(rb.velocity.y) < 0.01f)
        {
            canJump = true;
        }

        UpdateAnimationState();
    }
}
