using UnityEngine;

public class BackgroundParallax : MonoBehaviour
{
    public Transform target;
    public float parallaxFactor = 0.3f;
    public float spacingX = 10f;

    private float startX;
    private float spriteWidth;

    void Start()
    {
        startX = transform.position.x;
        spriteWidth = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    void LateUpdate()
    {
        if (target == null) return;

        // 只有 X 轴做视差
        float distX = target.position.x * parallaxFactor;

        transform.position = new Vector3(
            startX + distX,
            transform.position.y,   // ❗Y 轴不动
            transform.position.z
        );

        // X 循环
        float moveRange = spriteWidth + spacingX;
        float tempX = target.position.x * (1 - parallaxFactor);

        if (tempX > startX + moveRange)
            startX += moveRange;
        else if (tempX < startX - moveRange)
            startX -= moveRange;
    }
}
