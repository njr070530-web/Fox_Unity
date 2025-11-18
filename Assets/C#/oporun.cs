using UnityEngine;

public class OpoRun : MonoBehaviour
{
    private Rigidbody2D rb;
    private SpriteRenderer sprite;

    public float moveSpeed = 3f;      // 跑步速度
    public float moveDistance = 5f;   // 每次跑的距离

    private float startX;
    private int direction = -1;       // ← 先向左跑

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();

        startX = transform.position.x;
    }

    void Update()
    {
        // 水平移动
        rb.velocity = new Vector2(moveSpeed * direction, rb.velocity.y);

        // 朝向翻转（direction < 0 时朝左）
        sprite.flipX = (direction > 0);

        // 跑到指定偏移距离 → 转向
        if (Mathf.Abs(transform.position.x - startX) >= moveDistance)
        {
            // 下次反方向跑
            direction *= -1;

            // 更新新的“起点”
            startX = transform.position.x;
        }
    }
}
