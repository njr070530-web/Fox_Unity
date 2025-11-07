using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    private Rigidbody2D rb;
    private SpriteRenderer sprite;
    private Animator anim;

    [Header("Movement Settings")]
    public float maxSpeed = 5f;
    public float maxJumpForce = 10f;

    // 跳跃控制
    private bool canJump = true;
    private float lastJumpTime = 0f;
    public float jumpCooldown = 0.3f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    void UpdateAnimationState()
    {
        // 控制跑步动画和朝向
        if (rb.velocity.x > 0.01f)
        {
            anim.SetBool("running", true);
            sprite.flipX = false;
        }
        else if (rb.velocity.x < -0.01f)
        {
            anim.SetBool("running", true);
            sprite.flipX = true;
        }
        else
        {
            anim.SetBool("running", false);
        }

        // 跳跃动画
        anim.SetBool("jumping", !canJump);
    }

    // 水平移动
    public void Idle()
    {
        rb.velocity = new Vector2(0, rb.velocity.y);
    }
    public void Move(float direction, float speedMultiplier)
    {
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
